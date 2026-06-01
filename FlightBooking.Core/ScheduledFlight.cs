using System.Collections.Generic;

namespace FlightBooking.Core
{
    public class ScheduledFlight
    {
        private readonly List<Plane> _alternativeAircraft;
        private readonly FlightSummaryCalculator _summaryCalculator;
        private readonly FlightSummaryReportFormatter _summaryReportFormatter;
        private IFlightProceedRuleSet _flightProceedRuleSet;

        public ScheduledFlight(FlightRoute flightRoute)
        {
            FlightRoute = flightRoute;
            Passengers = new List<Passenger>();
            _alternativeAircraft = new List<Plane>();
            _flightProceedRuleSet = new DefaultFlightProceedRuleSet();
            _summaryCalculator = new FlightSummaryCalculator();
            _summaryReportFormatter = new FlightSummaryReportFormatter();
        }

        public FlightRoute FlightRoute { get; private set; }
        public Plane Aircraft { get; private set; } = null!;
        public List<Passenger> Passengers { get; private set; }

        public void AddPassenger(Passenger passenger)
        {
            Passengers.Add(passenger);
        }

        public void SetAircraftForRoute(Plane aircraft)
        {
            Aircraft = aircraft;
        }

        public void SetFlightProceedRuleSet(IFlightProceedRuleSet flightProceedRuleSet)
        {
            _flightProceedRuleSet = flightProceedRuleSet;
        }

        public void AddAlternativeAircraft(Plane aircraft)
        {
            _alternativeAircraft.Add(aircraft);
        }

        public string GetSummary()
        {
            var summary = _summaryCalculator.Calculate(
                FlightRoute,
                Aircraft,
                Passengers,
                _flightProceedRuleSet,
                _alternativeAircraft);

            return _summaryReportFormatter.Format(summary);
        }
    }
}
