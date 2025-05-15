﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTechnology.Repository.CoreHelpers.Enums
{
    public enum OrderStatusType
    {
        /// <summary>
        /// Đơn hàng đã bị hủy
        /// </summary>
        CANCELLED = 1,

        /// <summary>
        /// Đơn hàng đã hoàn thành
        /// </summary>
        COMPLETED = 2,

        /// <summary>
        /// Đơn hàng đã xác nhận
        /// </summary>
        CONFIRMED = 3,

        /// <summary>
        /// Đơn hàng đang chờ xác nhận
        /// </summary>
        PENDING = 4,

        /// <summary>
        /// Đơn hàng đang xử lý
        /// </summary>
        PROCESSING = 5,

        /// <summary>
        /// Đơn hàng đang giao hàng
        /// </summary>
        SHIPPING = 6
    }
}
