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
using System.Linq;
using System.Linq.Expressions;

namespace MangagerBuyProduct.Controllers
{
    public class PurchaseContractController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        private readonly IAuditLogService _auditLogService;

        public PurchaseContractController(
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

        public Base_Asp_Core_MVC_with_IdentityContext Get_context()
        {
            return _context;
        }

        // ================== HÀM DÙNG LẠI ==================
        private void PopulateDropDowns()
        {
            // 1. trạng thái
            ViewData["Statuslst"] = Enum.GetValues(typeof(EnumPurchaseContract))
                .Cast<EnumPurchaseContract>()
                .Select(e => new SelectListItem
                {
                    Text = e.ToString(),
                    Value = ((int)e).ToString()
                })
                .ToList();

            // 2. vendor
            var vendors = _context.vendors
                .Where(v => v.Status == 0)
                .Select(v => new SelectListItem
                {
                    Text = v.VendorName,
                    Value = v.ID.ToString()
                })
                .ToList();
            ViewData["venderlst"] = vendors;

            // 3. product
            var result = _context.Products
                .Join(
                    _context.quota,
                    product => product.ID.ToString(),
                    quota => quota.ProductId,
                    (product, quota) => new
                    {
                        ProductId = product.ID,
                        ProductName = product.ProductName,
                        UsedQuantity = quota.UsedQuantity,
                        RemainingQuantity = quota.RemainingQuantity,
                        Quantity = quota.Quantity,
                    }
                )
                .ToList();

            var productList = _context.Products
            //.Where(p => p.Status == 0)      // nếu Product có trường Status thì có thể lọc thêm
            .Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                // Hiển thị mã + tên cho dễ nhìn
                Text  = $"{p.ProductCode} - {p.ProductName}"
                // Nếu không có ProductCode thì dùng mỗi ProductName cũng được:
                // Text = p.ProductName
            })
            .ToList();

