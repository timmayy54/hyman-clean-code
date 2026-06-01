using System;
using System.Text;

namespace FlightBooking.Core
{
    public class FlightSummaryReportFormatter
    {
        private static readonly string VerticalWhiteSpace = Environment.NewLine + Environment.NewLine;
        private static readonly string NewLine = Environment.NewLine;
        private const string Indentation = "    ";

        public string Format(FlightSummary summary)
        {
            var result = new StringBuilder();

            result.Append("Flight summary for ");
            result.Append(summary.FlightRoute.Title);
            result.Append(VerticalWhiteSpace);

            result.Append("Total passengers: ");
            result.Append(summary.SeatsTaken);
            result.Append(NewLine);
            result.Append(Indentation);
            result.Append("General sales: ");
            result.Append(summary.GeneralPassengerCount);
            result.Append(NewLine);
            result.Append(Indentation);
            result.Append("Loyalty member sales: ");
            result.Append(summary.LoyaltyMemberPassengerCount);
            result.Append(NewLine);
            result.Append(Indentation);
            result.Append("Airline employee comps: ");
            result.Append(summary.AirlineEmployeePassengerCount);
            result.Append(NewLine);
            result.Append(Indentation);
            result.Append("Discounted sales: ");
            result.Append(summary.DiscountedPassengerCount);

            result.Append(VerticalWhiteSpace);
            result.Append("Total expected baggage: ");
            result.Append(summary.TotalExpectedBaggage);

            result.Append(VerticalWhiteSpace);

            result.Append("Total revenue from flight: ");
            result.Append(summary.Revenue);
            result.Append(NewLine);
            result.Append("Total costs from flight: ");
            result.Append(summary.Costs);
            result.Append(NewLine);

            result.Append(summary.ProfitSurplus > 0 ? "Flight generating profit of: " : "Flight losing money of: ");
            result.Append(summary.ProfitSurplus);

            result.Append(VerticalWhiteSpace);

            result.Append("Total loyalty points given away: ");
            result.Append(summary.TotalLoyaltyPointsAccrued);
            result.Append(NewLine);
            result.Append("Total loyalty points redeemed: ");
            result.Append(summary.TotalLoyaltyPointsRedeemed);
            result.Append(NewLine);

            result.Append(VerticalWhiteSpace);

            result.Append(summary.CanProceed ? "THIS FLIGHT MAY PROCEED" : "FLIGHT MAY NOT PROCEED");

            if (!summary.CanProceed && summary.AlternativeAircraft.Count > 0)
            {
                result.Append(NewLine);
                result.Append("Other more suitable aircraft are:");

                foreach (var aircraft in summary.AlternativeAircraft)
                {
                    result.Append(NewLine);
                    result.Append(aircraft.Name);
                    result.Append(" could handle this flight.");
                }
            }

            return result.ToString();
        }
    }
}
