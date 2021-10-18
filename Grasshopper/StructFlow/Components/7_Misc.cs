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
    public class ZipFolder : GH_Component
    {
        public ZipFolder()
            : base("Zip Folder", "ZipFolder",
                "Create a Zip folder from a folder on your computer",
                "StructFlow", "7.0 Miscellaneous")
        {
        }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Enable", "E", "Enable the Zip Execution", GH_ParamAccess.item,false);
            pManager.AddTextParameter("Folder Path", "FP", "Path of folder to be Zipped", GH_ParamAccess.item);
            pManager.AddTextParameter("Zip Directory", "D", "Optional folder to place Zipped Folder. If not specified folder will be created in same directory as folder path", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("Success", "S", "Folder Zipped Successfully", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            bool go = false;
            string folderpath = "";
            string Success = "";

            string zippath = "";

            if (!DA.GetData(0,ref go)) return;
            if (!DA.GetData(1, ref folderpath)) return;
            DA.GetData(2, ref zippath);

            if(go)
            {
                Success = StructFlow.Misc.ZipTools.ZipFolder(folderpath, zippath).ToString();
            }
            DA.SetData(0, Success);
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
            get { return new Guid("2AD1F618-CABE-4F25-94E9-2FAE8B7AB09B"); }
        }
    }
}
