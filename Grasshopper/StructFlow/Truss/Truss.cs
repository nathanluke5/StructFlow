using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace StructFlow.Truss
{
    public enum TrussType
    {
        Pratt,
        Howe,
        Warren,
        WarrenwVerts,
        WarrenFlipped,
        WarrenFlippedwVerts,
        Xtruss,
        Ktruss,
        KtrussFlipped
    }
}
