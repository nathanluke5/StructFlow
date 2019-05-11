using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;
using StructFlow.Core;

namespace StructFlow.Truss
{
    class ChordDefinition
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
