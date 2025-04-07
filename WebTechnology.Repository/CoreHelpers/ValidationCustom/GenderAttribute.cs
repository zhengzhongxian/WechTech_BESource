using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.CoreHelpers.ValidationCustom
{
    public class GenderAttribute : ValidationAttribute
    {
        private readonly string[] _allowedGenders = { "Nam", "Nữ" };
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            string gender = value.ToString();
            return _allowedGenders.Contains(gender);
        }
        public override string FormatErrorMessage(string name)
        {
            return $"Giới tính phải là 'Nam' hoặc 'Nữ'.";
        }
    }
}
