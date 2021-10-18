using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using StructFlow.Utils;
using StructFlow.Grasshopper;
using Rhino;
using Rhino.DocObjects;
using Rhino.Collections;
using GH_IO;
using GH_IO.Serialization;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace StructFlow.Miscilaneuos
{
    class GeometryTable
    {
        public static void GenerateTable(Plane insertPlane, List<Guid> geometry, List<string> keys, string heading, string headDimStyle, string textDimStyle, double padding, double maxColumnWidth, bool flipTable, bool totals, ref List<List<Polyline>> CellHeading, ref List<List<Polyline>> CellBody, ref List<List<TextGoo>> TextHeading, ref List<List<TextGoo>> TextBody)
        {
            Rhino.DocObjects.Tables.DimStyleTable dimstyles = Rhino.RhinoDoc.ActiveDoc.DimStyles;

            Rhino.DocObjects.DimensionStyle headstyle = dimstyles.FindName(headDimStyle);
            Rhino.DocObjects.DimensionStyle bodystyle = dimstyles.FindName(textDimStyle);
            Rhino.RhinoDoc doc = Rhino.RhinoDoc.ActiveDoc;

            //ref geometry by guid
            List<Rhino.DocObjects.RhinoObject> items = new List<Rhino.DocObjects.RhinoObject>();
            List<System.Collections.Specialized.NameValueCollection> keyValue = new List<System.Collections.Specialized.NameValueCollection>();
            foreach (Guid id in geometry)
            {
                Rhino.DocObjects.RhinoObject obj = doc.Objects.Find(id);
                items.Add(obj);
                keyValue.Add(obj.Attributes.GetUserStrings());
            }

            //Get Keys of first object in list. These should be the same for all
            //***UPDATE THIS IN FUTURE**** Make so potentially object may not all have the same keys. Can create a dictionary of keys
            List<string> userKeys = new List<string>(keyValue[0].AllKeys);
            List<string> userKeysIntersect = new List<string>(userKeys.Intersect(keys).ToList());
            List<string> userKeysTable = new List<string>();

            double columnBuffer = Rhino3d.RhinoUtils.RhinoModelUnit() == 2 ? padding : (padding / 1000);
            double rowBuffer = Rhino3d.RhinoUtils.RhinoModelUnit() == 2 ? padding : (padding / 1000);

            //Need to sort the userKeysTable based on input keys
            for (int i = 0; i < keys.Count; i++)
            {
                foreach (string k in userKeysIntersect)
                {
                    if (keys[i] == k)
                        userKeysTable.Add(keys[i]);
                }
            }

            List<List<string>> userValuesTable = new List<List<string>>();
            //This Creates table lists. Should keep code to generate a csv file. No this can easily be created by another plug-in.
            foreach (string key in userKeysTable)
            {
                List<string> values = new List<string>();
                foreach (System.Collections.Specialized.NameValueCollection pair in keyValue)
                {
                    values.Add(pair.GetValues(key)[0]);
                }
                userValuesTable.Add(values);
            }

            //***********Check Whether to Add totals here or not!!***********

            //Basically just adding the userKeys to the user value lists. May want to have different text type at different stage.
            List<List<string>> tableLists = new List<List<string>>(userValuesTable);
            int jcounter = 0;
            foreach (string key in userKeysTable)
            {
                tableLists[jcounter].Insert(0, key);
                jcounter++;
            }


            //Check whether to flip table or not
            if (flipTable)
            {
                tableLists = ListUtils.FlipList2D(tableLists);
            }

            //Wrap Text to satisfy a certain max column width
            for (int i = 0; i < tableLists.Count; i++)
            {
                for (int j = 0; j < tableLists[i].Count; j++)
                {
                    string wrappedtext = WrapText(tableLists[i][j], columnBuffer, maxColumnWidth, bodystyle);
                    tableLists[i][j] = wrappedtext;
                }
            }




            //Get Column Widths.
            List<double> columnwidth = new List<double>();
            foreach (List<string> strlist in tableLists)
            {
                columnwidth.Add(ColumnWidth(strlist, columnBuffer, bodystyle));
            }

            //Get Row Heights
            double rowheightHeading = headstyle.TextHeight + rowBuffer * 2;

            double rowheighttext = bodystyle.TextHeight + rowBuffer * 2;

            //IMPLEMENT THIS TONIGHT
            //List<double> rowheighttext = new List<double>();

            //List<List<string>> rowLists = ListUtilities.FlipList2D(tableLists);
            //foreach (List<string> strlist in tableLists)
            //{
            //    rowheighttext.Add(RowHeight(strlist, rowBuffer, bodystyle));
            //}

            //*********BUILD TABLE*************
            List<string> headingtext = new List<string>();
            headingtext.Add(heading);

            double headingwidth = ColumnWidth(headingtext, rowBuffer, headstyle);

            double bodywidthtotal = 0.0;
            foreach (double width in columnwidth)
                bodywidthtotal += width;

            if (headingwidth > bodywidthtotal)
            {
                double dev = (headingwidth - bodywidthtotal) / columnwidth.Count;
                for (int i = 0; i < columnwidth.Count; i++)
                {
                    columnwidth[i] = columnwidth[i] + dev;
                }
            }
            else
                headingwidth = bodywidthtotal;

            //Generate the Table Transform
            Transform TableTransform = Transform.PlaneToPlane(Plane.WorldXY, insertPlane);

            //Heading Cell and Text
            List<Point3d> headpoints = new List<Point3d>();
            headpoints.Add(new Point3d(0, 0, 0));
            headpoints.Add(new Point3d(0, rowheightHeading, 0));
            headpoints.Add(new Point3d(headingwidth, rowheightHeading, 0));
            headpoints.Add(new Point3d(headingwidth, 0, 0));
            headpoints.Add(new Point3d(0, 0, 0));
            Polyline headingcell = new Polyline(headpoints);
            headingcell.Transform(TableTransform);
            Point3d headingctr = headingcell.CenterPoint();
            insertPlane.Origin = headingctr;

            Rhino.Display.Text3d textH = new Rhino.Display.Text3d(heading, insertPlane, headstyle.TextHeight);
            textH.HorizontalAlignment = TextHorizontalAlignment.Center;
            textH.VerticalAlignment = TextVerticalAlignment.Middle;

            TextGoo HeadingGoo = new TextGoo(textH);

            //Body Cell and Text lists
            List<List<Polyline>> cells = new List<List<Polyline>>();
            //List<List<Point3d>> cellctrs = new List<List<Point3d>>();
            List<List<TextGoo>> bodytext = new List<List<TextGoo>>();

            //Cell points
            Point3d ptone = new Point3d(0, 0, 0);
            Point3d pttwo = new Point3d(0, 0, 0);
            Point3d ptthree = new Point3d(0, 0, 0);
            Point3d ptfour = new Point3d(0, 0, 0);
            Point3d ptfive = new Point3d(0, 0, 0);

            double columnwidthone = 0.0;
            double columnwidthtwo = 0.0;

            for (int i = 0; i < tableLists.Count; i++)
            {
                List<Polyline> row = new List<Polyline>();
                //List<Point3d> rowpts = new List<Point3d>();
                List<TextGoo> textrow = new List<TextGoo>();
                if (i == 0)
                    columnwidthone = 0.0;
                else
                    columnwidthone += columnwidth[i - 1];

                columnwidthtwo += columnwidth[i];

                for (int j = 0; j < tableLists[0].Count; j++)
                {
                    List<Point3d> pointlist = new List<Point3d>();

                    if (i == 0)
                        ptone.X = ptfour.X = 0;
                    else
                        ptone.X = ptfour.X = columnwidthone;
                    ptone.Y = j * rowheighttext * -1;
                    pttwo.X = ptthree.X = columnwidthtwo;
                    pttwo.Y = j * rowheighttext * -1;
                    ptthree.Y = (j + 1) * rowheighttext * -1;
                    ptfour.Y = (j + 1) * rowheighttext * -1;

                    ptfive = ptone;
                    pointlist.Add(ptone);
                    pointlist.Add(pttwo);
                    pointlist.Add(ptthree);
                    pointlist.Add(ptfour);
                    pointlist.Add(ptfive);

                    Polyline cell = new Polyline(pointlist);
                    cell.Transform(TableTransform);
                    row.Add(cell);
                    insertPlane.Origin = cell.CenterPoint();

                    Rhino.Display.Text3d textB = new Rhino.Display.Text3d(tableLists[i][j], insertPlane, bodystyle.TextHeight);
                    textB.HorizontalAlignment = TextHorizontalAlignment.Center;
                    textB.VerticalAlignment = TextVerticalAlignment.Middle;
                    TextGoo celltext = new TextGoo(textB);
                    textrow.Add(celltext);
                    //rowpts.Add(cell.CenterPoint());
                }
                cells.Add(row);
                bodytext.Add(textrow);
                //cellctrs.Add(rowpts);
            }
            //Plane.WorldXY.Origin = cell.CentrePoint();

            //var flattentext = bodytext.SelectMany(i => i).ToList();
            //var flattencells = cells.SelectMany(i => i).ToList();

            List<List<Polyline>> headlist = new List<List<Polyline>>(1);
            headlist.Add(new List<Polyline>(){headingcell});

            CellHeading = headlist;
            //CellBody = flattencells;
            CellBody = cells;

            List<List<TextGoo>> headtextlist = new List<List<TextGoo>>(1);
            headtextlist.Add(new List<TextGoo>() { HeadingGoo });

            TextHeading = headtextlist;
            //TextBody = flattentext;
            TextBody = bodytext;
        }

        public static double RowHeight(List<string> inputText, double padding, Rhino.DocObjects.DimensionStyle style)
        {
            double height = 0;

            Point3d origin = new Point3d(0, 0, 0);
            Vector3d zvector = new Vector3d(0, 0, 1.0);
            Plane plane = new Plane(origin, zvector);

            //UNITS ONLY WORK FOR MM and m atm.
            if (inputText == null)
            {
                double stdheight = style.TextHeight;
            }
            else
            {
                foreach (string str in inputText)
                {
                    Rhino.Display.Text3d temptext = new Rhino.Display.Text3d(str, plane, style.TextHeight);
                    Rhino.Geometry.BoundingBox bbox = temptext.BoundingBox;
                    Rhino.Geometry.Box box = new Rhino.Geometry.Box(bbox);
                    double length = box.Y.Length;
                    if (length > height)
                        height = length;
                }
            }
            return height + 2 * padding;
        }


        public static string WrapText(string inputText, double padding, double maxcolumnWidth, Rhino.DocObjects.DimensionStyle style)
        {
            List<string> wrappedLines = new List<string>();

            string [] textitems = inputText.Split(' ');

            string curLine = "";

            //If single word > maxcolumn width do not wrap
            if (textitems.Length == 1)
            {
                return textitems[0];
            }

            for (int i = 0; i < textitems.Length; i++)
            {
                string check = curLine + " " + textitems[i];
                Rhino.Display.Text3d temptext = new Rhino.Display.Text3d(check, Plane.WorldXY, style.TextHeight);
                Rhino.Geometry.BoundingBox bbox = temptext.BoundingBox;
                Rhino.Geometry.Box box = new Rhino.Geometry.Box(bbox);
                double length = box.X.Length;
 
                //need to check whether first word is greater than max column width and update the value if it is.
                if (length + (2 * padding) < maxcolumnWidth)
                {
                    curLine += String.IsNullOrEmpty(curLine) ? textitems[i] : " " + textitems[i];
                    if (i == textitems.Length - 1)
                    {
                        wrappedLines.Add(curLine);
                        if (wrappedLines.Count > 1)
                            return String.Join("\n", wrappedLines.ToArray());
                        else
                            return wrappedLines[0]; 
                    }
                }
                else
                {
                    wrappedLines.Add(curLine);
                    curLine = textitems[i];
                }
            }
            if (wrappedLines.Count > 1)
                return String.Join("\n", wrappedLines.ToArray());
            else
                return wrappedLines[0];
        }

        public static double ColumnWidth(List<string> inputText, double padding, Rhino.DocObjects.DimensionStyle style)
        {
            double width = 0;

            Point3d origin = new Point3d(0, 0, 0);
            Vector3d zvector = new Vector3d(0, 0, 1.0);
            Plane plane = new Plane(origin, zvector);

            //UNITS ONLY WORK FOR MM and m atm.
            if (inputText == null)
            {
                double stdwidth = 0.050;
                if (Rhino3d.RhinoUtils.RhinoModelUnit() == 2)
                {
                    stdwidth = stdwidth * 1000;
                }
                return stdwidth;
            }
            else
            {
                foreach (string str in inputText)
                {
                    Rhino.Display.Text3d temptext = new Rhino.Display.Text3d(str, plane, style.TextHeight);
                    Rhino.Geometry.BoundingBox bbox = temptext.BoundingBox;
                    Rhino.Geometry.Box box = new Rhino.Geometry.Box(bbox);
                    double length = box.X.Length;
                    if (length > width)
                        width = length;
                }
            }
            return width + 2 * padding;
        }
    }
}
