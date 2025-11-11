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
    public class IngredientsGroupController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        public IngredientsGroupController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService)
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
            IEnumerable<IngredientsGroup> objCatlist = _context.ingredientsGroups;
            return View(objCatlist);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            IngredientsGroup objItem = new IngredientsGroup();
            string prefix = "IG_";
            Expression<Func<IngredientsGroup, string>> codeSelector = c => c.IngredientsGroupCode;
            string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);
            objItem.IngredientsGroupCode = autoCode;

            ViewBag.Statuslst = Enum.GetValues(typeof(ActiveStatus))
                            .Cast<ActiveStatus>()
                            .Select(e => new SelectListItem
                            {
                                Text = e.ToString(),
                                Value = e.ToString()
                            }).ToList();

            return View(objItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(IngredientsGroup empobj)
        {
            if (ModelState.IsValid)
            {

                _context.ingredientsGroups.Add(empobj);
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
            var empfromdb = _context.ingredientsGroups.Find(Id);
            ViewBag.Statuslst = Enum.GetValues(typeof(ActiveStatus))
                           .Cast<ActiveStatus>()
                           .Select(e => new SelectListItem
                           {
                               Text = e.ToString(),
                               Value = e.ToString()
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
        public IActionResult Edit(IngredientsGroup empobj)
        {
            if (ModelState.IsValid)
            {
                _context.ingredientsGroups.Update(empobj);
                _context.SaveChanges();
                TempData["ResultOk"] = "Cập nhập dữ liệu thành công !";
                return RedirectToAction("Index");
            }
            return View(empobj);
        }
    }
}
