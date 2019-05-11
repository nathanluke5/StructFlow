using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using StructFlow.Core;

namespace StructFlow.Components
{
    public class SFGrid : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public SFGrid()
          : base("SF Grid", "SFGrid",
              "Create Primary and Secondary grids from a list of lines",
              "StructFlow", "Model")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            // Use the pManager object to register your input parameters.
            // You can often supply default values when creating parameters.
            // All parameters must have the correct access type. If you want 
            // to import lists or trees of values, modify the ParamAccess flag.
            pManager.AddCurveParameter("Primary", "P", "Primary Grid Curves", GH_ParamAccess.list);
            pManager.AddCurveParameter("Primary Int", "P1", "Intermediate Primary Grid Curves", GH_ParamAccess.list);
            pManager.AddCurveParameter("Secondary", "S", "Secondary Grid Curves", GH_ParamAccess.list);
            pManager.AddCurveParameter("Secondary Int", "S1", "Intermediate Secondary Grid Curves", GH_ParamAccess.list);

            // If you want to change properties of certain parameters, 
            // you can use the pManager instance to access them by index:
            //pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[3].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            // Use the pManager object to register your output parameters.
            // Output parameters do not have default values, but they too must have the correct access type.

            pManager.AddTextParameter("Info", "IN", "Provides Component Output Information", GH_ParamAccess.item);
            pManager.AddPointParameter("Grid Points", "GP", "Grid Points", GH_ParamAccess.tree);
            pManager.AddPointParameter("Int Grid Points", "GP1", "Intermediate Grid Points", GH_ParamAccess.tree);
            pManager.AddVectorParameter("Primary Vectors", "PV", "Unitized Vector of Primary Curves", GH_ParamAccess.list);
            pManager.AddVectorParameter("Secondary Vectors", "SV", "Unitized Vector of Secondary Curves", GH_ParamAccess.list);
            pManager.AddNumberParameter("Primary Grid Distances", "DP", "Distance between Primary Grid Points", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Secondary Grid Distances", "DS", "Distance between Secondary Grid Points", GH_ParamAccess.tree);

            // Sometimes you want to hide a specific parameter from the Rhino preview.
            // You can use the HideParameter() method as a quick way:
            //pManager.HideParameter(0);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // First, we need to retrieve all data from the input parameters.
            // We'll start by declaring variables and assigning them starting values.

            //Plane plane = Plane.WorldXY;
            //double radius0 = 0.0;
            //double radius1 = 0.0;
            //int turns = 0;

            List<Curve> Primary = new List<Curve>();
            List<Curve> IntPrimary = new List<Curve>();
            List<Curve> Secondary = new List<Curve>();
            List<Curve> IntSecondary = new List<Curve>();

            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetDataList(0, Primary)) return;
            //if (!DA.GetDataList(1, IntPrimary)) return;
            if (!DA.GetDataList(2, Secondary)) return;
            //if (!DA.GetDataList(3, IntSecondary)) return;

            // We should now validate the data and warn the user if invalid data is supplied.
            if (Primary == null || Secondary == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input Primary and Secondary Curves");
                return;
            }

            // We're set to create the spiral now. To keep the size of the SolveInstance() method small, 
            // The actual functionality will be in a different method:
            //Curve spiral = CreateSpiral(plane, radius0, radius1, turns);

            List<List<Double>> distPrimInt = new List<List<Double>>();
            List<List<Double>> distSecInt = new List<List<Double>>();

            List<Vector3d> primVecs = new List<Vector3d>(Grid.GridUnitVectors(Primary));
            List<Vector3d> secVecs = new List<Vector3d>(Grid.GridUnitVectors(Secondary));

            DataTree<Point3d> gridPoints = new DataTree<Point3d>(TreeUtilities.ListToTree(Intersection.CurveCurve(Primary, Secondary, out distPrimInt, out distSecInt)));

            DataTree<Double> distPrimIntTree = new DataTree<Double>(TreeUtilities.ListToTree(distPrimInt));
            DataTree<Double> distSecIntTree = new DataTree<Double>(TreeUtilities.ListToTree(distSecInt));

            string Information = "To be implemented";

            // Finally assign the spiral to the output parameter.

            DA.SetData(0, Information);
            DA.SetDataTree(1, gridPoints);
            DA.SetDataTree(2, gridPoints);
            DA.SetDataList(3, primVecs);
            DA.SetDataList(4, secVecs);
            DA.SetDataTree(5, distPrimIntTree);
            DA.SetDataTree(6, distSecIntTree);
        }

        /// <summary>
        /// The Exposure property controls where in the panel a component icon 
        /// will appear. There are seven possible locations (primary to septenary), 
        /// each of which can be combined with the GH_Exposure.obscure flag, which 
        /// ensures the component will only be visible on panel dropdowns.
        /// </summary>
        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2e867d7b-4061-4b65-b2ff-7cb28447d5f8"); }
        }
    }
}
