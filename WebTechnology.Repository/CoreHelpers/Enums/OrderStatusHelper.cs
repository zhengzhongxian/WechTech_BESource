﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.CoreHelpers.Enums
{
    public static class OrderStatusHelper
    {
        private static readonly Dictionary<OrderStatusType, string> OrderStatusIdMap = new Dictionary<OrderStatusType, string>
        {
            { OrderStatusType.CANCELLED, "CANCELLED" },
            { OrderStatusType.COMPLETED, "COMPLETED" },
            { OrderStatusType.CONFIRMED, "CONFIRMED" },
            { OrderStatusType.PENDING, "PENDING" },
            { OrderStatusType.PROCESSING, "PROCESSING" },
            { OrderStatusType.SHIPPING, "SHIPPING" }
        };

        public static string ToOrderStatusIdString(this OrderStatusType orderStatusType)
        {
            return OrderStatusIdMap[orderStatusType];
        }

        public static OrderStatusType ToOrderStatusType(this string orderStatusId)
        {
            return OrderStatusIdMap.FirstOrDefault(x => x.Value == orderStatusId).Key;
        }
    }
}
