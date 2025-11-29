using Base_Asp_Core_MVC_with_Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace MangagerBuyProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipmentRequestApiController : ControllerBase
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _uid;

        public ShipmentRequestApiController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> uid)
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

                // JOIN: ShipmentRequest -> PurchaseOrder -> PurchaseContract
                var customerData =
                    from tempcustomer in _context.shipmentRequests
                    join poTmp in _context.purchaseOrders
                        on tempcustomer.PurchaseOrderId equals poTmp.ID.ToString() into tempTable
                    from po in tempTable.DefaultIfEmpty()
                    join pcTmp in _context.purchaseContracts
                        on po.PurchaseContractId equals pcTmp.ID.ToString() into tempTable2
                    from pc in tempTable2.DefaultIfEmpty()
                    select new
                    {
                        tempcustomer.ID,
                        tempcustomer.ShipmentRequestCode,
                        tempcustomer.ShipmentRequestType,
                        // Mã đặt hàng (code của PurchaseOrder)
                        PurchaseOrderCode = po != null ? po.PurchaseOrderCode : string.Empty,
                        // Tên hợp đồng mua hàng (name của PurchaseContract)
                        PurchaseContractName = pc != null ? pc.PurchaseContractName : string.Empty,
                        tempcustomer.Status
                    };

                // filter theo search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m =>
                        m.ShipmentRequestCode.Contains(searchValue) ||
                        m.PurchaseOrderCode.Contains(searchValue) ||
                        m.PurchaseContractName.Contains(searchValue));
                }

                // LUÔN sắp xếp mới → cũ theo mã SHIP_...
                customerData = customerData
                    .OrderByDescending(m => m.ShipmentRequestCode);

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
