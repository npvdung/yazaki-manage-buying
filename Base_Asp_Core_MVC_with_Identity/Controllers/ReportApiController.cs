using Base_Asp_Core_MVC_with_Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;   // nếu bạn đang dùng OrderBy chuỗi

namespace MangagerBuyProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportApiController : ControllerBase
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _uid;

        public ReportApiController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> uid)
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

                // JOIN chi tiết đơn hàng với sản phẩm & đơn hàng
                var customerData = from d in _context.purchaseOrderDetails
                                   join p in _context.Products
                                        on d.ProductId equals p.ID.ToString() into tempTable
                                   from product in tempTable.DefaultIfEmpty()
                                   join o in _context.purchaseOrders
                                        on d.PurchaseOrderId equals o.ID.ToString() into tempTable2
                                   from order in tempTable2.DefaultIfEmpty()
                                   select new
                                   {
                                       // dùng camelCase đúng với JS
                                       id = d.ID,
                                       quantity = d.Quantity,
                                       taxAmount = d.TaxAmount,
                                       price = d.Price,
                                       discountAmount = d.DiscountAmount,
                                       totalAmount = d.TotalAmount,
                                       productName = product != null ? product.ProductName : ""
                                   };

                // Tính tổng tiền hàng
                double totalAmount = customerData.Sum(x => (double)x.totalAmount);

                // Sort
                if (!string.IsNullOrWhiteSpace(sortColumn) &&
                    !string.IsNullOrWhiteSpace(sortColumnDirection))
                {
                    customerData = customerData.OrderBy($"{sortColumn} {sortColumnDirection}");
                }

                // Search
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
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
