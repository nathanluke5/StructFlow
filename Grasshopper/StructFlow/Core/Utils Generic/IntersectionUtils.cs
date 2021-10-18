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
    class IntersectionUtils
    {
        public static List<List<Point3d>> CurveCurve(List<Curve> primCurves, List<Curve> secCurves, out List<List<double>> primDistances, out List<List<double>> secDistances)
        {
            //Curve/Curve Intersections
            const double intersection_tolerance = 0.001;  //update to rhino doc tolerance
            const double overlap_tolerance = 0.0;         //update to rhino doc tolerance

            List<List<Point3d>> nestedPoints = new List<List<Point3d>>();

            foreach (Curve curvei in primCurves)
            {
                List<Point3d> points = new List<Point3d>();
                foreach (Curve curvej in secCurves)
                {
                    var tempevent = Rhino.Geometry.Intersect.Intersection.CurveCurve(curvei, curvej, intersection_tolerance, overlap_tolerance);
                    points.Add(tempevent[0].PointA);
                }
                nestedPoints.Add(points);
            }

            //Primary Distances
            primDistances = Grid.PointDistances(nestedPoints);

            //Secondary Distances
            secDistances = Grid.PointDistances(ListUtils.FlipList2D(nestedPoints));

            return nestedPoints;
        }
    }
}
