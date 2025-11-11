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

namespace Base_Asp_Core_MVC_with_Identity.Controllers
{
    public class ProductController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        private readonly IAuditLogService _auditLogService;
        public ProductController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService, IAuditLogService auditLogService)
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

        private void PopulateDropDowns(Product? model = null)
        {
            // 1. Manufacturer
            ViewData["ManufacturerLst"] = _context.manufactures
                .Select(m => new SelectListItem
                {
                    Text     = m.ManufactureName,
                    Value    = m.ID.ToString(),
                    Selected = (model != null && model.ManufacturerId == m.ID.ToString())
                })
                .ToList();

            // 2. Vendor
            ViewData["VendorLst"] = _context.vendors
                .Where(v => v.Status == 0)
                .Select(v => new SelectListItem
                {
                    Text     = v.VendorName,
                    Value    = v.ID.ToString(),
                    Selected = (model != null && model.VenderId == v.ID.ToString())
                })
                .ToList();

            // 3. Quota / Category
            ViewData["QuotaLst"] = _context.Categories
                .Select(c => new SelectListItem
                {
                    Text     = c.CategoryName,
                    Value    = c.ID.ToString(),
                    Selected = (model != null && model.QuotaId == c.ID.ToString())
                })
                .ToList();
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            IEnumerable<Product> objCatlist = _context.Products;
            return View(objCatlist);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var product = new Product();
            product.Description = "không";

            // 1. Manufacturer
            var manufacturers = _context.manufactures.ToList();
            var manufacturerItems = new List<SelectListItem>();
            foreach (var m in manufacturers)
            {
                manufacturerItems.Add(new SelectListItem
                {
                    Text  = m.ManufactureName,   // tên hiển thị
                    Value = m.ID.ToString()      // giá trị lưu
                });
            }
            ViewData["ManufacturerLst"] = manufacturerItems;

            // 2. Ingredient group
            // var ingredientGroups = _context.ingredientsGroups.ToList();
            // var ingredientItems = new List<SelectListItem>();
            // foreach (var g in ingredientGroups)
            // {
            //     ingredientItems.Add(new SelectListItem
            //     {
            //         Text  = g.IngredientsGroupName,
            //         Value = g.ID.ToString()
            //     });
            // }
            // ViewData["IngredientGroupLst"] = ingredientItems;

            // 3. Vendor
            var vendors = _context.vendors.Where(v => v.Status == 0).ToList();
            var vendorItems = new List<SelectListItem>();
            foreach (var v in vendors)
            {
                vendorItems.Add(new SelectListItem
                {
                    Text  = v.VendorName,
                    Value = v.ID.ToString()
                });
            }
            ViewData["VendorLst"] = vendorItems;

            // 4. Category (Quota)
            var categories = _context.Categories.ToList();
            var categoryItems = new List<SelectListItem>();
            foreach (var c in categories)
            {
                categoryItems.Add(new SelectListItem
                {
                    Text  = c.CategoryName,
                    Value = c.ID.ToString()
                });
            }
            ViewData["QuotaLst"] = categoryItems;
            PopulateDropDowns();
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product empobj)
        {
            if (ModelState.IsValid)
            {
                empobj.SupplierId = "08dd062c-86e1-4449-8dd1-2fe5b28af344";
                _context.Products.Add(empobj);
                _context.SaveChanges();
                await _auditLogService.LogAsync(
                userId: User.Identity.Name,
                action: "Create",
                tableName: "Item",
                recordId: Guid.NewGuid().ToString(),
                changes: JsonConvert.SerializeObject("log") // Ghi thông tin chi tiết sản phẩm
        );
                TempData["ResultOk"] = "Tạo dữ liệu thành công !";
                return RedirectToAction("Index");
            }
            PopulateDropDowns(empobj);
            return View(empobj);
        }

        public IActionResult Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            // 1. Manufacturer
            var manufacturers = _context.manufactures.ToList()
                .Select(m => new SelectListItem
                {
                    Text     = m.ManufactureName,
                    Value    = m.ID.ToString(),
                    Selected = m.ID.ToString() == product.ManufacturerId   // chọn sẵn
                })
                .ToList();
            ViewData["ManufacturerLst"] = manufacturers;

            // 2. Ingredient group
            // var ingredientGroups = _context.ingredientsGroups.ToList()
            //     .Select(g => new SelectListItem
            //     {
            //         Text     = g.IngredientsGroupName,
            //         Value    = g.ID.ToString(),
            //         Selected = g.ID.ToString() == product.IngredientGroupId
            //     })
            //     .ToList();
            // ViewData["IngredientGroupLst"] = ingredientGroups;

            // 3. Vendor
            var vendors = _context.vendors.Where(v => v.Status == 0).ToList()
                .Select(v => new SelectListItem
                {
                    Text     = v.VendorName,
                    Value    = v.ID.ToString(),
                    Selected = v.ID.ToString() == product.VenderId
                })
                .ToList();
            ViewData["VendorLst"] = vendors;

            // 4. Category (quota)
            var categories = _context.Categories.ToList()
                .Select(c => new SelectListItem
                {
                    Text     = c.CategoryName,
                    Value    = c.ID.ToString(),
                    Selected = c.ID.ToString() == product.QuotaId
                })
                .ToList();
            ViewData["QuotaLst"] = categories;

            // Phần này giữ nguyên logic cũ của anh để fill mấy textbox readonly
            var manu = _context.manufactures
                            .FirstOrDefault(_ => _.ID.ToString() == product.ManufacturerId);
            if (manu != null)
                product.MANUFACTURER_Data = manu.ManufactureCode;

            // var ing = _context.ingredientsGroups
            //                 .FirstOrDefault(_ => _.ID.ToString() == product.IngredientGroupId);
            // if (ing != null)
            //     product.INGREDIENTS_GROUP_Data = ing.Content;

            var ven = _context.vendors
                            .FirstOrDefault(_ => _.ID.ToString() == product.VenderId);
            if (ven != null)
            {
                var cur = _context.Currency
                                .FirstOrDefault(_ => _.ID.ToString() == ven.CurrencyId);
                if (cur != null)
                    product.CURRENCYNAME = cur.CurrencyName;
            }

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Product empobj)
        {
            if (ModelState.IsValid)
            {
                empobj.SupplierId = "08dd062c-86e1-4449-8dd1-2fe5b28af344";
                _context.Products.Update(empobj);
                _context.SaveChanges();

                await _auditLogService.LogAsync(
                    userId: User.Identity.Name,
                    action: "Update",
                    tableName: "Products",
                    recordId: empobj.ID.ToString(),
                    changes: JsonConvert.SerializeObject(empobj.ProductCode)
                );

                TempData["ResultOk"] = "Cập nhập dữ liệu thành công !";
                return RedirectToAction("Index");
            }
            ViewData["ManufacturerLst"] = _context.manufactures.ToList()
                .Select(m => new SelectListItem
                {
                    Text     = m.ManufactureName,
                    Value    = m.ID.ToString(),
                    Selected = m.ID.ToString() == empobj.ManufacturerId
                })
                .ToList();

            // 3. Vendor
            ViewData["VendorLst"] = _context.vendors.Where(v => v.Status == 0).ToList()
                .Select(v => new SelectListItem
                {
                    Text     = v.VendorName,
                    Value    = v.ID.ToString(),
                    Selected = v.ID.ToString() == empobj.VenderId
                })
                .ToList();

            // 4. Quota
            ViewData["QuotaLst"] = _context.Categories.ToList()
                .Select(c => new SelectListItem
                {
                    Text     = c.CategoryName,
                    Value    = c.ID.ToString(),
                    Selected = c.ID.ToString() == empobj.QuotaId
                })
                .ToList();
            PopulateDropDowns(empobj);
            return View(empobj);

        }
        [HttpGet]
        public IActionResult GetDataForDropdown(string id, string type)
        {
            // Kiểm tra loại dropdown để lấy dữ liệu
            if (type == "Manufacturer")
            {
                var manufacturer = _context.manufactures.FirstOrDefault(m => m.ID.ToString() == id);
                if (manufacturer != null)
                {
                    return Json(new { value = manufacturer.ManufactureCode });
                }
            }
            // else if (type == "IngredientGroup")
            // {
            //     var category = _context.ingredientsGroups.FirstOrDefault(c => c.ID.ToString() == id);
            //     if (category != null)
            //     {
            //         return Json(new { value = category.Content });
            //     }
            // }
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
