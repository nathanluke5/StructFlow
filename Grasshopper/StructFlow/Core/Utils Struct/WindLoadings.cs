using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructFlow.Misc;
using System.Data;

namespace StructFlow.UtilsStruct
{

    public class CodeTable
    {
        public string mName;

    }


    public class WindLoadings
    {
        public class CfigResult
        {
            internal double Min = 0.0;
            internal double Max = 0.0;
            internal string Log = "";

            public CfigResult() { }
            public CfigResult(double min, double max, string log)
            {
                Min = min;
                Max = max;
            }
        }

        public class CodeTable : DataTable
        {
            private string[] mHeaders = new string[] { };
            private string[] mCaptions = new string[] { };
            private string[] mTypes = new string[] { };
            private List<string[]> mInputData = new List<string[]>();
            int mIdRowIndex = 0; //primary column inwhich is used for selecting information 
            private Dictionary<string, double> mVariables = new Dictionary<string, double>(); //The variables inwhich are used in generating table data 

            public string mCode;
            public string mYear;
            public string mDescription;

            public CodeTable(string name, string[] headers, string[] captions, string[] types, List<string[]> inputdata, Dictionary<string, double> variables, int rowindex) : base(name)
            {
                mHeaders = headers;
                mCaptions = captions;
                mTypes = types;
                mInputData = inputdata;
                mVariables = variables;
                mIdRowIndex = rowindex;

                GenerateTable();
            }

            private void GenerateTable()
            {

                //ADD THE CODE BELOW TO A CODE DATABASE SET
                string[] headers = new string[] { "r/d", "U1", "U2", "T1", "T2", "D1", "D2" };
                string[] captions = new string[] { "r/d ratio", "Windward quarter [U1]", "Windward quarter[U2]", "Centre half [T1]", "Centre half [T2]", "Leeward quarter [D1]", "Leeward quarter[D2]" };
                string[] types = new string[] { "System.Double", "System.Double", "System.Double", "System.Double", "System.Double", "System.Double", "System.Double" };
                int idrow = 0;

                List<string[]> inputdata = new List<string[]>();
                inputdata.Add(new string[] { "0.09", "-(0.2+0.4*h/r)", "0.0", "-(0.55+0.2*h/r)", "-(0.55+0.2*h/r)", "-(0.4+0.2*h/r)", "0.0" });
                inputdata.Add(new string[] { "0.09", "(0.3-0.4*h/r)", "0.0", "-(0.55+0.2*h/r)", "-(0.55+0.2*h/r)", "-(0.25+0.2*h/r)", "0.0" });
                inputdata.Add(new string[] { "0.09", "(0.3-0.4*h/r)", "0.0", "-(0.55+0.2*h/r)", "-(0.55+0.2*h/r)", "-(0.1+0.2*h/r)", "0.0" });

                SetColumnData(this, mHeaders, mTypes, mCaptions, idrow);
                //I could add these tables to a dataset if I wanted to but will not for now. 
                SetRowData(this, mInputData, mVariables);

            }


            //public void GenerateInBuiltTableByName

            private void SetColumnData(DataTable table, string[] headers, string[] types, string[] captions, int primarycolindex)
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    DataColumn column = new DataColumn();
                    column.DataType = System.Type.GetType(types[i]);
                    column.ColumnName = headers[i];
                    column.Caption = captions[i];
                    if (i == primarycolindex)
                    {
                        column.ReadOnly = true;
                        column.Unique = true;
                    }
                    else
                    {
                        column.ReadOnly = true;
                        column.Unique = true;
                    }
                    table.Columns.Add(column);
                }
                // Make the ID column the primary key column.
                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = table.Columns[headers[primarycolindex]];
                table.PrimaryKey = PrimaryKeyColumns;

            }

            private void SetRowData(DataTable table, List<string[]> rowarrays, Dictionary<string, double> var)
            {
                for (int i = 0; i < rowarrays.Count; i++)
                {
                    DataRow row = table.NewRow();
                    for (int j = 0; j < rowarrays[0].Length; j++)
                    {
                        string eq = rowarrays[i][j];
                        ReplaceVariables(eq, var);
                        row[table.Columns[j].ColumnName] = new DataTable().Compute(eq, null);
                    }
                    table.Rows.Add(row);
                }
            }


            private string ReplaceVariables(string equation, Dictionary<string, double> vars)
            {
                foreach (KeyValuePair<string, double> entry in vars)
                {
                    if (equation.Contains(entry.Key))
                    {
                        equation.Replace(entry.Key, entry.Value.ToString());
                    }
                }
                return equation;
            }


            public int FindInterpolationIndexRow(DataTable table, string field, double number)
            {
                //get all values jk

                return -1;
            }

            public int FindInterpolationIndexColumn(DataRow row, double number)
            {


                return -1;
            }

        }

        public class AS1170
        {

            public class TableC3 : SFDataTable
            {
                /// <summary>
                /// r is the rise of the arch. 
                /// </summary>
                internal double r;
                /// <summary>
                /// d is the depth of the roof.
                /// </summary>
                internal double d;
                /// <summary>
                /// h is the average roof height. 
                /// </summary>
                internal double h;

                internal int interpolationIndex;
                private string log = "";
                public enum TableC3AreaTypes { U, T, D }
                internal DataTable CodeDataTable;

                public TableC3() { }
                public TableC3(double rise, double depth, double height)
                {
                    //method which loads the specific table. 
                    r = rise;
                    d = depth;
                    h = height;

                    double hrRatio = h / r;
                    double rdRatio = r / d;

                    //also set variable dictionary
                    Dictionary<string, double> variables = new Dictionary<string, double>();

                    //Do Table Checks Here Maybe
                    if (hrRatio > 2)
                        hrRatio = 2;

                    variables["h/r"] = hrRatio;

                    DataTable CodeDataTable = MakeTable(variables);

                    for (int i = 0; i < CodeDataTable.Rows.Count; i++)
                    {

                    }








                }

                public CfigResult GetCfig(DataTable table, TableC3AreaTypes areatype)
                {
                    CfigResult result = new CfigResult();

                    double uppermin = 0.0;
                    double uppermax = 0.0;

                    int rowcount = table.Rows.Count - 1;

                    //find row indexs for interpolation. 
                    //int index =
                    //(double)table.Rows[rowcount]["r/d"]




                    if (areatype == TableC3AreaTypes.U)
                    {

                    }
                    if (areatype == TableC3AreaTypes.T)
                    {

                    }
                    if (areatype == TableC3AreaTypes.D)
                    {

                    }




                    return result;
                }

            }





            public static CfigResult TableC3(double r, double d, double h, TableC3AreaTypes zone)
            {




                return cfig;
            }

        }
    }
}
