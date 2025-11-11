using Base_Asp_Core_MVC_with_Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MangagerBuyProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotaApiController : ControllerBase
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _uid;

        public QuotaApiController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> uid)
        {
            _uid = uid;
            _context = context;
        }
        public IActionResult Index()
        {
            try
            {
                //isAdmin = User.IsInRole(Roles.Admin.ToString()) ? true : false;
                //isDoctor = User.IsInRole(Roles.Doctor.ToString()) ? true : false;
                //isParent = User.IsInRole(Roles.Parent.ToString()) ? true : false;
                //idUser = _uid.GetUserId(HttpContext.User);

                var draw = Request.Query["draw"].FirstOrDefault();
                var start = Request.Query["start"].FirstOrDefault();
                var length = Request.Query["length"].FirstOrDefault();
                var sortColumn = Request.Query["columns[" + Request.Query["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Query["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Query["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                //var customerData = (from tempcustomer in _context.quota select tempcustomer);

                var customerData = from tempcustomer in _context.quota
                                   join temp1 in _context.Categories on tempcustomer.CategoryId equals temp1.ID.ToString() into tempTable
                                   from leftJoinData in tempTable.DefaultIfEmpty()
                                   join temp2 in _context.Products on tempcustomer.ProductId equals temp2.ID.ToString() into tempTable1
                                   from leftJoinData1 in tempTable1.DefaultIfEmpty()
                                   select new
                                   {
                                       // Chọn các trường từ bảng Childrens và OtherTable
                                       tempcustomer.ID,
                                       tempcustomer.QuotaCode,
                                       tempcustomer.QuotaName,
                                       categoryName = leftJoinData.CategoryName,
                                       productCode = leftJoinData1.ProductCode,
                                       usedQuantity = tempcustomer.UsedQuantity,
                                       remainingQuantity = tempcustomer.RemainingQuantity,
                                       tempcustomer.Status
                                   };
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.QuotaCode.Contains(searchValue) || m.QuotaName.Contains(searchValue));
                }

                recordsTotal = customerData.Count();
                int sttCounter = skip + 1;
                var data = customerData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
