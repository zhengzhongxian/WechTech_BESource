using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Products
{
    public class GetListProductTrends
    {
        public string? Ptsid { get; set; }

        public string? Productid { get; set; }

        public string? Trend { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
