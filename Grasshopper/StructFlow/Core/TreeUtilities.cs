using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;
using Grasshopper.Kernel.Data;
using Grasshopper;

namespace StructFlow.Core
{
    class TreeUtilities
    {
        public static List<List<Point3d>> TreeToList(DataTree<Point3d> Points)
        {
            List<List<Point3d>> outPoints = new List<List<Point3d>>();

            for (int i = 0; i < Points.BranchCount; i++)
            {
                List<Point3d> temppoints = new List<Point3d>();

                for (int j = 0; j < Points.Branch(i).Count; j++)
                {
                    temppoints.Add(Points.Branch(i)[j]);
                }
                outPoints.Add(temppoints);
            }
            return outPoints;
        }

        public static DataTree<Point3d> ListToTree(List<List<Point3d>> listPoints)
        {
            DataTree<Point3d> treePoint = new DataTree<Point3d>();

            for (int i = 0; i < listPoints.Count; i++)
            {
                GH_Path pth = new GH_Path(i);

                for (int j = 0; j < listPoints[i].Count; j++)
                {
                    treePoint.Add(listPoints[i][j], pth);
                }
            }
            return treePoint;
        }

        public static DataTree<double> ListToTree(List<List<double>> valueLists)
        {
            DataTree<double> treeValues = new DataTree<double>();

            for (int i = 0; i < valueLists.Count; i++)
            {
                GH_Path pth = new GH_Path(i);

                for (int j = 0; j < valueLists[i].Count; j++)
                {
                    treeValues.Add(valueLists[i][j], pth);
                }
            }
            return treeValues;
        }
    }
}
