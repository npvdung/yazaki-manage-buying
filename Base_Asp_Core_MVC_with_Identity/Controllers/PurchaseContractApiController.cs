using Base_Asp_Core_MVC_with_Identity.CommonFile.Enum;
using Base_Asp_Core_MVC_with_Identity.CommonFile.IServiceCommon;
using Base_Asp_Core_MVC_with_Identity.Data;
using Base_Asp_Core_MVC_with_Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace MangagerBuyProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseContractApiController : ControllerBase
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        public PurchaseContractApiController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService)
        {
            _context = context;
            _userManager = userManager;
            _commonService = commonService;
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
                //var customerData = (from tempcustomer in _context.Categories select tempcustomer);

                var customerData = from tempcustomer in _context.purchaseContracts
                                   join temp1 in _context.vendors on tempcustomer.VenderId equals temp1.ID.ToString() into tempTable
                                   from leftJoinData in tempTable.DefaultIfEmpty()
                                   select new
                                   {
                                       tempcustomer.ID,
                                       tempcustomer.PurchaseContractCode,
                                       tempcustomer.PurchaseContractName,
                                       leftJoinData.VendorName,
                                       tempcustomer.TotalAmountIncludeTaxAndDiscount,
                                       tempcustomer.Status
                                   };
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m => m.PurchaseContractName.Contains(searchValue) || m.PurchaseContractCode.Contains(searchValue));
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
        [HttpPost]
        [Route("Approved")]
        public IActionResult Approved(Guid id)
        {
            Guid tempId = Guid.Empty;
            if (id != null)
            {
                var empfromdb = _context.purchaseContracts.Find(id);
                PurchaseOrder temp = new PurchaseOrder();
                string prefix = "ORDER_";
                Expression<Func<PurchaseOrder, string>> codeSelector = c => c.PurchaseOrderCode;
                string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);

                temp.PurchaseOrderCode = autoCode;
                temp.EmployeeId = empfromdb.EmployeeId;
                temp.PurchaseContractId = empfromdb.ID.ToString();
                temp.Description = empfromdb.Description;
                temp.Status = (int)EnumPurchaseOrder.WaitShip;
                _context.purchaseOrders.Add(temp);
                _context.SaveChanges();

                var empDetailsContract = _context.purchaseContractDetails.Where(x => x.PurchaseContractId == id.ToString()).ToList();
                foreach (var item in empDetailsContract)
                {
                    var temObj = new PurchaseOrderDetails
                    {
                        PurchaseOrderId = temp.ID.ToString(),
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        TaxAmount = item.TaxAmount,
                        Price = item.Price,
                        DiscountAmount = item.DiscountAmount,
                        TotalAmount = item.TotalAmount,
                    };
                    _context.purchaseOrderDetails.Add(temObj);
                    _context.SaveChanges();
                }
                empfromdb.Status = (int)EnumPurchaseContract.Done;
                _context.purchaseContracts.Update(empfromdb);
                _context.SaveChanges();
                tempId = temp.ID;
                
            }
            return Ok(new { tempId = tempId });
            //return RedirectToAction("Edit", "PurchaseOrder", new { id = "08dd0bcf-94c4-4890-8073-ec061c953937" });
            //return RedirectToAction("Edit", "PurchaseOrder", new { Id = tempId });

        }
    }
}
