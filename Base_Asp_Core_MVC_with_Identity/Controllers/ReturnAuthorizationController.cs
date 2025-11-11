using Base_Asp_Core_MVC_with_Identity.CommonFile.Enum;
using Base_Asp_Core_MVC_with_Identity.CommonFile.IServiceCommon;
using Base_Asp_Core_MVC_with_Identity.CommonMethod;
using Base_Asp_Core_MVC_with_Identity.Data;
using Base_Asp_Core_MVC_with_Identity.Models;
using MangagerBuyProduct.CommonFile.IServiceCommon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace MangagerBuyProduct.Controllers
{
    public class ReturnAuthorizationController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        private readonly IAuditLogService _auditLogService;
        public ReturnAuthorizationController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService, IAuditLogService auditLogService)
        {
            _context = context;
            _userManager = userManager;
            _commonService = commonService;
            _auditLogService = auditLogService;
        }

        public Base_Asp_Core_MVC_with_IdentityContext Get_context()
        {
            return _context;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            IEnumerable<ReturnAuthorization> objCatlist = _context.returnAuthorizations;
            return View(objCatlist);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ReturnAuthorization objItem = new ReturnAuthorization();
            string prefix = "IG_";
            Expression<Func<ReturnAuthorization, string>> codeSelector = c => c.ReturnAuthorizationCode;
            string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);
            objItem.ReturnAuthorizationCode = autoCode;
            List<PurchaseOrder> lst3 = _context.purchaseOrders.ToList();
            List<SelectListItem> dropData3 = new List<SelectListItem>();
            foreach (var item in lst3)
            {
                if (item.ID.ToString() != null)
                {
                    dropData3.Add(new SelectListItem { Text = item.ID.ToString(), Value = item.PurchaseOrderCode.ToString() });
                }
            }
            var productList = (from p in _context.Products
                               select new
                               {
                                   ProductId = p.ID,
                                   ProductName = p.ProductName,
                               }).ToList();

            List<SelectListItem> itemData = new List<SelectListItem>();

            foreach (var item in productList)
            {
                itemData.Add(new SelectListItem
                {
                    Text = $"{item.ProductName}",
                    Value = item.ProductId.ToString()
                });
            }
            ViewBag.product_lst = itemData;
            ViewBag.PurchaseOrderLst = dropData3;
            objItem.DateShip = DateTime.Now;
            return View(objItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ReturnAuthorization empobj)
        {

            if (ModelState.IsValid)
            {
                empobj.ID = Guid.NewGuid();
                _context.returnAuthorizations.Add(empobj);
                _context.SaveChanges();
                ////var temp = _context.shipmentRequests.Find(Guid.Parse(empobj.ShipmentRequestId));

                //temp.Status = EnumShip.DoneShip.ToString();
                //_context.Update(temp);
                //_context.SaveChanges();

                var temp3 = _context.purchaseOrders.Find(Guid.Parse(empobj.PurchaseOrderId));
                temp3.Status = (int)EnumPurchaseOrder.CancelShip;
                _context.Update(temp3);
                _context.SaveChanges();
                await _auditLogService.LogAsync(
               userId: User.Identity.Name,
               action: "Create",
               tableName: "ReturnAuthorization",
               recordId: Guid.NewGuid().ToString(),
               changes: JsonConvert.SerializeObject("log") // Ghi thông tin chi tiết sản phẩm
       );
                TempData["ResultOk"] = "Tạo dữ liệu thành công !";
                return RedirectToAction("Index");
            }

            return View(empobj);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Guid Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var empfromdb = _context.returnAuthorizations.Find(Id);
            List<PurchaseOrder> lst3 = _context.purchaseOrders.ToList();
            List<SelectListItem> dropData3 = new List<SelectListItem>();
            foreach (var item in lst3)
            {
                if (item.ID.ToString() != null)
                {
                    dropData3.Add(new SelectListItem { Text = item.ID.ToString(), Value = item.PurchaseOrderCode.ToString() });
                }
            }

            ViewBag.PurchaseOrderLst = dropData3;


            if (empfromdb == null)
            {
                return NotFound();
            }
            return View(empfromdb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(ReturnAuthorization empobj)
        {
            if (ModelState.IsValid)
            {
                _context.returnAuthorizations.Update(empobj);
                _context.SaveChanges();
                await _auditLogService.LogAsync(
             userId: User.Identity.Name,
             action: "Create",
             tableName: "ReturnAuthorization",
             recordId: Guid.NewGuid().ToString(),
             changes: JsonConvert.SerializeObject("log") // Ghi thông tin chi tiết sản phẩm
     );
                TempData["ResultOk"] = "Cập nhập dữ liệu thành công !";
                return RedirectToAction("Index");
            }
            return View(empobj);
        }
        [HttpGet]
        public IActionResult GetDataForDropdown(string id, string type)
        {
            // Kiểm tra loại dropdown để lấy dữ liệu
            if (type == "Manufacturer")
            {
                var item = _context.purchaseOrders.FirstOrDefault(v => v.ID.ToString() == id);
                if (item == null)
                {
                    return NotFound("Vendor not found");
                }

                // Lấy thông tin các sản phẩm liên quan đến Vendor
                var Contract = _context.purchaseContracts
                    .Where(p => p.ID.ToString() == item.PurchaseContractId)
                    .FirstOrDefault();
                var details = _context.purchaseContractDetails.Where(x => x.PurchaseContractId == Contract.ID.ToString()).ToList();
                // Tạo đối tượng trả về
                var result = new
                {
                    TotalAmount = Contract.TotalAmount, // Tên loại tiền tệ
                    TotalAmountIncludeTax = Contract.TotalAmountIncludeTax,    // Địa chỉ Vendor
                    TotalDiscoutAmount = Contract.TotalDiscoutAmount, // Người liên hệ
                    TotalAmountIncludeTaxAndDiscount = Contract.TotalAmountIncludeTaxAndDiscount,
                    Details = details,// Điều khoản thanh toán
                };

                return Json(result);

            }

            return Json(new { value = "" });
        }
    }
}
