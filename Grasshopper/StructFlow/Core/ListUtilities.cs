using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;


namespace StructFlow.Core
{
    class ListUtilities
    {
        public static List<List<Point3d>> FlipList2D(List<List<Point3d>> listPoints)
        {
            List<List<Point3d>> flippedList = new List<List<Point3d>>(); //values in example
                                                                         //new columns is number of points in first list as assume 2d array
            for (int i = 0; i < listPoints[0].Count; i++)
            {
                List<Point3d> newlistrow = new List<Point3d>();
                foreach (List<Point3d> pointlist in listPoints)
                {
                    newlistrow.Add(pointlist[i]);
                }
                flippedList.Add(newlistrow);
            }
            return flippedList;
        }

        public static Dictionary<int, List<int>> MemberSegmentationInt(List<Curve> Members, List<int> numSegs, bool oneWay)
        {

            Dictionary<int, List<int>> indexDic = new Dictionary<int, List<int>>();

            List<int> titleRange = Enumerable.Range(1, numSegs.Count + 1).ToList();

            List<int> listTitles = new List<int>();

            foreach (int num in titleRange)
            {
                listTitles.Add(num);
            }

            int numMems = Members.Count;

            List<int> memRange = Enumerable.Range(0, numMems - 1).ToList();

            List<List<int>> indexList = new List<List<int>>();

            int a = 0;
            int d = numMems;

            for (int i = 0; i < titleRange.Count; i++)
            {
                if (i < numSegs.Count)
                {
                    List<int> e = new List<int>();
                    List<int> f = new List<int>();
                    if (i != 0)
                    {
                        a = a + numSegs[i - 1];
                        d = d - numSegs[i - 1];
                    }
                    e = Enumerable.Range(a, numSegs[i]).ToList();
                    indexList.Add(e);
                    if (oneWay == false) //If two way
                    {
                        f = Enumerable.Range(d, numSegs[i]).ToList();
                        indexList.Add(f);
                        indexDic[listTitles[i]] = e.Concat(f).ToList();
                    }
                    else
                        indexDic[listTitles[i]] = e;
                }
                else
                {
                    //get all items not populated in indexlist
                    //create this into a method
                    List<int> indexTake = new List<int>();
                    foreach (List<int> List in indexList)
                    {
                        foreach (int z in List)
                            indexTake.Add(z);
                    }
                    indexDic[listTitles[i]] = memRange.Except(indexTake).ToList();
                }
            }
            return indexDic;
        }

        public static Dictionary<string, List<int>> MemberSegmentation(List<Curve> Members, int numSegs, int memSegs, bool oneWay)
        {
            //need to output the curves not the indexs.

            //Create Dictionary Keys
            Dictionary<string, List<int>> indexDic = new Dictionary<string, List<int>>();
            List<string> listTitles = new List<string>();
            List<int> titleRange = Enumerable.Range(1, numSegs + 1).ToList();

            foreach (int num in titleRange)
            {
                listTitles.Add("Mem0" + Convert.ToString(num));
            }

            int numMems = Members.Count;
            List<int> memRange = Enumerable.Range(0, numMems - 1).ToList();

            List<List<int>> indexList = new List<List<int>>();

            for (int i = 0; i < titleRange.Count; i++)
            {
                if (i < numSegs)
                {
                    int a;
                    int d;
                    List<int> e = new List<int>();
                    List<int> f = new List<int>();

                    if (i == 0)
                        a = 0;
                    else
                        a = (i * memSegs);

                    d = numMems - ((i + 1) * memSegs);

                    e = Enumerable.Range(a, memSegs).ToList();
                    indexList.Add(e);

                    if (oneWay == false)
                    {
                        f = Enumerable.Range(d, memSegs).ToList();
                        indexList.Add(f);
                        indexDic[listTitles[i]] = e.Concat(f).ToList();
                    }
                    else
                        indexDic[listTitles[i]] = e;
                }
                else
                {
                    //get all items not populated in indexlist
                    //create this into a method
                    List<int> indexTake = new List<int>();
                    foreach (List<int> List in indexList)
                    {
                        foreach (int z in List)
                            indexTake.Add(z);
                    }
                    indexDic[listTitles[i]] = memRange.Except(indexTake).ToList();
                }
            }
            return indexDic;
        }
    }
}
