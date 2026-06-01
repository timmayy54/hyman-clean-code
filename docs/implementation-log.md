# Implementation Log

## 1. Baseline safety net

Before adding any new functionality, I first locked down the existing behaviour.

The goal was to preserve the original PDF sample output and make sure I did not accidentally break the current flight summary logic while refactoring.

I created an xUnit test project:

```text
FlightBooking.Core.Tests
```

I then wrote a characterization test in:

```text
FlightBooking.Core.Tests/ScheduledFlightSummaryTests.cs
```

The test was:

```csharp
GetSummary_ReturnsExistingPdfSampleReport()
```

This test created the London-to-Paris flight, added the exact passengers from the PDF example, and asserted the complete summary report.

After that, I added smaller tests for the existing business behaviour:

```csharp
GetSummary_CountsGeneralPassengerRevenueCostAndBaggage()
GetSummary_CountsCashLoyaltyMemberRevenueBaggageAndAccruedPoints()
GetSummary_CountsRedeemingLoyaltyMemberBaggageAndRedeemedPoints()
GetSummary_CountsAirlineEmployeeCompCostAndBaggage()
GetSummary_RejectsUnprofitableFlights()
GetSummary_RejectsFlightsOverCapacity()
GetSummary_RejectsFlightsBelowMinimumOccupancy()
```

I did this because a full report test protects the overall output, but smaller tests make it much easier to understand what broke if a future change causes a regression.

I confirmed the tests passed before starting the refactor:

```powershell
dotnet test .\FlightBookingProblem.sln
```

## 2. Refactor before adding features

The original `ScheduledFlight.GetSummary()` method did too much. It calculated passenger counts, revenue, costs, baggage, loyalty points, business-rule decisions, and report text all in one method.

Before adding the new challenge features, I refactored that behaviour into smaller classes while keeping `ScheduledFlight.GetSummary()` as the public entry point.

I split the responsibilities as follows:

| Type | Responsibility |
| --- | --- |
| `ScheduledFlight` | Holds route, aircraft, passengers, selected rule set, and alternative aircraft |
| `FlightSummary` | Carries calculated summary data |
| `FlightSummaryCalculator` | Calculates revenue, costs, baggage, loyalty totals, passenger counts, proceed status, and alternatives |
| `FlightSummaryReportFormatter` | Converts a `FlightSummary` into the required text report |
| `IFlightProceedRuleSet` | Represents a selectable set of flight proceed rules |
| `DefaultFlightProceedRuleSet` | Preserves the original proceed rules |

After this refactor, I reran:

```powershell
dotnet test .\FlightBookingProblem.sln
```

All existing tests still passed, which confirmed that the refactor had preserved behaviour.

## 3. Discounted passenger

The first new feature was the `Discounted` passenger type.

The requirements were:

- discounted passengers pay half the route base price;
- they do not receive baggage allowance;
- they do not accrue loyalty points;
- they are shown in the summary report alongside the other passenger types.

I wrote the test first:

```csharp
GetSummary_CountsDiscountedPassengerAtHalfPriceWithNoBaggage()
```

The test added one discounted passenger:

```csharp
flight.AddPassenger(new Passenger
{
    Type = PassengerType.Discounted,
    Name = "Daisy",
    Age = 28
});
```

It then asserted that the summary contained:

```text
Total passengers: 1
    Discounted sales: 1
Total expected baggage: 0
Total revenue from flight: 50
Total costs from flight: 50
Total loyalty points given away: 0
```

The test initially failed because `PassengerType.Discounted` did not exist yet. That confirmed the test was exercising the missing feature.

I then implemented the feature by:

1. Adding `Discounted` to the `PassengerType` enum.
2. Adding `DiscountedPassengerCount` to `FlightSummary`.
3. Updating `FlightSummaryCalculator` so discounted passengers add half the base price to revenue, increment the discounted count, and do not add baggage or loyalty points.
4. Updating `FlightSummaryReportFormatter` so the report includes:

   ```text
       Discounted sales: X
   ```

After implementation, I reran the test suite and confirmed the discounted passenger test passed.

## 4. Relaxed rules

The second feature was selectable flight proceed rule sets.

The default rules were already present in the original code:

- revenue must exceed cost;
- passenger count must be less than the number of aircraft seats;
- passenger occupancy must be greater than the route minimum takeoff percentage.

The new relaxed rules keep the capacity and occupancy rules, but allow an unprofitable flight to proceed when airline employee occupancy is greater than the route minimum takeoff percentage.

I wrote the test first:

```csharp
GetSummary_UsesRelaxedRulesWhenSelected()
```

The test selected the relaxed rule set:

```csharp
flight.SetFlightProceedRuleSet(new RelaxedFlightProceedRuleSet());
```

It then added 9 airline employees to a 12-seat aircraft.

I chose 9 employees because:

```text
9 / 12 = 0.75
```

The route minimum takeoff percentage is:

```text
0.7
```

So employee occupancy is greater than the required minimum.

The expected result was that the flight would lose money:

```text
Flight losing money of: -450
```

but still end with:

```text
THIS FLIGHT MAY PROCEED
```

The test initially failed because `RelaxedFlightProceedRuleSet` did not exist yet.

I implemented `RelaxedFlightProceedRuleSet` using the existing `IFlightProceedRuleSet` abstraction.

The key logic was:

