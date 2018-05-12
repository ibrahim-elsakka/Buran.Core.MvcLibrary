using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buran.Core.Library.Reflection;
using Buran.Core.Library.Utils;
using Buran.Core.MvcLibrary.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Buran.Core.MvcLibrary.LogUtil;
using Buran.Core.MvcLibrary.Resource;
using Buran.Core.MvcLibrary.AdminPanel.Controls;
using Buran.Core.MvcLibrary.AdminPanel.Utils;

namespace Buran.Core.MvcLibrary.AdminPanel.Controllers
{
    public class EditController<T, Z> : ListController<T, Z>
         where T : class, new()
         where Z : class
    {
        protected string ViewEditPopup = "EditPopup";
        protected string ViewEdit = "Edit";
        protected string ViewCreatePopup = "CreatePopup";
        protected string ViewCreate = "Create";

        protected string CreateAction = "Create";
        protected string CreateJsAction = "";
        protected string EditAction = "Edit";
        protected string EditJsAction = "";

        protected string CreateSaveAndCreateUrl = string.Empty;
        protected string CreateReturnListUrl = string.Empty;
        protected string EditReturnListUrl = string.Empty;

        protected EditController(bool popupEditor, Z context)
         : base(popupEditor, context)
        {
            if (popupEditor)
            {
                EditAction = "EditPopup";
                CreateAction = "CreatePopup";
            }
        }

        public override void OnIndex(int? subId = null)
        {
            PageMenu.Items.Add(new EditorPageMenuItem
            {
                ItemType = EditPageMenuItemType.Insert,
                Title = UI.New,
                IconClass = "fa fa-plus",
                Url = Url.Action("Create")
            });

            base.OnIndex(subId);
        }

        public virtual void AddNewItem(T item)
        {
        }

        #region CREATE
        public virtual IActionResult Create()
        {
            var _queryDictionary = QueryHelpers.ParseQuery(Request.QueryString.ToString());
            var _queryItems = _queryDictionary.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
            var gridItem = _queryItems.FirstOrDefault(d => d.Key == "grid");
            var grid = "";
            grid = gridItem.Value;

            ViewBag.EditMode = false;
            ViewBag.CreateAction = CreateAction;
            ViewBag.Title = GetTitle(TitleType.Create);
            ViewBag.Grid = grid;
            BuildCreateMenu();

            var item = new T();
            AddNewItem(item);

            ViewBag.PageMenu = PageMenu;
            return View(PopupEditor ? ViewCreatePopup : ViewCreate, item);
        }

        public virtual void OnCreateSaveItem(T item)
        {

        }

        public virtual void OnAfterCreateSaveItem(T item)
        {

        }

        [HttpPost]
        public virtual IActionResult Create(int keepEdit, T item)
        {
            OnCreateSaveItem(item);
            if (Repo.Create(item))
            {
                OnAfterCreateSaveItem(item);
                if (keepEdit == 0)
                    return CreateReturnListUrl.IsEmpty()
                        ? (ActionResult)RedirectToAction("Index")
                        : Redirect(CreateReturnListUrl);
                if (keepEdit == 1)
                {
                    var keyFieldName = Digger2.GetKeyFieldNameFirst(typeof(T));
                    var itemIdValue = Digger.GetObjectValue(item, keyFieldName);
                    return RedirectToAction("Edit", new { id = itemIdValue });
                }
                return CreateSaveAndCreateUrl.IsEmpty()
                    ? (ActionResult)RedirectToAction("Create")
                    : Redirect(CreateSaveAndCreateUrl);
            }
            ViewBag.EditMode = false;
            ViewBag.CreateAction = CreateAction;
            ViewBag.Title = GetTitle(TitleType.Create);
            BuildCreateMenu();
            OnErrorCreateSaveItem(item);
            ViewBag.PageMenu = PageMenu;
            return View(PopupEditor ? ViewCreatePopup : ViewCreate, item);
        }

        public virtual void OnErrorCreateSaveItem(T item)
        {

        }

        [HttpPost]
        public virtual JsonResult CreatePopup(T item)
        {
            var r = new JsonResultViewModel();
            OnCreateSaveItem(item);
            if (Repo.Create(item))
            {
                OnAfterCreateSaveItem(item);
                r.Ok = true;
                if (!CreateJsAction.IsEmpty())
                    r.JsFunction = CreateJsAction;
            }
            else
                r.Error = MvcLogger.GetErrorMessage(ModelState);
            return Json(r);
        }
        #endregion


        #region EDIT
        public virtual void OnEditItem(T item)
        {

        }

        public virtual IActionResult Edit(int id)
        {
            var item = Repo.GetItem(id);
            if (item == null)
                return NotFound();
            if (!OnEditCheck(item))
                return NotFound();

            var _queryDictionary = QueryHelpers.ParseQuery(Request.QueryString.ToString());
            var _queryItems = _queryDictionary.SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value)).ToList();
            var gridItem = _queryItems.FirstOrDefault(d => d.Key == "grid");
            var grid = "";
            grid = gridItem.Value;

