using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.Text;

namespace SCMS.UI.GeneralHelper
{
    public static class CheckBoxList
    {
        /// <summary>
        /// Custom Helper for a CheckBoxList
        /// http://miroprocessordev.blogspot.com/2012/04/generic-checkboxlist-in-aspnet-mvc-3.html
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="helper"></param>
        /// <param name="name"></param>
        /// <param name="items"></param>
        /// <param name="textField"></param>
        /// <param name="valueField"></param>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        public static MvcHtmlString CheckBoxLists<T>(this HtmlHelper helper,
                                               String name,
                                               IEnumerable<T> items,
                                               String textField,
                                               String valueField,
                                               IEnumerable<T> selectedItems = null)
        {
            Type itemstype = typeof(T);
            PropertyInfo textfieldInfo = itemstype.GetProperty(textField, typeof(String));
            PropertyInfo valuefieldInfo = itemstype.GetProperty(valueField);

            TagBuilder tag;
            StringBuilder checklist = new StringBuilder();
            foreach (var item in items)
            {
                tag = new TagBuilder("input");
                tag.Attributes["type"] = "checkbox";
                tag.Attributes["value"] = valuefieldInfo.GetValue(item, null).ToString();
                tag.Attributes["name"] = name;
                if (selectedItems != null && selectedItems.Contains(item))
                {
                    tag.Attributes["checked"] = "checked";
                }
                tag.InnerHtml = textfieldInfo.GetValue(item, null).ToString();
                checklist.Append(tag.ToString());
                checklist.Append("<br />");
            }
            return MvcHtmlString.Create(checklist.ToString());
        }
    }
}