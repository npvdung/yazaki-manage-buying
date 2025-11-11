using Base_Asp_Core_MVC_with_Identity.CommonFile.Enum;
using Base_Asp_Core_MVC_with_Identity.CommonFile.IServiceCommon;
using Base_Asp_Core_MVC_with_Identity.Data;
using Base_Asp_Core_MVC_with_Identity.Models;
using MangagerBuyProduct.CommonFile.IServiceCommon;
using MangagerBuyProduct.Models.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace MangagerBuyProduct.Controllers
{
    public class PurchaseOrderController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        private readonly IAuditLogService _auditLogService;

        public PurchaseOrderController(
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

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            IEnumerable<PurchaseOrder> objCatlist = _context.purchaseOrders;
            return View(objCatlist);
        }

        // ================= helper cho Shipment =================
        private void PopulateShipmentViewData()
        {
            // danh sách đơn mua (PO)
            var poSelect = _context.purchaseOrders
                .Select(po => new SelectListItem
                {
                    Value = po.ID.ToString(),          // VALUE phải là ID
                    Text = po.PurchaseOrderCode        // TEXT để hiển thị
                })
                .ToList();

            ViewData["PurchaseOrders"] = poSelect;

            // danh sách trạng thái ship
            var statusSelect = Enum.GetValues(typeof(EnumShip))
                .Cast<EnumShip>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString()
                })
                .ToList();

            ViewData["ShipStatus"] = statusSelect;
        }

        // ================== EDIT (GET) cũ của bạn giữ nguyên ==================
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Guid Id)
        {
            // ... phần này mình để nguyên vì không dính lỗi hiện tại ...
            // (nếu muốn cũng có thể chuyển qua ViewData như PurchaseContract)
            var empfromdb = _context.purchaseOrders.Find(Id);
            if (empfromdb == null) return NotFound();

            ViewBag.Statuslst = Enum.GetValues(typeof(ApprodedStatus))
                .Cast<ApprodedStatus>()
                .Select(e => new SelectListItem
                {
                    Text = e.ToString(),
                    Value = ((int)e).ToString()
                }).ToList();

            var contractSelect = _context.purchaseContracts
                .Select(pc => new SelectListItem
                {
                    Value = pc.ID.ToString(),
                    Text = pc.PurchaseContractName
                }).ToList();
            ViewBag.purchaseContractLst = contractSelect;

            var productSelect = _context.Products
                .Select(p => new SelectListItem
                {
                    Value = p.ID.ToString(),
                    Text = p.ProductName
                }).ToList();
            ViewBag.productlst = productSelect;

            var viewObj = new PurchaseOrderView
            {
                ID = empfromdb.ID,
                PurchaseOrderCode = empfromdb.PurchaseOrderCode,
                PurchaseContractId = empfromdb.PurchaseContractId,
                EmployeeId = empfromdb.EmployeeId,
                Description = empfromdb.Description,
                Status = empfromdb.Status,
                PurchaseOrderDetails = new List<PurchaseOrderDetails>()
            };

            var dataTemp = _context.purchaseContracts
                .FirstOrDefault(x => x.ID.ToString() == empfromdb.PurchaseContractId);

            if (dataTemp != null)
            {
                viewObj.TotalAmount = Math.Round(dataTemp.TotalAmount ?? 0m, 2);
                viewObj.TotalAmountIncludeTax = Math.Round(dataTemp.TotalAmountIncludeTax ?? 0m, 2);
                viewObj.TotalDiscoutAmount = Math.Round(dataTemp.TotalDiscoutAmount ?? 0m, 2);
                viewObj.TotalAmountIncludeTaxAndDiscount = Math.Round(dataTemp.TotalAmountIncludeTaxAndDiscount ?? 0m, 2);
            }

            var empDetails = _context.purchaseOrderDetails
                .Where(x => x.PurchaseOrderId == Id.ToString())
                .ToList();

            foreach (var item in empDetails)
                viewObj.PurchaseOrderDetails.Add(item);

            return View(viewObj);
        }

        // ================== SHIPMENT (GET) ==================
        public IActionResult Shipment(Guid Id)
        {
            var order = _context.purchaseOrders.Find(Id);
            if (order == null)
                return NotFound();

            // tạo code ship
            string prefix = "SHIP_";
            Expression<Func<ShipmentRequest, string>> codeSelector = c => c.ShipmentRequestCode;
            string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);

            var model = new ShipmentRequest
            {
                ShipmentRequestCode = autoCode,
                PurchaseOrderId = Id.ToString()
            };

            // nạp dropdown
            PopulateShipmentViewData();

            // đổ tiền từ contract
            var contract = _context.purchaseContracts
                .FirstOrDefault(x => x.ID.ToString() == order.PurchaseContractId);

            if (contract != null)
            {
                model.TotalAmount = Math.Round(contract.TotalAmount ?? 0m, 2);
                model.TotalAmountIncludeTax = Math.Round(contract.TotalAmountIncludeTax ?? 0m, 2);
                model.TotalDiscoutAmount = Math.Round(contract.TotalDiscoutAmount ?? 0m, 2);
                model.TotalAmountIncludeTaxAndDiscount = Math.Round(contract.TotalAmountIncludeTaxAndDiscount ?? 0m, 2);
                model.EmplouyeeId = contract.EmployeeId;
            }

            model.Status = EnumShip.ProcessShip.ToString();
            model.DateShip = DateTime.Now;

            return View(model);
        }

        // ================== SHIPMENT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Shipment(ShipmentRequest empobj)
        {
            if (!ModelState.IsValid)
            {
                // QUAN TRỌNG: nạp lại dropdown khi validate fail
                PopulateShipmentViewData();
                return View(empobj);
            }

            empobj.ID = Guid.NewGuid();
            _context.shipmentRequests.Add(empobj);
            _context.SaveChanges();

            var temp1 = _context.purchaseOrders.Find(Guid.Parse(empobj.PurchaseOrderId));
            if (temp1 != null)
            {
                temp1.Status = (int)EnumPurchaseOrder.ProcessShip;
                _context.purchaseOrders.Update(temp1);
                _context.SaveChanges();
            }

            await _auditLogService.LogAsync(
                userId: User.Identity.Name,
                action: "Create",
                tableName: "Shipment",
                recordId: empobj.ID.ToString(),
                changes: JsonConvert.SerializeObject("log")
            );

            TempData["ResultOk"] = "Cập nhật dữ liệu thành công !";
            return RedirectToAction("Index");
        }
    }
}
