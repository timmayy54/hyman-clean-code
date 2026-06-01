# Design Notes

## Refactoring shape

`ScheduledFlight` remains the public entry point used by the console app, but it no longer owns all calculation, business-rule, and text-formatting behavior directly.

The core responsibilities are now split as follows:

| Type | Responsibility |
| --- | --- |
| `ScheduledFlight` | Holds route, aircraft, passengers, selected rule set, and available alternatives |
| `FlightSummaryCalculator` | Calculates passenger counts, revenue, costs, baggage, loyalty totals, proceed status, and suitable alternatives |
| `FlightSummary` | Carries calculated summary data |
| `FlightSummaryReportFormatter` | Converts a `FlightSummary` into the required report text |
| `IFlightProceedRuleSet` | Represents a selectable set of proceed rules |
| `DefaultFlightProceedRuleSet` | Preserves the original proceed rules |
| `RelaxedFlightProceedRuleSet` | Allows unprofitable flights when airline employee occupancy exceeds the route minimum |

This keeps the feature additions out of one large report-building method and gives the airline a clear extension point for future rule sets.

## Rule-set interpretation

The relaxed rule requirement says revenue need not exceed cost "if the number of airline employees aboard is greater than the minimum percentage of passengers required."

This implementation treats that as employee occupancy:

```text
airline employee count / aircraft seat count > route minimum takeoff percentage
```

The capacity and minimum occupancy rules still apply.

## Aircraft alternatives

Alternative aircraft are supplied to the scheduled flight with `AddAlternativeAircraft`.

When the scheduled aircraft cannot proceed, each alternative aircraft is evaluated using the selected rule set and the same passenger manifest. Only alternatives that pass the selected rule set are listed in the report.

