# Implementation Log

## 2026-06-01

- Read the PDF challenge brief.
- Confirmed the project is a small .NET 8 solution with a console app and a core library.
- Confirmed `FlightRoute` and `Plane` are fixed by the brief and should not be changed.
- Confirmed .NET SDK `8.0.421` is installed at `C:\Program Files\dotnet\dotnet.exe`.
- Started with documentation and a test-first plan before changing production code.
- Added a characterization test for the PDF sample. The running code contains one extra blank line before the final proceed message compared with the PDF's visual formatting, so the test locks the actual current behavior.
- Refactored `ScheduledFlight.GetSummary()` into calculation, proceed-rule, and report-formatting collaborators while keeping the public entry point.
- Added feature tests for discounted passengers, relaxed rules, and alternative aircraft before implementing those features.
- Added the discounted passenger summary line to the golden report as required additional information.
- Updated the console app with `add discounted`, `use relaxed rules`, and `use default rules` commands.
- Added sample alternative aircraft to the console setup.
- Removed nullable initialization warnings without changing the public shape of the dependent `Plane` class.
- Verified `dotnet build .\FlightBookingProblem.sln` succeeds with 0 warnings and 0 errors.
- Verified `dotnet test .\FlightBookingProblem.sln` passes with 12 tests.
- Smoke-tested the console app for discounted passengers, relaxed rules, and alternative aircraft.
- Added startup help text and a command prompt to the console app.
