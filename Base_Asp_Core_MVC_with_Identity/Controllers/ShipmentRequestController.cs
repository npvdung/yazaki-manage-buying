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
    public class ShipmentRequestController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        private readonly IAuditLogService _auditLogService;

        public ShipmentRequestController(
            Base_Asp_Core_MVC_with_IdentityContext context,
            UserManager<UserSystemIdentity> userManager,
            ICommonService commonService,
            IAuditLogService auditLogService)
        {
            _context = context;
            _userManager = userManager;
            _commonService = commonService;
            _auditLogService = auditLogService;
        }

        public Base_Asp_Core_MVC_with_IdentityContext Get_context() => _context;

        private void PopulateShipmentRequestViewData()
        {
            // PO dropdown: Value = ID, Text = Code
            var poList = _context.purchaseOrders
                .Select(po => new SelectListItem
                {
                    Value = po.ID.ToString(),
                    Text = po.PurchaseOrderCode
                })
                .ToList();
            ViewData["PurchaseOrders"] = poList;

            var statusList = Enum.GetValues(typeof(EnumShip))
                .Cast<EnumShip>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.GetDisplayName() ?? e.ToString()
                })
                .ToList();
            ViewData["ShipStatus"] = statusList;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            IEnumerable<ShipmentRequest> objCatlist = _context.shipmentRequests
                .OrderByDescending(x => x.ID)
                .ToList();

            return View(objCatlist);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            string prefix = "SHIP_";
            Expression<Func<ShipmentRequest, string>> codeSelector = c => c.ShipmentRequestCode;
            string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);

            var model = new ShipmentRequest
            {
                ShipmentRequestCode = autoCode,
                Status = EnumShip.ProcessShip.ToString(),
                DateShip = DateTime.Now
            };

            var user = await _userManager.GetUserAsync(User);
            model.EmplouyeeId = user.Id;

            PopulateShipmentRequestViewData();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ShipmentRequest empobj)
        {
            var user = await _userManager.GetUserAsync(User);
            empobj.EmplouyeeId = user.Id;

            if (!ModelState.IsValid)
            {
                PopulateShipmentRequestViewData();
                return View(empobj);
            }

            _context.shipmentRequests.Add(empobj);
            _context.SaveChanges();

            await _auditLogService.LogAsync(
                userId: User.Identity.Name,
                action: "Create",
                tableName: "ShipmentRequest",
                recordId: empobj.ID.ToString(),
                changes: JsonConvert.SerializeObject("log"));

            TempData["ResultOk"] = "Tạo dữ liệu thành công !";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Guid Id)
        {
            var empfromdb = _context.shipmentRequests.Find(Id);
            if (empfromdb == null)
                return NotFound();

            PopulateShipmentRequestViewData();

            // lấy contract để đổ số tiền
            var po = _context.purchaseOrders.FirstOrDefault(v => v.ID.ToString() == empfromdb.PurchaseOrderId);
            if (po != null)
            {
                var contract = _context.purchaseContracts.FirstOrDefault(p => p.ID.ToString() == po.PurchaseContractId);
                if (contract != null)
                {
                    empfromdb.TotalAmount = Math.Round(contract.TotalAmount ?? 0m, 2);
                    empfromdb.TotalAmountIncludeTax = Math.Round(contract.TotalAmountIncludeTax ?? 0m, 2);
                    empfromdb.TotalDiscoutAmount = Math.Round(contract.TotalDiscoutAmount ?? 0m, 2);
                    empfromdb.TotalAmountIncludeTaxAndDiscount = Math.Round(contract.TotalAmountIncludeTaxAndDiscount ?? 0m, 2);
                }
            }

            if (empfromdb.Status == null)
                empfromdb.Status = EnumShip.ProcessShip.ToString();

            return View(empfromdb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(ShipmentRequest empobj)
        {
            if (!ModelState.IsValid)
            {
                PopulateShipmentRequestViewData();
                return View(empobj);
            }

            _context.shipmentRequests.Update(empobj);
            _context.SaveChanges();

            await _auditLogService.LogAsync(
                userId: User.Identity.Name,
                action: "Update",
                tableName: "ShipmentRequest",
                recordId: empobj.ID.ToString(),
                changes: JsonConvert.SerializeObject("log"));

            TempData["ResultOk"] = "Cập nhập dữ liệu thành công !";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetDataForDropdown(string id, string type)
        {
            if (type == "Manufacturer")
            {
                var po = _context.purchaseOrders.FirstOrDefault(v => v.ID.ToString() == id);
                if (po == null)
                    return NotFound("Order not found");

                var contract = _context.purchaseContracts
                    .FirstOrDefault(p => p.ID.ToString() == po.PurchaseContractId);

                var result = new
                {
                    totalAmount = contract?.TotalAmount ?? 0,
                    totalAmountIncludeTax = contract?.TotalAmountIncludeTax ?? 0,
                    totalDiscoutAmount = contract?.TotalDiscoutAmount ?? 0,
                    totalAmountIncludeTaxAndDiscount = contract?.TotalAmountIncludeTaxAndDiscount ?? 0,
                };

                return Json(result);
            }

            return Json(new { value = "" });
        }
    }
}
