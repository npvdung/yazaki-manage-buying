using Base_Asp_Core_MVC_with_Identity.CommonFile.Enum;
using Base_Asp_Core_MVC_with_Identity.CommonFile.IServiceCommon;
using Base_Asp_Core_MVC_with_Identity.Data;
using Base_Asp_Core_MVC_with_Identity.Models;
using MangagerBuyProduct.CommonFile.IServiceCommon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Security.Claims;

namespace MangagerBuyProduct.Controllers
{
    public class QuotaController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        private readonly IAuditLogService _auditLogService;
        private void LoadDropdowns()
        {
            // 1. sản phẩm
            var products = _context.Products.ToList();
            var productSelect = products
                .Select(p => new SelectListItem
                {
                    Value = p.ID.ToString(),        // value phải là ID
                    Text  = p.ProductName           // text để hiển thị
                })
                .ToList();
            ViewData["ProductList"] = productSelect;

            // 2. loại vật tư
            var categories = _context.Categories.ToList();
            var categorySelect = categories
                .Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text  = c.CategoryName
                })
                .ToList();
            ViewData["CategoryList"] = categorySelect;

            // 3. trạng thái
            var statusSelect = Enum.GetValues(typeof(ActiveStatus))
                .Cast<ActiveStatus>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text  = e.ToString()
                })
                .ToList();
            ViewData["StatusList"] = statusSelect;
        }

        public QuotaController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService, IAuditLogService auditLogService)
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
            IEnumerable<Quota> objCatlist = _context.quota;
            return View(objCatlist);
        }

        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var objItem = new Quota();

            string prefix = "DMT_";
            Expression<Func<Quota, string>> codeSelector = c => c.QuotaCode;
            string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);
            objItem.QuotaCode = autoCode;

            // giá trị mặc định
            objItem.RemainingQuantity = 0;
            objItem.UsedQuantity = 0;

            // nạp dropdown
            LoadDropdowns();

            return View(objItem);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Quota empobj)
        {
            var user = await _userManager.GetUserAsync(User);
            empobj.EmployeeId = user.Id;

            if (ModelState.IsValid)
            {
                _context.quota.Add(empobj);
                _context.SaveChanges();

                await _auditLogService.LogAsync(
                    userId: User.Identity.Name,
                    action: "Create",
                    tableName: "Quota",
                    recordId: empobj.ID.ToString(),
                    changes: JsonConvert.SerializeObject("Log")
                );

                TempData["ResultOk"] = "Tạo dữ liệu thành công !";
                return RedirectToAction("Index");
            }

            // QUAN TRỌNG: nạp lại dropdown để view không lỗi
            LoadDropdowns();
            return View(empobj);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var empfromdb = _context.quota.Find(id);
            if (empfromdb == null)
                return NotFound();

            // nạp dropdown
            LoadDropdowns();

            // bổ sung dữ liệu hiển thị
            var product = _context.Products.FirstOrDefault(p => p.ID.ToString() == empfromdb.ProductId);
            if (product != null)
                empfromdb.ProductData = product.ProductCode;

            var user = await _userManager.FindByIdAsync(empfromdb.EmployeeId);
            if (user != null)
                empfromdb.EmployeeName = user.FirstName + " " + user.LastName;

            return View(empfromdb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Quota empobj)
        {
            var user = await _userManager.GetUserAsync(User);
            empobj.EmployeeId = user.Id;

            if (ModelState.IsValid)
            {
                _context.quota.Update(empobj);
                _context.SaveChanges();

                await _auditLogService.LogAsync(
                    userId: User.Identity.Name,
                    action: "Update",
                    tableName: "Quota",
                    recordId: empobj.ID.ToString(),
                    changes: JsonConvert.SerializeObject("Log")
                );

                TempData["ResultOk"] = "Cập nhập dữ liệu thành công !";
                return RedirectToAction("Index");
            }

            // nếu model lỗi -> load lại dropdown
            LoadDropdowns();
            return View(empobj);
        }


        [HttpGet]
        public IActionResult GetDataForDropdown(string id, string type)
        {
            // Kiểm tra loại dropdown để lấy dữ liệu
            if (type == "Manufacturer")
            {
                var manufacturer = _context.Products.FirstOrDefault(m => m.ID.ToString() == id);
                if (manufacturer != null)
                {
                    return Json(new { value = manufacturer.ProductCode });
                }
            }
            else if (type == "IngredientGroup")
            {
                var category = _context.ingredientsGroups.FirstOrDefault(c => c.ID.ToString() == id);
                if (category != null)
                {
                    return Json(new { value = category.Content });
                }
            }
            else if (type == "Vendor")
            {
                var category = _context.vendors.FirstOrDefault(c => c.ID.ToString() == id);
                var currency = _context.Currency.Where(_ => _.ID.ToString() == category.CurrencyId).FirstOrDefault();
                if (category != null)
                {
                    return Json(new { value = currency.CurrencyName });
                }
            }
            return Json(new { value = "" });
        }
    }
}
