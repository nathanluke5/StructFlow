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
    public class PlaneUtils
    {
        /// <summary>
        /// Compares plane Z-direction with a guide point
        /// </summary>
        /// <returns></returns>
        public static bool AlignPlaneZ(ref Plane plane, Point3d refpoint, bool opposite)
        {
            Point3d origin = plane.Origin;
            Vector3d testvector = origin - refpoint;

            double test = testvector * plane.ZAxis;

            if (test < 0 && !opposite)
            {
                Vector3d revY = plane.YAxis;
                revY.Reverse();
                plane = new Plane(origin, plane.XAxis, revY);
                return true;
            }
            else if (test > 0 && opposite)
            {
                Vector3d revY = plane.YAxis;
                revY.Reverse();
                plane = new Plane(origin, plane.XAxis, revY);
                return true;
            }
            else
                return false;
        }
    }
}
