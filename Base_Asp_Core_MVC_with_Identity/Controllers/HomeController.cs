using Base_Asp_Core_MVC_with_Identity.Data;
using Base_Asp_Core_MVC_with_Identity.Models;
using MangagerBuyProduct.Models;
using MangagerBuyProduct.Models.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using Base_Asp_Core_MVC_with_Identity.CommonFile.Enum;


namespace Base_Asp_Core_MVC_with_Identity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;

        public HomeController(ILogger<HomeController> logger, Base_Asp_Core_MVC_with_IdentityContext context)
        {
            _context = context;
            _logger = logger;
        }
        public Base_Asp_Core_MVC_with_IdentityContext Get_context()
        {
            return _context;
        }

        public IActionResult Index()
        {
            // ===== 1. % LƯỢNG ĐƠN MUA HOÀN THÀNH =====
            // Ở đây hiểu "đơn mua" là HỢP ĐỒNG MUA (PurchaseContract)
            var totalContracts = _context.purchaseContracts.Count();
            var doneContracts  = _context.purchaseContracts
                                        .Count(x => x.Status == (int)EnumPurchaseContract.Done);

            int completedPercent = 0;
            if (totalContracts > 0)
            {
                completedPercent = (int)Math.Round(doneContracts * 100.0 / totalContracts, 0);
            }

            // ===== 2. TIỀN NHẬP HÀNG – KHỚP "BÁO CÁO MUA HÀNG" =====
            // Lấy tổng từ purchaseOrderDetails.TotalAmount (giống ReportApiController)
            decimal totalPurchaseAmount = _context.purchaseOrderDetails
                                                .Sum(d => d.TotalAmount ?? 0);

            // ===== 3. TIỀN TRẢ LẠI HÀNG – KHỚP "BÁO CÁO TRẢ HÀNG" =====
            // Lấy các chi tiết đơn có phiếu trả hàng
            var totalReturnAmount = (from d in _context.purchaseOrderDetails
                                    join r in _context.returnAuthorizations
                                        on d.PurchaseOrderId equals r.PurchaseOrderId
                                    select d.TotalAmount ?? 0).Sum();

            // ===== 4. LỊCH SỬ (giữ nguyên nhưng sort theo thời gian mới nhất) =====
            var objCatlist = _context.AuditLogs
                                    .OrderByDescending(x => x.Timestamp)
                                    .Take(7)
                                    .ToList();

            var viewModel = new HomeViewModel
            {
                Sales      = completedPercent,     // % đơn mua hoàn thành
                Revenue    = totalPurchaseAmount,  // Tiền nhập hàng
                NewClients = totalReturnAmount,    // Tiền trả lại hàng
                Logs       = objCatlist
            };

            // ===== 5. DỮ LIỆU BIỂU ĐỒ LƯU LƯỢNG NHẬP HÀNG THEO THÁNG =====
            // Không có cột CreatedDate, nên mình dùng AuditLogs.Timestamp cho PurchaseContract

            // Lấy tất cả hợp đồng + log tạo hợp đồng
            var contracts = _context.purchaseContracts
                                    .OrderBy(c => c.ID) // để có thứ tự ổn định
                                    .ToList();

            var contractLogs = _context.AuditLogs
                .Where(a => a.TableName == "PurchaseContract" && a.Action == "Create")
                .OrderBy(a => a.Timestamp)
                .ToList();

            // Mảng 12 tháng, mặc định = 0
            decimal[] revenueData = new decimal[12];

            int pairCount = Math.Min(contracts.Count, contractLogs.Count);
            for (int i = 0; i < pairCount; i++)
            {
                var contract = contracts[i];
                var log      = contractLogs[i];

                int monthIndex = log.Timestamp.Month - 1; // Tháng 1 -> index 0
                if (monthIndex >= 0 && monthIndex < 12)
                {
                    // Có thể dùng TotalAmount hoặc TotalAmountIncludeTaxAndDiscount tùy bạn muốn
                    decimal value = contract.TotalAmount ?? 0;
                    revenueData[monthIndex] += value;
                }
            }

            // Tạo label T1..T12
            var labels = new List<string>();
            for (int i = 1; i <= 12; i++)
            {
                labels.Add("T" + i);
            }

            ViewData["RevenueData"] = JsonConvert.SerializeObject(revenueData);
            ViewData["Labels"]      = JsonConvert.SerializeObject(labels);

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}