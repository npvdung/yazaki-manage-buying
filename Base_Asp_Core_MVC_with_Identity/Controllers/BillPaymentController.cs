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
using System.Data;
using System.Linq.Expressions;

namespace MangagerBuyProduct.Controllers
{

    public class BillPaymentController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        private readonly IAuditLogService _auditLogService;
        public BillPaymentController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService, IAuditLogService auditLogService)
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
            IEnumerable<BillPayment> objCatlist = _context.BillPayment;
            return View(objCatlist);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var objItem = new BillPayment();

            // sinh mã
            string prefix = "PAY_";
            Expression<Func<BillPayment, string>> codeSelector = c => c.BillPaymentCode;
            string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);
            objItem.BillPaymentCode = autoCode;

            // lấy danh sách phiếu nhập + mã yêu cầu giao
            var result = _context.ItemReceipt
                .Join(
                    _context.shipmentRequests,
                    ir => ir.ShipmentRequestId,
                    sr => sr.ID.ToString(),
                    (ir, sr) => new
                    {
                        ProductId = ir.ID,
                        NameCode  = sr.ShipmentRequestCode
                    })
                .ToList();

            var itemReceiptSelect = new List<SelectListItem>();
            foreach (var item in result)
            {
                itemReceiptSelect.Add(new SelectListItem
                {
                    Text  = item.NameCode,              // hiển thị mã phiếu
                    Value = item.ProductId.ToString()   // lưu ID phiếu nhập
                });
            }
            ViewData["ItemReceiptIdLst"] = itemReceiptSelect;

            // status
            ViewData["StatusLst"] = Enum
                .GetValues(typeof(EnumReceipt))
                .Cast<EnumReceipt>()
                .Select(e => new SelectListItem
                {
                    Text  = e.GetDisplayName(),
                    Value = ((int)e).ToString()
                })
                .ToList();

            objItem.Status          = (int)EnumReceipt.Success;
            var user                = await _userManager.GetUserAsync(User);
            objItem.EmployeeId      = user.Id;
            objItem.BillPaymentDate = DateTime.Now;

            return View(objItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(BillPayment empobj)
        {
            var user = await _userManager.GetUserAsync(User);
            empobj.EmployeeId = user.Id;

            if (ModelState.IsValid)
            {
                empobj.ID = Guid.NewGuid();
                _context.BillPayment.Add(empobj);
                await _auditLogService.LogAsync(
               userId: User.Identity.Name,
               action: "Create",
               tableName: "BillPayment",
               recordId: Guid.NewGuid().ToString(),
               changes: JsonConvert.SerializeObject("log") // Ghi thông tin chi tiết sản phẩm
       );
                _context.SaveChanges();
                TempData["ResultOk"] = "Tạo dữ liệu thành công !";
                return RedirectToAction("Index");
            }

            return View(empobj);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Guid id)
        {
            var bill = _context.BillPayment.Find(id);
            if (bill == null) return NotFound();

            // danh sách phiếu nhập
            var result = _context.ItemReceipt
                .Join(
                    _context.shipmentRequests,
                    ir => ir.ShipmentRequestId,
                    sr => sr.ID.ToString(),
                    (ir, sr) => new
                    {
                        ProductId = ir.ID,
                        NameCode  = sr.ShipmentRequestCode
                    })
                .ToList();

            var itemReceiptSelect = new List<SelectListItem>();
            foreach (var item in result)
            {
                itemReceiptSelect.Add(new SelectListItem
                {
                    Text  = item.NameCode,
                    Value = item.ProductId.ToString()
                });
            }
            ViewData["ItemReceiptIdLst"] = itemReceiptSelect;

            // status
            ViewData["StatusLst"] = Enum
                .GetValues(typeof(EnumReceipt))
                .Cast<EnumReceipt>()
                .Select(e => new SelectListItem
                {
                    Text  = e.GetDisplayName(),
                    Value = ((int)e).ToString()
                })
                .ToList();

            return View(bill);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(BillPayment model)
        {
            if (!ModelState.IsValid)
            {
                // nếu lỗi, nạp lại dropdown rồi trả về view
                ViewData["ItemReceiptIdLst"] = _context.ItemReceipt
                    .Join(
                        _context.shipmentRequests,
                        ir => ir.ShipmentRequestId,
                        sr => sr.ID.ToString(),
                        (ir, sr) => new SelectListItem
                        {
                            Text  = sr.ShipmentRequestCode,
                            Value = ir.ID.ToString()
                        })
                    .ToList();

                ViewData["StatusLst"] = Enum
                    .GetValues(typeof(EnumReceipt))
                    .Cast<EnumReceipt>()
                    .Select(e => new SelectListItem
                    {
                        Text  = e.GetDisplayName(),
                        Value = ((int)e).ToString()
                    })
                    .ToList();

                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            model.EmployeeId = user.Id;

            _context.BillPayment.Update(model);
            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                userId: User.Identity?.Name,
                action: "Edit",
                tableName: "BillPayment",
                recordId: model.ID.ToString(),
                changes: JsonConvert.SerializeObject("log")
            );

            TempData["ResultOk"] = "Cập nhật dữ liệu thành công!";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult GetDataForDropdown(string id, string type)
        {
            // Kiểm tra loại dropdown để lấy dữ liệu
            if (type == "Manufacturer")
            {
                var temp = _context.ItemReceipt.Where(x => x.ID.ToString() == id).FirstOrDefault();

                var temp1 = _context.shipmentRequests.Where(x => x.ID.ToString() == temp.ShipmentRequestId).FirstOrDefault();
                // Tìm thông tin Vendor theo ID
                var item = _context.purchaseOrders.FirstOrDefault(v => v.ID.ToString() == temp1.PurchaseOrderId);
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
        public async Task<IActionResult> Payment(Guid id)
        {
            var itemReceipt = _context.ItemReceipt.Find(id);
            if (itemReceipt == null) return NotFound();

            var bill = new BillPayment();

            // sinh mã
            string prefix = "PAY_";
            Expression<Func<BillPayment, string>> codeSelector = c => c.BillPaymentCode;
            string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);
            bill.BillPaymentCode = autoCode;

            // lấy chuỗi liên quan để tính tiền như bạn đang làm
            var shipment = _context.shipmentRequests
                .FirstOrDefault(x => x.ID.ToString() == itemReceipt.ShipmentRequestId);

            var po = _context.purchaseOrders
                .FirstOrDefault(x => x.ID.ToString() == shipment.PurchaseOrderId);

            var contract = _context.purchaseContracts
                .FirstOrDefault(x => x.ID.ToString() == po.PurchaseContractId);

            bill.BillPaymentDate   = DateTime.Now;
            bill.TotalAmount       = Math.Round((decimal)contract.TotalAmount, 2);
            bill.BillPaymentAmount = Math.Round((decimal)contract.TotalAmount, 2);
            bill.ItemReceiptId     = id.ToString();

            // dropdown phiếu nhập
            var receiptList = _context.ItemReceipt
                .Join(
                    _context.shipmentRequests,
                    ir => ir.ShipmentRequestId,
                    sr => sr.ID.ToString(),
                    (ir, sr) => new SelectListItem
                    {
                        Text  = sr.ShipmentRequestCode,
                        Value = ir.ID.ToString()
                    })
                .ToList();
            ViewData["ItemReceiptIdLst"] = receiptList;

            // dropdown trạng thái
            ViewData["StatusLst"] = Enum
                .GetValues(typeof(EnumReceipt))
                .Cast<EnumReceipt>()
                .Select(e => new SelectListItem
                {
                    Text  = e.GetDisplayName(),
                    Value = ((int)e).ToString()
                })
                .ToList();

            bill.Status = (int)EnumReceipt.Success;

            var user = await _userManager.GetUserAsync(User);
            bill.EmployeeId = user.Id;

            return View(bill);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Payment(BillPayment model)
        {
            if (!ModelState.IsValid)
            {
                // nạp lại dropdown nếu lỗi
                ViewData["ItemReceiptIdLst"] = _context.ItemReceipt
                    .Join(
                        _context.shipmentRequests,
                        ir => ir.ShipmentRequestId,
                        sr => sr.ID.ToString(),
                        (ir, sr) => new SelectListItem
                        {
                            Text  = sr.ShipmentRequestCode,
                            Value = ir.ID.ToString()
                        })
                    .ToList();

                ViewData["StatusLst"] = Enum
                    .GetValues(typeof(EnumReceipt))
                    .Cast<EnumReceipt>()
                    .Select(e => new SelectListItem
                    {
                        Text  = e.GetDisplayName(),
                        Value = ((int)e).ToString()
                    })
                    .ToList();

                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            model.EmployeeId = user.Id;
            model.ID = Guid.NewGuid();

            _context.BillPayment.Add(model);
            await _context.SaveChangesAsync();

            TempData["ResultOk"] = "Tạo dữ liệu thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
