using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SCMS.CoreBusinessLogic.Paging;

namespace System.Web.Mvc
{
    public static class PagingExtension
    {
        public static MvcHtmlString Paging(this HtmlHelper html, IPagedList pagedList, string url, string pagePlaceHolder)
        {

            var sb = new StringBuilder();

            // only show paging if we have more items than the page size
            if (pagedList.TotalCount > pagedList.PageSize)
            {
                sb.Append("<div class=\"pagination\">");
                sb.Append("<ul>");

                sb.Append(String.Format("<li class=\"prev{0}\"><a href=\"{1}\">&larr; Previous</a></li>",
                    pagedList.HasPreviousPage ? "":" disabled",
                    pagedList.HasPreviousPage ? url.Replace(pagePlaceHolder, pagedList.PageIndex.ToString()) : "#"));

                for (int i = 0; i < pagedList.TotalPages; i++)
                {
                    sb.Append(String.Format("<li{0}><a href=\"{1}\">{2}</a></li>", 
                        i == pagedList.PageIndex ? " class=\"active\"" : "",
                        i == pagedList.PageIndex ? "#" : url.Replace(pagePlaceHolder, (i + 1).ToString()),
                        (i + 1)));
                }

                sb.Append(String.Format("<li class=\"next{0}\"><a href=\"{1}\">Next &rarr;</a></li>",
                    pagedList.HasNextPage ? "" : " disabled",
                    pagedList.HasNextPage ? url.Replace(pagePlaceHolder, (pagedList.PageIndex + 2).ToString()) : "#"));

                sb.Append("</ul>");
            }

            return MvcHtmlString.Create(sb.ToString());
        }
    }

    
}