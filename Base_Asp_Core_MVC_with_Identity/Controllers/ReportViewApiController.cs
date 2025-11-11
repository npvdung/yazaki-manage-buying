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
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                //var customerData = (from tempcustomer in _context.Categories select tempcustomer);

                //var customerData = from tempcustomer in _context.purchaseOrderDetails
                //                    join temp1 in _context.returnAuthorizations on tempcustomer.PurchaseOrderId equals temp1.PurchaseOrderId into tempTable
                //                   from leftJoinData in tempTable.DefaultIfEmpty()
                //                   //join temp2 in _context.purchaseOrderDetails on tempcustomer.PurchaseOrderId equals temp2.ID.ToString() into tempTable2
                //                   //from leftJoinData2 in tempTable2.DefaultIfEmpty()
                //                   join temp3 in _context.Products on tempcustomer.ProductId equals temp3.ID.ToString() into tempTable3
                //                   from leftJoinData3 in tempTable3.DefaultIfEmpty()
                //                   select new
                //                   {
                //                       tempcustomer.ID,
                //                       tempcustomer.Quantity,
                //                       tempcustomer.TaxAmount,
                //                       tempcustomer.Price,
                //                       tempcustomer.DiscountAmount,
                //                       tempcustomer.TotalAmount,
                //                       leftJoinData3.ProductName,
                //                   };

                var customerData = from tempcustomer in _context.purchaseOrderDetails
                                   join temp1 in _context.returnAuthorizations
                                   on tempcustomer.PurchaseOrderId equals temp1.PurchaseOrderId
                                   join temp3 in _context.Products
                                   on tempcustomer.ProductId equals temp3.ID.ToString()
                                   select new
                                   {
                                       tempcustomer.ID,
                                       tempcustomer.Quantity,
                                       tempcustomer.TaxAmount,
                                       tempcustomer.Price,
                                       tempcustomer.DiscountAmount,
                                       tempcustomer.TotalAmount,
                                       temp3.ProductName
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
