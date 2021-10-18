using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructFlow.Utils
{
    public class NumberUtils
    {
        //Thanksto
        //https://stackoverflow.com/questions/2826262/round-a-decimal-to-the-nearest-quarter-in-c-sharp
        //amountToRound => input amount
        //nearestOf => .25 if round to quater, 0.01 for rounding to 1 cent, 1 for rounding to $1
        //fairness => btween 0 to 0.9999999___.
        //            0 means floor and 0.99999... means ceiling. But for ceiling, I would recommend, Math.Ceiling
        //            0.5 = Standard Rounding function. It will round up the border case. i.e. 1.5 to 2 and not 1.
        //            0.4999999... non-standard rounding function. Where border case is rounded down. i.e. 1.5 to 1 and not 2.
        //            0.75 means first 75% values will be rounded down, rest 25% value will be rounded up.
        public static decimal RoundToFactor(decimal amountToRound, decimal nearstOf, decimal fairness)
        {
            return Math.Floor(amountToRound / nearstOf + fairness) * nearstOf;
        }

        /// <summary>
        /// does linear interpolation
        /// </summary>
        /// <param name="x"></param>
        /// <param name="x0"></param>
        /// <param name="x1"></param>
        /// <param name="y0"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        public static double InterpolationLinear(double x, double x0, double x1, double y0, double y1)
        {
            double y = 0.0;
            y = y0 + (x - x0) * ((y1 - y0) / (x1 - x0));
            return y;
        }

    }
}
