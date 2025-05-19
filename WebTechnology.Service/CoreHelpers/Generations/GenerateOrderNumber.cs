using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Service.CoreHelpers.Generations
{
    public class GenerateOrderNumber
    {
        public static string Generate()
        {
            // Tạo 8 số ngẫu nhiên
            Random random = new Random();
            int randomNumber = random.Next(10000000, 99999999); // Số ngẫu nhiên từ 10000000 đến 99999999

            return $"ORD-{randomNumber}";
        }
    }
}
