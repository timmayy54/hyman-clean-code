namespace FlightBooking.Core
{
    public interface IFlightProceedRuleSet
    {
        bool CanProceed(FlightSummary summary);
    }
}
