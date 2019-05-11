using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace StructFlow.Core
{
    class Grid
    {
        public static List<List<double>> PointDistances(List<List<Point3d>> pointsLists)
        {
            List<List<double>> distancesList = new List<List<double>>();

            for (int i = 0; i < pointsLists.Count(); i++)
            {
                List<double> distances = new List<double>();

                for (int j = 0; j < pointsLists[i].Count() - 1; j++)
                {
                    distances.Add(pointsLists[i][j].DistanceTo(pointsLists[i][j + 1]));
                }
                distancesList.Add(distances);
            }
            return distancesList;
        }

        public static List<Vector3d> GridUnitVectors(List<Curve> gridLines)
        {
            List<Vector3d> vectors = new List<Vector3d>();

            foreach (Curve crv in gridLines)
            {
                Vector3d start = new Vector3d(crv.PointAtStart);
                start.Unitize();
                Vector3d end = new Vector3d(crv.PointAtEnd);
                end.Unitize();
                vectors.Add((end - start));
            }
            return vectors;
        }
    }
}
