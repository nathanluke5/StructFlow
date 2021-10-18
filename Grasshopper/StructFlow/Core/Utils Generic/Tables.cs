using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructFlow.Misc
{
    public class Tables
    {
        public class Table
        {
            internal string mName;
            internal List<TableField> mTableFields; //Column Fields //may need secondary field values which are assciated with upper field values and vice verse for rows.
            internal List<TableRow> mTableRows;

            public Table() { }
            public Table(string name, List<TableField> fields, List<TableRow> rows)
            {
                mName = name;
                mTableFields = fields;
                mTableRows = rows;
            }

            public void PrintTable()
            {
                //print an html version of the table to a browser/viewer of some sort.
            }
        }

        public class TableCode : Table
        {
            internal List<TableField> mTableFieldsRow;
            internal string mTableNumber;
            internal string mCodeRef;

            public TableCode() { }
            public TableCode(string name, string tablenumber, string coderef, List<TableField> fields, List<TableRow> rows) : base(name, fields, rows)
            {
                mTableNumber = tablenumber;
                mCodeRef = coderef;
            }

            public TableCode(string name, string tablenumber, string coderef, List<TableField> colfields, List<TableField> rowfields, List<TableRow> rows) : base(name, colfields, rows)
            {
                mTableNumber = tablenumber;
                mCodeRef = coderef;
            }
        }

        public class TableField
        {
            internal string mName = "";
            internal string mKey = "";
            internal string mDescription = "";
            internal string mUnits = "";
            internal int mMultipleCols = 1;

            public TableField() { }
            public TableField(string name, string key, string description, string units)
            {
                mName = name;
                mKey = key;
                mDescription = description;
                mUnits = units;
            }
        }


        public class TableFieldValuePair
        {
            internal TableField mField = new TableField();
            internal string mValue = "";

            public TableFieldValuePair() { }
            public TableFieldValuePair(TableField field, string value)
            {
                mField = field;
                mValue = value;
            }
        }

        public class TableRow
        {
            internal List<TableFieldValuePair> mKeyValues = new List<TableFieldValuePair>();

            public TableRow() { }
            public TableRow(List<TableFieldValuePair> keyvalues)
            {
                mKeyValues = keyvalues;
            }

            public bool TryGetValue(string fieldkey, out string value)
            {
                value = null;
                if (fieldkey == null)
                    return false;

                List<string> fields = mKeyValues.ConvertAll(x => x.mField.mKey).ToList();
                if (fields.Contains(fieldkey))
                {
                    value = mKeyValues.Where(x => x.mField.mKey == fieldkey).Select(x => x.mValue).First();
                    return true;
                }
                else
                    return false;
            }
        }


    }
}
