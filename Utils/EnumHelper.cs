using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Buran.Core.MvcLibrary.Utils
{
    public static class EnumHelper
    {
        private class EnumList
        {
            public int Value { get; set; }
            public string Name { get; set; }
        }

        public static SelectList GetSelectList(Type type, object selectedItem = null)
        {
            var rList = new List<EnumList>();
            var fields = type.GetFields();
            foreach (var field in fields.Where(d => d.IsLiteral))
            {
                var addItem = new EnumList();
                var displayAttr = (DisplayAttribute[])field.GetCustomAttributes(typeof(DisplayAttribute), false);
                var value = field.GetValue(null);
                addItem.Value = (int)Convert.ChangeType(value, typeof(int));
                if (displayAttr.Length > 0)
                    addItem.Name = displayAttr[0].GetName();
                rList.Add(addItem);
            }
            return new SelectList(rList, "Value", "Name", selectedItem);
        }

        public static string GetEnumDisplayText(Type type, int id)
        {
            var fields = type.GetFields();
            var addItem = new EnumList();
            foreach (var field in fields.Where(d => d.IsLiteral && (int)d.GetValue(null) == id))
            {
                var displayAttr = (DisplayAttribute[])field.GetCustomAttributes(typeof(DisplayAttribute), false);
                if (displayAttr.Length > 0)
                    return displayAttr[0].GetName();
            }
            return string.Empty;
        }

        public static string GetEnumDisplayText(Type type, string name)
        {
            var fields = type.GetFields();
            foreach (var field in fields.Where(d => d.IsLiteral))
            {
                var displayAttr = (DisplayAttribute[])field.GetCustomAttributes(typeof(DisplayAttribute), false);
                if (field.Name != name) continue;
                if (displayAttr.Length > 0)
                    return displayAttr[0].GetName();
            }
            return string.Empty;
        }
    }
}