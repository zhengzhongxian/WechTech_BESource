using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebTechnology.Repository.DTOs.Cart;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.Interfaces;

namespace WebTechnology.API.Controllers
{
    /// <summary>
    /// API quản lý giỏ hàng
    /// </summary>
    /// <remarks>
    /// Controller này cung cấp các API để quản lý giỏ hàng của khách hàng, bao gồm:
    /// - Thêm sản phẩm vào giỏ hàng
    /// - Lấy danh sách sản phẩm trong giỏ hàng
    /// - Cập nhật số lượng sản phẩm trong giỏ hàng
    /// - Xóa sản phẩm khỏi giỏ hàng
    ///
    /// Tất cả các API trong controller này đều yêu cầu xác thực và chỉ dành cho khách hàng.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "CustomerOnly")]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;
        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng
        /// </summary>
        /// <remarks>
        /// API này cho phép khách hàng thêm sản phẩm vào giỏ hàng của mình.
        ///
        /// **Quyền truy cập:**
        /// - Chỉ khách hàng đã đăng nhập mới có thể thêm sản phẩm vào giỏ hàng
        /// - Yêu cầu token JWT trong header Authorization
        ///
        /// **Cấu trúc dữ liệu gửi lên:**
        /// - **productId**: ID của sản phẩm cần thêm vào giỏ hàng (bắt buộc)
        /// - **quantity**: Số lượng sản phẩm (bắt buộc, phải > 0)
        ///
        /// **Xử lý của hệ thống:**
        /// - Kiểm tra sản phẩm tồn tại: Nếu sản phẩm không tồn tại, trả về lỗi 404
        /// - Kiểm tra số lượng tồn kho: Nếu số lượng yêu cầu vượt quá tồn kho, trả về lỗi 400
        /// - Thêm sản phẩm vào giỏ hàng: Nếu sản phẩm đã có trong giỏ hàng, cập nhật số lượng
        ///
        /// **Ví dụ request:**
        /// ```
        /// POST /api/CartItem/add-to-cart
        /// Authorization: {token}
        /// Content-Type: application/json
        ///
        /// {
        ///   "productId": "123456",
        ///   "quantity": 2
        /// }
        /// ```
        ///
        /// **Ví dụ response thành công:**
        /// ```json
        /// {
        ///   "success": true,
        ///   "message": "Thêm vào giỏ hàng thành công",
        ///   "statusCode": 200,
        ///   "data": "Thêm vào giỏ hàng thành công"
        /// }
        /// ```
        ///
        /// **Ví dụ response lỗi (sản phẩm không tồn tại):**
        /// ```json
        /// {
        ///   "success": false,
        ///   "message": "Sản phẩm không tồn tại",
        ///   "statusCode": 404,
        ///   "data": null
        /// }
        /// ```
        ///
        /// **Ví dụ response lỗi (số lượng vượt quá tồn kho):**
        /// ```json
        /// {
        ///   "success": false,
        ///   "message": "Số lượng yêu cầu (5) vượt quá số lượng tồn kho (3)",
        ///   "statusCode": 400,
        ///   "data": null
        /// }
        /// ```
        ///
        /// **Ví dụ response lỗi (sản phẩm đã hết hàng):**
        /// ```json
        /// {
        ///   "success": false,
        ///   "message": "Sản phẩm đã hết hàng",
        ///   "statusCode": 400,
        ///   "data": null
        /// }
        /// ```
        /// </remarks>
        /// <param name="cartItem">Thông tin sản phẩm cần thêm vào giỏ hàng</param>
        /// <returns>Kết quả thêm sản phẩm vào giỏ hàng</returns>
        /// <response code="200">Thêm sản phẩm vào giỏ hàng thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ hoặc số lượng vượt quá tồn kho</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="404">Không tìm thấy sản phẩm</response>
        /// <response code="500">Lỗi server</response>
        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] CreateCartItemDTO cartItem)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _cartItemService.AddToCart(cartItem, token);
            return StatusCode((int)response.StatusCode, response);
        }
        /// <summary>
        /// Lấy danh sách sản phẩm trong giỏ hàng có phân trang
        /// </summary>
        /// <remarks>
        /// API này trả về danh sách các sản phẩm trong giỏ hàng của khách hàng hiện tại, có phân trang.
        ///
        /// <para><strong>Quyền truy cập:</strong></para>
        /// <list type="bullet">
        /// <item>Chỉ khách hàng đã đăng nhập mới có thể xem giỏ hàng của mình</item>
        /// <item>Yêu cầu token JWT trong header Authorization</item>
        /// </list>
        ///
        /// <para><strong>Các tham số truy vấn:</strong></para>
        /// <list type="bullet">
        /// <item><c>pageNumber</c>: Số trang (bắt đầu từ 1, mặc định: 1)</item>
        /// <item><c>pageSize</c>: Số lượng sản phẩm mỗi trang (tối đa 50, mặc định: 10)</item>
        /// </list>
        ///
        /// <para><strong>Xử lý của hệ thống:</strong></para>
        /// <list type="bullet">
        /// <item>Lấy ID khách hàng từ token JWT</item>
        /// <item>Lấy danh sách sản phẩm trong giỏ hàng của khách hàng</item>
        /// <item>Kiểm tra trạng thái tồn tại của sản phẩm: Nếu sản phẩm không còn tồn tại, thêm trường isProductExists = false</item>
        /// <item>Kiểm tra số lượng tồn kho: Nếu số lượng trong giỏ hàng vượt quá tồn kho, thêm trường isStockAvailable = false</item>
        /// <item>Phân trang kết quả theo pageNumber và pageSize</item>
        /// </list>
        ///
        /// <para><strong>Ví dụ request:</strong></para>
        /// <code>
        /// GET /api/CartItem/get-list-cart-item?pageNumber=1&amp;pageSize=10
        /// Authorization: Bearer {token}
        /// </code>
        ///
        /// <para><strong>Ví dụ response thành công:</strong></para>
        /// <code>
        /// {
        ///   "success": true,
        ///   "message": "Lấy danh sách sản phẩm trong giỏ hàng thành công nhé các FE",
        ///   "statusCode": 200,
        ///   "data": {
        ///     "items": [
        ///       {
        ///         "id": "cart-item-123",
        ///         "cartId": "cart-456",
        ///         "productId": "prod-789",
        ///         "quantity": 2,
        ///         "getProductToCart": {
        ///           "productName": "Sữa tươi nguyên kem",
        ///           "productPriceIsDefault": 250000,
        ///           "productPriceIsActive": 230000,
        ///           "productImgData": "base64-encoded-image-data",
        ///           "totalPrice": 460000
        ///         },
        ///         "isProductExists": true,
        ///         "isStockAvailable": true,
        ///         "isOutOfStock": false,
        ///         "availableStock": 10
        ///       }
        ///     ],
        ///     "metadata": {
        ///       "totalCount": 5,
        ///       "pageSize": 10,
        ///       "currentPage": 1,
        ///       "totalPages": 1,
        ///       "hasNext": false,
        ///       "hasPrevious": false
        ///     }
        ///   }
        /// }
        /// </code>
        ///
        /// <para><strong>Lưu ý cho frontend developers:</strong></para>
        ///
        /// <para><strong>Các trường kiểm tra trạng thái sản phẩm:</strong></para>
        /// <list type="bullet">
        /// <item><c>isProductExists</c>: Cho biết sản phẩm còn tồn tại trong hệ thống hay không
        ///   <list type="bullet">
        ///     <item><c>true</c>: Sản phẩm vẫn còn tồn tại và đang hoạt động</item>
        ///     <item><c>false</c>: Sản phẩm đã bị xóa hoặc vô hiệu hóa</item>
        ///   </list>
        /// </item>
        /// <item><c>isOutOfStock</c>: Cho biết sản phẩm đã hết hàng hay chưa
        ///   <list type="bullet">
        ///     <item><c>true</c>: Sản phẩm đã hết hàng hoàn toàn (availableStock = 0)</item>
        ///     <item><c>false</c>: Sản phẩm vẫn còn hàng (availableStock > 0)</item>
        ///   </list>
        /// </item>
        /// <item><c>isStockAvailable</c>: Cho biết số lượng trong giỏ hàng có vượt quá tồn kho hay không
        ///   <list type="bullet">
        ///     <item><c>true</c>: Số lượng trong giỏ hàng hợp lệ (quantity ≤ availableStock)</item>
        ///     <item><c>false</c>: Số lượng trong giỏ hàng vượt quá tồn kho (quantity > availableStock)</item>
        ///   </list>
        /// </item>
        /// <item><c>availableStock</c>: Số lượng tồn kho hiện tại của sản phẩm
        ///   <list type="bullet">
        ///     <item>Giá trị số nguyên cho biết chính xác số lượng sản phẩm còn trong kho</item>
        ///     <item>Nếu = 0, sản phẩm đã hết hàng</item>
        ///   </list>
        /// </item>
        /// </list>
        ///
        /// <para><strong>Logic xử lý trên frontend:</strong></para>
        /// <list type="bullet">
        /// <item>Nếu <c>isProductExists = false</c>:
        ///   <list type="bullet">
        ///     <item>Hiển thị thông báo "Sản phẩm không còn tồn tại"</item>
        ///     <item>Vô hiệu hóa nút tăng/giảm số lượng</item>
        ///     <item>Thêm nút "Xóa" để loại bỏ sản phẩm khỏi giỏ hàng</item>
        ///   </list>
        /// </item>
        /// <item>Nếu <c>isOutOfStock = true</c>:
        ///   <list type="bullet">
        ///     <item>Hiển thị thông báo "Sản phẩm đã hết hàng"</item>
        ///     <item>Vô hiệu hóa nút tăng số lượng</item>
        ///     <item>Có thể hiển thị nút "Thông báo khi có hàng"</item>
        ///   </list>
        /// </item>
        /// <item>Nếu <c>isStockAvailable = false</c>:
        ///   <list type="bullet">
        ///     <item>Hiển thị thông báo "Số lượng vượt quá tồn kho (còn X sản phẩm)"</item>
        ///     <item>Tự động điều chỉnh số lượng về giá trị tối đa có thể (= availableStock)</item>
        ///     <item>Hoặc hiển thị cảnh báo và yêu cầu người dùng điều chỉnh</item>
        ///   </list>
        /// </item>
        /// <item>Sử dụng <c>availableStock</c> để:
        ///   <list type="bullet">
        ///     <item>Hiển thị số lượng tồn kho còn lại (VD: "Còn 5 sản phẩm")</item>
        ///     <item>Giới hạn số lượng tối đa có thể đặt</item>
        ///     <item>Hiển thị cảnh báo khi tồn kho thấp (VD: "Chỉ còn 2 sản phẩm")</item>
        ///   </list>
        /// </item>
        /// </list>
        ///
        /// <para><strong>Phân trang:</strong></para>
        /// <list type="bullet">
        /// <item>Sử dụng thông tin phân trang trong <c>metadata</c> để hiển thị điều hướng trang</item>
        /// </list>
        /// </remarks>
        /// <returns>Danh sách sản phẩm trong giỏ hàng có phân trang</returns>
        /// <response code="200">Trả về danh sách sản phẩm trong giỏ hàng</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="404">Không tìm thấy giỏ hàng</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("get-list-cart-item")]
        public async Task<IActionResult> GetListCartItem([FromQuery] CartItemQueryRequest request)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _cartItemService.GetListCartItem(token, request);
            return StatusCode((int)response.StatusCode, response);
        }
        /// <summary>
        /// Cập nhật sản phẩm trong giỏ hàng
        /// </summary>
        /// <remarks>
        /// API này cho phép khách hàng cập nhật thông tin sản phẩm trong giỏ hàng của mình.
        ///
        /// **Quyền truy cập:**
        /// - Chỉ khách hàng đã đăng nhập mới có thể cập nhật giỏ hàng của mình
        /// - Yêu cầu token JWT trong header Authorization
        ///
        /// **Cấu trúc dữ liệu gửi lên:**
        /// - Sử dụng JSON Patch để cập nhật từng trường cụ thể
        /// - Chỉ có thể cập nhật trường `quantity` (số lượng)
        ///
        /// **Xử lý của hệ thống:**
        /// - Kiểm tra sản phẩm tồn tại trong giỏ hàng: Nếu không tồn tại, trả về lỗi 404
        /// - Kiểm tra số lượng tồn kho: Nếu số lượng mới vượt quá tồn kho, trả về lỗi 400
        /// - Cập nhật số lượng sản phẩm trong giỏ hàng
        ///
        /// **Ví dụ request:**
        /// ```
        /// PATCH /api/CartItem/update-cart-item/cart-item-123
        /// Authorization: Bearer {token}
        /// Content-Type: application/json-patch+json
        ///
        /// [
        ///   {
        ///     "op": "replace",
        ///     "path": "/quantity",
        ///     "value": 3
        ///   }
        /// ]
        /// ```
        ///
        /// **Ví dụ response thành công:**
        /// ```json
        /// {
        ///   "success": true,
        ///   "message": "Cập nhật sản phẩm trong giỏ hàng thành công",
        ///   "statusCode": 200,
        ///   "data": "Cập nhật sản phẩm trong giỏ hàng thành công"
        /// }
        /// ```
        ///
        /// **Ví dụ response lỗi (số lượng vượt quá tồn kho):**
        /// ```json
        /// {
        ///   "success": false,
        ///   "message": "Số lượng yêu cầu (5) vượt quá số lượng tồn kho (3)",
        ///   "statusCode": 400,
        ///   "data": null
        /// }
        /// ```
        ///
        /// **Ví dụ response lỗi (sản phẩm đã hết hàng):**
        /// ```json
        /// {
        ///   "success": false,
        ///   "message": "Sản phẩm đã hết hàng",
        ///   "statusCode": 400,
        ///   "data": null
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">ID của sản phẩm trong giỏ hàng cần cập nhật</param>
        /// <param name="patchDoc">Thông tin cập nhật theo định dạng JSON Patch</param>
        /// <returns>Kết quả cập nhật sản phẩm trong giỏ hàng</returns>
        /// <response code="200">Cập nhật sản phẩm trong giỏ hàng thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ hoặc số lượng vượt quá tồn kho</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="404">Không tìm thấy sản phẩm trong giỏ hàng</response>
        /// <response code="500">Lỗi server</response>
        [HttpPatch("update-cart-item/{id}")]
        public async Task<IActionResult> UpdateCartItem(string id, [FromBody] JsonPatchDocument<CartItem> patchDoc)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _cartItemService.UpdateCartItem(id, patchDoc, token);
            return StatusCode((int)response.StatusCode, response);
        }
        /// <summary>
        /// Xóa sản phẩm khỏi giỏ hàng
        /// </summary>
        /// <remarks>
        /// API này cho phép khách hàng xóa sản phẩm khỏi giỏ hàng của mình.
        ///
        /// **Quyền truy cập:**
        /// - Chỉ khách hàng đã đăng nhập mới có thể xóa sản phẩm khỏi giỏ hàng của mình
        /// - Yêu cầu token JWT trong header Authorization
        ///
        /// **Xử lý của hệ thống:**
        /// - Kiểm tra sản phẩm tồn tại trong giỏ hàng: Nếu không tồn tại, trả về lỗi 404
        /// - Kiểm tra giỏ hàng thuộc về người dùng hiện tại: Nếu không, trả về lỗi 403
        /// - Xóa sản phẩm khỏi giỏ hàng
        ///
        /// **Ví dụ request:**
        /// ```
        /// DELETE /api/CartItem/delete-cart-item/cart-item-123
        /// Authorization: Bearer {token}
        /// ```
        ///
        /// **Ví dụ response thành công:**
        /// ```json
        /// {
        ///   "success": true,
        ///   "message": "Xóa sản phẩm trong giỏ hàng thành công",
        ///   "statusCode": 200,
        ///   "data": "Xóa sản phẩm trong giỏ hàng thành công"
        /// }
        /// ```
        ///
        /// **Ví dụ response lỗi (không tìm thấy sản phẩm trong giỏ hàng):**
        /// ```json
        /// {
        ///   "success": false,
        ///   "message": "Không tìm thấy sản phẩm trong giỏ hàng",
        ///   "statusCode": 404,
        ///   "data": null
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">ID của sản phẩm trong giỏ hàng cần xóa</param>
        /// <returns>Kết quả xóa sản phẩm khỏi giỏ hàng</returns>
        /// <response code="200">Xóa sản phẩm khỏi giỏ hàng thành công</response>
        /// <response code="400">Dữ liệu không hợp lệ</response>
        /// <response code="401">Không có quyền truy cập</response>
        /// <response code="403">Không có quyền xóa sản phẩm này</response>
        /// <response code="404">Không tìm thấy sản phẩm trong giỏ hàng</response>
        /// <response code="500">Lỗi server</response>
        [HttpDelete("delete-cart-item/{id}")]
        public async Task<IActionResult> DeleteCartItem(string id)
        {
            string token = Request.Headers["Authorization"].ToString();
            var response = await _cartItemService.DeleteCartItem(id, token);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
