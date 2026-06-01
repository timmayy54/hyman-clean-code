using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightBooking.Core
{
    public class FlightSummaryCalculator
    {
        public FlightSummary Calculate(
            FlightRoute flightRoute,
            Plane aircraft,
            IEnumerable<Passenger> passengers,
            IFlightProceedRuleSet ruleSet,
            IEnumerable<Plane> alternativeAircraft)
        {
            var passengerList = passengers.ToList();
            var summary = new FlightSummary(flightRoute, aircraft);

            foreach (var passenger in passengerList)
            {
                AddPassengerToSummary(summary, passenger);
            }

            summary.CanProceed = ruleSet.CanProceed(summary);

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

            return summary;
        }

        private static void AddPassengerToSummary(FlightSummary summary, Passenger passenger)
        {
            switch (passenger.Type)
            {
                case PassengerType.General:
                    summary.Revenue += summary.FlightRoute.BasePrice;
                    summary.TotalExpectedBaggage++;
                    summary.GeneralPassengerCount++;
                    break;
                case PassengerType.LoyaltyMember:
                    if (passenger.IsUsingLoyaltyPoints)
                    {
                        var loyaltyPointsRedeemed = Convert.ToInt32(Math.Ceiling(summary.FlightRoute.BasePrice));
                        passenger.LoyaltyPoints -= loyaltyPointsRedeemed;
                        summary.TotalLoyaltyPointsRedeemed += loyaltyPointsRedeemed;
                    }
                    else
                    {
                        summary.TotalLoyaltyPointsAccrued += summary.FlightRoute.LoyaltyPointsGained;
                        summary.Revenue += summary.FlightRoute.BasePrice;
                    }

                    summary.TotalExpectedBaggage += 2;
                    summary.LoyaltyMemberPassengerCount++;
                    break;
                case PassengerType.AirlineEmployee:
                    summary.TotalExpectedBaggage++;
                    summary.AirlineEmployeePassengerCount++;
                    break;
                case PassengerType.Discounted:
                    summary.Revenue += summary.FlightRoute.BasePrice / 2;
                    summary.DiscountedPassengerCount++;
                    break;
            }

            summary.Costs += summary.FlightRoute.BaseCost;
            summary.SeatsTaken++;
        }

        private static FlightSummary CopyForAircraft(FlightSummary source, Plane aircraft)
        {
            return new FlightSummary(source.FlightRoute, aircraft)
            {
                SeatsTaken = source.SeatsTaken,
                GeneralPassengerCount = source.GeneralPassengerCount,
                LoyaltyMemberPassengerCount = source.LoyaltyMemberPassengerCount,
                AirlineEmployeePassengerCount = source.AirlineEmployeePassengerCount,
                DiscountedPassengerCount = source.DiscountedPassengerCount,
                TotalExpectedBaggage = source.TotalExpectedBaggage,
                Revenue = source.Revenue,
                Costs = source.Costs,
                TotalLoyaltyPointsAccrued = source.TotalLoyaltyPointsAccrued,
                TotalLoyaltyPointsRedeemed = source.TotalLoyaltyPointsRedeemed
            };
        }
    }
}
