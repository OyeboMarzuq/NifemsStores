using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Application.Helper
{
    public class Helper
    {
        public static string GetGreetingMessage()
        {
            var currentHour = DateTime.Now.Hour;

            if (currentHour >= 5 && currentHour < 12)
            {
                return "Good Morning, Guess You've A Splendid Night";
            }
            else if (currentHour >= 12 && currentHour < 17)
            {
                return "Good Afternoon";
            }
            else if (currentHour >= 17 && currentHour < 21)
            {
                return "Good Evening";
            }
            else
            {
                return "Good Night";
            }
        }

        public static int GetCurrentYear()
        {
            return DateTime.Now.Year;
        }

        public decimal RoundToNearestTen(decimal number)
        {
            decimal remainder = number % 10;
            decimal value = (remainder >= 5) ? number - remainder + 10 : number - remainder;

            return value;
        }

        public decimal CalculateUnitSellingPrice(decimal costPrice, int totalUnit)
        {
            return costPrice / totalUnit;
        }

        public decimal RoundToNearestHundred(decimal number)
        {
            return Math.Round(number / 100, MidpointRounding.AwayFromZero) * 100;
        }

        public static string GetFormattedQuantity(int totalPieces, int itemsPerPack)
        {
            int packs = totalPieces / itemsPerPack;
            int pieces = totalPieces % itemsPerPack;
            return $"{packs} pack(s), {pieces} piece(s)";
        }
    }
}