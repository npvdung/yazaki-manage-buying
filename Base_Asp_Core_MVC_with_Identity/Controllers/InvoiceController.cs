using Base_Asp_Core_MVC_with_Identity.CommonFile.Enum;
using Base_Asp_Core_MVC_with_Identity.CommonFile.IServiceCommon;
using Base_Asp_Core_MVC_with_Identity.Data;
using Base_Asp_Core_MVC_with_Identity.Models;
using Base_Asp_Core_MVC_with_Identity.Models.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace Base_Asp_Core_MVC_with_Identity.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private UserManager<UserSystemIdentity> _userManager;
        private readonly ICommonService _commonService;

        public InvoiceController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> userManager, ICommonService commonService)
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
            IEnumerable<Invoice> objCatlist = _context.Invoices;
            return View(objCatlist);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            Invoice product = new Invoice();

            string prefix = "SP_";
            var ramdonId = Guid.NewGuid();
            Expression<Func<Invoice, string>> codeSelector = c => c.InvoiceCode;
            string autoCode = _commonService.GenerateCategoryCode(prefix, codeSelector);
            product.InvoiceCode = autoCode;

            var viewModel = new InvoiceViewModel();
            viewModel.InvoiceCode = autoCode;
            viewModel.Description = "Không";
            viewModel.InvoiceDate = DateTime.Now;
            viewModel.ID = ramdonId;
            viewModel.ProductDetails = Enumerable.Range(0, 5).Select(_ => new Models.View.Invoice_Details()
            {
                Description = "Khong",
            }).ToList();


            var productList = (from p in _context.Products
                               join s in _context.suppliers on p.SupplierId equals s.ID.ToString()
                               select new
                               {
                                   ProductId = p.ID,
                                   ProductName = p.ProductName,
                                   SupplierName = s.SupplierName
                               }).ToList();

            List<SelectListItem> itemData = new List<SelectListItem>();

            foreach (var item in productList)
            {
                itemData.Add(new SelectListItem
                {
                    Value = $"{item.ProductName} - {item.SupplierName}",
                    Text = item.ProductId.ToString()
                });
            }
            ViewBag.product_lst = itemData;

            var customerList = (from p in _context.Customers
                               select new
                               {
                                   customerId = p.ID,
                                   customerName = p.FullName,
                               }).ToList();

            List<SelectListItem> itemDataCustomer = new List<SelectListItem>();

            foreach (var item in customerList)
            {
                itemDataCustomer.Add(new SelectListItem
                {
                    Value = $"{item.customerName}",
                    Text = item.customerId.ToString()
                });
            }
            ViewBag.customer_lst = itemDataCustomer;
            viewModel.UserId = "0d413f2c-1d8d-4c9e-a237-c6abdfc12f2b";
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(ImportViewModel empobj)
        {
            empobj.UserId = "0d413f2c-1d8d-4c9e-a237-c6abdfc12f2b";
            empobj.SupplierId = "08dc620d-b70b-4bec-8957-92617c38b23b";
            empobj.Description = "Không";
            empobj.TotalAmount = (decimal)empobj.TotalAmount;
            empobj.TotalAmount = 300000;
            //var empobj = new ImportViewModel();
            //if (ModelState.IsValid)
            //{
            //thêm vào bảng master
            var master = new Import_Product()
            {
                ImportCode = empobj.ImportCode,
                ImportName = empobj.ImportName,
                ImportDate = empobj.ImportDate,
                SupplierId = empobj.SupplierId,
                Description = empobj.Description,
                TotalAmount = empobj.TotalAmount,
                UserId = empobj.UserId,
            };
            _context.ImportsProduct.Add(master);
            //_context.SaveChanges();

            var details = new List<Models.Import_Product_Details>();


            //thêm vào bảng details
            foreach (Models.View.Import_Product_Details item in empobj.ProductDetails)
            {
                if (item.ProduceId != null)
                {
                    details.Add(new Models.Import_Product_Details()
                    {
                        ImportProductId = master.ID.ToString(),
                        Description = "Không",
                        ProductionBatch = DateTime.Now,
                        ManufacturingDate = DateTime.Now,
                        ExpirationData = item.ExpirationData,
                        Unit = 1,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        ProduceId = item.ProduceId,

                    });
                }
            }

            _context.ImportProductDetails.AddRange(details);


            //thêm vào kho

            //khi thêm vào thì check nếu mặt hàng đó chưa có (Có productId , nhà cung cấp, và hạn sử dụng thì update)

            var stock = new Inventory();
            List<Inventory> allStock = _context.inventory.ToList();
            //var existStock = allStock.FindAll(x => details.Select(y => y.ProduceId).Contains(x.ID.ToString())
            //                        && details.Select(z => z.ExpirationData).Contains(x.Date));
            //foreach (var item in existStock)
            //{
            //    var id = item.ID.ToString();
            //    var itemE = _context.stocks.Find(id);
            //    itemE.Quantity += item.Quantity;
            //    _context.stocks.Update(itemE);
            //}
            //var idExist = existStock.Select(x => x.ProductId.ToString());
            //foreach (var item in details)
            //{
            //    if (!idExist.Contains(item.ProduceId))
            //    {
            //        var itemStock = new Inventory()
            //        {
            //            ProductId = item.ProduceId,
            //            ExpirationData = item.ExpirationData,
            //            ManufacturingDate = item.ManufacturingDate,
            //            PriceSecond = item.Price,
            //            ProductionBatch = item.ProductionBatch,
            //            Quantity = item.Quantity.ToString(),
            //        };
            //        _context.stocks.Add(itemStock);
            //    }
            //}
            _context.SaveChanges();

            //tìm ra đúng loại và cập nhật số lượng

            //còn không thì thêm thếm
            //còn không thì thêm mới
            TempData["ResultOk"] = "Tạo dữ liệu thành công !";
            return RedirectToAction("Index");
            //}

            return View(empobj);
        }
    }
}
