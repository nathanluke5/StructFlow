using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using StructFlow.Utils;
using Grasshopper.Kernel.Parameters;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;

namespace StructFlow.Components
{
    #region Member Selection

    public class SFMemberSplit : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public SFMemberSplit()
          : base("Member Type Split", "MemSplit",
              "takes the web elements of a truss and splits them into seperate geometry outputs based on the number of elements in each group",
              "StructFlow", "4.0 Utilities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Members", "M", "List of Curves", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Members Per Segment", "NS", "No of members in each segment. Left overs will be grouped in there own segment", GH_ParamAccess.list);
            pManager.AddBooleanParameter("One Way", "OW", "False - Simply Supported / True - Cantilever Type", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component. //This has variable aswell. 
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(String.Empty, String.Empty, "List index one", GH_ParamAccess.list);
            pManager.AddGenericParameter(String.Empty, String.Empty, "List index one", GH_ParamAccess.list);
            pManager.AddGenericParameter(String.Empty, String.Empty, "List index one", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // First, we need to retrieve all data from the input parameters.
            // We'll start by declaring variables and assigning them starting values.

            List<Curve> curves = new List<Curve>();
            List<int> numSegs = new List<int>();
            bool oneWay = false;

            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            if (!DA.GetDataList(0, curves)) return;
            if (!DA.GetDataList(1, numSegs)) return;
            if (!DA.GetData(2, ref oneWay)) return;

            // We should now validate the data and warn the user if invalid data is supplied.
            if (curves == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input Curves");
                return;
            }
            if (numSegs == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Define Segment Lists");
                return;
            }

            //***********Need to check if segments is greater than the limit of members**************

            //Define output lists. Generally create seperate methods in the core and output to these parametes.

            Dictionary<int, List<int>> temp = ListUtils.MemberSegmentationInt(curves, numSegs, oneWay);

            foreach (KeyValuePair<int, List<int>> item in temp)
            {
                if (item.Key > Params.Output.Count)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Number of Lists Exceeds Outputs. Add Output Parameters or Right click and Match Outputs");
                    return;
                }
                else
                    DA.SetDataList((item.Key - 1), item.Value);
            }
            VariableParameterMaintenance();
            Params.OnParametersChanged();
            this.ExpireSolution(true);
        }

        // ADD MENU ITEMS
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);

