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
    public class CurrencyController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        public CurrencyController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService)
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
            IEnumerable<Currency> objCatlist = _context.Currency;
            return View(objCatlist);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            Currency category = new Currency();
            //string prefix = "CC_";
            //Expression<Func<Currency, string>> codeSelector = c => c.CurrencyCode;
            //string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);
            //category.CurrencyCode = autoCode;
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Currency empobj)
        {
            if (ModelState.IsValid)
            {

                _context.Currency.Add(empobj);
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
            var empfromdb = _context.Currency.Find(Id);

            if (empfromdb == null)
            {
                return NotFound();
            }
            return View(empfromdb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(Currency empobj)
        {
            if (ModelState.IsValid)
            {
                _context.Currency.Update(empobj);
                _context.SaveChanges();
                TempData["ResultOk"] = "Cập nhập dữ liệu thành công !";
                return RedirectToAction("Index");
            }
            return View(empobj);
        }
    }
}
