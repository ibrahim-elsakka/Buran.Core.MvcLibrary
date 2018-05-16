using System;
using System.Collections.Generic;

namespace Buran.Core.MvcLibrary.Grid.Columns
{
    public class DataColumn
    {
        public string ObjectValueFunction { get; set; }

        public Type ObjectValueConverter { get; set; }

        public DataColumn()
        {
            Sortable = true;
            SortField = "";
            Filterable = true;
            FilterField = "";
            Visible = true;
            DataColumnType = DataColumnTypes.BoundColumn;
            EditorType = ColumnTypes.Label;
            ValueConverter = new List<DataValueConverter>();
            DataFormat = string.Empty;
            Alignment = AlignmentTypes.Left;
            HeaderCssClass = "";
        }

        public DataColumn(string fieldName)
        {
            FieldName = fieldName;
            Sortable = true;
            SortField = "";
            Filterable = true;
            FilterField = "";
            Visible = true;
            DataColumnType = DataColumnTypes.BoundColumn;
            EditorType = ColumnTypes.Label;
            ValueConverter = new List<DataValueConverter>();
            DataFormat = string.Empty;
            Alignment = AlignmentTypes.Left;
            HeaderCssClass = "";
        }

        public enum AlignmentTypes
        {
            Left,
            Right,
            Center
        }
        public AlignmentTypes Alignment { get; set; }

        public string HeaderCssClass { get; set; }

        public string FieldName { get; set; }

        public string Caption { get; set; }

        public string Format { get; set; }

        public int Width { get; set; }

        public bool Sortable { get; set; }

        public string SortField { get; set; }

        public bool Filterable { get; set; }

        public string FilterField { get; set; }

        public string FilterValue { get; set; }

        public bool Visible { get; set; }

        public DataColumnTypes DataColumnType { get; set; }

        public ColumnTypes EditorType { get; set; }

        public List<DataValueConverter> ValueConverter { get; set; }

        public string DataFormat { get; set; }
    }
}
