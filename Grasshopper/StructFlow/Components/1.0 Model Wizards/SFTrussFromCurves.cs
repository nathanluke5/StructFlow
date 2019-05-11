using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using StructFlow.Core;
using StructFlow.Truss;

namespace StructFlow.Components
{
    public class SFTrussFromCurves : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public SFTrussFromCurves()
          : base("SF 2DTruss From Curves", "SF2DTrussFromCurves",
              "This component will be used to build a number of typical truss types from two input curves including:" +
                "Pratt" +
                "Howe" +
                "Warren" +
                "X-Truss" +
                "K-Truss" +
                "This component should be used inconjuction with truss chord builder components.",
              "StructFlow", "Model")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve 1", "C1", "Curve 1", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curve 2", "C2", "Curve 2", GH_ParamAccess.item);
            pManager.AddTextParameter("Truss Type", "TT", "SF Truss Type", GH_ParamAccess.item);
            pManager.AddNumberParameter("Segments", "S", "No of Segments", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Web Direction One Way", "WD", "False - Simply Supported / True - Cantilever Type", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Verticals Snap Plane", "VP", "Provide desired plain for vertical alignment", GH_ParamAccess.item);  //update this to import a list of planes 
            pManager.AddNumberParameter("Plane Toggle", "FV", "Toogle between locking vertical member plane and not", GH_ParamAccess.item); //this should beable to toogle through multiple vertical alignment types.

            pManager[5].Optional = true;
            pManager[6].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Info", "INFO", "Information about the Component", GH_ParamAccess.item);
            pManager.AddCurveParameter("Primary Chord", "PC", "Top chord analysis members", GH_ParamAccess.list);
            pManager.AddCurveParameter("Secondary Chord", "SC", "Bottom chord analysis members", GH_ParamAccess.list);
            pManager.AddCurveParameter("Verticals", "V", "Vertical analysis members", GH_ParamAccess.list);
            pManager.AddCurveParameter("Webs", "W", "Web analysis members", GH_ParamAccess.list);
            pManager.AddCurveParameter("Webs", "W", "Web analysis members", GH_ParamAccess.list);
            pManager.AddPointParameter("Primary Chord Nodes", "PCN", "Web analysis members", GH_ParamAccess.list);
            pManager.AddPointParameter("Secondary Chord Nodes", "SCN", "Web analysis members", GH_ParamAccess.list);
            pManager.AddCurveParameter("Documentation Members", "DOC", "All Truss Members-Not Split", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            /*
            //Declare Variables
            Curve curveOne = null;
            Curve curveTwo = null;
            string type = "";
            int segments = 10;
            Boolean oneWay = false;
            Plane vertRefPlane = Plane.Unset;
            int vertToogle = 0;

            //Assign Initial Values
            if (!DA.GetData(0, ref curveOne)) return;
            if (!DA.GetData(0, ref curveTwo)) return;
            if (!DA.GetData(0, ref type)) return;
            if (!DA.GetData(0, ref segments)) return;
            if (!DA.GetData(0, ref oneWay)) return;
            if (!DA.GetData(0, ref vertRefPlane)) return;
            if (!DA.GetData(0, ref vertToogle)) return;


            //Set Truss Type
            if (!Enum.TryParse<TrussType>(_trussType, out type))
                type = TrussType.Pratt;

            //Points from lines
            List<Point3d> BCPoints = new List<Point3d>();
            BCPoints = ChordDefinition.TrussPtsFromLines(_lineBC, _lineTC, _numSeg, _forceVert)[0];
            nodesBC = BCPoints;
            List<Point3d> TCPoints = new List<Point3d>();
            TCPoints = ChordDefinition.TrussPtsFromLines(_lineBC, _lineTC, _numSeg, _forceVert)[1];
            nodesTC = TCPoints;

            //Create Chords
            analysisTC = ModelUtilities.PointsToLines(TCPoints);
            analysisBC = ModelUtilities.PointsToLines(BCPoints);

            //Creat Verticals
            List<Line> verticals = Truss.Verticals.AllVerts(BCPoints, TCPoints, type);
            analysisV = verticals;

            //Create Webs

            List<Line> webs = new List<Line>();

            switch (type)
            {
                case TrussType.Pratt:
                    webs = Webs.Pratt(BCPoints, TCPoints, _oneWay, _numSeg);
                    break;
                case TrussType.Howe:
                    webs = Webs.Howe(BCPoints, TCPoints, _oneWay, _numSeg);
                    break;
                case TrussType.Warren & TrussType.WarrenwVerts:
                    webs = Webs.Warren(BCPoints, TCPoints, _oneWay, _numSeg);
                    break;
                case TrussType.WarrenFlipped & TrussType.WarrenFlippedwVerts:
                    webs = Webs.WarrenFlipped(BCPoints, TCPoints, _oneWay, _numSeg);
                    break;
                case TrussType.Xtruss:
                    webs = Webs.Xtruss(BCPoints, TCPoints, _oneWay, _numSeg);
                    break;
                case TrussType.Ktruss:
                    webs = Webs.Ktruss(BCPoints, TCPoints, _oneWay, _numSeg, ref verticals);
                    break;
                case TrussType.KtrussFlipped:
                    webs = Webs.KtrussFlipped(BCPoints, TCPoints, _oneWay, _numSeg, ref verticals);
                    break;
                default:
                    webs = Webs.Pratt(BCPoints, TCPoints, _oneWay, _numSeg);
                    break;
            }


            analysisW = webs;
            */
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
            get { return new Guid("c8512329-46a5-4216-895a-905c9e489a42"); }
        }
    }
}