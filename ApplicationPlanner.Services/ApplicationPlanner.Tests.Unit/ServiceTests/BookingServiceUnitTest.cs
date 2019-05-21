using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
namespace ApplicationPlanner.Tests.Unit.ServiceTests
{
    [TestClass]
    public class BookingServiceUnitTest
    {
        private BookingService bookingService;
        private Mock<IRoomRepo> roomRepo;
        [TestInitialize]
        public void InitTest()
        {
            roomRepo = new Mock<IRoomRepo>();
            bookingService = new BookingService(roomRepo.Object);
        }
            
        [TestMethod]
        
        public void IsBookingValid_NonSmoking_Valid()
        {
            roomRepo.Setup(repo => repo.GetRoomDetail(1)).Returns(new Room { MaxGuests=4,
                PetsAllowed =false, SmokingAllowed= false });
            var booking = bookingService.isBookingValid(1, new Booking { IsSmoking = false });
            Assert.IsTrue(booking);
        }
        [TestMethod]
        public void IsBookingValid_Smoker_InValid()
        {
            roomRepo.Setup(repo => repo.GetRoomDetail(1)).Returns(new Room { MaxGuests = 4,
                PetsAllowed = false, SmokingAllowed = false });
            var booking = bookingService.isBookingValid(1, new Booking { IsSmoking = true });
            Assert.IsFalse(booking);
        }

        [TestMethod]
        public void IsBookingValid_HasPet_InValid()
        {
            roomRepo.Setup(repo => repo.GetRoomDetail(1)).Returns(new Room { MaxGuests = 4,
                PetsAllowed = false, SmokingAllowed = false });
            var booking = bookingService.isBookingValid(1, new Booking { HasPet = true });
            Assert.IsFalse(booking);
        }

        public void IsBookingValid_NoPets_Valid()
        {
            roomRepo.Setup(repo => repo.GetRoomDetail(1)).Returns(new Room
            {
                MaxGuests = 4,
                PetsAllowed = false,
                SmokingAllowed = false
            });
            var booking = bookingService.isBookingValid(1, new Booking { HasPet = false });
            Assert.IsFalse(booking);
        }

        [DataTestMethod]
        [DataRow(false,false,0,true)]
        [DataRow(true, false, 0, true)]
        public void IsBookingValid(bool isSmoking, bool hasPet, int guests, bool result)
        {
            roomRepo.Setup(repo => repo.GetRoomDetail(1)).Returns(new Room
            {
                MaxGuests = 4,
                PetsAllowed = false,
                SmokingAllowed = false
            });
            var booking = bookingService.isBookingValid(1, new Booking { HasPet = hasPet , IsSmoking = isSmoking, NumberOfGuests = guests});
            Assert.AreEqual(result, booking);
        }
        //Room allow smoking, pets, noOfGuests then successful

        //[TestMethod]
        //public void IsBookingValid_HasPet_InValid()
        //{
        //    var bookingService = new BookingService(null);
        //    var booking = bookingService.isBookingValid(1, new Booking { HasPet = true });
        //    Assert.IsFalse(booking);
        //}
    }
}
