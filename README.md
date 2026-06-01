# Flight Booking Clean Code Challenge

This solution refactors the original flight booking code and adds the requested challenge features:

- discounted passengers;
- selectable default and relaxed proceed-rule sets;
- alternative aircraft suggestions when the scheduled aircraft cannot proceed;
- an xUnit test suite covering existing behavior and new functionality;
- documentation of requirements, design decisions, TDD plan, console usage, and submission steps.

The easiest way to review the work is to start with `docs/implementation-log.md`. It details the process I followed: protecting the original behaviour with tests, refactoring under green tests, then adding each requested feature test-first.

## Run tests

```powershell
dotnet test .\FlightBookingProblem.sln
```

## Build

```powershell
dotnet build .\FlightBookingProblem.sln
```

## Run console demo

```powershell
dotnet run --project .\FlightBookingConsole\FlightBooking.Console.csproj
```

## Documentation

- `docs/requirements.md`
- `docs/tdd-plan.md`
- `docs/design-notes.md`
- `docs/console-usage.md`
- `docs/implementation-log.md`
- `docs/submission-checklist.md`
