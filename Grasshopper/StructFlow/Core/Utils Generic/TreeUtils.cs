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
        public class TreeUtils
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

            public static DataTree<Curve> ListToTree(List<List<Curve>> listCurves)
            {
                DataTree<Curve> treeCurve = new DataTree<Curve>();

                for (int i = 0; i < listCurves.Count; i++)
                {
                    GH_Path pth = new GH_Path(i);

                    for (int j = 0; j < listCurves[i].Count; j++)
                    {
                        treeCurve.Add(listCurves[i][j], pth);
                    }
                }
                return treeCurve;
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

            public static DataTree<Polyline> ListToTree(List<List<Polyline>> list)
            {
                DataTree<Polyline> tree = new DataTree<Polyline>();

                for (int i = 0; i < list.Count; i++)
                {
                    GH_Path pth = new GH_Path(i);

                    for (int j = 0; j < list[i].Count; j++)
                    {
                        tree.Add(list[i][j], pth);
                    }
                }
                return tree;
            }

            public static DataTree<StructFlow.Grasshopper.TextGoo> ListToTree(List<List<StructFlow.Grasshopper.TextGoo>> list)
            {
                DataTree<StructFlow.Grasshopper.TextGoo> tree = new DataTree<StructFlow.Grasshopper.TextGoo>();

                for (int i = 0; i < list.Count; i++)
                {
                    GH_Path pth = new GH_Path(i);

                    for (int j = 0; j < list[i].Count; j++)
                    {
                        tree.Add(list[i][j], pth);
                    }
                }
                return tree;
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

        public static DataTree<int> ListToTree(List<List<int>> valueLists)
        {
            DataTree<int> treeValues = new DataTree<int>();

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

        public static DataTree<string> ListToTree(List<List<string>> valueLists)
        {
            DataTree<string> treeValues = new DataTree<string>();

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
