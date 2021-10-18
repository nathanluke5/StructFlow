using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using StructFlow;
using Rhino.DocObjects;
using Grasshopper.Kernel.Types;

namespace StructFlow.Components
{
    public class PadString : GH_Component
    {
        public PadString() : base("Pad String", "ZipFolder", "Pad s string with ", "StructFlow", "5.0 Document") { }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Value", "V", "String or Value to add padding too", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Length", "L", "Length of Output String", GH_ParamAccess.item);
            pManager.AddTextParameter("Char", "C", "Optional Char to Pad Value with. Default '0'", GH_ParamAccess.item, "0");
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("Out", "O", "Padded String", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            string value = "";
            int length = 0;
            string temp = "";
            char ch = '0';
            string output = "";

            if (!DA.GetData(0, ref value)) return;
            if (!DA.GetData(1, ref length)) return;
            if (DA.GetData(2, ref temp))
            {
                if (temp.Length == 1)
                {
                    ch = (char)temp[0];
                }
                else
                    base.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Provide single charactor only");
            }

            output = StructFlow.Utils.StringUtils.AddPadding(value, ch, length);

            DA.SetData(0, output);
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
            get { return new Guid("{20DF22D0-8720-41A6-BDE8-B4920B314819}"); }
        }
    }

    public class NumbersFromString : GH_Component
    {
        public NumbersFromString() : base("Numbers From String", "NumFromStr", "Get a List of Numbers from a String which includes all types of letters and charactors", "StructFlow", "5.0 Document") { }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("String", "S", "String to search for numbers", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("Numbers", "O", "List of Numbers", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            string input = "";

            List<double> numbers = new List<double>();

            if (!DA.GetData(0, ref input)) return;

            numbers = StructFlow.Utils.StringUtils.FindNumbersInString(input);

            DA.SetDataList(0, numbers);
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
            get { return new Guid("{676D0A5B-85B8-4314-ABF0-B33C4035D22D}"); }
        }
    }

    public class RoundNumberToFactor : GH_Component
    {
        public RoundNumberToFactor() : base("Round Number To Factor", "RdToFactor", "Round a number to a certain Factorial with a given fairness", "StructFlow", "5.0 Document") { }
        public override GH_Exposure Exposure { get { return GH_Exposure.primary; } }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Value", "V", "Amount to Round", GH_ParamAccess.item);
            pManager.AddNumberParameter("Nearest", "N", "Factor to round too.(i.e 0.25 if round to quater, 0.01 for rounding to 1/100th, 1 for rounding to nearest Integer, 10 for round to closest 10.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Fairness", "F",
                "A number between 0 and .99999..... (0 means floor and 0.99999... means ceiling)." + "\r\n" +
                "0.5 = Standard Rounding function. It will round up the border case. i.e. 1.5 to 2 and not 1." + "\r\n" +
                "0.4999999... non-standard rounding function. Where border case is rounded down. i.e. 1.5 to 1 and not 2." + "\r\n" +
                "0.75 means first 75% values will be rounded down, rest 25% value will be rounded up.", GH_ParamAccess.item, 0.5);
            pManager[2].Optional = true;
        }
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddTextParameter("Rounded Values", "V", "List of Numbers", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double input = 0.0;
            double nearest = 0.0;
            double fairness = 0.0;
            double output = 0.0;

            if (!DA.GetData(0, ref input)) return;
            if (!DA.GetData(1, ref nearest)) return;
            decimal inputdec = (decimal)input;
            decimal nearestdec = (decimal)nearest;

            decimal fairnessdec = 0.0m;
            if (DA.GetData(2, ref fairness))
            {
                if (fairness < 1.0)
                {
                    fairnessdec = (decimal)fairness;
                }
                else
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "fairness to be between 0 and 0.9999");
            }

            output = (double)StructFlow.Utils.NumberUtils.RoundToFactor(inputdec, nearestdec, fairnessdec);

            DA.SetData(0, output);
        }

        protected override System.Drawing.Bitmap Icon { get { return null; } }
        public override Guid ComponentGuid { get { return new Guid("{5D463A6E-8E8C-4B5E-A65D-2D4F0F58BD2D}"); } }
    }

    public class CreateGeometryTable : GH_Component
    {
        public CreateGeometryTable() : base("CAD Table From User Text", "TableFromUT", "Generate a CAD Table from geometry with associated usertext keys and values", "StructFlow", "5.0 Document") { }
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane for table", GH_ParamAccess.item); //0
            pManager.AddGeometryParameter("Geometry", "G", "Geometry to mine usertext from", GH_ParamAccess.list); //1
            pManager.AddTextParameter("Keys", "K", "List of geometry keys to include in the Table", GH_ParamAccess.list); //2
            pManager.AddTextParameter("Heading", "H", "Table Heading", GH_ParamAccess.item, "Table 1"); //3
            pManager.AddTextParameter("H Dim Style", "HD","String matching the Rhino dimension style to be used for the Heading Text", GH_ParamAccess.item); //4
            pManager.AddTextParameter("B Dim Style", "BD", "String matching the Rhino dimension style to be used for the Body Text", GH_ParamAccess.item); //5
            pManager.AddNumberParameter("Padding", "P", "Text paddingle value in mm", GH_ParamAccess.item, 0.125); //6
            pManager.AddNumberParameter("Max Column Width", "W", "Maximum column width dimension to enable text wrapping", GH_ParamAccess.item); //7
            pManager.AddBooleanParameter("Totals", "T", "Add Totals for values where applicable", GH_ParamAccess.item, false); //8
            pManager.AddBooleanParameter("Flip", "F", "Flip table matrix", GH_ParamAccess.item, false); //9
            pManager[0].Optional = pManager[3].Optional = pManager[4].Optional = pManager[5].Optional = pManager[6].Optional = pManager[7].Optional = pManager[8].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {

            pManager.AddGenericParameter("Cells Heading", "CH", "PolyLine cells surrounding heading", GH_ParamAccess.item);
            pManager.AddGenericParameter("Cells Body", "CB", "Polyline cells surrounding body", GH_ParamAccess.item);
            pManager.AddGenericParameter("Text Heading", "TH", "Text for heading", GH_ParamAccess.item);
            pManager.AddGenericParameter("Text Body", "TB", "2D Array of body text including heading keys", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Plane pln = Plane.WorldXY;
            List<IGH_GeometricGoo> geometry = new List<IGH_GeometricGoo>();
            List<Guid> guids = new List<Guid>();
            List<string> keys = new List<string>();
            string heading = "";
            string headDimStyle = "";
            string textDimStyle = "";
            double padding = 25;
            double maxColumnWidth = 500;
            bool flip = false;
            //totals to be implmented if required
            bool totals = false;

            List<List<Polyline>> headingcells = new List<List<Polyline>>();
            List<List<Polyline>> bodycells = new List<List<Polyline>>();
            List<List<StructFlow.Grasshopper.TextGoo>> Heading = new List<List<StructFlow.Grasshopper.TextGoo>>();
            List<List<StructFlow.Grasshopper.TextGoo>> Body = new List<List<StructFlow.Grasshopper.TextGoo>>();

            //Get geometry GUIDS - ASSUME THIS IS CORRECT
            DA.GetData(0, ref pln);
            if (!DA.GetDataList(1, geometry)) return;
            foreach (IGH_GeometricGoo geom in geometry)
            {
                guids.Add(geom.ReferenceID);
            }
            if (!DA.GetDataList(2, keys)) return;
            DA.GetData(3, ref heading);
            DA.GetData(4, ref headDimStyle);
            DA.GetData(5, ref textDimStyle);
            DA.GetData(6, ref padding);
            DA.GetData(7, ref maxColumnWidth);
            DA.GetData(8, ref totals);
            DA.GetData(9, ref flip);

            Miscilaneuos.GeometryTable.GenerateTable(pln, guids, keys, heading, headDimStyle, textDimStyle, padding, maxColumnWidth, flip, totals, ref headingcells, ref bodycells, ref Heading, ref Body);

            List<Polyline> cells = headingcells.SelectMany(c => c).ToList();
            List<Polyline> cells2 = bodycells.SelectMany(c => c).ToList();
            List<StructFlow.Grasshopper.TextGoo> text = Heading.SelectMany(t => t).ToList();
            List<StructFlow.Grasshopper.TextGoo> text2 = Body.SelectMany(t => t).ToList();

            DA.SetDataList(0, cells);
            DA.SetDataList(1, cells2);
            DA.SetDataList(2, text);
            DA.SetDataList(3, text2);
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
            get { return new Guid("{75F55D43-6382-4367-B50F-34A1C3EE7488}"); }
        }
    }

}