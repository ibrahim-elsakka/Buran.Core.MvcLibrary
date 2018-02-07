using System;

namespace Buran.Core.MvcLibrary.Data.Attributes
{
    public class ComboBoxDataAttribute : Attribute //, IModelMetadataAware
    {
        public Type Repository { get; set; }
        public string QueryName { get; set; }
        public bool ShowSelect { get; set; }

        public ComboBoxDataAttribute(Type repository, string queryName = "GetSelectList", bool showSelect = true)
        {
            Repository = repository;
            QueryName = queryName;
            ShowSelect = showSelect;
        }
    }
}
