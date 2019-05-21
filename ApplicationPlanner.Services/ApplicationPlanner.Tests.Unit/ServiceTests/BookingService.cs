using System;

namespace ApplicationPlanner.Tests.Unit.ServiceTests
{
    public class BookingService
    {
        private IRoomRepo roomRepo;

        public BookingService(IRoomRepo roomsRepo)
        {
            this.roomRepo = roomsRepo;
        }

        public bool isBookingValid(int roomId, Booking booking)
        {
            var smokingValidation = true;
            var petsValidation = true;
            var noOfGuestsValidation = true;

            var roomDetial = this.roomRepo.GetRoomDetail(roomId);

            var guestIsSmoking = booking.IsSmoking;
            var bringingPet = booking.HasPet;
            var noOfGuests = booking.NumberOfGuests;

            smokingValidation = !roomDetial.SmokingAllowed && guestIsSmoking ? false : true;
            petsValidation = booking.HasPet ? false : true;

            return smokingValidation && petsValidation && noOfGuestsValidation;
        }
    }
}