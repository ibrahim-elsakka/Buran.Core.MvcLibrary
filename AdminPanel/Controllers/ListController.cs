using System;
using System.Linq;
using Buran.Core.Library.Utils;
using Buran.Core.MvcLibrary.LogUtil;
using Buran.Core.MvcLibrary.Extenders;
using Buran.Core.MvcLibrary.Repository;
using Microsoft.AspNetCore.Mvc;
using Buran.Core.MvcLibrary.Utils;
using Buran.Core.MvcLibrary.Resource;
using System.Resources;
using Buran.Core.MvcLibrary.AdminPanel.Controls;

namespace Buran.Core.MvcLibrary.AdminPanel.Controllers
{
    public class ListController<T, Z> : AdminController
        where T : class
        where Z : class
    {
        protected Z _context;
        protected ResourceManager rsMan;

        protected string DeleteJsAction = "Reload();";
        protected string DeleteJsAction2 = "Reload('{0}');";

        protected bool PopupEditor { get; set; }
        public EditorPageMenu PageMenu { get; set; }

        #region MENU
        protected void BuildCreateMenu()
        {
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Back,
                Title = UI.Back,
                IconClass = "fa fa-angle-left",
                Url = Url.Action("Index")
            });
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Reset,
                Title = UI.Reset,
                IconClass = "fa fa-reply",
                Url = Url.Action("Create")
            });
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Save,
                Title = UI.Save,
                IconClass = "fa fa-check",
                Id = "btnFormSubmit"
            });
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.SaveContinue,
                Title = UI.SaveEdit,
                IconClass = "fa fa-check-circle",
                Id = "btnFormSubmitEdit"
            });
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.SaveNew,
                Title = UI.SaveNew,
                IconClass = "fa fa-check-circle",
                Id = "btnFormSubmitNew"
            });
        }

        protected void BuildEditMenu(object id)
        {
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Back,
                Title = UI.Back,
                IconClass = "fa fa-angle-left",
                Url = Url.Action("Index")
            });
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Reset,
                Title = UI.Reset,
                IconClass = "fa fa-reply",
                Url = Url.Action("Edit", new { id })
            });
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Save,
                Title = UI.Save,
                IconClass = "fa fa-check",
                Id = "btnFormSubmit"
            });
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.SaveContinue,
                Title = UI.SaveEdit,
                IconClass = "fa fa-check-circle",
                Id = "btnFormSubmitEdit"
            });
        }
        #endregion

        protected IGenericRepository<T> Repo;

        protected ListController(bool popupEditor, Z context)
        {
            _context = context;
            PopupEditor = popupEditor;
            PageMenu = new EditorPageMenu();
        }

        protected enum TitleType
        {
            List,
            Create,
            Editor
        }

        protected virtual string GetTitleTableName()
        {
            return typeof(T).Name;
        }

        protected string GetTitle(TitleType type)
        {
            var r = GetTitleTableName();
            var suffixName = "";
            if (type == TitleType.List)
            {
                r += "_ListTitle";
                suffixName = UI.TitleList;
            }
            else if (type == TitleType.Create)
            {
                r += "_EditorTitle";
                suffixName = UI.TitleCreate;
            }
            else if (type == TitleType.Editor)
            {
                r += "_EditorTitle";
                suffixName = UI.TitleEdit;
            }
            if (rsMan != null)
            {
                var rsm = rsMan.GetString(r);
                return (rsm.IsEmpty() ? r : rsm) + suffixName;
            }
            return string.Empty;
        }

        public virtual IActionResult Index(int? subId = null)
        {
            if (ViewBag.Title == null)
                ViewBag.Title = GetTitle(TitleType.List);
            OnIndex(subId);
            ViewBag.PageMenu = PageMenu;
            ViewBag.PopupEditor = PopupEditor;
            return View();
        }

        public virtual void OnIndex(int? subId = null)
        {
        }

        public virtual IActionResult ListView(int? subId = null)
        {
            ViewBag.ParentId = subId;
            var model = OnListDataLoad(subId);
            return PartialView(model);
        }

        public virtual IQueryable OnListDataLoad(int? subId = null)
        {
            return Repo.GetList();
        }




        public virtual IActionResult Index2(string subId = null)
        {
            if (ViewBag.Title == null)
                ViewBag.Title = GetTitle(TitleType.List);
            OnIndex2(subId);
            ViewBag.PageMenu = PageMenu;
            ViewBag.PopupEditor = PopupEditor;
            return View();
        }

        public virtual void OnIndex2(string subId = null)
        {
        }

        public virtual IActionResult ListView2(string subId = null)
        {
            ViewBag.ParentId = subId;
            var model = OnListDataLoad2(subId);
            return PartialView(model);
        }

        public virtual IQueryable OnListDataLoad2(string subId = null)
        {
            return Repo.GetList();
        }



        [HttpPost]
        public virtual IActionResult Delete(int id, string grid)
        {
            try
            {
                var item = Repo.GetItem(id);
                if (item == null)
                    return new JavascriptResult(StringExt.JsAlert(UI.NotFound));
                if (OnDeleteCheck(item))
                {
                    if (Repo.Delete(item))
                    {
                        OnAfterDelete(id);
                        return new JavascriptResult(!grid.IsEmpty()
                            ? string.Format(DeleteJsAction2, grid)
                            : DeleteJsAction);
                    }
                    return new JavascriptResult(StringExt.JsAlert(MvcLogger.GetErrorMessage(ModelState)));
                }
                return new JavascriptResult(StringExt.JsAlert(UI.NoAccess));

            }
            catch (Exception ex)
            {
                return new JavascriptResult(StringExt.JsAlert(MvcLogger.GetErrorMessage(ex)));
            }
        }

        public virtual bool OnDeleteCheck(T item)
        {
            return true;
        }

        public virtual void OnAfterDelete(int id)
        {
        }
    }
}
