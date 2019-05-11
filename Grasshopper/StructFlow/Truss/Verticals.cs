using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace StructFlow.Truss
{
    class Verticals
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
}
