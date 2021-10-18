using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;
using Rhino.Geometry;

namespace StructFlow.Optimise
{
    public class CurveOptimise
    {
        public class PointWithTolerance
        {
            public Point3d point;
            public double tolerance;

            public PointWithTolerance(Point3d pt, double tol)
            {
                point = pt;
                tolerance = tol;
            }

            /// <summary>
            /// Compares two different lists of points and checks whether any points in one list are within tolerance of one in the other
            /// </summary>
            /// <returns></returns>
            public static bool PtClashCheckOkay(List<Point3d> baselistPt, List<PointWithTolerance> checklistPts)
            {
                foreach (PointWithTolerance ipt in checklistPts)
                {
                    int test = Rhino.Collections.Point3dList.ClosestIndexInList(baselistPt, ipt.point);
                    if (ipt.point.DistanceTo(baselistPt[test]) < ipt.tolerance)
                    {
                        return false;
                    }
                }
                return true;
            }


        }

        /// <summary>
        /// This method will optimise the amount of standard length units you can get along a curve without division points being hit wihtin a certain tolerance.
        /// </summary>
        public static List<Point3d> LengthsWithinCurve(Curve crv, double stdLength, double minLength, double maxLength, List<Point3d> points, List<double> tols, double moveIncrement, out bool isSolution, out string info)
        {

            List<PointWithTolerance> PtTolList = new List<PointWithTolerance>();
            int count = 0;
            foreach (Point3d pt in points)
            {

                PtTolList.Add(new PointWithTolerance(pt, tols[count]));
                count++;
            }

            List<Point3d> curvepts = new List<Point3d>();  //Split position list
            isSolution = false;
            info = "";

            double crvLength = crv.GetLength();
            int maxStdLengths = (int)(crvLength / stdLength);
            double variation = crvLength - (maxStdLengths * stdLength);


            //Define number of standard lengths and special one and two length
            int stdLengths = maxStdLengths;
            double specLengthOne = variation;
            double specLengthTwo = 0.0;
            double specLengthThree = 0.0;
            double specLengthFour = 0.0;

            int numSpecials = 1;

            if (crvLength <= maxLength)
            {
                numSpecials = 0;
                isSolution = true;
                info += "Input Length smaller than Max Length - No Joints required";
                return curvepts;
            }

            if (numSpecials > 0 && variation < minLength)
            {
                numSpecials = 2;
                stdLengths = stdLengths - 1;
                variation = variation + stdLength;
                info += "2 Specials Required as Minimum\r\n";
            }

            if (numSpecials == 1)
            {
                List<double> distList = Enumerable.Repeat(stdLength, stdLengths).ToList();

                for (int i = 0; i <= distList.Count; i++)
                {
                    List<double> tempList = new List<double>(distList);
                    tempList.Insert(i, specLengthOne);

                    List<Point3d> ptsoncurve = Utils.CurveUtils.PointsfromValues(crv, tempList);

                    if (!Utils.PointUtils.PtClashCheckOkay(points, ptsoncurve, tols))
                        continue;
                    else
                    {
                        isSolution = true;
                        info += "Single Special Solution Found!";
                        return ptsoncurve;
                    }
                }
                if (isSolution == false)
                {
                    numSpecials = 2;
                    info += "No Single Special Solution Found\r\n";
                }
            }

            if (numSpecials == 2)
            {

                double specLengthOneStart;
                double specLengthTwoStart;
                int iterations = 1000;
                List<double> distList = new List<double>();

                if (crvLength <= numSpecials * maxLength) //Checks to see whether a 2 length special should be forced 
                {
                    variation = crvLength;

                    //Set Start lengths for iterations. START AT MAX LENGTH AND REDUCE FROM THERE SO I DONT HAVE TO CHANGE DIRECTIONS
                    if (maxLength > variation + minLength)
                    {
                        specLengthOne = variation - minLength;
                    }
                    else if ((variation - minLength) >= maxLength)
                    {
                        specLengthOne = maxLength;
                    }
                    else
                        specLengthOne = variation - minLength;

                    specLengthTwo = variation - specLengthOne;

                    specLengthOneStart = specLengthOne;
                    specLengthTwoStart = specLengthTwo;

                    for (int z = 1; z < iterations; z++)
                    {
                        specLengthOne = specLengthOne - moveIncrement;
                        specLengthTwo = specLengthTwo + moveIncrement;

                        if (specLengthOne <= specLengthTwoStart)
                        {
                            break;
                        }

                        if (CurveOptimise.TwoSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthTwo, ref curvepts))
                        {
                            info += "2 Special Solution Found! \r\n";
                            info += specLengthOne + "," + specLengthTwo;
                            isSolution = true;
                            return curvepts;
                        }
                    }
                }

                //calc special lengths - Can only be one option
                if (variation <= (minLength * 2))
                {
                    stdLengths = maxStdLengths - 1;
                    variation = stdLength + variation;
                    specLengthOne = specLengthTwo = variation / 2;
                }
                else
                {
                    specLengthOne = specLengthTwo = variation / 2;
                }

                distList = Enumerable.Repeat(stdLength, stdLengths).ToList();

                //Two equal length specials.
                if (CurveOptimise.TwoSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthTwo, ref curvepts))
                {
                    info += "2 equal length Solution Found";
                    isSolution = true;
                    return curvepts;
                }

