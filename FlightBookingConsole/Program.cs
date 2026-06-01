using System;
using FlightBooking.Core;

namespace FlightBookingProblem
{
    class Program
    {
        private static ScheduledFlight _scheduledFlight = null!;

        static void Main(string[] args)
        {
            SetupAirlineData();
            PrintWelcomeMessage();
            
            string command = "";
            do
            {
                PrintPrompt();
                command = Console.ReadLine() ?? "";
                var enteredText = command.ToLower();
                if (enteredText.Contains("print summary"))
                {
                    Console.WriteLine();
                    Console.WriteLine(_scheduledFlight.GetSummary());
                }
                else if (enteredText.Contains("add general"))
                {
                    string[] passengerSegments = enteredText.Split(' ');
                    _scheduledFlight.AddPassenger(new Passenger
                    {
                        Type = PassengerType.General, 
                        Name = passengerSegments[2], 
                        Age = Convert.ToInt32(passengerSegments[3])
                    });
                }
                else if (enteredText.Contains("add loyalty"))
                {
                    string[] passengerSegments = enteredText.Split(' ');
                    _scheduledFlight.AddPassenger(new Passenger
                    {
                        Type = PassengerType.LoyaltyMember, 
                        Name = passengerSegments[2], 
                        Age = Convert.ToInt32(passengerSegments[3]),
                        LoyaltyPoints = Convert.ToInt32(passengerSegments[4]),
                        IsUsingLoyaltyPoints = Convert.ToBoolean(passengerSegments[5]),
                    });
                }
                else if (enteredText.Contains("add airline"))
                {
                    string[] passengerSegments = enteredText.Split(' ');
                    _scheduledFlight.AddPassenger(new Passenger
                    {
                        Type = PassengerType.AirlineEmployee, 
                        Name = passengerSegments[2], 
                        Age = Convert.ToInt32(passengerSegments[3]),
                    });
                }
                else if (enteredText.Contains("add discounted"))
                {
                    string[] passengerSegments = enteredText.Split(' ');
                    _scheduledFlight.AddPassenger(new Passenger
                    {
                        Type = PassengerType.Discounted,
                        Name = passengerSegments[2],
                        Age = Convert.ToInt32(passengerSegments[3]),
                    });
                }
                else if (enteredText.Contains("use relaxed rules"))
                {
                    _scheduledFlight.SetFlightProceedRuleSet(new RelaxedFlightProceedRuleSet());
                }
                else if (enteredText.Contains("use default rules"))
                {
                    _scheduledFlight.SetFlightProceedRuleSet(new DefaultFlightProceedRuleSet());
                }
                else if (enteredText.Contains("exit"))
                {
                    Environment.Exit(1);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("UNKNOWN INPUT");
                    Console.ResetColor();
                }
            } while (command != "exit");
        }

        private static void PrintWelcomeMessage()
        {
            Console.WriteLine("Flight Booking Console");
            Console.WriteLine();
            Console.WriteLine("Available commands:");
            Console.WriteLine("  add general NAME AGE");
            Console.WriteLine("  add loyalty NAME AGE LOYALTY_POINTS USING_POINTS_TRUE_OR_FALSE");
            Console.WriteLine("  add airline NAME AGE");
            Console.WriteLine("  add discounted NAME AGE");
            Console.WriteLine("  use default rules");
            Console.WriteLine("  use relaxed rules");
            Console.WriteLine("  print summary");
            Console.WriteLine("  exit");
            Console.WriteLine();
        }

        private static void PrintPrompt()
        {
            Console.Write("> ");
        }

        private static void SetupAirlineData()
        {
            FlightRoute londonToParis = new FlightRoute("London", "Paris")
            {
                BaseCost = 50, 
                BasePrice = 100, 
                LoyaltyPointsGained = 5,
                MinimumTakeOffPercentage = 0.7
            };

            _scheduledFlight = new ScheduledFlight(londonToParis);

            _scheduledFlight.SetAircraftForRoute(
                new Plane { Id = 123, Name = "Antonov AN-2", NumberOfSeats = 12 });

            _scheduledFlight.AddAlternativeAircraft(new Plane { Id = 456, Name = "Bombardier Q400", NumberOfSeats = 14 });
            _scheduledFlight.AddAlternativeAircraft(new Plane { Id = 789, Name = "ATR 640", NumberOfSeats = 16 });
        }
    }
}
