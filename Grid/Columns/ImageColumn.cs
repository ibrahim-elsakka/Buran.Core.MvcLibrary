using System.Drawing;

namespace Buran.Core.MvcLibrary.Grid.Columns
{
    public class ImageColumn : DataColumn
    {
        public ImageColumn()
        {
            EditorType = ColumnTypes.Image;
            ImageSize = new Size(0, 0);
        }

        public Size ImageSize { get; set; }

        public string ImageUrlFormat { get; set; }
    }
}
