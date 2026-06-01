namespace FlightBooking.Core.Tests;

public class ScheduledFlightFeatureTests
{
    [Fact]
    public void GetSummary_CountsDiscountedPassengerAtHalfPriceWithNoBaggage()
    {
        var flight = CreateLondonToParisFlight();

        flight.AddPassenger(new Passenger { Type = PassengerType.Discounted, Name = "Daisy", Age = 28 });

        var summary = flight.GetSummary();

        Assert.Contains("Total passengers: 1", summary);
        Assert.Contains("    Discounted sales: 1", summary);
        Assert.Contains("Total expected baggage: 0", summary);
        Assert.Contains("Total revenue from flight: 50", summary);
        Assert.Contains("Total costs from flight: 50", summary);
        Assert.Contains("Total loyalty points given away: 0", summary);
    }

    [Fact]
    public void GetSummary_UsesRelaxedRulesWhenSelected()
    {
        var flight = CreateLondonToParisFlight();
        flight.SetFlightProceedRuleSet(new RelaxedFlightProceedRuleSet());

        for (var i = 0; i < 9; i++)
        {
            flight.AddPassenger(new Passenger { Type = PassengerType.AirlineEmployee, Name = $"Employee{i}", Age = 30 });
        }

        var summary = flight.GetSummary();

        Assert.Contains("Flight losing money of: -450", summary);
        Assert.EndsWith("THIS FLIGHT MAY PROCEED", summary);
    }

    [Fact]
    public void GetSummary_ListsSuitableAlternativeAircraftWhenScheduledAircraftCannotProceed()
    {
        var flight = CreateLondonToParisFlight(seats: 8);
        flight.AddAlternativeAircraft(new Plane { Id = 456, Name = "Bombardier Q400", NumberOfSeats = 12 });
        flight.AddAlternativeAircraft(new Plane { Id = 789, Name = "Jumbo", NumberOfSeats = 100 });

        AddGeneralPassengers(flight, count: 10);

        var summary = flight.GetSummary();

        Assert.Contains("FLIGHT MAY NOT PROCEED", summary);
        Assert.Contains("Other more suitable aircraft are:", summary);
        Assert.Contains("Bombardier Q400 could handle this flight.", summary);
        Assert.DoesNotContain("Jumbo could handle this flight.", summary);
    }

    [Fact]
    public void GetSummary_DoesNotListAlternativeAircraftSectionWhenNoAlternativeCanProceed()
    {
        var flight = CreateLondonToParisFlight(seats: 8);
        flight.AddAlternativeAircraft(new Plane { Id = 789, Name = "Jumbo", NumberOfSeats = 100 });

        AddGeneralPassengers(flight, count: 10);

        var summary = flight.GetSummary();

        Assert.Contains("FLIGHT MAY NOT PROCEED", summary);
        Assert.DoesNotContain("Other more suitable aircraft are:", summary);
    }

    private static ScheduledFlight CreateLondonToParisFlight(int seats = 12)
    {
        var londonToParis = new FlightRoute("London", "Paris")
        {
            BaseCost = 50,
            BasePrice = 100,
            LoyaltyPointsGained = 5,
            MinimumTakeOffPercentage = 0.7
        };

        var scheduledFlight = new ScheduledFlight(londonToParis);
        scheduledFlight.SetAircraftForRoute(new Plane { Id = 123, Name = "Antonov AN-2", NumberOfSeats = seats });

        return scheduledFlight;
    }

    private static void AddGeneralPassengers(ScheduledFlight flight, int count)
    {
        for (var i = 0; i < count; i++)
        {
            flight.AddPassenger(new Passenger { Type = PassengerType.General, Name = $"Passenger{i}", Age = 30 });
        }
    }
}
