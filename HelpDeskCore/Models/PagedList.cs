using System;
using System.Collections;
using System.Collections.Generic;

namespace HelpDeskCore.Models
{
  public class PagedList<T> : IEnumerable<T>
  {
    public PagedList(IEnumerable<T> items, int itemCount, int pageNumber, int pageSize, int total)
    {
      if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
      Items = items ?? throw new ArgumentNullException(nameof(items));
      PageNumber = pageNumber;
      PageSize = pageSize;
      ItemCount = itemCount;
      TotalCount = total;
    }

    public IEnumerable<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int ItemCount { get; }
    public int TotalCount { get; }

    public int PageCount { get => Convert.ToInt32(Math.Ceiling(TotalCount / (double)PageSize)); }

    public bool MorePages { get => PageNumber < PageCount; }

    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
  }
}
