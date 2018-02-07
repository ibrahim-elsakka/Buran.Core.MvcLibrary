using System;
using Buran.Core.MvcLibrary.Resource;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Buran.Core.MvcLibrary.Extenders
{
    public static class FileUploaderExtender
    {
        public static HtmlString FileUploader(this IHtmlHelper html, string name, string title, string imgPath, string uploadUrl)
        {
            var div = string.Format(@"<div class='form-group' id='div{0}'>
    <label for='{0}' class='col-sm-3 control-label'>{1}</label>
    <div class='col-sm-8'>
        <img id='img{0}' src='{2}' class='img-thumbnail img-fileupload'><br>
        <button type='button' class='btnFileUploader btn btn-xs btn-default' id='btn{0}'>
            <i class='fa fa-upload'></i> {3}
        </button>
        <input id='file{0}' type='file' name='file{0}' class='fileinput hide' data-url='{4}'>
    </div>
</div>",
                name,
                title,
                imgPath,
                UI.Upload,
                uploadUrl
                );
            return new HtmlString(div);
        }

        public static long GetImageTempId()
        {
            return -1 * DateTime.Now.Ticks;
        }

        public static HtmlString FileUploader2(this IHtmlHelper html, string name, string imgPath, string uploadUrl)
        {
            var div = string.Format(@"<img id='img{0}' src='{1}' class='img-thumbnail img-fileupload'><br>
        <button type='button' class='btnFileUploader btn btn-xs btn-default' id='btn{0}'>
            <i class='fa fa-upload'></i> {2}
        </button>
        <input id='file{0}' type='file' name='file{0}' class='fileinput hide' data-url='{3}'>",
                name,
                imgPath,
                UI.Upload,
                uploadUrl
                );
            return new HtmlString(div);
        }
    }
}