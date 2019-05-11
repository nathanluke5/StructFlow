using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace StructFlow.Core
{
    class ModelUtilities
    {
        public static List<Line> PointsToLines(List<Point3d> points)
        {
            List<Line> lines = new List<Line>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                Line temp = new Line(points[i], points[i + 1]);
                lines.Add(temp);
            }
            return lines;
        }


    }
}
