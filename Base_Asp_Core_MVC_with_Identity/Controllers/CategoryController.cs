using Base_Asp_Core_MVC_with_Identity.Areas.Identity.Data;
using Base_Asp_Core_MVC_with_Identity.CommonFile.Enum;
using Base_Asp_Core_MVC_with_Identity.CommonFile.IServiceCommon;
using Base_Asp_Core_MVC_with_Identity.Data;
using Base_Asp_Core_MVC_with_Identity.Models;
using MangagerBuyProduct.CommonFile.IServiceCommon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Base_Asp_Core_MVC_with_Identity.Controllers
{
    public class CategoryController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        private readonly IAuditLogService _auditLogService;
        public CategoryController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService, IAuditLogService auditLogService)
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
            IEnumerable<Category> objCatlist = _context.Categories;
            return View(objCatlist);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            Category objItem = new Category();
            string prefix = "DMT_";
            Expression<Func<Category, string>> codeSelector = c => c.CategoryCode;
            string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);
            objItem.CategoryCode = autoCode;
            objItem.Description = "không";
            return View(objItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Category empobj)
        {
            if (ModelState.IsValid)
            {

                _context.Categories.Add(empobj);
                _context.SaveChanges();
                TempData["ResultOk"] = "Tạo dữ liệu thành công !";
                await _auditLogService.LogAsync(
                userId: User.Identity.Name,
                action: "Create",
                tableName: "Category",
                recordId: Guid.NewGuid().ToString(),
                changes: JsonConvert.SerializeObject("Categpru") // Ghi thông tin chi tiết sản phẩm
        );
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
            var empfromdb = _context.Categories.Find(Id);

            if (empfromdb == null)
            {
                return NotFound();
            }
            return View(empfromdb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Category empobj)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(empobj);
                _context.SaveChanges();
                TempData["ResultOk"] = "Cập nhập dữ liệu thành công !";
                await _auditLogService.LogAsync(
               userId: User.Identity.Name,
               action: "Update",
               tableName: "Category",
               recordId: Guid.NewGuid().ToString(),
               changes: JsonConvert.SerializeObject("Categpru"));
                return RedirectToAction("Index");
            }
            return View(empobj);
        }
        public IActionResult Delete(Guid Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var empfromdb = _context.Categories.Find(Id);

            if (empfromdb == null)
            {
                return NotFound();
            }
            return View(empfromdb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteEmp(Guid Id)
        {
            var deleterecord = _context.Categories.Find(Id);
            if (deleterecord == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(deleterecord);
            _context.SaveChanges();
            TempData["ResultOk"] = "Thông tin xoá thành công !";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAjax(Guid id)
        {
            try
            {
                var entity = await _context.Categories.FindAsync(id);
                if (entity == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy loại vật tư." });
                }

                _context.Categories.Remove(entity);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xoá loại vật tư thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Xoá thất bại: " + ex.Message });
            }
        }

    }
}
