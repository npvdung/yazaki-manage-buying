using Base_Asp_Core_MVC_with_Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;

namespace MangagerBuyProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportViewApiController : ControllerBase
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _uid;

        public ReportViewApiController(
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

                // ===============================
                // JOIN LẤY ĐẦY ĐỦ THÔNG TIN TRẢ HÀNG
                // ===============================
                var customerData =
                    from d in _context.purchaseOrderDetails
                    join o in _context.purchaseOrders
                        on d.PurchaseOrderId equals o.ID.ToString() into poTable
                    from order in poTable.DefaultIfEmpty()

                    join ra in _context.returnAuthorizations
                        on order.ID.ToString() equals ra.PurchaseOrderId into raTable
                    from ra in raTable.DefaultIfEmpty()
                    where ra != null   // chỉ lấy đơn trả hàng

                    join p in _context.Products
                        on d.ProductId equals p.ID.ToString() into prodTable
                    from product in prodTable.DefaultIfEmpty()

                    join c in _context.purchaseContracts
                        on order.PurchaseContractId equals c.ID.ToString() into contractTable
                    from contract in contractTable.DefaultIfEmpty()

                    join v in _context.vendors
                        on contract.VenderId equals v.ID.ToString() into vendorTable
                    from vendor in vendorTable.DefaultIfEmpty()

                    join cur in _context.Currency
                        on vendor.CurrencyId equals cur.ID.ToString() into currencyTable
                    from currency in currencyTable.DefaultIfEmpty()

                    select new
                    {
                        id             = d.ID,
                        quantity       = d.Quantity,
                        taxAmount      = d.TaxAmount,
                        price          = d.Price,
                        discountAmount = d.DiscountAmount,
                        totalAmount    = d.TotalAmount,

                        productName = product != null ? product.ProductName : string.Empty,

                        // ⭐ LẤY NGÀY TRẢ LẠI HÀNG
                        returnDate = ra != null ? ra.DateShip : (DateTime?)null,
                        orderCode      = ra.ReturnAuthorizationCode,

                        // ⭐ LÝ DO TRẢ HÀNG -> Note
                        reasonReturn = ra != null ? ra.Note : string.Empty,

                        vendorName   = vendor   != null ? vendor.VendorName : string.Empty,
                        currencyName = currency != null ? currency.Symbol    : string.Empty
                    };

                // Tổng tiền hàng
                double totalAmount =
                    customerData.Sum(x => (double)(x.totalAmount ?? 0m));

                // Sort
                if (!string.IsNullOrWhiteSpace(sortColumn) &&
                    !string.IsNullOrWhiteSpace(sortColumnDirection))
                {
                    customerData = customerData.OrderBy($"{sortColumn} {sortColumnDirection}");
                }

                // Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m =>
                        m.productName.Contains(searchValue) ||
                        m.vendorName.Contains(searchValue)  ||
                        m.currencyName.Contains(searchValue) ||
                        m.orderCode.Contains(searchValue));
                }

                var recordsTotal = customerData.Count();
                var data = customerData.Skip(skip).Take(pageSize).ToList();

                return Ok(new
                {
                    draw,
                    recordsTotal,
                    recordsFiltered = recordsTotal,
                    totalAmount,
                    data
                });
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
