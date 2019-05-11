using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Grasshopper.Kernel;
using Grasshopper;
using System.Drawing;
using Rhino.Geometry;
using Grasshopper.Kernel.Parameters;
using System.Windows.Forms;
using StructFlow.Core;


namespace StructFlow.Components
{
    public class SFMemberSplit : GH_Component, IGH_VariableParameterComponent
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public SFMemberSplit()
          : base("SF Member Type Split", "SFMemSplit",
              "takes the web elements of a truss and splits them into seperate geometry outputs based on the number of elements in each group",
              "StructFlow", "Model Utilities")
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

            Dictionary<int, List<int>> temp = ListUtilities.MemberSegmentationInt(curves, numSegs, oneWay);

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
                    Params.RegisterOutputParam(new Param_GenericObject() { NickName = string.Empty, Name = string.Empty}) ;
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
}