using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace StructFlow.Truss
{
    class Webs
    {
        public static List<Line> Pratt(List<Point3d> PCPts, List<Point3d> SCPts, bool oneWay, int numSeg)
        {
            List<Line> WebMems = new List<Line>();

            bool isEven;
            isEven = (numSeg % 2 == 0) ? true : false;
            int endPt = PCPts.Count - 1;
            int mid = (int)System.Math.Ceiling((double)PCPts.Count / 2) - 1;

            if (oneWay == false)
            {
                if (isEven == true)
                {
                    for (int i = 0; i <= endPt; i++)
                    {
                        if (i < mid)
                        {
                            Line templine = new Line(SCPts[i], PCPts[i + 1]);
                            WebMems.Add(templine);
                        }
                        if (i >= mid && i < endPt)
                        {
                            Line templine = new Line(PCPts[i], SCPts[i + 1]);
                            WebMems.Add(templine);
                        }
                    }
                }
                else if (isEven == false)
                {
                    for (int i = 0; i <= endPt; i++)
                    {
                        if (i < mid + 1)
                        {
                            Line templine = new Line(SCPts[i], PCPts[i + 1]);
                            WebMems.Add(templine);
                        }
                        if (i >= mid && i < endPt)
                        {
                            Line templine = new Line(PCPts[i], SCPts[i + 1]);
                            WebMems.Add(templine);
                        }
                    }
                }
            }
            else if (oneWay == true)
            {
                for (int i = 0; i <= endPt; i++)
                {
                    if (i < endPt)
                    {
                        Line templine = new Line(SCPts[i], PCPts[i + 1]);
                        WebMems.Add(templine);
                    }
                }
            }
            return WebMems;
        }

        public static List<Line> Howe(List<Point3d> PCPts, List<Point3d> SCPts, bool oneWay, int numSeg)
        {
            List<Line> WebMems = new List<Line>();

            //check if amount of segments are even
            bool isEven;
            isEven = (numSeg % 2 == 0) ? true : false;
            int endPt = PCPts.Count - 1;
            int mid = (int)System.Math.Ceiling((double)PCPts.Count / 2) - 1;

            if (oneWay == false)
            {
                if (isEven == true)
                {
                    for (int i = 0; i <= endPt; i++)
                    {
                        if (i < mid)
                        {
                            Line templine = new Line(PCPts[i], SCPts[i + 1]);
                            WebMems.Add(templine);
                        }
                        if (i >= mid && i < (endPt))
                        {
                            Line templine = new Line(SCPts[i], PCPts[i + 1]);
                            WebMems.Add(templine);
                        }
                    }
                }
                else if (isEven == false)
                {
                    for (int i = 0; i <= endPt; i++)
                    {
                        if (i < mid + 1)
                        {
                            Line templine = new Line(PCPts[i], SCPts[i + 1]);
                            WebMems.Add(templine);
                        }
                        if (i >= mid && i < endPt)
                        {
                            Line templine = new Line(SCPts[i], PCPts[i + 1]);
                            WebMems.Add(templine);
                        }
                    }
                }
            }
            else if (oneWay == true)
            {
                for (int i = 0; i <= endPt; i++)
                {
                    if (i < endPt)
                    {
                        Line templine = new Line(PCPts[i], SCPts[i + 1]);
                        WebMems.Add(templine);
                    }
                }
            }
            return WebMems;
        }

        public static List<Line> Warren(List<Point3d> PCPts, List<Point3d> SCPts, bool oneWay, int numSeg)
        {
            List<Line> WebMems = new List<Line>();

            //check if amount of segments are even
            bool isEven;
            isEven = (numSeg % 2 == 0) ? true : false;
            int endPt = PCPts.Count - 1;
            int mid = (int)System.Math.Ceiling((double)PCPts.Count / 2) - 1;

            for (int i = 0; i < endPt; i++)
            {
                if (i == 0 || i % 2 == 0)
                {
                    Line templine = new Line(PCPts[i], SCPts[i + 1]);
                    WebMems.Add(templine);
                }
                else
                {
                    Line templine = new Line(SCPts[i], PCPts[i + 1]);
                    WebMems.Add(templine);
                }
            }
            return WebMems;
        }

        public static List<Line> WarrenFlipped(List<Point3d> PCPts, List<Point3d> SCPts, bool oneWay, int numSeg)
        {
            List<Line> WebMems = new List<Line>();
            //check if amount of segments are even
            bool isEven;
            isEven = (numSeg % 2 == 0) ? true : false;
            int endPt = PCPts.Count - 1;
            int mid = (int)System.Math.Ceiling((double)PCPts.Count / 2) - 1;

            for (int i = 0; i < endPt; i++)
            {
                if (i == 0 || i % 2 == 0)
                {
                    Line templine = new Line(SCPts[i], PCPts[i + 1]);
                    WebMems.Add(templine);
                }
                else
                {
                    Line templine = new Line(PCPts[i], SCPts[i + 1]);
                    WebMems.Add(templine);
                }
            }
            return WebMems;
        }

        public static List<Line> Xtruss(List<Point3d> PCPts, List<Point3d> SCPts, bool oneWay, int numSeg)
        {
            List<Line> WebMems = new List<Line>();

            //check if amount of segments are even
            bool isEven;
            isEven = (numSeg % 2 == 0) ? true : false;
            int endPt = PCPts.Count - 1;
            int mid = (int)System.Math.Ceiling((double)PCPts.Count / 2) - 1;

            for (int i = 0; i < endPt; i++)
            {
                Line templineone = new Line(SCPts[i], PCPts[i + 1]);
                WebMems.Add(templineone);
                Line templinetwo = new Line(PCPts[i], SCPts[i + 1]);
                WebMems.Add(templinetwo);
            }
            return WebMems;
        }

        public static List<Line> Ktruss(List<Point3d> PCPts, List<Point3d> SCPts, bool oneWay, int numSeg, ref List<Line> vertMems)
        {
            List<Line> WebMems = new List<Line>();

            //check if amount of segments are even
            bool isEven;
            isEven = (numSeg % 2 == 0) ? true : false;
            int endPt = PCPts.Count - 1;
            int mid = (int)System.Math.Ceiling((double)PCPts.Count / 2) - 1;

            List<Point3d> MidVertPts = new List<Point3d>();
            foreach (Line line in vertMems)
            {
                MidVertPts.Add(line.PointAt(0.5));
            }

            //create list for divided vertical segments - not sure if this is best way to do it
            List<Line> analVert = new List<Line>();

            if (oneWay == false)
            {
                //works for is even true and false
                if (isEven == false)
                {
                    //Print("hello"); - cannot get this to work
                }
                for (int i = 0; i <= endPt; i++)
                {
                    if (i < mid)
                    {
                        if (i == 0)
                        {
                            analVert.Add(new Line(PCPts[i], SCPts[i]));
                            WebMems.Add(new Line(PCPts[i], MidVertPts[i + 1]));
                            WebMems.Add(new Line(SCPts[i], MidVertPts[i + 1]));
                        }
                        else
                        {
                            analVert.Add(new Line(PCPts[i], MidVertPts[i]));
                            analVert.Add(new Line(MidVertPts[i], SCPts[i]));
                            WebMems.Add(new Line(PCPts[i], MidVertPts[i + 1]));
                            WebMems.Add(new Line(SCPts[i], MidVertPts[i + 1]));
                        }
                    }
                    if (i >= mid && i < (endPt + 1))
                    {
                        if (i < endPt)
                        {
                            analVert.Add(new Line(PCPts[i], MidVertPts[i]));
                            analVert.Add(new Line(MidVertPts[i], SCPts[i]));
                            WebMems.Add(new Line(PCPts[i + 1], MidVertPts[i]));
                            WebMems.Add(new Line(SCPts[i + 1], MidVertPts[i]));
                        }
                        else
                        {
                            analVert.Add(new Line(PCPts[i], SCPts[i]));
                        }
                    }
                }
            }
            if (oneWay == true)
            {
                for (int i = 0; i <= endPt; i++)
                {
                    if (i == 0)
                    {
                        analVert.Add(new Line(PCPts[i], SCPts[i]));
                        WebMems.Add(new Line(PCPts[i], MidVertPts[i + 1]));
                        WebMems.Add(new Line(SCPts[i], MidVertPts[i + 1]));
                    }
                    else if (i > 0 && i < endPt)
                    {
                        analVert.Add(new Line(PCPts[i], MidVertPts[i]));
                        analVert.Add(new Line(MidVertPts[i], SCPts[i]));
                        WebMems.Add(new Line(PCPts[i], MidVertPts[i + 1]));
                        WebMems.Add(new Line(SCPts[i], MidVertPts[i + 1]));
                    }
                    else
                    {
                        analVert.Add(new Line(PCPts[i], SCPts[i]));
                    }
                }
            }
            return WebMems;
        }

        public static List<Line> KtrussFlipped(List<Point3d> PCPts, List<Point3d> SCPts, bool oneWay, int numSeg, ref List<Line> vertMems)
        {
            List<Line> WebMems = new List<Line>();

            bool isEven;
            isEven = (numSeg % 2 == 0) ? true : false;
            int endPt = PCPts.Count - 1;
            int mid = (int)System.Math.Ceiling((double)PCPts.Count / 2) - 1;
            List<Point3d> MidVertPts = new List<Point3d>();
            foreach (Line line in vertMems)
            {
                MidVertPts.Add(line.PointAt(0.5));
            }

            //create list for divided vertical segments - not sure if this is best way to do it
            List<Line> analVert = new List<Line>();

            if (oneWay == false)
            {
                //works for is even true and false
                if (isEven == false)
                {
                    //Print("hello"); - cannot get this to work
                }

                for (int i = 0; i <= endPt; i++)
                {
                    if (i <= mid)
                    {
                        if (i < mid)
                        {
                            analVert.Add(new Line(PCPts[i], MidVertPts[i]));
                            analVert.Add(new Line(MidVertPts[i], SCPts[i]));
                            WebMems.Add(new Line(MidVertPts[i], PCPts[i + 1]));
                            WebMems.Add(new Line(MidVertPts[i], SCPts[i + 1]));
                        }
                        else
                        {
                            analVert.Add(new Line(PCPts[i], SCPts[i]));
                        }
                    }
                    if (i >= mid && i < (endPt + 1))
                    {
                        if (i < endPt)
                        {
                            analVert.Add(new Line(PCPts[i], MidVertPts[i]));
                            analVert.Add(new Line(MidVertPts[i], SCPts[i]));
                            WebMems.Add(new Line(PCPts[i], MidVertPts[i + 1]));
                            WebMems.Add(new Line(SCPts[i], MidVertPts[i + 1]));
                        }
                        else
                        {
                            analVert.Add(new Line(PCPts[i], SCPts[i]));
                        }
                    }
                }
            }
            if (oneWay == true)
            {
                for (int i = 0; i <= endPt; i++)
                {
                    if (i < endPt)
                    {
                        analVert.Add(new Line(PCPts[i], MidVertPts[i]));
                        analVert.Add(new Line(MidVertPts[i], SCPts[i]));
                        WebMems.Add(new Line(MidVertPts[i], PCPts[i + 1]));
                        WebMems.Add(new Line(MidVertPts[i], SCPts[i + 1]));
                    }
                    else
                    {
                        analVert.Add(new Line(PCPts[i], SCPts[i]));
                    }
                }
            }
            return WebMems;
        }

    }
}
