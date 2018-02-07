using Buran.Core.Library.Reflection;
using Buran.Core.MvcLibrary.Grid.Columns;
using Buran.Core.MvcLibrary.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Web;

namespace Buran.Core.MvcLibrary.Grid
{
    public class DataValueConverter
    {
        public object Value { get; set; }
        public string Label { get; set; }
    }

    class ValueConverter
    {
        private static Regex RegexDataFormat = new Regex(@"{(\w+)}", RegexOptions.Compiled & RegexOptions.IgnoreCase);
        private static Regex RegexDataFormat2 = new Regex(@"{(\w+(\.\w+)+)}", RegexOptions.Compiled & RegexOptions.IgnoreCase);

        public static string GetFieldValue(object item, string fieldName)
        {
            var value = Digger.GetObjectValue(item, fieldName) ?? String.Empty;
            return value.ToString();
        }

        public static object ValueToLabel(List<DataValueConverter> valueConverters, object value)
        {
            if (valueConverters.Count > 0)
            {
                foreach (var converter in valueConverters)
                {
                    if (value.Equals(converter.Value))
                    {
                        return converter.Label;
                    }
                }
            }
            return value;
        }

        public static object InspectDataFormat(object item, DataColumn field, string dataFormat = null)
        {
            if (field.EditorType == ColumnTypes.Link)
            {
                dataFormat = HttpUtility.UrlDecode(dataFormat);
            }

            if (string.IsNullOrWhiteSpace(dataFormat))
                dataFormat = field.DataFormat;
            if (string.IsNullOrWhiteSpace(dataFormat))
            {
                var fieldValue = ValueToLabel(field.ValueConverter, Digger.GetObjectValue(item, field.FieldName));
                return fieldValue ?? string.Empty;
            }
            var val = dataFormat;

            var ma = RegexDataFormat.Matches(dataFormat);
            if (ma.Count > 0)
            {
                foreach (Match m in ma)
                    if (m.Success)
                    {
                        var fieldName = m.Groups[1].Value;
                        var fieldValue = Digger.GetObjectValue(item, fieldName);
                        if (field.FieldName == fieldName)
                            fieldValue = ValueToLabel(field.ValueConverter, fieldValue);
                        var fv = fieldValue != null ? fieldValue.ToString() : string.Empty;
                        val = val.Replace(m.ToString(), fv);
                    }
            }
            ma = RegexDataFormat2.Matches(dataFormat);
            if (ma.Count > 0)
            {
                foreach (Match m in ma)
                    if (m.Success)
                    {
                        var fieldName = m.Groups[1].Value;
                        var fieldValue = Digger.GetObjectValue(item, fieldName);
                        if (field.FieldName == fieldName)
                            fieldValue = ValueToLabel(field.ValueConverter, fieldValue);
                        var fv = fieldValue != null ? fieldValue.ToString() : string.Empty;
                        val = val.Replace(m.ToString(), fv);
                    }
            }
            return val;
        }

        public static string GetValue(IHtmlHelper helper, object item, DataColumn field)
        {
            var urlEncoder = UrlEncoder.Create(new TextEncoderSettings());
            var r = string.Empty;

            #region IMAGE
            if (field.EditorType == ColumnTypes.Image)
            {
                var col = field as ImageColumn;

                var valUrl = InspectDataFormat(item, field, col.ImageUrlFormat);
                if (valUrl == null)
                    return r;

                r += "<img src='" + valUrl + "'";
                if (col.ImageSize.Height > 0)
                    r += " height='" + urlEncoder.Encode(col.ImageSize.Height.ToString()) + "'";
                if (col.ImageSize.Width > 0)
                    r += " width='" + urlEncoder.Encode(col.ImageSize.Width.ToString()) + "'";
                r += "/>";
            }
            #endregion
            #region LINK
            else if (field.EditorType == ColumnTypes.Link)
            {
                var col = field as LinkColumn;
                var valUrl = InspectDataFormat(item, field, col.NavigateUrlFormat);
                var label = string.IsNullOrWhiteSpace(col.Text) ? InspectDataFormat(item, field) : col.Text;
                if (valUrl == null)
                    return r;

                r += "<a href='" + valUrl + "'";
                if (!string.IsNullOrWhiteSpace(col.Target))
                    r += " target='" + col.Target + "'";
                if (!string.IsNullOrWhiteSpace(col.CssClass))
                    r += " class='" + col.CssClass + "'";
                r += ">" + label + "</a>";
            }
            #endregion
            #region CHECKBOX
            else if (field.EditorType == ColumnTypes.CheckBox)
            {
                var val = InspectDataFormat(item, field);
                if (val == null)
                    return r;
                r += "<input name='select-" + field.FieldName +
                    "' id='select-" + field.FieldName + "' type='checkbox' value='" + val + "'/>";
            }
            #endregion
            else
            {
                var val = InspectDataFormat(item, field);
                if (val == null)
                    return r;

                var valType = val.GetType();

                if (valType.IsEnum)
                {
                    r = helper.Encode(EnumHelper.GetEnumDisplayText(valType, (int)val));
                }
                else if (valType.FullName == "System.Boolean")
                {
                    bool bVal;
                    if (bool.TryParse(val.ToString(), out bVal))
                    {
                        r += "<input type='checkbox' disabled='disabled' value='" + bVal + "'";
                        if (bVal)
                            r += " checked='checked'";
                        r += "/>";
                    }
                    else r = helper.Encode(val);
                }
                else if (valType.FullName.IndexOf("System.DateTime") > -1)
                {
                    DateTime bVal;
                    r = helper.Encode(DateTime.TryParse(val.ToString(), out bVal) ? bVal.ToString(field.Format) : val);
                }
                else if (valType.FullName.IndexOf("System.Int32") > -1)
                {
                    int bVal;
                    r = helper.Encode(int.TryParse(val.ToString(), out bVal) ? bVal.ToString(field.Format) : val);
                }
                else if (valType.FullName.IndexOf("System.Decimal") > -1)
                {
                    decimal bVal;
                    r = helper.Encode(decimal.TryParse(val.ToString(), out bVal) ? bVal.ToString(field.Format) : val);
                }
                else
                {
                    r = field.EditorType == ColumnTypes.Label ? helper.Encode(val.ToString()) : val.ToString();
                }
            }
            return r;
        }
    }
}
