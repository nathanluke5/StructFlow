using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using StructFlow.Core;

namespace StructFlow.Components
{
    public class SFLinesFromPoints : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public SFLinesFromPoints()
          : base("SF Lines from Points", "SFLinesFromPts",
              "Creates a list of Lines from a set of points",
              "StructFlow", "Model Utilities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "List of Points", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Lines", "L", "Output List of Lines", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<Point3d> points = new List<Point3d>();

            if (!DA.GetDataList(0, points)) return;

            List<Line> lines = new List<Line>(ModelUtilities.PointsToLines(points));

            DA.SetDataList(0, lines);
            

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("c702ad87-142e-470a-b635-0cb2b39437a5"); }
        }
    }
}