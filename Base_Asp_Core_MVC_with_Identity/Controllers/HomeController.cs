using Base_Asp_Core_MVC_with_Identity.Data;
using Base_Asp_Core_MVC_with_Identity.Models;
using MangagerBuyProduct.Models;
using MangagerBuyProduct.Models.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

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
            // Lấy các dữ liệu khác cho trang Home (ví dụ: thống kê)
            var sales = 75; // Tỉ lệ chuyển đổi
            var revenue = 4300; // Doanh thu
            var newClients = 6782; // Số khách hàng mới

            // Lấy 10 bản ghi log gần nhất
            //var logs = _context.AuditLogs
            //    .Take(10)
            //    .ToList();
            IEnumerable<AuditLog> objCatlist = _context.AuditLogs.Take(7).ToList();
            var lst1 = _context.purchaseContracts
                   .Count(x => x.Status == (int)EnumPurchaseContract.Done); // không cần ToList

            var lst2 = _context.purchaseContracts.Count();

            // tổng tiền hợp đồng
            var lst3 = _context.purchaseContracts
                            .AsEnumerable()                // ép về C#
                            .Sum(x => x.TotalAmount ?? 0); // null thì lấy 0

            // tổng tiền hoàn trả
            var lst4 = _context.returnAuthorizations
                            .AsEnumerable()
                            .Sum(x => x.AmountReturn ?? 0);

            if (lst2 == 0) lst2 = 1;

            var viewModel = new HomeViewModel
            {
                Sales   = (int)Math.Round((double)lst1 / lst2 * 100, 0),
                Revenue = lst3,
                NewClients = lst4,
                Logs = objCatlist.ToList()
            };


            // Lấy tất cả dữ liệu purchaseContracts từ cơ sở dữ liệu
            var contracts = _context.purchaseContracts
                                    .Select(x => x.TotalAmount) // Giả sử bạn có trường TotalAmount cho tiền
                                    .OrderBy(x => x) // Sắp xếp các hợp đồng (nếu có thứ tự sẵn thì không cần)
                                    .ToList();

            // Tạo danh sách revenueData để chứa tổng doanh thu cho từng tháng (12 tháng)
            var revenueData = new List<int>(new int[12]); // Mặc định là 0 cho tất cả các tháng
            int monthIndex = 0;
            int monthTotal = 0;
            int count = 0;

            // Duyệt qua từng bản ghi và tính tổng cho mỗi tháng (4 bản ghi cho mỗi tháng)
            foreach (var amount in contracts)
            {
                monthTotal += (int)amount; // Cộng dồn tổng doanh thu của tháng hiện tại
                count++;

                if (count == 1) // Mỗi tháng có 4 bản ghi
                {
                    revenueData[monthIndex] = monthTotal; // Gán tổng doanh thu cho tháng
                    monthIndex++; // Chuyển sang tháng kế tiếp
                    monthTotal = 0; // Reset tổng doanh thu cho tháng tiếp theo
                    count = 0; // Reset số lượng bản ghi
                }
            }

            // Nếu có tháng nào chưa đủ 4 bản ghi, gán 0 vào tháng đó
            if (count > 0 && monthIndex < 12)
            {
                revenueData[monthIndex] = 0; // Gán 0 nếu chưa đủ 4 bản ghi
            }

            // Đảm bảo có đủ 12 tháng trong danh sách (điều chỉnh tháng chưa có doanh thu)
            while (monthIndex < 12)
            {
                revenueData[monthIndex] = 0; // Thêm tháng 0 nếu không có dữ liệu
                monthIndex++;
            }

            // Tạo nhãn cho từng tháng (T1, T2,..., T12)
            var labels = new List<string>();
            for (int i = 1; i <= 12; i++)
            {
                labels.Add("T" + i); // Tạo nhãn cho từng tháng
            }

            // Truyền dữ liệu vào ViewBag dưới dạng JSON
            ViewData["RevenueData"] = JsonConvert.SerializeObject(revenueData);
            ViewData["Labels"]      = JsonConvert.SerializeObject(labels);

            //// Giả sử đây là dữ liệu doanh thu theo quý
            //var revenueData = new List<int> { 12000, 19000, 30000, 50000 , 12000, 19000, 30000, 50000, 12000, 19000, 30000, 50000 };  // Dữ liệu doanh thu
            //var labels = new List<string> { "T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "T10", "T11", "T12" }; // Nhãn cho các quý

            //// Truyền dữ liệu vào ViewBag dưới dạng JSON¥
            //ViewBag.RevenueData = JsonConvert.SerializeObject(revenueData);
            //ViewBag.Labels = JsonConvert.SerializeObject(labels);

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