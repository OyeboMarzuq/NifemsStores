using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NifemsStores.Application.Helper
{
    public class Calculator
    {
        public static decimal CalculateMarkupAmount(decimal costPrice, decimal markupPercentage)
        {
            if (markupPercentage < 0)
                throw new ArgumentException("Markup percentage cannot be negative");

            return (costPrice * markupPercentage) / 100;
        }

        public static decimal CalculatePackSellingPrice(decimal costPrice, decimal markupPercentage)
        {
            var markup = CalculateMarkupAmount(costPrice, markupPercentage);
            var sellingPrice = costPrice + markup;

            return RoundToNearestHundred(sellingPrice);
        }

        public static decimal CalculateUnitSellingPrice(decimal packSellingPrice, int totalUnitsInPack)
        {
            if (totalUnitsInPack <= 0)
                throw new ArgumentException("Total units must be greater than zero");

            var unitPrice = packSellingPrice / totalUnitsInPack;

            return RoundToNearestTen(unitPrice);
        }

        public static decimal CalculatePercentage(decimal total, decimal obtained)
        {
            if (total <= 0)
                throw new ArgumentException("Total must be greater than zero");

            return Math.Round((obtained / total) * 100, 2);
        }

        public static decimal RoundToNearestHundred(decimal number)
        {
            return Math.Round(number / 100, MidpointRounding.AwayFromZero) * 100;
        }

        public static decimal RoundToNearestTen(decimal number)
        {
            decimal remainder = number % 10;
            return (remainder >= 5) ? number - remainder + 10 : number - remainder;
        }
    }

}