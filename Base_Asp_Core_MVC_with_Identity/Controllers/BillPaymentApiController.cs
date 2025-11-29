using Base_Asp_Core_MVC_with_Identity.CommonFile.Enum;
using Base_Asp_Core_MVC_with_Identity.CommonMethod;
using Base_Asp_Core_MVC_with_Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace MangagerBuyProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillPaymentApiController : ControllerBase
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _uid;

        public BillPaymentApiController(
            Base_Asp_Core_MVC_with_IdentityContext context,
            UserManager<UserSystemIdentity> uid)
        {
            _uid = uid;
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                var draw        = Request.Query["draw"].FirstOrDefault();
                var start       = Request.Query["start"].FirstOrDefault();
                var length      = Request.Query["length"].FirstOrDefault();
                var searchValue = Request.Query["search[value]"].FirstOrDefault();

                int pageSize = !string.IsNullOrEmpty(length) ? Convert.ToInt32(length) : 0;
                int skip     = !string.IsNullOrEmpty(start)  ? Convert.ToInt32(start)  : 0;
                int recordsTotal = 0;

                // BillPayment -> ItemReceipt -> ShipmentRequest -> PurchaseOrder -> PurchaseContract
                var customerData =
                    from bill in _context.BillPayment
                    join ir in _context.ItemReceipt
                        on bill.ItemReceiptId equals ir.ID.ToString() into irTable
                    from ir in irTable.DefaultIfEmpty()

                    join ship in _context.shipmentRequests
                        on ir.ShipmentRequestId equals ship.ID.ToString() into shipTable
                    from ship in shipTable.DefaultIfEmpty()

                    join po in _context.purchaseOrders
                        on ship.PurchaseOrderId equals po.ID.ToString() into poTable
                    from po in poTable.DefaultIfEmpty()

                    join contract in _context.purchaseContracts
                        on po.PurchaseContractId equals contract.ID.ToString() into ctTable
                    from contract in ctTable.DefaultIfEmpty()

                    select new
                    {
                        bill.ID,
                        bill.BillPaymentCode,
                        bill.BillPaymentDate,
                        totalAmount = bill.TotalAmount,
                        bill.Status,

                        // Mã / tên đơn hàng để hiển thị
                        OrderCode = po != null ? po.PurchaseOrderCode : string.Empty,
                        OrderName = contract != null ? contract.PurchaseContractName : string.Empty,

                        // Trạng thái dạng text
                        StatusText = bill.Status.HasValue
                            ? ((EnumReceipt)bill.Status.Value).GetDisplayName()
                            : string.Empty
                    };

                // filter theo search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m =>
                        m.BillPaymentCode.Contains(searchValue) ||
                        m.OrderCode.Contains(searchValue) ||
                        m.OrderName.Contains(searchValue));
                }

                // Sắp xếp mới -> cũ theo ngày thanh toán
                customerData = customerData
                    .OrderByDescending(m => m.BillPaymentDate);

                recordsTotal = customerData.Count();

                var data = customerData
                    .Skip(skip)
                    .Take(pageSize)
                    .ToList();

                var jsonData = new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = data
                };

                return Ok(jsonData);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
