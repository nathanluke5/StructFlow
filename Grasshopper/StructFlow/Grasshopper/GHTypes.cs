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


namespace StructFlow.Grasshopper
{
    public sealed class TextGoo : GH_GeometricGoo<global::Rhino.Display.Text3d>, IGH_PreviewData, IGH_BakeAwareData
    {
        #region constructors
        public TextGoo() : this(new global::Rhino.Display.Text3d("Existence is pain", Plane.WorldXY, 10)) { }
        public TextGoo(global::Rhino.Display.Text3d text)
        {
            m_value = text;
        }

        private static global::Rhino.Display.Text3d DuplicateText3d(global::Rhino.Display.Text3d original)
        {
            if (original == null) return null;
            var text = new global::Rhino.Display.Text3d(original.Text, original.TextPlane, original.Height)
            {
                Bold = original.Bold,
                Italic = original.Italic,
                FontFace = original.FontFace,
                HorizontalAlignment = original.HorizontalAlignment,
                VerticalAlignment = original.VerticalAlignment
            };
            return text;
        }


        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return new TextGoo(DuplicateText3d(m_value));
        }
        #endregion
        #region properties
        public override string TypeName
        {
            get { return "3D Text"; }
        }
        public override string TypeDescription
        {
            get { return "3D Text"; }
        }
        public override string ToString()
        {
            if (m_value == null)
                return "<null>";
            return m_value.Text;
        }

        public override BoundingBox Boundingbox
        {
            get
            {
                if (m_value == null)
                    return BoundingBox.Empty;
                return m_value.BoundingBox;
            }
        }

        public override BoundingBox GetBoundingBox(Transform xform)
        {
            if (m_value == null)
                return BoundingBox.Empty;

            BoundingBox box = m_value.BoundingBox;
            Point3d[] corners = xform.TransformList(box.GetCorners());
            return new BoundingBox(corners);
        }
        #endregion
        #region methods
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            var text = DuplicateText3d(m_value);
            if (text == null)
                return new TextGoo(null);

            Plane plane = text.TextPlane;
            Point3d point = plane.PointAt(1, 1);

            plane.Transform(xform);
            point.Transform(xform);
            double dd = point.DistanceTo(plane.Origin);

            text.TextPlane = plane;
            text.Height *= dd / Math.Sqrt(2);
            return new TextGoo(text);
        }
        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            return DuplicateGeometry();
        }
        #endregion
        #region preview
        BoundingBox IGH_PreviewData.ClippingBox
        {
            get { return Boundingbox; }
        }
        void IGH_PreviewData.DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (m_value == null)
                return;
            args.Pipeline.Draw3dText(m_value, args.Color);
        }
        void IGH_PreviewData.DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            // Do not draw in meshing layer.
        }
        #endregion
        #region baking
        bool IGH_BakeAwareData.BakeGeometry(RhinoDoc doc, ObjectAttributes att, out Guid id)
        {
            id = Guid.Empty;
            if (m_value == null)
                return false;

            if (att == null)
                att = doc.CreateDefaultAttributes();

            id = doc.Objects.AddText(m_value, att);
            return true;
        }
        #endregion
    }
}
