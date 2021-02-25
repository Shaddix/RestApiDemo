using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RestApiDemo.WebApi.Pagination
{
    public class PagingList<T>
    {
        public PagingList(IEnumerable<T> data, int totalCount)
        {
            Data = data.ToList();
            TotalCount = totalCount;
        }

        [Required]
        public IList<T> Data { get; set; }

        public int TotalCount { get; set; }
    }
}