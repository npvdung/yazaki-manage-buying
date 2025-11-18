using Base_Asp_Core_MVC_with_Identity.CommonFile.Enum;
using Base_Asp_Core_MVC_with_Identity.CommonFile.IServiceCommon;
using Base_Asp_Core_MVC_with_Identity.Data;
using Base_Asp_Core_MVC_with_Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace MangagerBuyProduct.Controllers
{
    public class InventoryController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        public InventoryController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService)
        {
            _context = context;
            _userManager = userManager;
            _commonService = commonService;
        }

        public Base_Asp_Core_MVC_with_IdentityContext Get_context()
        {
            return _context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            IEnumerable<Inventory> objCatlist = _context.inventory;
            return View(objCatlist);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            Inventory objItem = new Inventory();
            string prefix = "KHO_";
            Expression<Func<Inventory, string>> codeSelector = c => c.InventoryCode;
            string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);
            objItem.InventoryCode = autoCode;
            return View(objItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Inventory empobj)
        {
            if (ModelState.IsValid)
            {

                _context.inventory.Add(empobj);
                _context.SaveChanges();
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
            var empfromdb = _context.inventory.Find(Id);

            if (empfromdb == null)
            {
                return NotFound();
            }
            return View(empfromdb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Inventory empobj)
        {
            if (ModelState.IsValid)
            {
                _context.inventory.Update(empobj);
                _context.SaveChanges();
                TempData["ResultOk"] = "Cập nhập dữ liệu thành công !";
                return RedirectToAction("Index");
            }
            return View(empobj);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAjax(Guid id)
        {
            try
            {
                var entity = await _context.inventory.FindAsync(id);
                if (entity == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy kho." });
                }

                // Xoá cứng:
                _context.inventory.Remove(entity);

                // Nếu sau này bị vướng khoá ngoại, bạn có thể đổi thành xoá mềm:
                // entity.Status = 1;  rồi _context.inventory.Update(entity);

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xoá kho thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Xoá thất bại: " + ex.Message });
            }
        }

    }
}
