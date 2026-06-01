namespace FlightBooking.Core.Tests;

public class ScheduledFlightSummaryTests
{
    [Fact]
    public void GetSummary_ReturnsExistingPdfSampleReport()
    {
        var flight = CreateLondonToParisFlight();

        flight.AddPassenger(new Passenger { Type = PassengerType.General, Name = "Steve", Age = 30 });
        flight.AddPassenger(new Passenger { Type = PassengerType.General, Name = "Mark", Age = 12 });
        flight.AddPassenger(new Passenger { Type = PassengerType.General, Name = "James", Age = 36 });
        flight.AddPassenger(new Passenger { Type = PassengerType.General, Name = "Jane", Age = 32 });
        flight.AddPassenger(new Passenger
        {
            Type = PassengerType.LoyaltyMember,
            Name = "John",
            Age = 29,
            LoyaltyPoints = 1000,
            IsUsingLoyaltyPoints = true
        });
        flight.AddPassenger(new Passenger
        {
            Type = PassengerType.LoyaltyMember,
            Name = "Sarah",
            Age = 45,
            LoyaltyPoints = 1250,
            IsUsingLoyaltyPoints = false
        });
        flight.AddPassenger(new Passenger
        {
            Type = PassengerType.LoyaltyMember,
            Name = "Jack",
            Age = 60,
            LoyaltyPoints = 50,
            IsUsingLoyaltyPoints = false
        });
        flight.AddPassenger(new Passenger { Type = PassengerType.AirlineEmployee, Name = "Trevor", Age = 47 });
        flight.AddPassenger(new Passenger { Type = PassengerType.General, Name = "Alan", Age = 34 });
        flight.AddPassenger(new Passenger { Type = PassengerType.General, Name = "Suzy", Age = 21 });

        var expected = string.Join(Environment.NewLine, [
            "Flight summary for London to Paris",
            "",
            "Total passengers: 10",
            "    General sales: 6",
            "    Loyalty member sales: 3",
            "    Airline employee comps: 1",
            "    Discounted sales: 0",
            "",
            "Total expected baggage: 13",
            "",
            "Total revenue from flight: 800",
            "Total costs from flight: 500",
            "Flight generating profit of: 300",
            "",
            "Total loyalty points given away: 10",
            "Total loyalty points redeemed: 100",
            "",
            "",
            "THIS FLIGHT MAY PROCEED"
        ]);

        Assert.Equal(expected, flight.GetSummary());
    }

    [Fact]
    public void GetSummary_CountsGeneralPassengerRevenueCostAndBaggage()
    {
        var flight = CreateLondonToParisFlight();

        flight.AddPassenger(new Passenger { Type = PassengerType.General, Name = "Steve", Age = 30 });

        var summary = flight.GetSummary();

        Assert.Contains("Total passengers: 1", summary);
        Assert.Contains("    General sales: 1", summary);
        Assert.Contains("Total expected baggage: 1", summary);
        Assert.Contains("Total revenue from flight: 100", summary);
        Assert.Contains("Total costs from flight: 50", summary);
    }

    [Fact]
    public void GetSummary_CountsCashLoyaltyMemberRevenueBaggageAndAccruedPoints()
    {
        var flight = CreateLondonToParisFlight();

        flight.AddPassenger(new Passenger
        {
            Type = PassengerType.LoyaltyMember,
            Name = "Sarah",
            Age = 45,
            LoyaltyPoints = 1250,
            IsUsingLoyaltyPoints = false
        });

        var summary = flight.GetSummary();

        Assert.Contains("    Loyalty member sales: 1", summary);
        Assert.Contains("Total expected baggage: 2", summary);
        Assert.Contains("Total revenue from flight: 100", summary);
        Assert.Contains("Total loyalty points given away: 5", summary);
        Assert.Contains("Total loyalty points redeemed: 0", summary);
    }

    [Fact]
    public void GetSummary_CountsRedeemingLoyaltyMemberBaggageAndRedeemedPoints()
    {
        var flight = CreateLondonToParisFlight();

        flight.AddPassenger(new Passenger
        {
            Type = PassengerType.LoyaltyMember,
            Name = "John",
            Age = 29,
            LoyaltyPoints = 1000,
            IsUsingLoyaltyPoints = true
        });

        var summary = flight.GetSummary();

        Assert.Contains("    Loyalty member sales: 1", summary);
        Assert.Contains("Total expected baggage: 2", summary);
        Assert.Contains("Total revenue from flight: 0", summary);
        Assert.Contains("Total loyalty points given away: 0", summary);
        Assert.Contains("Total loyalty points redeemed: 100", summary);
    }

    [Fact]
    public void GetSummary_CountsAirlineEmployeeCompCostAndBaggage()
    {
        var flight = CreateLondonToParisFlight();

        flight.AddPassenger(new Passenger { Type = PassengerType.AirlineEmployee, Name = "Trevor", Age = 47 });

        var summary = flight.GetSummary();

        Assert.Contains("    Airline employee comps: 1", summary);
        Assert.Contains("Total expected baggage: 1", summary);
        Assert.Contains("Total revenue from flight: 0", summary);
        Assert.Contains("Total costs from flight: 50", summary);
    }

    [Fact]
    public void GetSummary_RejectsUnprofitableFlights()
    {
        var flight = CreateLondonToParisFlight();

        flight.AddPassenger(new Passenger { Type = PassengerType.AirlineEmployee, Name = "Trevor", Age = 47 });

        var summary = flight.GetSummary();

        Assert.Contains("Flight losing money of: -50", summary);
        Assert.EndsWith("FLIGHT MAY NOT PROCEED", summary);
    }

    [Fact]
    public void GetSummary_RejectsFlightsOverCapacity()
    {
        var flight = CreateLondonToParisFlight(seats: 1);

        flight.AddPassenger(new Passenger { Type = PassengerType.General, Name = "Steve", Age = 30 });
        flight.AddPassenger(new Passenger { Type = PassengerType.General, Name = "Mark", Age = 12 });

        var summary = flight.GetSummary();

        Assert.Contains("Total passengers: 2", summary);
        Assert.EndsWith("FLIGHT MAY NOT PROCEED", summary);
    }

    [Fact]
    public void GetSummary_RejectsFlightsBelowMinimumOccupancy()
    {
        var flight = CreateLondonToParisFlight(seats: 12);

        flight.AddPassenger(new Passenger { Type = PassengerType.General, Name = "Steve", Age = 30 });

        var summary = flight.GetSummary();

        Assert.EndsWith("FLIGHT MAY NOT PROCEED", summary);
    }

    private static ScheduledFlight CreateLondonToParisFlight()
    {
        return CreateLondonToParisFlight(seats: 12);
    }

    private static ScheduledFlight CreateLondonToParisFlight(int seats)
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
}
