using ApplicationPlanner.Transcripts.Web.Models;
using ApplicationPlanner.Transcripts.Web.Services;
using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Text;


namespace ApplicationPlanner.Transcripts.Web.Schema
{
    public class TranscriptRequestsSchema : GraphQL.Types.Schema
    {
        public TranscriptRequestsSchema(TranscriptRequestQuery transcriptQuery, TranscriptRequestMutation transcriptMutation, IDependencyResolver dependencyResolver)
        {
            Query = transcriptQuery;
            Mutation = transcriptMutation;
            DependencyResolver = dependencyResolver;   
        }
    }

    public class TranscriptRequestMutation: ObjectGraphType<object>
    {

    }

    public class TranscriptRequestQuery : ObjectGraphType<object>
    {
        public TranscriptRequestQuery(ITranscriptRequestService transcriptRequestService)
        {
            Name = "Query";
            Field<StringGraphType>("hello",resolve:(context) => {
                 return "Congratulations! you got admission."; 
                });

            Field<StudentType>("student", resolve: (context) => {
                return new Student { FirstName = "Test", LastName = "complex object" };
            });

            Field<ListGraphType<TranscriptResponseModelType>>("transcriptrequests", resolve: (context) =>
            {
                var requests = transcriptRequestService.GetTranscriptRequestByPortfolioIdAsync(15829712 ,   179234);
                return requests;
            });
        }
    }

    public class StudentType: ObjectGraphType<Student>
    {
        public StudentType()
        {
           // Field(o => o.FirstName);
        }
    }

    public class Student
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

}