```csharp
var employeeOccupancy = summary.AirlineEmployeePassengerCount / (double)summary.Aircraft.NumberOfSeats;

var revenueRuleSatisfied =
    summary.ProfitSurplus > 0 ||
    employeeOccupancy > summary.FlightRoute.MinimumTakeOffPercentage;
```

I interpreted the requirement as employee occupancy percentage rather than raw employee count, because comparing a passenger count directly to a percentage would not make business sense.

The relaxed rules still keep the other two checks:

```csharp
summary.SeatsTaken < summary.Aircraft.NumberOfSeats
summary.SeatsTaken / seats > minimum percentage
```

After implementation, I reran the test suite and confirmed the relaxed-rules test passed.

## 5. Alternative aircraft

The third feature was to list alternative aircraft when the scheduled aircraft cannot handle the flight.

I wrote two tests first:

```csharp
GetSummary_ListsSuitableAlternativeAircraftWhenScheduledAircraftCannotProceed()
GetSummary_DoesNotListAlternativeAircraftSectionWhenNoAlternativeCanProceed()
```

In the first test, the scheduled aircraft had 8 seats and I added 10 general passengers, so the scheduled aircraft failed the capacity rule.

I then added two alternative aircraft:

```text
Bombardier Q400: 12 seats
Jumbo: 100 seats
```

The expected result was that the report listed:

```text
Other more suitable aircraft are:
Bombardier Q400 could handle this flight.
```

and did not list the Jumbo.

The Jumbo was intentionally unsuitable because the same proceed rules should apply to alternative aircraft. With 10 passengers on 100 seats:

```text
10 / 100 = 0.1
```

That is below the route's minimum takeoff percentage of `0.7`.

The second test added only an unsuitable alternative aircraft and asserted that the alternatives section was not printed.

I implemented this by adding alternative aircraft storage to `ScheduledFlight`:

```csharp
public void AddAlternativeAircraft(Plane aircraft)
{
    _alternativeAircraft.Add(aircraft);
}
```

Then, in `FlightSummaryCalculator`, after calculating whether the scheduled aircraft could proceed, I evaluated each alternative aircraft using the same selected rule set:

```csharp
if (!summary.CanProceed)
{
    foreach (var candidateAircraft in alternativeAircraft)
    {
        var candidateSummary = CopyForAircraft(summary, candidateAircraft);

        if (ruleSet.CanProceed(candidateSummary))
        {
            summary.AlternativeAircraft.Add(candidateAircraft);
        }
    }
}
```

I chose to reuse the selected rule set because alternative aircraft should be judged by the same business rules as the scheduled aircraft.

Finally, I updated `FlightSummaryReportFormatter` to append the alternatives section only when:

- the scheduled aircraft cannot proceed;
- at least one alternative aircraft can proceed.

After implementation, I reran the test suite and confirmed both alternative-aircraft tests passed.

## 6. Console commands

After the domain functionality was implemented and tested, I updated the sample console app so the new functionality could be exercised manually.

I added support for:

```text
add discounted NAME AGE
use relaxed rules
use default rules
```

I also added sample alternative aircraft in `SetupAirlineData()`:

```csharp
_scheduledFlight.AddAlternativeAircraft(new Plane { Id = 456, Name = "Bombardier Q400", NumberOfSeats = 14 });
_scheduledFlight.AddAlternativeAircraft(new Plane { Id = 789, Name = "ATR 640", NumberOfSeats = 16 });
```

I kept the console simple because the challenge says the console app is only a sample interaction app, not a production UI.

I smoke-tested the console manually with:

```text
add discounted Daisy 28
print summary
```

and with:

```text
use relaxed rules
```

followed by airline employee passengers.

I also tested an overbooked flight to confirm alternative aircraft were listed.

## 7. Startup help text

The original console app started with a blank line and waited for input, which was not very user-friendly.

I added startup help text and a command prompt in the console app.

I added:

```csharp
PrintWelcomeMessage();
```

at startup, and:

```csharp
PrintPrompt();
```

before each `ReadLine()`.

The console now prints:

```text
Flight Booking Console

Available commands:
  add general NAME AGE
  add loyalty NAME AGE LOYALTY_POINTS USING_POINTS_TRUE_OR_FALSE
  add airline NAME AGE
  add discounted NAME AGE
  use default rules
  use relaxed rules
  print summary
  exit

>
```

I kept this in the console project rather than the core project because it is presentation/help text, not domain logic.

## 8. Documentation and submission preparation

I added documentation covering:

- requirements;
- TDD plan;
- design decisions;
- console usage;
- implementation process;
- submission checklist.

I also added a `.gitignore` to exclude build outputs and local editor files:

```text
bin/
obj/
.vs/
.vscode/
TestResults/
*.user
*.suo
```

This is important because the challenge brief specifically says not to include build outputs such as `.exe`, `bin`, or `obj` folders in the submitted archive.

## 9. Final verification

At the end, I ran:

```powershell
dotnet build .\FlightBookingProblem.sln
```

and confirmed:

```text
0 warnings, 0 errors
```

I also ran:

```powershell
dotnet test .\FlightBookingProblem.sln
```

and confirmed:

```text
12 tests passed
```

The final process was:

1. protect existing behaviour with tests;
2. refactor under green tests;
3. add each new feature with a failing test first;
4. implement the smallest clean change needed to pass;
5. update the console demo;
6. document the work and verify the full solution.

