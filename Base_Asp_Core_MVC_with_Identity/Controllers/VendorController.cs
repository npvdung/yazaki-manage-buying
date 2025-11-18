using Base_Asp_Core_MVC_with_Identity.CommonFile.Enum;
using Base_Asp_Core_MVC_with_Identity.CommonFile.IServiceCommon;
using Base_Asp_Core_MVC_with_Identity.Data;
using Base_Asp_Core_MVC_with_Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace MangagerBuyProduct.Controllers
{
    public class VendorController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        public VendorController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService)
        {
            _context = context;
            _userManager = userManager;
            _commonService = commonService;
        }

        public Base_Asp_Core_MVC_with_IdentityContext Get_context()
        {
            return _context;
        }
        public IActionResult Index()
        {
            IEnumerable<Vendor> objCatlist = _context.vendors;
            return View(objCatlist);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            Vendor objItem = new Vendor();
            string prefix = "VD_";
            Expression<Func<Vendor, string>> codeSelector = c => c.VendorCode;
            string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);
            objItem.VendorCode = autoCode;
            objItem.Description = "không";

            List<Currency> ItemsLst = _context.Currency.ToList();
            List<SelectListItem> itemDatas = new List<SelectListItem>();
            foreach (var item in ItemsLst)
            {
                if (item.ID.ToString() != null)
                {
                    itemDatas.Add(new SelectListItem { Text = item.ID.ToString(), Value = item.CurrencyName.ToString() + '-' + item.Symbol });
                }
            }
            ViewBag.Currencylst = itemDatas;

            ViewBag.Statuslst = Enum.GetValues(typeof(ActiveVenderStatus))
                            .Cast<ActiveVenderStatus>()
                            .Select(e => new SelectListItem
                            {
                                Text = e.ToString(),
                                Value = ((int)e).ToString()
                            }).ToList();

            return View(objItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Vendor empobj)
        {
            if (ModelState.IsValid)
            {

                _context.vendors.Add(empobj);
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
            var empfromdb = _context.vendors.Find(Id);
            List<Currency> ItemsLst = _context.Currency.ToList();
            List<SelectListItem> itemDatas = new List<SelectListItem>();
            foreach (var item in ItemsLst)
            {
                if (item.ID.ToString() != null)
                {
                    itemDatas.Add(new SelectListItem { Text = item.ID.ToString(), Value = item.CurrencyName.ToString() + '-' + item.Symbol });
                }
            }
            ViewBag.Currencylst = itemDatas;

            ViewBag.Statuslst = Enum.GetValues(typeof(ActiveVenderStatus))
                            .Cast<ActiveVenderStatus>()
                            .Select(e => new SelectListItem
                            {
                                Text = e.ToString(),
                                Value = ((int)e).ToString()
                            }).ToList();
            if (empfromdb == null)
            {
                return NotFound();
            }
            return View(empfromdb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Vendor empobj)
        {
            if (ModelState.IsValid)
            {
                _context.vendors.Update(empobj);
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
                var entity = await _context.vendors.FindAsync(id);
                if (entity == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy nhà cung cấp." });
                }

                _context.vendors.Remove(entity);   // hoặc entity.Status = 1; nếu bạn muốn xoá mềm
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xoá nhà cung cấp thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Xoá thất bại: " + ex.Message });
            }
        }

    }
}