            ViewBag.EditMode = true;
            ViewBag.EditAction = EditAction;
            ViewBag.Title = GetTitle(TitleType.Editor);
            ViewBag.Grid = grid;
            BuildEditMenu(id);

            OnEditItem(item);

            ViewBag.KeyFieldName = Digger2.GetKeyFieldNameFirst(typeof(T));
            ViewBag.KeyFieldValue = id;

            ViewBag.PageMenu = PageMenu;
            return View(PopupEditor ? ViewEditPopup : ViewEdit, item);
        }

        public virtual bool OnEditCheck(T item)
        {
            return true;
        }

        public virtual void OnEditSaveItem(T item)
        {

        }

        public virtual bool OnEditSaveCheck(T item)
        {
            return true;
        }

        private int _editId;
        [HttpPost]
        public virtual IActionResult Edit(int keepEdit, T item)
        {
            var keyFieldName = Digger2.GetKeyFieldNameFirst(typeof(T));
            var v = Digger.GetObjectValue(item, keyFieldName);
            if (v != null)
            {
                int.TryParse(v.ToString(), out _editId);
                if (_editId > 0)
                {
                    var org = Repo.GetItem(_editId);
                    var x = TryUpdateModelAsync(org);
                    if (OnEditSaveCheck(org))
                    {
                        OnEditSaveItem(org);
                        if (Repo.Edit(org))
                        {
                            if (keepEdit == 0)
                                return EditReturnListUrl.IsEmpty()
                                    ? (ActionResult)RedirectToAction("Index")
                                    : Redirect(EditReturnListUrl);

                            return RedirectToAction("Edit", new { id = _editId });
                        }
                        ViewBag.EditMode = true;
                        ViewBag.EditAction = EditAction;
                        ViewBag.Title = GetTitle(TitleType.Editor);
                        BuildEditMenu(_editId);
                        ViewBag.PageMenu = PageMenu;
                        OnEditItem(org);
                        return View(PopupEditor ? ViewEditPopup : ViewEdit, org);
                    }
                    return NotFound();
                }
            }
            return NotFound();
        }

        [HttpPost]
        public virtual async Task<JsonResult> EditPopup(T item)
        {
            var r = new JsonResultViewModel();
            var keyFieldName = Digger2.GetKeyFieldNameFirst(typeof(T));
            var v = Digger.GetObjectValue(item, keyFieldName);
            if (v != null)
            {
                int.TryParse(v.ToString(), out _editId);
                if (_editId > 0)
                {
                    var org = Repo.GetItem(_editId);
                    var x = await TryUpdateModelAsync(org);

                    OnEditSaveItem(org);
                    if (Repo.Edit(org))
                    {
                        r.Ok = true;
                        if (!EditJsAction.IsEmpty())
                            r.JsFunction = EditJsAction;
                    }
                    else
                        r.Error = MvcLogger.GetErrorMessage(ModelState);
                }
            }
            else
                r.Error = "NOT FOUND";
            return Json(r);
        }

        #endregion
    }
}
