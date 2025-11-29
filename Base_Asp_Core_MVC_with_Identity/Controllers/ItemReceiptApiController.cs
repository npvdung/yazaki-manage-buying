using Base_Asp_Core_MVC_with_Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace MangagerBuyProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemReceiptApiController : ControllerBase
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _uid;

        public ItemReceiptApiController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> uid)
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

                var customerData =
                    from tempcustomer in _context.ItemReceipt
                    // join shipment
                    join temp1 in _context.shipmentRequests
                        on tempcustomer.ShipmentRequestId equals temp1.ID.ToString() into tempTable
                    from leftJoinData in tempTable.DefaultIfEmpty()
                    // join stock
                    join temp2 in _context.inventory
                        on tempcustomer.StockId equals temp2.ID.ToString() into tempTable2
                    from leftJoinData1 in tempTable2.DefaultIfEmpty()
                    // join purchase order
                    join po in _context.purchaseOrders
                        on leftJoinData.PurchaseOrderId equals po.ID.ToString() into poTable
                    from poData in poTable.DefaultIfEmpty()
                    // join purchase contract (để lấy tên đơn hàng = tên hợp đồng)
                    join contract in _context.purchaseContracts
                        on poData.PurchaseContractId equals contract.ID.ToString() into contractTable
                    from contractData in contractTable.DefaultIfEmpty()
                    select new
                    {
                        tempcustomer.ID,
                        inventoryName = leftJoinData1 != null ? leftJoinData1.Name : string.Empty,
                        shipCode      = leftJoinData  != null ? leftJoinData.ShipmentRequestCode : string.Empty,
                        orderCode     = poData        != null ? poData.PurchaseOrderCode : string.Empty,
                        orderName     = contractData  != null ? contractData.PurchaseContractName : string.Empty,
                        tempcustomer.Status
                    };

                // filter theo search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m =>
                        m.inventoryName.Contains(searchValue) ||
                        m.shipCode.Contains(searchValue)      ||
                        m.orderCode.Contains(searchValue)     ||
                        m.orderName.Contains(searchValue));
                }

                // LUÔN sắp xếp mới → cũ theo mã vận đơn
                customerData = customerData
                    .OrderByDescending(m => m.shipCode);

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
