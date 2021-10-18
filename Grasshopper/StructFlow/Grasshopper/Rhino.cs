using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using StructFlow;
using Rhino;
using Rhino.DocObjects;
using Rhino.Collections;
using GH_IO;
using GH_IO.Serialization;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace StructFlow.Rhino3d
{
    public class RhinoUtils
    {
        public static int RhinoModelUnit()
        {
            //mm = 2 //m = 4
            RhinoDoc active = Rhino.RhinoDoc.ActiveDoc;
            return (int)active.ModelUnitSystem;
        }

    }
}
