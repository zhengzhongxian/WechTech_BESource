using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.DTOs.Dimensions
{
    public class DimensionDTO
    {
        public string DimensionId { get; set; }
        public decimal? WeightValue { get; set; }

        public decimal? HeightValue { get; set; }

        public decimal? WidthValue { get; set; }

        public decimal? LengthValue { get; set; }
    }
}
