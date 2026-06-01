# Submission Checklist

Before sending the solution:

1. Run the full test suite.

   ```powershell
   dotnet test .\FlightBookingProblem.sln
   ```

2. Run a full build.

   ```powershell
   dotnet build .\FlightBookingProblem.sln
   ```

3. Remove build output folders before zipping.

   ```powershell
   Get-ChildItem -Recurse -Directory -Include bin,obj,TestResults | Remove-Item -Recurse -Force
   ```

4. Zip the source folder after the build outputs are removed.

Do not include `.exe` files, `bin` folders, or `obj` folders in the submitted archive.

