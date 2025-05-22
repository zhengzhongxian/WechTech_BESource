# Thay đổi cần thực hiện để kiểm tra IsRoot = false khi áp dụng voucher

## 1. Thay đổi trong OrderService.cs

Trong file `WebTechnology.Service/Services/Implementations/OrderService.cs`, cần thêm điều kiện kiểm tra `IsRoot = false` sau đoạn kiểm tra `UsageLimit` (khoảng dòng 357):

```csharp
// Tìm voucher theo mã
var vouchers = await _voucherRepository.FindAsync(v => v.Code == voucherCode && v.IsActive == true);
var voucher = vouchers.FirstOrDefault();

if (voucher == null) continue;

// Kiểm tra điều kiện áp dụng voucher
if (voucher.MinOrder.HasValue && productTotal < voucher.MinOrder.Value)
    continue;

if (voucher.StartDate > DateTime.UtcNow || voucher.EndDate < DateTime.UtcNow)
    continue;

if (voucher.UsageLimit.HasValue && voucher.UsedCount >= voucher.UsageLimit)
    continue;

// Chỉ áp dụng voucher không phải là voucher gốc (IsRoot = false)
if (voucher.IsRoot == true)
    continue;
```

## 2. Thay đổi trong ApplyVoucherRepository.cs

Trong file `WebTechnology.Repository/Repositories/Implementations/ApplyVoucherRepository.cs`, cần thêm điều kiện kiểm tra `IsRoot = false` sau đoạn kiểm tra `UsageLimit` (khoảng dòng 63):

```csharp
// Verificar si el voucher está dentro de su período de validez
var now = DateTime.UtcNow;
if (voucher.StartDate > now || voucher.EndDate < now)
{
    return false; // Voucher fuera de período de validez
}

// Verificar si el voucher ha alcanzado su límite de uso
if (voucher.UsageLimit.HasValue && voucher.UsedCount >= voucher.UsageLimit)
{
    return false; // Voucher ha alcanzado su límite de uso
}

// Verificar si el voucher no es un voucher raíz (IsRoot = false)
if (voucher.IsRoot == true)
{
    return false; // No se puede aplicar un voucher raíz
}
```

## Giải thích

Những thay đổi này sẽ đảm bảo rằng:

1. Trong quá trình tạo đơn hàng, hệ thống sẽ chỉ áp dụng các voucher có `IsRoot = false` (không phải voucher gốc).
2. Trong repository khi áp dụng voucher vào đơn hàng, hệ thống cũng sẽ kiểm tra điều kiện `IsRoot = false`.

Điều này giúp đảm bảo rằng các voucher gốc (IsRoot = true) sẽ không được áp dụng trực tiếp vào đơn hàng, mà chỉ được sử dụng làm mẫu để tạo ra các voucher con cho người dùng.