            Menu_AppendItem(menu, "Match Outputs", Menu_MyCustomItemClicked);
        }

        private void Menu_MyCustomItemClicked(Object sender, EventArgs e)
        {
            //NEED TO UPDATE THIS
            int outputs = 4;
            MatchParameters(outputs);
            VariableParameterMaintenance();
            Params.OnParametersChanged();
            this.ExpireSolution(true);
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            //Input parameters cannot be upated. Output can be Updated
            if (side == GH_ParameterSide.Input)
                return false;
            if (index == 0)
                return false;
            return true;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            //Input parameters cannot be upated. Output can be removed
            if (side == GH_ParameterSide.Input)
                return false;
            if ((Params.Output.Count <= 3))
                return false;
            else
                return true;
        }


        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {

            Param_GenericObject output = new Param_GenericObject();
            Params.RegisterOutputParam(output, index);
            //param.Name = GH_ComponentParamServer.InventUniqueNickname("ABCDEFGHIJKLMNOPQRSTUVWXYZ", Params.Input);
            output.NickName = string.Empty;
            //param.Description = "Param" + (Params.Input.Count + 1);
            return output;
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            Params.UnregisterOutputParameter(Params.Output[index]);
            return true;
        }

        public void VariableParameterMaintenance()
        {
            for (Int32 i = 0; i <= Params.Output.Count - 1; i++)
            {
                int outputnumber = i + 1;
                IGH_Param param = Params.Output[i];
                if ((param.NickName == string.Empty))
                    param.NickName = "{" + outputnumber.ToString() + "}";

                param.Name = string.Format("Values {0}", outputnumber);
                param.Description = "Grouped Indexs";
                param.Access = GH_ParamAccess.list;
            }
        }

        private void MatchParameters(int outputs)
        {
            if (Params.Output.Count >= outputs)
                return;
            do
            {
                if (Params.Output.Count >= outputs)
                    break;
                if (Params.Output.Count < outputs)
                    Params.RegisterOutputParam(new Param_GenericObject() { NickName = string.Empty, Name = string.Empty });
                else
                    Params.UnregisterOutputParameter(Params.Output[Params.Output.Count - 1]);
            }
            while (true);
            VariableParameterMaintenance();

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
            get { return new Guid("2d0e212d-b4b3-4162-9a69-4993bd0896ee"); }
        }
    }
    #endregion

    #region Sorting and Filtering
    public class ClosestCurve : GH_Component
    {
        public ClosestCurve() : base("Closest Curve", "ClosestCrv", "Gets the closest (or furtherest) curve in a list of curves to a reference point", "StructFlow", "4.0 Utilities")
        { }
        public override GH_Exposure Exposure { get { return GH_Exposure.primary; } }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "List of curves to test for flip operation", GH_ParamAccess.list);
            pManager.AddPointParameter("Point", "Pt", "Point of Curve geometry to use as basis for flip operation", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Furtherest", "F", "Choose whether to get closest or furtherest from pt", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Param", "P", "Param on curves items to generate test point", GH_ParamAccess.item, 0.5);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Closest of furtherest curve in list", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Index", "i", "Index of closest or furthest point in list", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> inputCrvs = new List<Curve>();
            Point3d pt = new Point3d();
            bool furtherest = false;
            double param = 0.5;
            int index = 0;

            if (!DA.GetDataList(0, inputCrvs)) return;
            if (!DA.GetData(1, ref pt)) return;
            if (!DA.GetData(2, ref furtherest)) return;
            if (DA.GetData(3, ref param))
            {
                if (param > 1.0 || param < 0)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Parameter to be between 0 and 1.0");
                }
            }
            Curve result = null;
            if (pt != null)
            {
                result = StructFlow.Utils.CurveUtils.ClosestCurve(inputCrvs, pt, furtherest, param, ref index);
            }
            else
            {
                result = inputCrvs[index];
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No point set");
            }
            DA.SetData(0, result);
            DA.SetData(1, index);
        }
        protected override System.Drawing.Bitmap Icon { get { return null; } }
        public override Guid ComponentGuid { get { return new Guid("{7EF788C0-ACD2-4733-A497-86B6D7186615}"); } }
    }

    public class GroupCurvesCommonPoint : GH_Component
    {
        public GroupCurvesCommonPoint() : base("Group Curves Common Point", "GroupCrvbyPt", "Gets the closest (or furtherest) curve in a list of curves to a reference point", "StructFlow", "4.0 Utilities")
        { }
        public override GH_Exposure Exposure { get { return GH_Exposure.primary; } }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "List of curves to test for common point", GH_ParamAccess.list);
            pManager.AddPointParameter("Point", "Pt", "Points to use as guide for group sorting. Optional", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "T", "Tolerance to use for common node. If nothing supplied Rhino Doc tolerance will be used", GH_ParamAccess.item);
            pManager[1].Optional = pManager[2].Optional = true;
            //Potential to add sorting vector
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Curves grouped by common point", GH_ParamAccess.tree);
            pManager.AddPointParameter("Points", "P", "Index of closest or furthest point in list", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> inputCrvs = new List<Curve>();
            List<Point3d> ptlist = new List<Point3d>();
            double tol = Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;

            if (!DA.GetDataList(0, inputCrvs)) return;
            DA.GetDataList(1, ptlist);
            DA.GetData(2, ref tol);


            List<List<Curve>> result = new List<List<Curve>>();
            List<Point3d> ptresult = null;
            List<List<int>> crvIndexs = new List<List<int>>();

            if (ptlist.Count == 0)
            {
                result = StructFlow.Utils.CurveUtils.GroupCurvesByCommonPoint(inputCrvs, tol, out ptresult, out crvIndexs);
            }
            else
            {
                //Finish
                result = StructFlow.Utils.CurveUtils.GroupCurvesByCommonPoint(inputCrvs, ptlist, tol, out ptresult);
            }
            DA.SetDataTree(0, Utils.TreeUtils.ListToTree(result));
            DA.SetDataList(1, ptresult);
        }
        protected override System.Drawing.Bitmap Icon { get { return null; } }
        public override Guid ComponentGuid { get { return new Guid("{2F726679-EEE8-45CD-AF0E-4A85CAB68457}"); } }
    }

    public class AlignCurves : GH_Component
    {
        public AlignCurves() : base("Align Curves", "AlignCrvs", "Aligns (Flip where required) a list of curves based on a common point or a guidance curve. " +
            "If using a guidance curve it will find the closest point on the guidance curve to perform the operation", "StructFlow", "4.0 Utilities") { }
        public override GH_Exposure Exposure { get { return GH_Exposure.primary; } }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "List of curves to test for flip operation", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Geometry", "G", "Point of Curve geometry to use as basis for flip operation", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Opposite", "O", "Choose whether to have Z in same or opposite direction to guide vector", GH_ParamAccess.item, false);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Output Curves", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Flipped", "F", "true is plane filled false if not", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve inputCrv = null;
            Curve refcrv = null;
            Point3d refpt = new Point3d();
            IGH_Goo geom = null;

            bool opp = false;
            bool flipped;

            if (!DA.GetData(0, ref inputCrv)) return;
            if (!DA.GetData(1, ref geom))
            {
                return;
            }
            else
            {
                if (GH_Convert.ToPoint3d(geom, ref refpt, GH_Conversion.Both))
                {
                }
                else if (GH_Convert.ToCurve(geom, ref refcrv, GH_Conversion.Both))
                {
                    double tp = 0.5;
                    if (refcrv.ClosestPoint(inputCrv.PointAtNormalizedLength(0.5), out tp))
                    {
                        refpt = refcrv.PointAt(tp);
                    }
                }
                else
                    base.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Provide input Point3d or Curve");
            }
            if (!DA.GetData(2, ref opp)) return;

            flipped = Utils.CurveUtils.AlignCurves(ref inputCrv, refpt, opp);

            DA.SetData(0, inputCrv);
            DA.SetData(1, flipped);

        }
        protected override System.Drawing.Bitmap Icon { get { return null; } }
        public override Guid ComponentGuid { get { return new Guid("{99734921-CA33-4719-93BD-7EB5236BB7D7}"); } }
    }

    public class AlignPlaneZ : GH_Component
    {
        public AlignPlaneZ() : base("Align Planes Z", "AlignZ", "Performs an align operation by Fliping the Z of a plane (without changing the X or Y vectors) " +
            "based on a direction vector from a point or curve input", "StructFlow", "4.0 Utilities")
        { }
        public override GH_Exposure Exposure { get { return GH_Exposure.primary; } }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane to test for flip operation", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Geometry", "G", "Point of Curve geometry to use as basis for flip operation", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Opposite", "O", "Choose whether to have Z in same or opposite direction to guide vector", GH_ParamAccess.item, false);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Planes", "P", "Output planes", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Flipped", "F", "true is plane filled false if not", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Plane inputPln = new Plane();

            Curve refcrv = null;
            Point3d refpt = new Point3d();
            IGH_Goo geom = null;

            bool opp = false;
            bool flipped;

            if (!DA.GetData(0, ref inputPln)) return;
            if (!DA.GetData(1, ref geom))
            {
                return;
            }
            else
            {
                if (GH_Convert.ToPoint3d(geom, ref refpt, GH_Conversion.Both))
                {
                }
                else if (GH_Convert.ToCurve(geom, ref refcrv, GH_Conversion.Both))
                {
                    double tp = 0.5;
                    if (refcrv.ClosestPoint(inputPln.Origin, out tp))
                    {
                        refpt = refcrv.PointAt(tp);
                    }
                }
                else
                    base.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Provide input Point3d or Curve");
            }    
            if (!DA.GetData(2, ref opp)) return;

            flipped = Utils.PlaneUtils.AlignPlaneZ(ref inputPln, refpt, opp);
            
            DA.SetData(0, inputPln);
            DA.SetData(1, flipped);
        }
        protected override System.Drawing.Bitmap Icon { get { return null; } }
        public override Guid ComponentGuid { get { return new Guid("{8D3A493E-DD4C-4213-B790-2771E5F5EE56}"); } }
    }



    #endregion

    #region Geometry Utilities

    public class ClosestPoints : GH_Component
    {
        public ClosestPoints() : base("Closest Points", "ClosePts", "Finds the closest points together from two lists of Points", "StructFlow", "4.0 Utilities") { }
        public override GH_Exposure Exposure { get { return GH_Exposure.primary; } }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points One", "P1", "First List of points", GH_ParamAccess.list);
            pManager.AddPointParameter("Points Two", "P2", "Second List of points", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Furtherest", "F", "Choose whether to find closest or furtherest", GH_ParamAccess.item, false);
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Point One", "P1", "Point in first list of points", GH_ParamAccess.item);
            pManager.AddPointParameter("Point Two", "P2", "Point in second list of points", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Index One", "i1", "Index of Point in first list of points", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Index Two", "i2", "Index of Point in second list of points", GH_ParamAccess.item);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<Point3d> ptsone = new List<Point3d>();
            List<Point3d> ptstwo = new List<Point3d>();
            bool further = false;

            Point3d ptone = new Point3d();
            Point3d pttwo = new Point3d();
            int ione = 0;
            int itwo = 0;


            if (!DA.GetDataList(0, ptsone)) return;
            if (!DA.GetDataList(1, ptstwo)) return;
            if (!DA.GetData(2, ref further)) return;

            Utils.PointUtils.ClosetPoints(ptsone, ptstwo, further, out ptone, out pttwo, out ione, out itwo);

            DA.SetData(0, ptone);
            DA.SetData(1, pttwo);
            DA.SetData(2, ione);
            DA.SetData(3, itwo);
        }
        protected override System.Drawing.Bitmap Icon { get { return null; } }
        public override Guid ComponentGuid { get { return new Guid("{544A99F8-44C9-4FC3-8ED8-F7ADBF053088}"); } }
    }


    #endregion


}