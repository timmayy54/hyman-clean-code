using System.Collections.Generic;

namespace FlightBooking.Core
{
    public class FlightSummary
    {
        public FlightSummary(FlightRoute flightRoute, Plane aircraft)
        {
            FlightRoute = flightRoute;
            Aircraft = aircraft;
            AlternativeAircraft = new List<Plane>();
        }

        public FlightRoute FlightRoute { get; }
        public Plane Aircraft { get; }
        public int SeatsTaken { get; set; }
        public int GeneralPassengerCount { get; set; }
        public int LoyaltyMemberPassengerCount { get; set; }
        public int AirlineEmployeePassengerCount { get; set; }
        public int DiscountedPassengerCount { get; set; }
        public int TotalExpectedBaggage { get; set; }
        public double Revenue { get; set; }
        public double Costs { get; set; }
        public double ProfitSurplus => Revenue - Costs;
        public int TotalLoyaltyPointsAccrued { get; set; }
        public int TotalLoyaltyPointsRedeemed { get; set; }
        public bool CanProceed { get; set; }
        public List<Plane> AlternativeAircraft { get; }
    }
}
