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
    public class ItemReceiptController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        private readonly IAuditLogService _auditLogService;

        public ItemReceiptController(
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

        private void LoadDropdowns(string? selectedShipmentId = null, string? selectedStockId = null)
        {
            // 1. danh sách phiếu giao đang giao
            var shipmentList = _context.shipmentRequests
                .Where(x => x.Status == EnumShip.ProcessShip.ToString())
                .Select(x => new SelectListItem
                {
                    Text = x.ShipmentRequestCode,
                    Value = x.ID.ToString(),
                    Selected = (selectedShipmentId != null && selectedShipmentId == x.ID.ToString())
                })
                .ToList();
            ViewData["shipmentRequestLst"] = shipmentList;

            // 2. danh sách kho
            var stockList = _context.inventory
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.ID.ToString(),
                    Selected = (selectedStockId != null && selectedStockId == x.ID.ToString())
                })
                .ToList();
            ViewData["stockLst"] = stockList;

            // 3. danh sách trạng thái nhập
            var statusList = Enum.GetValues(typeof(EnumReceipt))
                .Cast<EnumReceipt>()
                .Select(e => new SelectListItem
                {
                    Text = e.GetDisplayName(),
                    Value = ((int)e).ToString()
                })
                .ToList();
            ViewData["Statuslst"] = statusList;
        }

        public Base_Asp_Core_MVC_with_IdentityContext Get_context()
        {
            return _context;
        }

        // ================== INDEX ==================
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            IEnumerable<ItemReceipt> objCatlist = _context.ItemReceipt;
            return View(objCatlist);
        }

        // ================== CREATE (GET) ==================
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new ItemReceipt
            {
                Status = (int)EnumReceipt.Success
            };

            LoadDropdowns();
            return View(model);
        }

        // ================== CREATE (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ItemReceipt empobj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            empobj.EmployeeId = user.Id;

            if (ModelState.IsValid)
            {
                empobj.ID = Guid.NewGuid();
                _context.ItemReceipt.Add(empobj);
                _context.SaveChanges();

                // --- cập nhật ShipmentRequest ---
                var shipment = _context.shipmentRequests
                    .FirstOrDefault(x => x.ID.ToString() == empobj.ShipmentRequestId);
                if (shipment != null)
                {
                    shipment.Status = EnumShip.DoneShip.ToString();
                    _context.shipmentRequests.Update(shipment);
                    _context.SaveChanges();

                    // --- cập nhật PurchaseOrder ---
                    var po = _context.purchaseOrders
                        .FirstOrDefault(x => x.ID.ToString() == shipment.PurchaseOrderId);

                    if (po != null)
                    {
                        po.Status = (int)EnumPurchaseOrder.DoneShip;
                        _context.purchaseOrders.Update(po);
                        _context.SaveChanges();

                        // --- cập nhật quota (NẾU CÓ) ---
                        if (po.Status == (int)EnumPurchaseOrder.DoneShip)
                        {
                            var poDetails = _context.purchaseOrderDetails
                                .Where(x => x.PurchaseOrderId == po.ID.ToString())
                                .ToList();

                            foreach (var item in poDetails)
                            {
                                var quotaRow = _context.quota
                                    .FirstOrDefault(x => x.ProductId == item.ProductId);

                                // Có quota thì mới cập nhật, không có thì bỏ qua
                                if (quotaRow != null)
                                {
                                    quotaRow.UsedQuantity =
                                        (quotaRow.UsedQuantity ?? 0) + (int)(item.Quantity ?? 0);

                                    quotaRow.RemainingQuantity =
                                        (quotaRow.RemainingQuantity ?? 0) - (int)(item.Quantity ?? 0);

                                    _context.quota.Update(quotaRow);
                                }
                            }
                            _context.SaveChanges();
                        }
                    }
                }

                await _auditLogService.LogAsync(
                    userId: User.Identity.Name,
                    action: "Create",
                    tableName: "ItemReceipt",
                    recordId: empobj.ID.ToString(),
                    changes: JsonConvert.SerializeObject("log"));

                TempData["ResultOk"] = "Tạo dữ liệu thành công !";
                return RedirectToAction("Index");
            }

            LoadDropdowns(empobj.ShipmentRequestId, empobj.StockId);
            return View(empobj);
        }

        // ================== EDIT (GET) ==================
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Guid Id)
        {
            var empfromdb = _context.ItemReceipt.Find(Id);
            if (empfromdb == null)
            {
                return NotFound();
            }

            LoadDropdowns(empfromdb.ShipmentRequestId, empfromdb.StockId);

            // Lấy lại tiền dựa trên hợp đồng
            var ship = _context.shipmentRequests
                .FirstOrDefault(x => x.ID.ToString() == empfromdb.ShipmentRequestId);
            if (ship != null)
            {
                var po = _context.purchaseOrders
                    .FirstOrDefault(v => v.ID.ToString() == ship.PurchaseOrderId);
                if (po != null)
                {
                    var contract = _context.purchaseContracts
                        .FirstOrDefault(p => p.ID.ToString() == po.PurchaseContractId);
                    if (contract != null)
                    {
                        empfromdb.TotalAmount = Math.Round(contract.TotalAmount ?? 0m, 2);
                        empfromdb.TotalAmountIncludeTax = Math.Round(contract.TotalAmountIncludeTax ?? 0m, 2);
                        empfromdb.TotalDiscoutAmount = Math.Round(contract.TotalDiscoutAmount ?? 0m, 2);
                        empfromdb.TotalAmountIncludeTaxAndDiscount =
                            Math.Round(contract.TotalAmountIncludeTaxAndDiscount ?? 0m, 2);
                    }
                }
            }

            return View(empfromdb);
        }

        // ================== EDIT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(ItemReceipt empobj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            empobj.EmployeeId = user.Id;

            if (ModelState.IsValid)
            {
                _context.ItemReceipt.Update(empobj);
                _context.SaveChanges();

                await _auditLogService.LogAsync(
                    userId: User.Identity.Name,
                    action: "Update",
                    tableName: "ItemReceipt",
                    recordId: empobj.ID.ToString(),
                    changes: JsonConvert.SerializeObject("log"));

                TempData["ResultOk"] = "Cập nhập dữ liệu thành công !";
                return RedirectToAction("Index");
            }

            LoadDropdowns(empobj.ShipmentRequestId, empobj.StockId);
            return View(empobj);
        }

        // ================== AJAX GET DATA ==================
        [HttpGet]
        public IActionResult GetDataForDropdown(string id, string type)
        {
            if (type == "Manufacturer")
            {
                // id: ID của shipment
                if (!Guid.TryParse(id, out var shipId))
                {
                    return BadRequest("Invalid shipment id");
                }

                var ship = _context.shipmentRequests.Find(shipId);
                if (ship == null)
                {
                    return NotFound("Shipment not found");
                }

                var po = _context.purchaseOrders
                    .FirstOrDefault(v => v.ID.ToString() == ship.PurchaseOrderId);
                if (po == null)
                {
                    return NotFound("Purchase order not found");
                }

                var contract = _context.purchaseContracts
                    .FirstOrDefault(p => p.ID.ToString() == po.PurchaseContractId);
                if (contract == null)
                {
                    return NotFound("Purchase contract not found");
                }

                var result = new
                {
                    TotalAmount = contract.TotalAmount,
                    TotalAmountIncludeTax = contract.TotalAmountIncludeTax,
                    TotalDiscoutAmount = contract.TotalDiscoutAmount,
                    TotalAmountIncludeTaxAndDiscount = contract.TotalAmountIncludeTaxAndDiscount
                };

                return Json(result);
            }

            return Json(new { value = "" });
        }

        // ================== ITEMRECEIPT (GET) ==================
        [Authorize(Roles = "Admin")]
        public IActionResult ItemReceipt(Guid Id)
        {
            var objItem = new ItemReceipt();

            if (Id != Guid.Empty)
            {
                objItem.ShipmentRequestId = Id.ToString();

                var ship = _context.shipmentRequests.Find(Id);
                if (ship != null)
                {
                    var po = _context.purchaseOrders
                        .FirstOrDefault(v => v.ID.ToString() == ship.PurchaseOrderId);
                    if (po != null)
                    {
                        var contract = _context.purchaseContracts
                            .FirstOrDefault(v => v.ID.ToString() == po.PurchaseContractId);
                        if (contract != null)
                        {
                            objItem.TotalAmount = Math.Round(contract.TotalAmount ?? 0m, 2);
                            objItem.TotalDiscoutAmount = Math.Round(contract.TotalDiscoutAmount ?? 0m, 2);
                            objItem.TotalAmountIncludeTaxAndDiscount =
                                Math.Round(contract.TotalAmountIncludeTaxAndDiscount ?? 0m, 2);
                            objItem.TotalAmountIncludeTax =
                                Math.Round(contract.TotalAmountIncludeTax ?? 0m, 2);
                        }
                    }
                }
            }

            objItem.Status = (int)EnumReceipt.Success;
            LoadDropdowns(objItem.ShipmentRequestId, objItem.StockId);
            return View(objItem);
        }

        // ================== ITEMRECEIPT (POST) ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ItemReceipt(ItemReceipt empobj)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            empobj.EmployeeId = user.Id;

            if (ModelState.IsValid)
            {
                empobj.ID = Guid.NewGuid();
                _context.ItemReceipt.Add(empobj);
                _context.SaveChanges();

                var ship = _context.shipmentRequests
                    .FirstOrDefault(x => x.ID.ToString() == empobj.ShipmentRequestId);
                if (ship != null)
                {
                    ship.Status = EnumShip.DoneShip.ToString();
                    _context.shipmentRequests.Update(ship);
                    _context.SaveChanges();

                    var po = _context.purchaseOrders
                        .FirstOrDefault(x => x.ID.ToString() == ship.PurchaseOrderId);
                    if (po != null)
                    {
                        po.Status = (int)EnumPurchaseOrder.DoneShip;
                        _context.purchaseOrders.Update(po);
                        _context.SaveChanges();

                        if (po.Status == (int)EnumPurchaseOrder.DoneShip)
                        {
                            var poDetails = _context.purchaseOrderDetails
                                .Where(x => x.PurchaseOrderId == po.ID.ToString())
                                .ToList();

                            foreach (var item in poDetails)
                            {
                                var quotaRow = _context.quota
                                    .FirstOrDefault(x => x.ProductId == item.ProductId);

                                if (quotaRow != null)
                                {
                                    quotaRow.UsedQuantity =
                                        (quotaRow.UsedQuantity ?? 0) + (int)(item.Quantity ?? 0);

                                    quotaRow.RemainingQuantity =
                                        (quotaRow.RemainingQuantity ?? 0) - (int)(item.Quantity ?? 0);

                                    _context.quota.Update(quotaRow);
                                }
                            }

                            _context.SaveChanges();
                        }
                    }
                }

                TempData["ResultOk"] = "Tạo dữ liệu thành công !";
                return RedirectToAction("Index");
            }

            LoadDropdowns(empobj.ShipmentRequestId, empobj.StockId);
            return View(empobj);
        }
    }
}
