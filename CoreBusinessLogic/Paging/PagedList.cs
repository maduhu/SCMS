using System.Collections.Generic;
using System.Linq;

namespace SCMS.CoreBusinessLogic.Paging
{
    /// <summary>
    /// Paged list
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public int CurrentPageFrom { get; private set; }
        public int CurrentPageTo { get; private set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            int total = source.Count();
            this.TotalCount = total;
            this.TotalPages = pageSize > 0 ? total / pageSize : 0;

            if (pageSize > 0 && total % pageSize > 0)
                TotalPages++;

            if (pageSize > 0)
            {
                this.PageSize = pageSize;
                this.PageIndex = pageIndex;
                this.AddRange(source.Skip(pageIndex*pageSize).Take(pageSize).ToList());
            }else
            {
                this.AddRange(source.ToList());
                this.PageSize = Count;
                this.PageIndex = 0;
            }

            if(Count > 0)
            {
                CurrentPageFrom = (PageSize*PageIndex) + 1;
                CurrentPageTo = CurrentPageFrom + Count - 1;
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        public PagedList(IList<T> source, int pageIndex, int pageSize)
        {
            TotalCount = source.Count();
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
            if (Count > 0)
            {
                CurrentPageFrom = (PageSize * PageIndex) + 1;
                CurrentPageTo = CurrentPageFrom + Count - 1;
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="totalCount">Total count</param>
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int totalCount)
        {
            TotalCount = totalCount;
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.AddRange(source);
            if (Count > 0)
            {
                CurrentPageFrom = (PageSize * PageIndex) + 1;
                CurrentPageTo = CurrentPageFrom + Count - 1;
            }
        }

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }
        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }
    }
}
