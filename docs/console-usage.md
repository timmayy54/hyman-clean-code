# Console Usage

Run the app:

```powershell
dotnet run --project .\FlightBookingConsole\FlightBooking.Console.csproj
```

The app waits for commands without printing a prompt.

## Commands

```text
add general NAME AGE
add loyalty NAME AGE LOYALTY_POINTS USING_POINTS_TRUE_OR_FALSE
add airline NAME AGE
add discounted NAME AGE
use default rules
use relaxed rules
print summary
exit
```

## Original sample plus new discounted line

```text
add general Steve 30
add general Mark 12
add general James 36
add general Jane 32
add loyalty John 29 1000 true
add loyalty Sarah 45 1250 false
add loyalty Jack 60 50 false
add airline Trevor 47
add general Alan 34
add general Suzy 21
print summary
```

The summary now includes:

```text
    Discounted sales: 0
```

That is the required additional passenger-type information.

## Discounted passenger example

```text
add discounted Daisy 28
print summary
```

Expected effects:

- discounted sales increases by 1;
- revenue increases by half the route base price;
- baggage does not increase;
- loyalty points do not increase.

## Relaxed rules example

```text
use relaxed rules
add airline A 30
add airline B 30
add airline C 30
add airline D 30
add airline E 30
add airline F 30
add airline G 30
add airline H 30
add airline I 30
print summary
```

The flight is unprofitable, but can proceed under relaxed rules because airline employee occupancy is above the route minimum.

## Alternative aircraft example

Add 13 general passengers, then print the summary. The scheduled Antonov AN-2 has 12 seats, so the report lists configured alternatives that satisfy the selected proceed rules.
