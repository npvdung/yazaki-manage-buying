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
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace MangagerBuyProduct.Controllers
{
    public class ItemReceiptController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        private readonly IAuditLogService _auditLogService;
        public ItemReceiptController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService, IAuditLogService auditLogService)
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
                    // hiển thị mã phiếu giao
                    Text = x.ShipmentRequestCode,
                    // value là GUID để bind vào ItemReceipt.ShipmentRequestId
                    Value = x.ID.ToString(),
                    Selected = (selectedShipmentId != null && selectedShipmentId == x.ID.ToString())
                })
                .ToList();

            ViewData["shipmentRequestLst"] = shipmentList;

            // 2. danh sách kho
            var stockList = _context.inventory
                .Select(x => new SelectListItem
                {
                    Text = x.Name,              // tên kho để hiển thị
                    Value = x.ID.ToString(),    // GUID để lưu
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
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            IEnumerable<ItemReceipt> objCatlist = _context.ItemReceipt;
            return View(objCatlist);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new ItemReceipt();
            // nạp dropdown
            LoadDropdowns();
            // mặc định trạng thái
            model.Status = (int)EnumReceipt.Success;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ItemReceipt empobj)
        {
            var user = await _userManager.GetUserAsync(User);
            empobj.EmployeeId = user.Id;

            if (ModelState.IsValid)
            {
                // --- phần save của bạn giữ nguyên ---
                empobj.ID = Guid.NewGuid();
                _context.ItemReceipt.Add(empobj);
                _context.SaveChanges();

                var temp = _context.shipmentRequests.Find(Guid.Parse(empobj.ShipmentRequestId));
                temp.Status = EnumShip.DoneShip.ToString();
                _context.Update(temp);
                _context.SaveChanges();

                var temp3 = _context.purchaseOrders.Find(Guid.Parse(temp.PurchaseOrderId));
                temp3.Status = (int)EnumPurchaseOrder.DoneShip;
                _context.Update(temp3);
                _context.SaveChanges();

                if (temp3.Status == (int)EnumPurchaseOrder.DoneShip)
                {
                    var tempList = _context.purchaseOrderDetails
                                        .Where(x => x.PurchaseOrderId == temp3.ID.ToString())
                                        .ToList();
                    foreach (var item in tempList)
                    {
                        var updateData = _context.quota
                                                .Where(x => x.ProductId == item.ProductId)
                                                .FirstOrDefault();
                        updateData.UsedQuantity += (int)item.Quantity;
                        _context.Update(updateData);
                        _context.SaveChanges();
                    }
                }

                await _auditLogService.LogAsync(
                    userId: User.Identity.Name,
                    action: "Create",
                    tableName: "ItemReceipt",
                    recordId: Guid.NewGuid().ToString(),
                    changes: JsonConvert.SerializeObject("log")
                );

                TempData["ResultOk"] = "Tạo dữ liệu thành công !";
                return RedirectToAction("Index");
            }

            // ModelState lỗi → nạp lại dropdown + return view
            LoadDropdowns(empobj.ShipmentRequestId, empobj.StockId);
            return View(empobj);
        }

        
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Guid Id)
        {
            var empfromdb = _context.ItemReceipt.Find(Id);
            if (empfromdb == null)
            {
                return NotFound();
            }

            // nạp dropdown, chọn đúng giá trị đang có
            LoadDropdowns(empfromdb.ShipmentRequestId, empfromdb.StockId);

            // phần lấy lại tiền từ hợp đồng của bạn giữ nguyên
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
                        empfromdb.TotalAmount = Math.Round((decimal)contract.TotalAmount, 2);
                        empfromdb.TotalAmountIncludeTax = Math.Round((decimal)contract.TotalAmountIncludeTax, 2);
                        empfromdb.TotalDiscoutAmount = Math.Round((decimal)contract.TotalDiscoutAmount, 2);
                        empfromdb.TotalAmountIncludeTaxAndDiscount = Math.Round((decimal)contract.TotalAmountIncludeTaxAndDiscount, 2);
                    }
                }
            }

            return View(empfromdb);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(ItemReceipt empobj)
        {
            var user = await _userManager.GetUserAsync(User);
            empobj.EmployeeId = user.Id;

            if (ModelState.IsValid)
            {
                _context.ItemReceipt.Update(empobj);
                _context.SaveChanges();
                await _auditLogService.LogAsync(
                    userId: User.Identity.Name,
                    action: "Create",
                    tableName: "ItemReceipt",
                    recordId: Guid.NewGuid().ToString(),
                    changes: JsonConvert.SerializeObject("log")
                );
                TempData["ResultOk"] = "Cập nhập dữ liệu thành công !";
                return RedirectToAction("Index");
            }

            // lỗi → nạp lại cho view
            LoadDropdowns(empobj.ShipmentRequestId, empobj.StockId);
            return View(empobj);
        }



        [HttpGet]
        public IActionResult GetDataForDropdown(string id, string type)
        {
            // Kiểm tra loại dropdown để lấy dữ liệu
            if (type == "Manufacturer")
            {
                //id này là id của ship
                var tempItem = _context.shipmentRequests.Find(Guid.Parse(id));

                // Tìm thông tin Vendor theo ID
                var item = _context.purchaseOrders.FirstOrDefault(v => v.ID.ToString() == tempItem.PurchaseOrderId);
                if (item == null)
                {
                    return NotFound("Vendor not found");
                }

                // Lấy thông tin các sản phẩm liên quan đến Vendor
                var Contract = _context.purchaseContracts
                    .Where(p => p.ID.ToString() == item.PurchaseContractId)
                    .FirstOrDefault();

                // Tạo đối tượng trả về
                var result = new
                {
                    TotalAmount = Contract.TotalAmount, // Tên loại tiền tệ
                    TotalAmountIncludeTax = Contract.TotalAmountIncludeTax,    // Địa chỉ Vendor
                    TotalDiscoutAmount = Contract.TotalDiscoutAmount, // Người liên hệ
                    TotalAmountIncludeTaxAndDiscount = Contract.TotalAmountIncludeTaxAndDiscount, // Điều khoản thanh toán
                };

                return Json(result);

            }

            return Json(new { value = "" });
        }
        
        [Authorize(Roles = "Admin")]
        public IActionResult ItemReceipt(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                // nếu không có Id thì vẫn cần dropdown rỗng
                LoadDropdowns();
                return View(new ItemReceipt());
            }

            var objItem = new ItemReceipt
            {
                ShipmentRequestId = Id.ToString()
            };

            // phần lấy tiền giữ nguyên
            var tempItem = _context.shipmentRequests.Find(Id);
            if (tempItem != null)
            {
                var po = _context.purchaseOrders.FirstOrDefault(v => v.ID.ToString() == tempItem.PurchaseOrderId);
                var contract = _context.purchaseContracts.FirstOrDefault(v => v.ID.ToString() == po.PurchaseContractId);

                objItem.TotalAmount = Math.Round((decimal)contract.TotalAmount, 2);
                objItem.TotalDiscoutAmount = Math.Round((decimal)contract.TotalDiscoutAmount, 2);
                objItem.TotalAmountIncludeTaxAndDiscount = Math.Round((decimal)contract.TotalAmountIncludeTaxAndDiscount, 2);
                objItem.TotalAmountIncludeTax = Math.Round((decimal)contract.TotalAmountIncludeTax, 2);
            }

            // nạp dropdown + chọn sẵn
            LoadDropdowns(objItem.ShipmentRequestId, objItem.StockId);
            objItem.Status = (int)EnumReceipt.Success;
            return View(objItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ItemReceipt(ItemReceipt empobj)
        {
            var user = await _userManager.GetUserAsync(User);
            empobj.EmployeeId = user.Id;

            if (ModelState.IsValid)
            {
                empobj.ID = Guid.NewGuid();
                _context.ItemReceipt.Add(empobj);
                _context.SaveChanges();
                var temp = _context.shipmentRequests.Find(Guid.Parse(empobj.ShipmentRequestId));

                temp.Status = EnumShip.DoneShip.ToString();
                _context.Update(temp);
                _context.SaveChanges();

                var temp3 = _context.purchaseOrders.Find(Guid.Parse(temp.PurchaseOrderId));
                temp3.Status = (int)EnumPurchaseOrder.DoneShip;
                _context.Update(temp3);
                _context.SaveChanges();
                if (temp3.Status == (int)EnumPurchaseOrder.DoneShip)
                {
                    var tempList = _context.purchaseOrderDetails.Where(x => x.PurchaseOrderId == temp3.ID.ToString()).ToList();
                    foreach (var item in tempList)
                    {
                        var updateData = _context.quota.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                        updateData.UsedQuantity += (int)item.Quantity;
                        _context.Update(updateData);
                        _context.SaveChanges();
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
