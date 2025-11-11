using Base_Asp_Core_MVC_with_Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MangagerBuyProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorApiController : ControllerBase
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _uid;

        public VendorApiController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> uid)
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
                //var customerData = (from tempcustomer in _context.vendors select tempcustomer);

                var customerData = from tempcustomer in _context.vendors
                                   join tempA in _context.Currency on tempcustomer.CurrencyId equals tempA.ID.ToString() into tempTable
                                   from leftJoinData in tempTable.DefaultIfEmpty()
                                   select new
                                   {
                                       // Chọn các trường từ bảng Childrens và OtherTable
                                       tempcustomer.ID,
                                       tempcustomer.VendorCode,
                                       tempcustomer.VendorName,
                                       tempcustomer.Phone,
                                       tempcustomer.Status,
                                       //tempcustomer.Description,
                                       // Thêm các trường khác từ OtherTable
                                       leftJoinData.Symbol
                                   };
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.VendorCode.Contains(searchValue) || m.VendorName.Contains(searchValue));
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
