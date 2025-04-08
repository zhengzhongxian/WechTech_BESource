using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.Repository.Models.Pagination;

namespace WebTechnology.Service.Models
{
    public class PaginatedResult<T>
    {
        public PaginationMetadata Metadata { get; set; }
        public List<T> Items { get; set; }

        public PaginatedResult(List<T> items, PaginationMetadata metadata)
        {
            Items = items;
            Metadata = metadata;
        }
    }
}
