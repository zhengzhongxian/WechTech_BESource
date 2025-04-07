using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Service.CoreHelpers.Generations
{
    public class GenerateOtp
    {
        public static string Generate()
        {
            var otp = new Random().Next(100000, 999999).ToString();
            return otp;
        }
    }
}
