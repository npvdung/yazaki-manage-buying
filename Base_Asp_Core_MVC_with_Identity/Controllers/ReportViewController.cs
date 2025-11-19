using Base_Asp_Core_MVC_with_Identity.CommonFile.IServiceCommon;
using Base_Asp_Core_MVC_with_Identity.Data;
using Base_Asp_Core_MVC_with_Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MangagerBuyProduct.Controllers
{
    public class ReportViewController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;
        public ReportViewController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService)
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
            IEnumerable<PurchaseContractDetails> objCatlist = _context.purchaseContractDetails
                .OrderByDescending(x => x.ID)
                .ToList();
            return View(objCatlist);
        }
    }
}
