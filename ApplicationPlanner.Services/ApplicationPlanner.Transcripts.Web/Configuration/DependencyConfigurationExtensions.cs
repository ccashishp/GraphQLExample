using ApplicationPlanner.Transcripts.Core.Repositories;
using ApplicationPlanner.Transcripts.Web.Controllers;
using ApplicationPlanner.Transcripts.Web.Filters;
using ApplicationPlanner.Transcripts.Web.Services;
using CC.Cache;
using CC.Data;
using CC.Storage.Containers;
using CC.Storage.File;
using CC3.AuthServices.Token;
using CC3.AuthServices.Token.Entities;
using CC3.AuthServices.Token.JWT;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using RestSharp;
using System;
using System.Collections.Generic;
using CC.Common.Enum;
using CC.Storage;

namespace ApplicationPlanner.Transcripts.Web.Configuration
{
    public static class DependencyConfigurationExtensions
    {
        public static IServiceCollection AddTranscriptsDependencies(this IServiceCollection services, IConfiguration config)
        {
            var storageConfig = config.GetSection("StorageConfig").Get<StorageConfig>();
            var credentialsConfig = config.GetSection("CredentialsAPIConfig").Get<CredentialsAPIConfig>();

            // Repositories
            services.AddTransient<IInstitutionRepository, InstitutionRepository>();
            services.AddTransient<ITranscriptRequestRepository, TranscriptRequestRepository>();
            services.AddTransient<IStudentRepository, StudentRepository>();
            services.AddTransient<IGlobalSettingRepository, GlobalSettingRepository>();
            services.AddTransient<IQARepository, QARepository>();
            services.AddTransient<ISchoolSettingRepository, SchoolSettingRepository>();
            services.AddTransient<ITranscriptRepository, TranscriptRepository>();
            services.AddTransient<ITimeZoneRepository, TimeZoneRepository>();
            services.AddTransient<IOnboardingFlagsRepository, OnboardingFlagsRepository>();

            // Services
            services.AddTransient<IAvatarService>(c => new AvatarService(
                c.GetRequiredService<IDictionary<CountryType, IStorageAccount>>(),
                storageConfig.ImageServer,
                storageConfig.FileStorageAzureAvatarCoverImageFolder));
            services.AddTransient<IStudentService>(c => new StudentService(c.GetRequiredService<IAvatarService>()));
            services.AddTransient<ITranscriptRequestService>(c => new TranscriptRequestService(
                c.GetRequiredService<ITranscriptRequestRepository>(),
                c.GetRequiredService<IInstitutionRepository>(),
                c.GetRequiredService<IAvatarService>(),
                c.GetRequiredService<ITimeZoneRepository>()));
            services.AddTransient<ILinqWrapperService>(c => new LinqWrapperService());
            services.AddTransient<IAccessService>(c => new AccessService(c.GetRequiredService<ISchoolSettingRepository>()));
            services.AddTransient<ITranscriptProviderAPIService>(c => new CredentialsAPIService(new RestClient(credentialsConfig.Url), credentialsConfig));
            services.AddTransient<ITranscriptProviderService>(c => new TranscriptProviderService(
                c.GetRequiredService<ITranscriptRequestRepository>(),
                c.GetRequiredService<ISchoolSettingRepository>(),
                c.GetRequiredService<ITranscriptProviderAPIService>(),
                c.GetRequiredService<ICache>()));
            services.AddTransient<ITranscriptService>(c => new TranscriptService(c.GetRequiredService<IAvatarService>(), c.GetRequiredService<ITimeZoneRepository>()));
            services.AddTransient<IOnboardingFlagsService>(c => new OnboardingFlagsService(c.GetRequiredService<IOnboardingFlagsRepository>()));

            // Filters
            services.AddScoped(c => new ApplicationPlannerAuthorizationFilter(c.GetRequiredService<ITokenValidator>()));
            services.AddScoped(c => new QAAuthorizationFilter(config.GetSection("TokenConfig").Get<TokenConfig>()));

            // Controllers
            services.AddScoped<InstitutionsController>();
            services.AddScoped<RequestsController>();
            services.AddScoped<QAController>();

            return services;
        }

        public static IServiceCollection AddCoreDependencies(this IServiceCollection services, IConfiguration config)
        {
            var dataConfig = config.GetSection("DataConfig").Get<DataConfig>();
            var cacheConfig = config.GetSection("CacheConfig").Get<CacheConfig>();
            var tokenConfig = config.GetSection("TokenConfig").Get<TokenConfig>();
            var elmahIo = config.GetSection("ElmahIo").Get<TokenConfig>();
            var storageConfig = config.GetSection("StorageConfig").Get<StorageConfig>();
            
            // Cache
            services.AddTransient(s => new RedisConnection(cacheConfig.RedisServer).Multiplexer.GetDatabase());
            services.AddTransient<ISerialize>(s => new JsonNetSerializer());

            services.AddSingleton<ICache>((ctx) =>
            {
                var database = ctx.GetRequiredService<IDatabase>();
                var serializer = ctx.GetRequiredService<ISerialize>();
                return new Redis(database, serializer, cacheConfig.LongerCacheExpiryTime);
            });


            Console.WriteLine($"Using XelloDbServer: { dataConfig.XelloDbServer }");
            // Database
            services.AddTransient<ISql>((ctx) =>
            {
                var cache = ctx.GetRequiredService<ICache>();
                return new Sql(cache, new DbConnection(dataConfig.XelloDbServer));
            });

            // TokenValidator
            services.AddTransient<ITokenValidator>(s => new BaseJwtTokenValidator(tokenConfig));

            // File Storage
            services.AddTransient(s => new FileStorageInHouse(
                    storageConfig.FileStorageLocal,
                    storageConfig.ApiPath,
                    storageConfig.FileUploadFolderLocalVirtual)
            );

            services.AddTransient<IDictionary<CountryType, IStorageAccount>>(s =>
                new Dictionary<CountryType, IStorageAccount>
                {
                    { CountryType.Canada, new AzureAccount(storageConfig.FileStorageAzureCanadian) },
                    { CountryType.US, new AzureAccount(storageConfig.FileStorageAzure) }
                }
            );

            services.AddTransient<IDictionary<FileStorageType, IFileStorage>>(s =>
                new Dictionary<FileStorageType, IFileStorage>
                {
                    { FileStorageType.InHouse, new FileStorageWindowsAzure(storageConfig.FileStorageAzureCanadian) },
                    { FileStorageType.WindowsAzure, new FileStorageWindowsAzure(storageConfig.FileStorageAzure) }
                }
            );

            services.AddSingleton<FileStorageFactory>();

            // Container Storage
            services.AddTransient<IDictionary<StorageType, Func<string, IStorageContainer>>>(s =>
                new Dictionary<StorageType, Func<string, IStorageContainer>>
                {
                    {StorageType.CloudPrivateCanada, name => new PrivateAzureContainer(storageConfig.FileStorageAzureCanadian, name)},
                    {StorageType.CloudPublicCanada, name => new PublicAzureContainer(storageConfig.FileStorageAzureCanadian, name)},
                    {StorageType.CloudPublicUs, name => new PublicAzureContainer(storageConfig.FileStorageAzure, name)},
                    {StorageType.CloudPrivateUs, name => new PrivateAzureContainer(storageConfig.FileStorageAzure, name)}
                }
            );

            services.AddTransient<IStorageContainerFactory, StorageContainerFactory>();

            return services;
        }
    }
}
