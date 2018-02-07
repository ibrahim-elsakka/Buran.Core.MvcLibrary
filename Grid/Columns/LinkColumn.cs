namespace Buran.Core.MvcLibrary.Grid.Columns
{
    public class LinkColumn : DataColumn
    {
        public LinkColumn()
        {
            EditorType = ColumnTypes.Link;
            Target = string.Empty;
        }
        public string Target { get; set; }

        public string NavigateUrlFormat { get; set; }

        public string Text { get; set; }
        public string CssClass { get; set; }
    }
}
