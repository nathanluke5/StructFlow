using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;
using Grasshopper.Kernel.Data;
using Grasshopper;
using StructFlow.Model;

namespace StructFlow.Utils
{
    public class PointUtils
    {
        /// <summary>
        /// Compares two different lists of points and checks whether any points in one list are within tolerance of one in the other
        /// </summary>
        /// <returns></returns>
        public static bool PtClashCheckOkay(List<Point3d> baselistPt, List<Point3d> checklistPt, double tolerance)
        {
            foreach (Point3d ipt in checklistPt)
            {
                int test = Rhino.Collections.Point3dList.ClosestIndexInList(baselistPt, ipt);
                if (ipt.DistanceTo(baselistPt[test]) < tolerance)
                {
                    return false;
                }
            }
            return true;
        }

        //This and above are Supersceeded 
        public static bool PtClashCheckOkay(List<Point3d> baselistPt, List<Point3d> checklistPt, List<double> tolerance)
        {
            foreach (Point3d ipt in checklistPt)
            {
                int test = Rhino.Collections.Point3dList.ClosestIndexInList(baselistPt, ipt);
                if (ipt.DistanceTo(baselistPt[test]) < tolerance[test])
                {
                    return false;
                }
            }
            return true;
        }

        public static void ClosetPoints(List<Point3d> listOne, List<Point3d> listTwo, bool furtherest, out Point3d pointOne, out Point3d pointTwo, out int indexOne, out int indexTwo)
        {
            int iOne = 0;
            int iTwo = 0;
            double distance = 0.0;

            if (!furtherest)
                distance = Double.MaxValue;

            double dif = 0.0;
            int test = 0;
            for (int i = 0; i < listOne.Count; i++)
            {
                if (furtherest)
                {
                    test = FurtherestIndexInList(listTwo, listOne[i]); //Can I implement this as an extension method??
                    dif = listOne[i].DistanceTo(listTwo[test]);
                }
                else
                {
                    test = Rhino.Collections.Point3dList.ClosestIndexInList(listTwo, listOne[i]);
                    dif = listOne[i].DistanceTo(listTwo[test]);
                }
                if (!furtherest)
                {
                    if (distance > dif)
                    {
                        iOne = i;
                        iTwo = test;
                        distance = dif;
                    }
                }
                else
                {
                    if (distance < dif)
                    {
                        iOne = i;
                        iTwo = test;
                        distance = dif;
                    }
                }
            }
            pointOne = listOne[iOne];
            pointTwo = listTwo[iTwo];
            indexOne = iOne;
            indexTwo = iTwo;
        }

        public static int FurtherestIndexInList(List<Point3d> pointlist, Point3d testpoint)
        {
            int index = 0;
            double dist = 0.0;
            double testdist = 0.0;
            for (int i = 0; i < pointlist.Count; i++)
            {
                testdist = testpoint.DistanceTo(pointlist[i]);
                if (testdist > dist)
                {
                    dist = testdist;
                    index = i;
                }
            }
            return index;
        }

    }
}
