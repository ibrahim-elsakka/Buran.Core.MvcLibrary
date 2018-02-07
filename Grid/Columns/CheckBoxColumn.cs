namespace Buran.Core.MvcLibrary.Grid.Columns
{
    public class CheckBoxColumn : DataColumn
    {
        public CheckBoxColumn()
        {
            EditorType = ColumnTypes.CheckBox;
            Filterable = false;
            Sortable = false;
        }
    }
}
