using Base_Asp_Core_MVC_with_Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;   // dùng cho OrderBy với chuỗi

namespace MangagerBuyProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportApiController : ControllerBase
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _uid;

        public ReportApiController(
            Base_Asp_Core_MVC_with_IdentityContext context,
            UserManager<UserSystemIdentity> uid)
        {
            _uid = uid;
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                var draw                = Request.Query["draw"].FirstOrDefault();
                var start               = Request.Query["start"].FirstOrDefault();
                var length              = Request.Query["length"].FirstOrDefault();
                var sortColumn          = Request.Query["columns[" + Request.Query["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Query["order[0][dir]"].FirstOrDefault();
                var searchValue         = Request.Query["search[value]"].FirstOrDefault();

                int pageSize = !string.IsNullOrEmpty(length) ? Convert.ToInt32(length) : 0;
                int skip     = !string.IsNullOrEmpty(start)   ? Convert.ToInt32(start)   : 0;

                // JOIN: chi tiết đơn mua -> sản phẩm -> đơn mua -> hợp đồng -> nhà cung cấp -> tiền tệ -> yêu cầu giao hàng
                var customerData =
                    from d in _context.purchaseOrderDetails
                    join p in _context.Products
                        on d.ProductId equals p.ID.ToString() into tempTable
                    from product in tempTable.DefaultIfEmpty()
                    join o in _context.purchaseOrders
                        on d.PurchaseOrderId equals o.ID.ToString() into tempTable2
                    from order in tempTable2.DefaultIfEmpty()
                    join c in _context.purchaseContracts
                        on order.PurchaseContractId equals c.ID.ToString() into contractTable
                    from contract in contractTable.DefaultIfEmpty()
                    join v in _context.vendors
                        on contract.VenderId equals v.ID.ToString() into vendorTable
                    from vendor in vendorTable.DefaultIfEmpty()
                    join cur in _context.Currency
                        on vendor.CurrencyId equals cur.ID.ToString() into currencyTable
                    from currency in currencyTable.DefaultIfEmpty()
                    join ship in _context.shipmentRequests
                        on order.ID.ToString() equals ship.PurchaseOrderId into shipTable
                    from ship in shipTable.DefaultIfEmpty()
                    select new
                    {
                        id             = d.ID,
                        quantity       = d.Quantity,
                        taxAmount      = d.TaxAmount,
                        price          = d.Price,
                        discountAmount = d.DiscountAmount,
                        totalAmount    = d.TotalAmount,
                        orderCode      = order != null ? order.PurchaseOrderCode : string.Empty,

                        productName    = product != null ? product.ProductName : string.Empty,

                        // ngày giao hàng
                        orderDate      = ship != null ? ship.DateShip : null,

                        // ngày thanh toán gần nhất
                        paymentDate =
                            (from bp in _context.BillPayment
                             join ir in _context.ItemReceipt
                                 on bp.ItemReceiptId equals ir.ID.ToString()
                             join ship2 in _context.shipmentRequests
                                 on ir.ShipmentRequestId equals ship2.ID.ToString()
                             where ship2.PurchaseOrderId == order.ID.ToString()
                             orderby bp.BillPaymentDate descending
                             select bp.BillPaymentDate).FirstOrDefault(),

                        vendorName   = vendor   != null ? vendor.VendorName : string.Empty,

                        // LẤY TIỀN TỆ THEO CHUỖI: Vendor -> Currency.Symbol
                        currencyName = currency != null ? currency.Symbol    : string.Empty
                    };

                // Tính tổng tiền hàng
                double totalAmountAll = customerData.Sum(x => (double)x.totalAmount);

                // Sort
                if (!string.IsNullOrWhiteSpace(sortColumn) &&
                    !string.IsNullOrWhiteSpace(sortColumnDirection))
                {
                    customerData = customerData.OrderBy($"{sortColumn} {sortColumnDirection}");
                }

                // Search – tìm theo tên hàng, nhà cung cấp, loại tiền
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m =>
                        m.productName.Contains(searchValue) ||
                        m.vendorName.Contains(searchValue)  ||
                        m.currencyName.Contains(searchValue)||
                        m.orderCode.Contains(searchValue));
                }

                var recordsTotal = customerData.Count();

                var data = customerData.Skip(skip).Take(pageSize).ToList();

                return Ok(new
                {
                    draw,
                    recordsTotal,
                    recordsFiltered = recordsTotal,
                    totalAmount = totalAmountAll,
                    data
                });
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
