using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using StructFlow;

namespace StructFlow.Components
{
    public class SFLengthsWithinCurve : GH_Component
    {
        public SFLengthsWithinCurve()
            : base("Lengths Within Curve", "LengthWCrv",
                "Optimise lengths within a curve based on a set of input points inwhich joints in curves need to be a minimum distance away from. " +
                  "The component will run untill the first solution is found.",
                "StructFlow", "6.0 Optimise")
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to evaluate", GH_ParamAccess.item);
            pManager.AddNumberParameter("Std Length", "SL", "Desired standard length of curve", GH_ParamAccess.item);
            pManager.AddNumberParameter("Min Length", "mL", "Minimum length of curve", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Length", "ML", "Maximum length of curve", GH_ParamAccess.item);
            pManager.AddPointParameter("Points", "P", "Points to avoid when creating divisions in curves", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "T", "Tolerance required for each point", GH_ParamAccess.list); //originally I was going to specify one tolerance but now each point can have its own tolerance. Need to add condition if only one provided use for all. This means I can add multiple sets of points. Create a dictionary of points and tolerance values.
            pManager.AddNumberParameter("Move Increment", "I", "The increment move value for each iteration. Lowever the move increment for more complex tolerances. Decreasing the move increment will increase runtime", GH_ParamAccess.item);  
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddPointParameter("Points", "P", "The outputed division points", GH_ParamAccess.list);
            //pManager.AddCurveParameter("Curves", "C", "The outputed dividion curves", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Solution", "S", "True/False on whether a solution was found for the given curve", GH_ParamAccess.item);
            pManager.AddTextParameter("Info", "I", "Provides the optimisation information for a given curve", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Curve crv = null;
            double stdL = 0.0, minL = 0.0, maxL = 0.0;
            List<Point3d> pts = new List<Point3d>();
            List<double> tols = new List<double>();
            double moveincr = 0.0;


            List<Point3d> outPts = new List<Point3d>();
            bool solution = false;
            string info = "";

            if (!DA.GetData(0, ref crv)) return;
            if (!DA.GetData(1, ref stdL)) return;
            if (!DA.GetData(2, ref minL)) return;
            if (!DA.GetData(3, ref maxL)) return;
            if (!DA.GetDataList(4, pts)) return;
            if (!DA.GetDataList(5, tols)) return;
            if (!DA.GetData(6, ref moveincr)) return;

            if (crv != null)
            {
                outPts = StructFlow.Optimise.CurveOptimise.LengthsWithinCurve(crv, stdL, minL, maxL, pts, tols, moveincr, out solution, out info);
            }   

            DA.SetDataList(0, outPts);
            DA.SetData(1, solution);
            DA.SetData(2, info);
        }

        public override GH_Exposure Exposure
        {
            get { return GH_Exposure.primary; }
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("{10425E3B-9B47-4DD9-839B-A7D6AA6F929C}"); }
        }
    }
}
