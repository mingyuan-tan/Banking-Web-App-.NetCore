using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WDT_Assignment2.Utilities
{
    public static class Utilities
    {
        public static bool HasMoreThanNDecimalPlaces(this decimal value, int n) => decimal.Round(value, n) != value;
        public static bool HasMoreThanTwoDecimalPlaces(this decimal value) => value.HasMoreThanNDecimalPlaces(2);
    }
}
