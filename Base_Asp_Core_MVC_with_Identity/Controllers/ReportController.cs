using Base_Asp_Core_MVC_with_Identity.CommonFile.Enum;
using Base_Asp_Core_MVC_with_Identity.CommonFile.IServiceCommon;
using Base_Asp_Core_MVC_with_Identity.Data;
using Base_Asp_Core_MVC_with_Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MangagerBuyProduct.Controllers
{
    public class ReportController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        public ReportController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService)
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
            IEnumerable<PurchaseContractDetails> objCatlist = _context.purchaseContractDetails
                .OrderByDescending(x => x.ID)
                .ToList();
            return View(objCatlist);
        }

        //[Authorize(Roles = "Admin")]
        //public IActionResult IndexView2()
        //{
        //    IEnumerable<ReturnAuthorization> objCatlist = _context.returnAuthorizations;
        //    return View(objCatlist);
        //}
    }
}
