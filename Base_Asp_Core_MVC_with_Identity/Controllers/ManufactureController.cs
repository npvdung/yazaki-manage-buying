// using Base_Asp_Core_MVC_with_Identity.CommonFile.Enum;
// using Base_Asp_Core_MVC_with_Identity.CommonFile.IServiceCommon;
// using Base_Asp_Core_MVC_with_Identity.Data;
// using Base_Asp_Core_MVC_with_Identity.Models;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using System.Linq.Expressions;

// namespace MangagerBuyProduct.Controllers
// {
//     public class ManufactureController : Controller
//     {
//         private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
//         private UserManager<UserSystemIdentity> _userManager;
//         private readonly ICommonService _commonService;
//         public ManufactureController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService)
//         {
//             _context = context;
//             _userManager = userManager;
//             _commonService = commonService;
//         }

//         public Base_Asp_Core_MVC_with_IdentityContext Get_context()
//         {
//             return _context;
//         }
//         [Authorize(Roles = "Admin")]
//         public IActionResult Index()
//         {
//             IEnumerable<Manufacture> objCatlist = _context.manufactures;
//             return View(objCatlist);
//         }

//         [Authorize(Roles = "Admin")]
//         public IActionResult Create()
//         {
//             Manufacture objItem = new Manufacture();
//             string prefix = "MF_";
//             Expression<Func<Manufacture, string>> codeSelector = c => c.ManufactureCode;
//             string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);
//             objItem.ManufactureCode = autoCode;
//             objItem.Description = "không";
//             return View(objItem);
//         }

//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         [Authorize(Roles = "Admin")]
//         public IActionResult Create(Manufacture empobj)
//         {
//             if (ModelState.IsValid)
//             {

//                 _context.manufactures.Add(empobj);
//                 _context.SaveChanges();
//                 TempData["ResultOk"] = "Tạo dữ liệu thành công !";
//                 return RedirectToAction("Index");
//             }

//             return View(empobj);
//         }
//         [Authorize(Roles = "Admin")]
//         public IActionResult Edit(Guid Id)
//         {
//             if (Id == null)
//             {
//                 return NotFound();
//             }
//             var empfromdb = _context.manufactures.Find(Id);

//             if (empfromdb == null)
//             {
//                 return NotFound();
//             }
//             return View(empfromdb);
//         }

//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         [Authorize(Roles = "Admin")]
//         public IActionResult Edit(Manufacture empobj)
//         {
//             if (ModelState.IsValid)
//             {
//                 _context.manufactures.Update(empobj);
//                 _context.SaveChanges();
//                 TempData["ResultOk"] = "Cập nhập dữ liệu thành công !";
//                 return RedirectToAction("Index");
//             }
//             return View(empobj);
//         }
//     }
// }
