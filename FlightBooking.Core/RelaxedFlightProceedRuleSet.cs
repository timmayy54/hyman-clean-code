namespace FlightBooking.Core
{
    public class RelaxedFlightProceedRuleSet : IFlightProceedRuleSet
    {
        public bool CanProceed(FlightSummary summary)
        {
            var employeeOccupancy = summary.AirlineEmployeePassengerCount / (double)summary.Aircraft.NumberOfSeats;
            var revenueRuleSatisfied =
                summary.ProfitSurplus > 0 ||
                employeeOccupancy > summary.FlightRoute.MinimumTakeOffPercentage;

            return revenueRuleSatisfied &&
                   summary.SeatsTaken < summary.Aircraft.NumberOfSeats &&
                   summary.SeatsTaken / (double)summary.Aircraft.NumberOfSeats > summary.FlightRoute.MinimumTakeOffPercentage;
        }
    }
}
