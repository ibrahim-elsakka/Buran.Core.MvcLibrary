using System.Collections.Generic;

namespace Buran.Core.MvcLibrary.AdminPanel.Controls
{
    public enum EditPageMenuItemType
    {
        Other,
        Insert,
        Back,
        Reset,
        Save,
        SaveContinue,
        SaveNew,

    }

    public class EditorPageMenu
    {
        public List<EditorPageMenuItem> Items;

        public EditorPageMenu()
        {
            Items = new List<EditorPageMenuItem>();
        }
    }

    public class EditorPageMenuSubItem
    {
        public string Url { get; set; }
        public string Title { get; set; }
    }

    public class EditorPageMenuItem
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string IconClass { get; set; }
        public string ButtonClass { get; set; }
        public EditPageMenuItemType ItemType { get; set; }

        public List<EditorPageMenuSubItem> Items;

        public EditorPageMenuItem()
        {
            Items = new List<EditorPageMenuSubItem>();
        }
    }
}
