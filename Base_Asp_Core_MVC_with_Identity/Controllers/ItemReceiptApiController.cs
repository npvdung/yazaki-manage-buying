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
                    join temp1 in _context.shipmentRequests
                        on tempcustomer.ShipmentRequestId equals temp1.ID.ToString() into tempTable
                    from leftJoinData in tempTable.DefaultIfEmpty()
                    join temp2 in _context.inventory
                        on tempcustomer.StockId equals temp2.ID.ToString() into tempTable2
                    from leftJoinData1 in tempTable2.DefaultIfEmpty()
                    select new
                    {
                        tempcustomer.ID,
                        inventoryName = leftJoinData1 != null ? leftJoinData1.Name : string.Empty,
                        shipCode      = leftJoinData  != null ? leftJoinData.ShipmentRequestCode : string.Empty,
                        tempcustomer.Status
                    };

                // filter theo search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m =>
                        m.inventoryName.Contains(searchValue) ||
                        m.shipCode.Contains(searchValue));
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
