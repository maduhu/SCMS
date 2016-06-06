using System.Collections.Generic;

namespace SCMS.CoreBusinessLogic.Paging
{
    public interface IPagedList
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        int CurrentPageFrom { get; }
        int CurrentPageTo { get; }
    }

    /// <summary>
    /// Paged list interface
    /// </summary>
    public interface IPagedList<T> : IList<T>, IPagedList
    {
        
    }
}
