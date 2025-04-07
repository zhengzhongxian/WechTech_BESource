using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.CoreHelpers.Enums
{
    public static class RoleHelper
    {
        private static readonly Dictionary<RoleType, string> RoleIdMap = new Dictionary<RoleType, string>
        {
            { RoleType.Admin, "d3f3c3b3-5b5b-4b4b-9b9b-7b7b7b7b7b7b" },
            { RoleType.Customer, "e4f4c4b4-6c6c-4c4c-9c9c-8c8c8c8c8c8c" }
        };

        public static string ToRoleIdString(this RoleType roleType)
        {
            return RoleIdMap[roleType];
        }

        public static RoleType ToRoleType(this string roleId)
        {
            return RoleIdMap.FirstOrDefault(x => x.Value == roleId).Key;
        }
    }
}
