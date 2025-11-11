using Base_Asp_Core_MVC_with_Identity.CommonFile.IServiceCommon;
using Base_Asp_Core_MVC_with_Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                //var customerData = (from tempcustomer in _context.Categories select tempcustomer);

                var customerData = from tempcustomer in _context.purchaseOrderDetails
                                   join temp1 in _context.Products on tempcustomer.ProductId equals temp1.ID.ToString() into tempTable
                                   from leftJoinData in tempTable.DefaultIfEmpty()
                                   join temp2 in _context.purchaseOrders on tempcustomer.PurchaseOrderId equals temp2.ID.ToString() into tempTable2
                                   from leftJoinData2 in tempTable2.DefaultIfEmpty()
                                   select new
                                   {
                                       tempcustomer.ID,
                                       tempcustomer.Quantity,
                                       tempcustomer.TaxAmount,
                                       tempcustomer.Price,
                                       tempcustomer.DiscountAmount,
                                       tempcustomer.TotalAmount,
                                       leftJoinData.ProductName,
                                   };

                var totalAmount = (double)customerData.Sum(x => x.TotalAmount);
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.ProductName.Contains(searchValue));
                }

                recordsTotal = customerData.Count();
                int sttCounter = skip + 1;
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                return Ok(new
                {
                    draw,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    totalAmount,
                    data
                });
                //var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                //return Ok(jsonData);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