        ViewData["productList"] = productList;
        }
        // ===================================================

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var objCatlist = _context.purchaseContracts
                .OrderByDescending(x => x.PurchaseContractCode)
                .ToList();
            return View(objCatlist);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            // chuẩn bị dropdown
            PopulateDropDowns();

            string prefix = "HĐ_";
            Expression<Func<PurchaseContract, string>> codeSelector = c => c.PurchaseContractCode;
            string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);

            // Lấy user đang đăng nhập
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                // Không có user => bắt đăng nhập lại
                return Challenge(); // hoặc RedirectToAction("Login", "Account");
            }

            var objItem = new PurchaseContractView
            {
                PurchaseContractCode = autoCode,
                Description = "không",
                TotalAmountIncludeTaxAndDiscount = 0,
                TotalAmountIncludeTax = 0,
                TotalDiscoutAmount = 0,
                PurchaseContractDetails = new List<PurchaseContractDetails>(),
                EmployeeName = $"{user.FirstName} {user.LastName}",   // tên hiển thị
                Status = (int)EnumPurchaseContract.Wait               // trạng thái khởi tạo
            };

            return View(objItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(PurchaseContractView empobj)
        {
            var user = await _userManager.GetUserAsync(User);

            empobj.EmployeeId = user.FirstName + " " + user.LastName;
            empobj.Status = (int)ApprodedStatus.Process;

            var contract = new PurchaseContract
            {
                PurchaseContractCode = empobj.PurchaseContractCode,
                PurchaseContractName = empobj.PurchaseContractName,
                Description = empobj.Description,
                VenderId = empobj.VenderId,
                EmployeeId = empobj.EmployeeId,
                Unit = empobj.Unit,
                TotalAmount = empobj.TotalAmount,
                TotalAmountIncludeTax = empobj.TotalAmountIncludeTax,
                TotalDiscoutAmount = empobj.TotalDiscoutAmount,
                TotalAmountIncludeTaxAndDiscount = empobj.TotalAmountIncludeTaxAndDiscount,
                Status = empobj.Status
            };

            _context.purchaseContracts.Add(contract);
            _context.SaveChanges();

            if (empobj.PurchaseContractDetails != null)
            {
                var validDetails = empobj.PurchaseContractDetails
                    .Where(d => !string.IsNullOrWhiteSpace(d.ProductId))
                    .ToList();

                foreach (var detail in validDetails)
                {
                    var contractDetail = new Base_Asp_Core_MVC_with_Identity.Models.PurchaseContractDetails
                    {
                        PurchaseContractId = contract.ID.ToString(),
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity ?? 0,
                        Price = detail.Price ?? 0,
                        TotalAmount = detail.TotalAmount ?? 0,
                        TaxAmount = detail.TaxAmount ?? 0,
                        DiscountAmount = detail.DiscountAmount ?? 0,
                    };

                    _context.purchaseContractDetails.Add(contractDetail);
                }
                _context.SaveChanges();
            }

            await _auditLogService.LogAsync(
               userId: User.Identity.Name,
               action: "Create",
               tableName: "PurchaseContract",
               recordId: Guid.NewGuid().ToString(),
               changes: JsonConvert.SerializeObject("log")
            );

            TempData["ResultOk"] = "Tạo dữ liệu thành công !";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid Id)
        {
            var empfromdb = _context.purchaseContracts.Find(Id);
            if (empfromdb == null)
            {
                return NotFound();
            }

            // chuẩn bị dropdown
            PopulateDropDowns();

            var objItem = new PurchaseContractView
            {
                ID = empfromdb.ID,
                PurchaseContractCode = empfromdb.PurchaseContractCode,
                PurchaseContractName = empfromdb.PurchaseContractName,
                Description = empfromdb.Description,
                VenderId = empfromdb.VenderId,
                EmployeeId = empfromdb.EmployeeId,
                Unit = empfromdb.Unit,
                TotalAmount = Math.Round(empfromdb.TotalAmount ?? 0m, 2),
                TotalAmountIncludeTax = Math.Round(empfromdb.TotalAmountIncludeTax ?? 0m, 2),
                TotalDiscoutAmount = Math.Round(empfromdb.TotalDiscoutAmount ?? 0m, 2),
                TotalAmountIncludeTaxAndDiscount = Math.Round(empfromdb.TotalAmountIncludeTaxAndDiscount ?? 0m, 2),
                Status = empfromdb.Status ?? 0,
                PurchaseContractDetails = new List<PurchaseContractDetails>()
            };

            var empDetails = _context.purchaseContractDetails
                .Where(x => x.PurchaseContractId == empfromdb.ID.ToString())
                .ToList();

            foreach (var item in empDetails)
            {
                objItem.PurchaseContractDetails.Add(item);
            }

            var user = await _userManager.GetUserAsync(User);
            objItem.EmployeeName = user.FirstName + " " + user.LastName;

            string? currencyName = null;
            var vendor = _context.vendors.FirstOrDefault(v => v.ID.ToString() == objItem.VenderId);
            if (vendor != null && !string.IsNullOrEmpty(vendor.CurrencyId))
            {
                currencyName = _context.Currency
                    .Where(c => c.ID.ToString() == vendor.CurrencyId)
                    .Select(c => c.Symbol)
                    .FirstOrDefault();
            }
            objItem.CurrencyName = currencyName;

            return View(objItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(PurchaseContractView empobj)
        {
            var contract = await _context.purchaseContracts.FindAsync(empobj.ID);
            if (contract == null)
                return NotFound();

            contract.PurchaseContractCode = empobj.PurchaseContractCode;
            contract.PurchaseContractName = empobj.PurchaseContractName;
            contract.Description = empobj.Description;
            contract.VenderId = empobj.VenderId;
            contract.Unit = empobj.Unit;
            contract.TotalAmount = empobj.TotalAmount;
            contract.TotalAmountIncludeTax = empobj.TotalAmountIncludeTax;
            contract.TotalDiscoutAmount = empobj.TotalDiscoutAmount;
            contract.TotalAmountIncludeTaxAndDiscount = empobj.TotalAmountIncludeTaxAndDiscount;
            contract.Status = (int)ApprodedStatus.Process;

            var dbDetails = _context.purchaseContractDetails
                .Where(d => d.PurchaseContractId == contract.ID.ToString())
                .ToList();

            var formDetails = empobj.PurchaseContractDetails ?? new List<PurchaseContractDetails>();
            var formIds = formDetails
                .Where(d => !string.IsNullOrWhiteSpace(d.ProductId))
                .Select(d => d.ID)
                .ToList();

            var toDelete = dbDetails
                .Where(d => !formIds.Contains(d.ID))
                .ToList();
            if (toDelete.Any())
                _context.purchaseContractDetails.RemoveRange(toDelete);

            foreach (var detail in formDetails)
            {
                if (detail.ID == Guid.Empty)
                {
                    var newDetail = new Base_Asp_Core_MVC_with_Identity.Models.PurchaseContractDetails
                    {
                        ID = Guid.NewGuid(),
                        PurchaseContractId = contract.ID.ToString(),
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity ?? 0,
                        Price = detail.Price ?? 0,
                        TotalAmount = detail.TotalAmount ?? 0,
                        TaxAmount = detail.TaxAmount ?? 0,
                        DiscountAmount = detail.DiscountAmount ?? 0
                    };
                    _context.purchaseContractDetails.Add(newDetail);
                }
                else
                {
                    var existing = dbDetails.First(d => d.ID == detail.ID);
                    existing.ProductId = detail.ProductId;
                    existing.Quantity = detail.Quantity;
                    existing.Price = detail.Price;
                    existing.TotalAmount = detail.TotalAmount;
                    existing.TaxAmount = detail.TaxAmount;
                    existing.DiscountAmount = detail.DiscountAmount;
                    _context.purchaseContractDetails.Update(existing);
                }
            }

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                userId: User.Identity.Name,
                action: "Update",
                tableName: "PurchaseContract",
                recordId: contract.ID.ToString(),
                changes: JsonConvert.SerializeObject("log"));

            TempData["ResultOk"] = "Cập nhật dữ liệu thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetDataForDropdown(string id, string type)
        {
            if (type == "Manufacturer")
            {
                // Lấy các product theo VendorId, không JOIN quota
                var products = _context.Products
                    .Where(p => p.VenderId == id)          // VenderId là string
                    .Select(p => new
                    {
                        Value = p.ID.ToString(),
                        Text  = $"{p.ProductCode} - {p.ProductName}"
                        // hoặc chỉ Text = p.ProductName
                    })
                    .ToList();

                return Json(products);
            }

            return Json(new { value = "" });
        }


        [HttpGet]
        public IActionResult GetCurrencyName(string vendorId)
        {
            var currencyId = _context.vendors.FirstOrDefault(_ => _.ID.ToString() == vendorId);
            var currencyName = _context.Currency
                                       .Where(v => v.ID.ToString() == currencyId.CurrencyId)
                                       .Select(v => v.Symbol)
                                       .FirstOrDefault();

            if (currencyName == null)
            {
                return Json(new { currencyName = "Not Found" });
            }

            return Json(new { currencyName });
        }
    }
}
