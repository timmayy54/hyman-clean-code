# Clean Code Challenge Requirements

## Source

The requirements come from `Clean Code Challenge.pdf`.

## Existing behavior to preserve

The existing system allows a user to:

- add general passengers;
- add loyalty member passengers;
- add airline employee passengers;
- print a flight summary report.

The same passenger data must continue to generate the same summary report, with only the required new information added.

The `FlightRoute` and `Plane` classes must not change because the brief says other systems depend on them.

## Existing passenger types

| Passenger type | Fare | Baggage | Loyalty points |
| --- | --- | --- | --- |
| General | Pays the route base price | 1 bag | None |
| Loyalty member paying cash | Pays the route base price | 2 bags | Gains route loyalty points |
| Loyalty member using points | Pays no money, redeems route base price in points | 2 bags | Redeems points |
| Airline employee | Free | 1 bag | None |

## New feature 1: discounted passengers

Add a discounted passenger type:

- pays half the route base price;
- does not accrue loyalty points;
- is not allowed a bag;
- appears in the summary report alongside the other passenger type counts.

## New feature 2: selectable rule sets

The default flight proceed rules are:

- revenue generated from the flight must exceed the cost of the flight;
- the number of passengers cannot exceed the number of seats on the plane;
- the aircraft must have at least the route's minimum percentage of passengers booked.

The new relaxed rules keep the default rules except:

- revenue does not need to exceed cost when the number of airline employees aboard is greater than the minimum passenger percentage required.

The design should allow future rule sets without rewriting the summary report calculation.

## New feature 3: alternative aircraft

When the scheduled aircraft cannot handle the flight, the summary report should list other available aircraft that could handle it.

If there are no suitable alternatives, the report should not add an alternatives section.

