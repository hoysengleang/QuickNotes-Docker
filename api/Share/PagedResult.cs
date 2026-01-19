using System.Globalization;
using api.Models;
namespace api.Share {
    public class PagedResult<T> {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public PageMeta Meta { get; set; } = new PageMeta();
    }

    public class PageMeta {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public static PagedResult<T> Paginate<T>(IEnumerable<T> items, int totalCount, int page, int pageSize) {
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PagedResult<T> {
                Data = items,
                Meta = new PageMeta {
                    TotalCount = totalCount,
                    PageSize = pageSize,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    HasNextPage = page < totalPages,
                    HasPreviousPage = page > 1
                }
            };
        }
    }
}