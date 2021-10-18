using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;
using StructFlow;

namespace StructFlow.Model.Truss2D
{
    public enum TrussType { Pratt, Howe, Warren, WarrenwVerts, WarrenFlipped, WarrenFlippedwVerts, Xtruss, Ktruss, KtrussFlipped}

    public class Webs
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

    public class Verticals
    {
        public static List<Line> AllVerts(List<Point3d> TCPts, List<Point3d> BCPts, TrussType trussType)
        {
            List<Line> VertMems = new List<Line>();

            if (trussType == TrussType.Howe
                || trussType == TrussType.Pratt
                || trussType == TrussType.WarrenFlippedwVerts
                || trussType == TrussType.WarrenwVerts
                || trussType == TrussType.Xtruss
                || trussType == TrussType.Ktruss
                || trussType == TrussType.KtrussFlipped)

                for (int i = 0; i <= TCPts.Count - 1; i++)
                {
                    Line Vert = new Line(BCPts[i], TCPts[i]);
                    VertMems.Add(Vert);
                }

            else if (trussType == TrussType.Warren
                || trussType == TrussType.WarrenFlipped)
            {
                for (int i = 0; i <= TCPts.Count - 1; i++)
                {
                    if (i == 0 || i == TCPts.Count - 1)
                    {
                        Line Vert = new Line(BCPts[i], TCPts[i]);
                        VertMems.Add(Vert);
                    }
                }
            }
            return VertMems;
        }
    }

    public class ChordDefinition
    {
        //Update this code now!! Progress further later to determine whether bends in chords.
        private static bool CheckChordStraightness(Curve curve)
        {
            //get absolute tolerance of rhino doc
            double tolerance = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            //Is curve straight?
            if (curve.IsLinear(tolerance) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<Curve> DefinePrimaryChord(Curve curveOne, Curve curveTwo)
        {
            //New Lists for top and bottom curves
            List<Curve> TCBC = new List<Curve>();
            if (CheckChordStraightness(curveOne) == true)
            {
                TCBC.Add(curveOne);
                TCBC.Add(curveTwo);
            }
            else if (CheckChordStraightness(curveOne) == false)
            {
                if (CheckChordStraightness(curveTwo) == true)
                {
                    TCBC.Add(curveTwo);
                    TCBC.Add(curveOne);
                }
            }
            return TCBC;
        }

        //Create Bottom and Top Chord Pts
        public static List<List<Point3d>> TrussPtsFromLines(Curve PCCurve, Curve SCCurve, int Segments, int ForceVert)
        {
            List<List<Point3d>> ChordPts = new List<List<Point3d>>();

            //Check if truss is planar



            //define Primary and Secondary cord points
            List<Point3d> PCPts = new List<Point3d>();
            List<Point3d> SCPts = new List<Point3d>();

            //Create PC Points
            Point3d[] PCPtArray = new Point3d[] { };
            var t = PCCurve.DivideByCount(Segments, true, out PCPtArray);
            PCPts = PCPtArray.ToList();

            //splits boths curves evenly and joins lines
            if (ForceVert == 0)
            {
                Point3d[] SCPtArray = new Point3d[] { };
                var breaks = SCCurve.DivideByCount(Segments, true, out SCPtArray);
                SCPts = SCPtArray.ToList();
            }

            //Creates points on SC curve based on verticals to a directions z,x,y defualt to y for now.
            else if (ForceVert == 1)
            {
                foreach (Point3d pt in PCPts)
                {
                    //create temp line for intersection. <<<This needs improvement>>>!!!
                    Point3d tempPT = pt;
                    tempPT.Y = pt.Y + 5;
                    Line tempLine = new Rhino.Geometry.Line(pt, tempPT);
                    //intersection event
                    var tempevent = Rhino.Geometry.Intersect.Intersection.CurveCurve(SCCurve, tempLine.ToNurbsCurve(), 0.001, 0.001);
                    SCPts.Add(tempevent[0].PointA);
                }
            }

            //splits PC curves evenly and produces verticals perpendicular to the line
            //else if(ForceVert == 2)
            //{}

            ChordPts.Add(PCPts);
            ChordPts.Add(SCPts);

            return ChordPts;
        }
    }

}
