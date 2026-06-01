namespace FlightBooking.Core
{
    public class DefaultFlightProceedRuleSet : IFlightProceedRuleSet
    {
        public bool CanProceed(FlightSummary summary)
        {
            return summary.ProfitSurplus > 0 &&
                   summary.SeatsTaken < summary.Aircraft.NumberOfSeats &&
                   summary.SeatsTaken / (double)summary.Aircraft.NumberOfSeats > summary.FlightRoute.MinimumTakeOffPercentage;
        }
    }
}