                //Set Start lengths for iterations. START AT MAX LENGTH AND REDUCE FROM THERE SO I DONT HAVE TO CHANGE DIRECTIONS
                if (maxLength > variation + minLength)
                {
                    specLengthOne = variation - minLength;
                }
                else if ((variation - minLength) >= maxLength)
                {
                    specLengthOne = maxLength;
                }
                else
                    specLengthOne = variation - minLength;

                specLengthTwo = variation - specLengthOne;

                specLengthOneStart = specLengthOne;
                specLengthTwoStart = specLengthTwo;

                for (int z = 1; z < iterations; z++)
                {
                    specLengthOne = specLengthOne - moveIncrement;
                    specLengthTwo = specLengthTwo + moveIncrement;

                    if (specLengthOne <= specLengthTwoStart)
                    {
                        break;
                    }

                    if (CurveOptimise.TwoSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthTwo, ref curvepts))
                    {
                        info += "2 Special Solution Found! \r\n";
                        info += specLengthOne + "," + specLengthTwo;
                        isSolution = true;
                        return curvepts;
                    }
                }
                if (!isSolution)
                {
                    info += "No 2 Special Solution Found! \r\n";
                    numSpecials = numSpecials + 1; //3
                }
            }

            if (numSpecials == 3)
            {
                //Option 1
                if (variation < (minLength * numSpecials))
                {
                    stdLengths = maxStdLengths - 1;
                    variation = stdLength + variation;
                    specLengthOne = specLengthTwo = specLengthThree = variation / numSpecials;
                }
                else
                {
                    specLengthOne = specLengthTwo = specLengthThree = variation / numSpecials;
                }

                //Define start values
                double variationStart = variation;
                double stdLengthsStart = stdLength;

                List<double> distList = Enumerable.Repeat(stdLength, stdLengths).ToList();

                if (CurveOptimise.ThreeSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthTwo, specLengthThree, ref curvepts))
                {
                    info += "3 equal length Solution Found";
                    isSolution = true;
                    return curvepts;
                }

                //Set Spec Length one at max ---> provide method which will set spec length two and three. First set special length two to special length one. 
                //Set Start Lengths for Iterations
                if (maxLength > variation + minLength * 2)
                {
                    specLengthOne = variation - minLength * 2;
                }
                else if ((variation - minLength * 2) >= maxLength)   //This should be hit most of the time here
                {
                    specLengthOne = maxLength;
                }
                else
                    specLengthOne = variation - minLength * 2;

                double leftOver = 0.0;
                double specLengthOneStart = specLengthOne;
                double leftOverStart = variation - specLengthOneStart;

                int iterations = 2000;
                //Peform iteration where specLength one is also equal to spec length two first. 

                for (int c = 0; c < iterations; c++)
                {
                    if (c == 0)
                        specLengthOne = specLengthOne;
                    else
                        specLengthOne = specLengthOne - moveIncrement;

                    leftOver = variation - specLengthOne;

                    if (leftOver > 2 * maxLength)
                        break;
                    if (leftOver <= 2 * minLength || specLengthOne <= leftOverStart)
                        break;
                    if (specLengthOne <= minLength)
                        break;

                    if (leftOver >= specLengthOne && (leftOver - specLengthOne) > minLength)
                    {
                        specLengthTwo = specLengthOne;
                        specLengthThree = leftOver - specLengthTwo;
                        if (CurveOptimise.ThreeSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthOne, specLengthThree, ref curvepts))
                        {
                            info += "3 Special length Solution Found - 2 Similar";
                            info += specLengthOne + "," + specLengthTwo + "," + specLengthThree;
                            isSolution = true;
                            return curvepts;
                        }
                    }
                }
                if (!isSolution)
                    info += "NO 3 Special length (2 Similar) Solution Found Opt1\r\n";

                specLengthOne = specLengthOneStart;
                leftOver = 0;

                //Perform iteration where spec length two and spec length three are continually varying
                for (int d = 0; d < iterations; d++)
                {
                    if (d == 0)
                        specLengthOne = specLengthOne;
                    else
                        specLengthOne = specLengthOne - moveIncrement;

                    leftOver = variation - specLengthOne;

                    if (leftOver > 2 * maxLength)
                        break;
                    if (leftOver <= 2 * minLength || specLengthOne <= leftOverStart)
                        break;
                    if (specLengthOne <= minLength)
                        break;

                    List<double> specLengthsTwo = new List<double>();
                    List<double> specLengthsThree = new List<double>();

                    CurveOptimise.TwoSpecialSetLengthsVary(variation, specLengthOne, maxLength, minLength, moveIncrement, out specLengthsTwo, out specLengthsThree);

                    for (int i = 0; i < specLengthsTwo.Count; i++)
                    {
                        if (CurveOptimise.ThreeSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthsTwo[i], specLengthsThree[i], ref curvepts))
                        {
                            info += "3 Special length Solution Found";
                            info += specLengthOne + "," + specLengthsTwo[i] + "," + specLengthsThree[i];
                            isSolution = true;
                            return curvepts;
                        }
                    }
                }

                //Checking the second option
                bool optiontwo = false;
                if ((variation + stdLength) / numSpecials < maxLength)
                {
                    optiontwo = true;
                    variation = variation + stdLength;
                    stdLengths = stdLengths - 1;
                }

                if (optiontwo)
                {
                    specLengthOne = specLengthTwo = specLengthThree = variation / numSpecials;

                    distList = Enumerable.Repeat(stdLength, stdLengths).ToList();

                    if (CurveOptimise.ThreeSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthTwo, specLengthThree, ref curvepts))
                    {
                        info += "3 equal length Solution Found";
                        isSolution = true;
                        return curvepts;
                    }

                    //Set Start Lengths for Iterations
                    //Set Spec Length one at max ---> provide method which will set spec length two and three. First set special length two to special length one. 
                    if (maxLength > variation + minLength * 2)
                    {
                        specLengthOne = variation - minLength * 2;
                    }
                    else if ((variation - minLength * 2) >= maxLength)   //This should be hit most of the time here
                    {
                        specLengthOne = maxLength;
                    }
                    else
                        specLengthOne = variation - minLength * 2;

                    leftOver = 0.0;
                    specLengthOneStart = specLengthOne;
                    leftOverStart = variation - specLengthOneStart;

                    for (int c = 0; c < iterations; c++)
                    {
                        if (c == 0)
                            specLengthOne = specLengthOne;
                        else
                            specLengthOne = specLengthOne - moveIncrement;

                        leftOver = variation - specLengthOne;

                        if (leftOver > 2 * maxLength)
                            break;
                        if (leftOver <= 2 * minLength || specLengthOne <= leftOverStart)
                            break;
                        if (specLengthOne <= minLength)
                            break;

                        if (leftOver >= specLengthOne && (leftOver - specLengthOne) > minLength)
                        {
                            specLengthTwo = specLengthOne;
                            specLengthThree = leftOver - specLengthTwo;
                            if (CurveOptimise.ThreeSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthOne, specLengthThree, ref curvepts))
                            {
                                info += "3 Special length Solution Found - 2 Similar";
                                info += specLengthOne + "," + specLengthTwo + "," + specLengthThree;
                                isSolution = true;
                                return curvepts;
                            }
                        }
                    }
                    if (!isSolution)
                        info += "NO3 Special length (2 Similar) Solution Found Opt1\r\n";

                    specLengthOne = specLengthOneStart;
                    leftOver = leftOverStart;

                    for (int d = 0; d < iterations; d++)
                    {
                        if (d == 0)
                            specLengthOne = specLengthOne;
                        else
                            specLengthOne = specLengthOne - moveIncrement;

                        leftOver = variation - specLengthOne;

                        if (leftOver >= 2 * maxLength)
                            break;
                        if (leftOver <= 2 * minLength || specLengthOne <= leftOverStart)
                            break;
                        if (specLengthOne <= minLength)
                            break;

                        List<double> specLengthsTwo = new List<double>();
                        List<double> specLengthsThree = new List<double>();

                        CurveOptimise.TwoSpecialSetLengthsVary(variation, specLengthOne, maxLength, minLength, moveIncrement, out specLengthsTwo, out specLengthsThree);

                        for (int i = 0; i < specLengthsTwo.Count; i++)
                        {
                            if (CurveOptimise.ThreeSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthsTwo[i], specLengthsThree[i], ref curvepts))
                            {
                                info += "3 Special length Solution Found";
                                info += specLengthOne + "," + specLengthsTwo[i] + "," + specLengthsThree[i];
                                isSolution = true;
                                return curvepts;
                            }
                        }
                    }
                }
                variation = variationStart;
                stdLength = stdLengthsStart;
                //numSpecials = 4;
            }

            if (numSpecials == 4)
            {

                //Set start Lengths for even optimisation
                if (variation < (minLength * numSpecials))
                {
                    stdLengths = maxStdLengths - 1;
                    variation = stdLength + variation;
                    specLengthOne = specLengthTwo = specLengthThree = specLengthFour = variation / numSpecials;
                }
                else
                {
                    specLengthOne = specLengthTwo = specLengthThree = specLengthFour = variation / numSpecials;
                }

                List<double> distList = Enumerable.Repeat(stdLength, stdLengths).ToList();

                if (CurveOptimise.FourSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthTwo, specLengthThree, specLengthFour, ref curvepts))
                {
                    info += "4 equal length Solution Found";
                    isSolution = true;
                    return curvepts;
                }

                //Set start Lengths for uneven optimisation
                //Set Spec Length one at max ---> provide method which will set spec length two and three. First set special length two to special length one. 
                //Set Start Lengths for Iterations
                if (maxLength > variation + minLength * 3)
                {
                    specLengthOne = variation - minLength * 3;
                }
                else if ((variation - minLength * 3) >= maxLength)   //This should be hit most of the time here
                {
                    specLengthOne = maxLength;
                }
                else
                    specLengthOne = variation - minLength * 3;

                double leftOver = 0.0;
                double specLengthOneStart = specLengthOne;
                double leftOverStart = variation - specLengthOneStart;

                int iterations = 2000;

                for (int c = 0; c < iterations; c++)
                {
                    if (c == 0)
                        specLengthOne = specLengthOne;
                    else
                        specLengthOne = specLengthOne - moveIncrement;

                    leftOver = variation - specLengthOne;

                    if (leftOver > 3 * maxLength)
                        break;
                    if (leftOver <= 3 * minLength || specLengthOne <= leftOverStart)
                        break;
                    if (specLengthOne <= minLength)
                        break;

                    if (leftOver >= specLengthOne && (leftOver - specLengthOne) > minLength)
                    {
                        specLengthTwo = specLengthThree = specLengthOne;
                        specLengthFour = leftOver - (specLengthTwo+specLengthThree);
                        if (CurveOptimise.FourSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthOne, specLengthThree, specLengthFour, ref curvepts))
                        {
                            info += "4 Special length Solution Found - 3 Similar";
                            info += specLengthOne + "," + specLengthTwo + "," + specLengthThree;
                            isSolution = true;
                            return curvepts;
                        }
                    }
                }
                if (!isSolution)
                    info += "NO 4 Special length (3 Similar) Solution Found Opt1\r\n";

                //Optimisation where two specials are similar length
                specLengthOne = specLengthOneStart;
                leftOver = 0;
                leftOverStart = specLengthOne * 2;

                //Perform iteration where spec length two and spec length three are continually varying
                for (int d = 0; d < iterations; d++)
                {
                    if (d == 0)
                        specLengthOne = specLengthOne;
                    else
                        specLengthOne = specLengthOne - moveIncrement;

                    leftOver = variation - (specLengthOne + specLengthTwo);

                    specLengthThree = specLengthFour = leftOver / 2;

                    if (leftOver > 2 * maxLength)
                        break;
                    if (leftOver <= 2 * minLength || specLengthOne <= leftOverStart/2)
                        break;
                    if (specLengthOne <= minLength)
                        break;

                    //List<double> specLengthsTwo = new List<double>();
                    //List<double> specLengthsThree = new List<double>(); 

                    //CurveOptimise.TwoSpecialSetLengthsVary(variation, specLengthOne, maxLength, minLength, moveIncrement, out specLengthsTwo, out specLengthsThree);

                    if (CurveOptimise.FourSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthTwo, specLengthThree, specLengthFour, ref curvepts))
                    {
                        info += "3 Special length Solution Found";
                        info += specLengthOne + "," + specLengthTwo + "," + specLengthThree + "," + specLengthFour;
                        isSolution = true;
                        return curvepts;
                    }
                }


                //
                bool optiontwo = false;
                if ((variation + stdLength) / numSpecials < maxLength)
                {
                    optiontwo = true;
                    variation = variation + stdLength;
                    stdLengths = stdLengths - 1;
                }

                if (optiontwo)
                {
                    specLengthOne = specLengthTwo = specLengthThree = variation / numSpecials;

                    distList = Enumerable.Repeat(stdLength, stdLengths).ToList();

                    if (CurveOptimise.FourSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthTwo, specLengthThree, specLengthFour, ref curvepts))
                    {
                        info += "4 equal length Solution Found";
                        isSolution = true;
                        return curvepts;
                    }


                    //Set start Lengths for uneven optimisation
                    //Set Spec Length one at max ---> provide method which will set spec length two and three. First set special length two to special length one. 
                    //Set Start Lengths for Iterations
                    if (maxLength > variation + minLength * 3)
                    {
                        specLengthOne = variation - minLength * 3;
                    }
                    else if ((variation - minLength * 3) >= maxLength)   //This should be hit most of the time here
                    {
                        specLengthOne = maxLength;
                    }
                    else
                        specLengthOne = variation - minLength * 3;

                    leftOver = 0.0;
                    specLengthOneStart = specLengthOne;
                    leftOverStart = variation - specLengthOneStart;

                    iterations = 2000;

                    for (int c = 0; c < iterations; c++)
                    {
                        if (c == 0)
                            specLengthOne = specLengthOne;
                        else
                            specLengthOne = specLengthOne - moveIncrement;

                        leftOver = variation - specLengthOne;

                        if (leftOver > 3 * maxLength)
                            break;
                        if (leftOver <= 3 * minLength || specLengthOne <= leftOverStart)
                            break;
                        if (specLengthOne <= minLength)
                            break;

                        if (leftOver >= specLengthOne && (leftOver - specLengthOne) > minLength)
                        {
                            specLengthTwo = specLengthThree = specLengthOne;
                            specLengthFour = leftOver - (specLengthTwo + specLengthThree);
                            if (CurveOptimise.FourSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthOne, specLengthThree, specLengthFour, ref curvepts))
                            {
                                info += "4 Special length Solution Found - 3 Similar";
                                info += specLengthOne + "," + specLengthTwo + "," + specLengthThree;
                                isSolution = true;
                                return curvepts;
                            }
                        }
                    }
                    if (!isSolution)
                        info += "NO 4 Special length (3 Similar) Solution Found Opt1\r\n";

                    //Optimisation where two specials are similar length
                    specLengthOne = specLengthOneStart;
                    leftOver = 0;
                    leftOverStart = specLengthOne * 2;

                    //Perform iteration where spec length two and spec length three are continually varying
                    for (int d = 0; d < iterations; d++)
                    {
                        if (d == 0)
                            specLengthOne = specLengthOne;
                        else
                            specLengthOne = specLengthOne - moveIncrement;

                        leftOver = variation - (specLengthOne + specLengthTwo);

                        specLengthThree = specLengthFour = leftOver / 2;

                        if (leftOver > 2 * maxLength)
                            break;
                        if (leftOver <= 2 * minLength || specLengthOne <= leftOverStart / 2)
                            break;
                        if (specLengthOne <= minLength)
                            break;

                        //List<double> specLengthsTwo = new List<double>();
                        //List<double> specLengthsThree = new List<double>(); 

                        //CurveOptimise.TwoSpecialSetLengthsVary(variation, specLengthOne, maxLength, minLength, moveIncrement, out specLengthsTwo, out specLengthsThree);

                        if (CurveOptimise.FourSpecialOptimisation(crv, PtTolList, distList, specLengthOne, specLengthTwo, specLengthThree, specLengthFour, ref curvepts))
                        {
                            info += "3 Special length Solution Found";
                            info += specLengthOne + "," + specLengthTwo + "," + specLengthThree + "," + specLengthFour;
                            isSolution = true;
                            return curvepts;
                        }
                    }
                }
            }




            //Then check if two of the member can be the same


            //Then check if all of the members are different



            if (!isSolution)
                info += "***No solution found!!!***";
            return curvepts;
        }

        //maybe update to input a dictionary at some point
        private static void SetSpecialLengths(double variation, double minlength, double crvlength, ref int stdlengths, ref List<double> specone, ref List<double> spectwo)
        {
            return;

        }

        private static void SetSpecialLengths(double variation, double crvlength, ref int stdlengths, out int numOptions, ref List<double> specone, ref List<double> spectwo, ref List<double> specthree)
        {
            numOptions = 1;
            return;
        }

        private static void SetSpecialLengths(double variation, double crvlength, ref int stdlengths, out int numOptions, ref List<double> specone, ref List<double> spectwo, ref List<double> specthree, ref List<double> specfour)
        {
            numOptions = 1;
            return;
        }

        //****try to reduce to one method have an input array of special lengths potentially
        private static bool TwoSpecialOptimisation(Curve crv, List<PointWithTolerance> pointTols, List<double> distlist, double Lone, double Ltwo, ref List<Point3d> curvepts)
        {
            for (int i = 0; i <= distlist.Count; i++)
            {
                List<double> tempListone = new List<double>(distlist);
                tempListone.Insert(i, Lone);

                for (int j = 0; j <= tempListone.Count; j++)
                {
                    List<double> tempListtwo = new List<double>(tempListone);
                    tempListtwo.Insert(j, Ltwo);

                    List<Point3d> ptsoncurve = Utils.CurveUtils.PointsfromValues(crv, tempListtwo);

                    if (PointWithTolerance.PtClashCheckOkay(ptsoncurve, pointTols))
                    {
                        curvepts = ptsoncurve;
                        return true;
                    }
                }
            }
            return false;
        }
        private static bool ThreeSpecialOptimisation(Curve crv, List<PointWithTolerance> pointTols, List<double> distList, double Lone, double Ltwo, double Lthree, ref List<Point3d> curvepts)
        {
            for (int i = 0; i <= distList.Count; i++)
            {
                List<double> tempListone = new List<double>(distList);
                tempListone.Insert(i, Lone);

                for (int j = 0; j <= tempListone.Count; j++)
                {
                    List<double> tempListtwo = new List<double>(tempListone);
                    tempListtwo.Insert(j, Ltwo);

                    for (int z = 0; z <= tempListtwo.Count; z++)
                    {
                        List<double> tempListthree = new List<double>(tempListtwo);
                        tempListthree.Insert(z, Lthree);

                        List<Point3d> ptsoncurve = Utils.CurveUtils.PointsfromValues(crv, tempListthree);

                        if (PointWithTolerance.PtClashCheckOkay(ptsoncurve, pointTols))
                        {
                            curvepts = ptsoncurve;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        private static bool FourSpecialOptimisation(Curve crv, List<PointWithTolerance> pointTols, List<double> distList, double Lone, double Ltwo, double Lthree, double Lfour, ref List<Point3d> curvepts)
        {
            for (int i = 0; i <= distList.Count; i++)
            {
                List<double> tempListone = new List<double>(distList);
                tempListone.Insert(i, Lone);

                for (int j = 0; j <= tempListone.Count; j++)
                {
                    List<double> tempListtwo = new List<double>(tempListone);
                    tempListtwo.Insert(j, Ltwo);

                    for (int z = 0; z <= tempListone.Count; z++)
                    {
                        List<double> tempListthree = new List<double>(tempListtwo);
                        tempListtwo.Insert(z, Ltwo);

                        for (int y = 0; y <= tempListtwo.Count; y++)
                        {
                            List<double> tempListfour = new List<double>(tempListthree);
                            tempListfour.Insert(y, Lfour);

                            List<Point3d> ptsoncurve = Utils.CurveUtils.PointsfromValues(crv, tempListfour);

                            if (PointWithTolerance.PtClashCheckOkay(ptsoncurve, pointTols))
                            {
                                curvepts = ptsoncurve;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }


        private static void TwoSpecialSetLengthsVary(double variation, double specLone, double maxlength, double minlength, double increment, out List<double> Ltwo, out List<double> Lthree)
        {

            Ltwo = new List<double>();
            Lthree = new List<double>();

            double leftOver = variation - specLone;
            double lengthTwo = 0.0;
            double lengthThree = 0.0;

            //Set Ltwo to maxLength and iterate
            if (maxlength >= (leftOver + minlength))
            {
                lengthTwo = leftOver - minlength;
            }
            else if ((leftOver - minlength) >= maxlength)
            {
                lengthTwo = maxlength;
            }
            else
                lengthTwo = leftOver - minlength;

            lengthThree = leftOver - lengthTwo;

            double lengthTwoStart = lengthTwo;
            double lengthThreeStart = lengthThree;

            int iterations = 1500;

            for (int z = 1; z < iterations; z++)
            {
                lengthTwo = lengthTwo - increment;
                Ltwo.Add(lengthTwo);
                Lthree.Add(leftOver - lengthTwo);

                if (lengthTwo <= lengthThreeStart)
                {
                    return;
                }
            }

        }
    }
}
