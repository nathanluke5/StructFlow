using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;
using Grasshopper.Kernel.Data;
using Grasshopper;
using StructFlow.Model;
using StructFlow.Utils;

namespace StructFlow.Utils
{
    public class CurveUtils
    {
        /// <summary>
        /// Inputs a list of length points along a curve and outputs the point locations. 
        /// </summary>
        /// <returns></returns>
        /// 
        public static List<Point3d> PointsfromValues(Curve inputCurve, List<double> lengthlist)
        {
            double adder = 0;
            List<Point3d> points = new List<Point3d>();

            for (int i = 0; i < lengthlist.Count - 1; i++)
            {
                adder += lengthlist[i];
                points.Add(inputCurve.PointAtLength(adder));
            }
            return points;
        }

        public static Curve ClosestCurve(List<Curve> crvs, Point3d point, bool furtherest, double param, ref int index)
        {
            List<Point3d> curvePts = new List<Point3d>();
            List<double> distances = new List<double>();

            foreach (Curve curve in crvs)
            {
                Point3d pt = curve.PointAtNormalizedLength(param);
                curvePts.Add(pt);
                distances.Add(point.DistanceTo(pt));
            }

            double val;
            if (!furtherest)
                val = distances.Min();
            else
                val = distances.Max();

            index = distances.IndexOf(val);
            return crvs[index];
        }

        public static bool AlignCurves(ref Curve curve, Point3d refpoint, bool opposite)
        {
            Point3d end = curve.PointAtEnd;
            Point3d start = curve.PointAtStart;
            bool reversed = false;

            if (!opposite)
            {
                if (refpoint.DistanceTo(start) > refpoint.DistanceTo(end))
                    reversed = curve.Reverse();
            }
            else
            {
                if (refpoint.DistanceTo(start) < refpoint.DistanceTo(end))
                    reversed = curve.Reverse();
            }
            return reversed;
        }

        public static List<List<Curve>> GroupCurvesByCommonPoint(List<Curve> curves, double tol, out List<Point3d> outpoints, out List<List<int>> curveIndex)
        {
            List<Point3d> points = new List<Point3d>();
            

            foreach (Curve curve in curves)
            {
                points.Add(curve.PointAtStart);
                points.Add(curve.PointAtEnd);
            }

            Point3d[] culledpoints = new Point3d[] { };
            culledpoints = Point3d.CullDuplicates(points, tol);

            List<List<Curve>> groupedCurves = new List<List<Curve>>();
            List<List<int>> crvIndexs = new List<List<int>>();

            foreach (Point3d pt in culledpoints)
            {
                List<Curve> attachedcurves = new List<Curve>();
                List<int> attachedIndex = new List<int>();
                for (int i = 0; i < curves.Count; i++)
                {
                    Curve curve = (Curve)curves[i];
                    if (curve.PointAtStart.CompareTo(pt) == 0 || curve.PointAtEnd.CompareTo(pt) == 0)
                    {
                        attachedcurves.Add(curve);
                        attachedIndex.Add(i);
                    }
                }
                groupedCurves.Add(attachedcurves);
            }
            outpoints = culledpoints.ToList();
            curveIndex = crvIndexs;

            return groupedCurves;
        }

        public static List<List<Curve>> GroupCurvesByCommonPoint(List<Curve> curves, List<Point3d> guidePts, double tol, out List<Point3d> outpoints)
        {
            List<List<Curve>> groupedCurves = new List<List<Curve>>();
            List<Point3d> points = new List<Point3d>();

            outpoints = points;
            return groupedCurves;
        }
    }
}
