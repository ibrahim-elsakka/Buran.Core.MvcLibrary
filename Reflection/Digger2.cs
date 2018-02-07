using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Buran.Core.MvcLibrary.Reflection
{
    public static class Digger2
    {
        public static T GetMetaAttr<T>(ModelMetadata model)
        {
            if (model is DefaultModelMetadata defaultMetadata)
            {
                var displayAttribute = defaultMetadata.Attributes.Attributes
                    .OfType<T>()
                    .FirstOrDefault();
                if (displayAttribute != null)
                {
                    return displayAttribute;
                }
            }
            return default(T);
        }

        public static string GetTableName(Type type, bool clearTire = false)
        {
            var r = type.Name;
            var data = type.GetCustomAttributes(typeof(TableAttribute), false);
            if (data.Any())
            {
                var tableAttribute = data.First() as TableAttribute;
                if (tableAttribute != null)
                    r = tableAttribute.Name;
            }
            if (r.IndexOf("_", StringComparison.Ordinal) > -1)
                r = r.Split('_')[0];
            return r;
        }

        public static List<string> GetKeyFieldName(Type type)
        {
            var list = new List<string>();
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                var exists = prop.GetCustomAttributes(typeof(KeyAttribute), false).Any();
                if (exists)
                    list.Add(prop.Name);
            }
            return list;
        }

        public static string GetKeyFieldNameFirst(Type type)
        {
            return GetKeyFieldName(type).First();
        }
    }
}
