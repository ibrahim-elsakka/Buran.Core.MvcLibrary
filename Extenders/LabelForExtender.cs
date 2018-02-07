using Buran.Core.MvcLibrary.Utils;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Buran.Core.MvcLibrary.Extenders
{
    public static class LabelForExtender
    {
        public static HtmlString LabelRequiredFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression,
            string requiredSymbol = " *", string requiredCssClass = "editor-field-required")
        {
            return LabelRequiredFor(html, expression, null, requiredSymbol, requiredCssClass);
        }

        public static HtmlString LabelRequiredFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes,
            string requiredSymbol = " *", string requiredCssClass = "editor-field-required")
        {
            return LabelRequiredFor(html, expression, new RouteValueDictionary(htmlAttributes), requiredSymbol, requiredCssClass);
        }

        public static HtmlString LabelRequiredFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes,
            string requiredSymbol = " *", string requiredCssClass = "editor-field-required")
        {
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, html.ViewData, html.MetadataProvider);
            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            var labelText = metadata.Metadata.DisplayName ?? metadata.Metadata.PropertyName ?? htmlFieldName.Split('.').Last();

            if (String.IsNullOrEmpty(labelText))
            {
                return HtmlString.Empty;
            }

            var sb2 = new StringBuilder();
            var tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName));

            if (metadata.Metadata.IsRequired)
            {
                var sb = new StringBuilder();
                var required = new TagBuilder("span");
                required.AddCssClass(requiredCssClass);
                required.InnerHtml.AppendHtml(requiredSymbol);
                sb.Append(required.GetString());

                tag.InnerHtml.AppendHtml(new HtmlString(labelText));
                tag.InnerHtml.AppendHtml(new HtmlString(sb.ToString()));
            }
            else
                tag.InnerHtml.AppendHtml(new HtmlString(labelText));
            sb2.Append(tag.GetString());
            return new HtmlString(sb2.ToString());
        }

        public static HtmlString LabelRequiredFor<TModel>(this IHtmlHelper<TModel> html, string labelText,
            string forName, string className = null,
            bool required = false, string requiredSymbol = " *", string requiredCssClass = "editor-field-required")
        {
            var sb2 = new StringBuilder();
            var tag = new TagBuilder("label");
            tag.AddCssClass(className);
            tag.Attributes.Add("for", forName);
            if (required)
            {
                var requiredStr = new TagBuilder("span");
                requiredStr.AddCssClass(requiredCssClass);
                requiredStr.InnerHtml.AppendHtml(requiredSymbol);

                tag.InnerHtml.AppendHtml(labelText);
                tag.InnerHtml.AppendHtml(requiredStr.GetString());
            }
            else
                tag.InnerHtml.AppendHtml(labelText);
            sb2.Append(tag.GetString());
            return new HtmlString(sb2.ToString()); ;
        }

        public static HtmlString FieldTextFor<TModel, TValue>(this IHtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, html.ViewData, html.MetadataProvider);
            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            var labelText = metadata.Metadata.DisplayName ?? metadata.Metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            return String.IsNullOrEmpty(labelText) ? HtmlString.Empty : new HtmlString(labelText);
        }

        public static HtmlString FieldNameFor<TModel, TValue>(this IHtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression, string prefix = null)
        {
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, html.ViewData, html.MetadataProvider);
            var labelText = metadata.Metadata.PropertyName;
            if (!string.IsNullOrWhiteSpace(prefix))
                labelText = prefix + labelText;
            return String.IsNullOrEmpty(labelText) ? HtmlString.Empty : new HtmlString(labelText);
        }
    }
}
