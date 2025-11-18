using Base_Asp_Core_MVC_with_Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MangagerBuyProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportViewApiController : ControllerBase
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _uid;

        public ReportViewApiController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> uid)
        {
            _uid = uid;
            _context = context;
        }
        public IActionResult Index()
        {
            try
            {
                var draw = Request.Query["draw"].FirstOrDefault();
                var start = Request.Query["start"].FirstOrDefault();
                var length = Request.Query["length"].FirstOrDefault();
                var sortColumn = Request.Query["columns[" + Request.Query["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Query["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Query["search[value]"].FirstOrDefault();

                int pageSize = !string.IsNullOrEmpty(length) ? Convert.ToInt32(length) : 0;
                int skip = !string.IsNullOrEmpty(start) ? Convert.ToInt32(start) : 0;

                // Lấy các DÒNG ĐÃ TRẢ HÀNG:
                // chi tiết đơn mua (purchaseOrderDetails) mà có ReturnAuthorization tương ứng,
                // sau đó join với Products để lấy tên hàng.
                var customerData = from d in _context.purchaseOrderDetails
                                join r in _context.returnAuthorizations
                                        on d.PurchaseOrderId equals r.PurchaseOrderId
                                join p in _context.Products
                                        on d.ProductId equals p.ID.ToString()
                                select new
                                {
                                    id = d.ID,
                                    quantity = d.Quantity,
                                    taxAmount = d.TaxAmount,
                                    price = d.Price,
                                    discountAmount = d.DiscountAmount,
                                    totalAmount = d.TotalAmount,
                                    productName = p.ProductName
                                };

                double totalAmount = customerData.Sum(x => (double)x.totalAmount);

                // Sort
                if (!string.IsNullOrWhiteSpace(sortColumn) &&
                    !string.IsNullOrWhiteSpace(sortColumnDirection))
                {
                    customerData = customerData.OrderBy($"{sortColumn} {sortColumnDirection}");
                }

                // Search theo tên hàng
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.productName.Contains(searchValue));
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
