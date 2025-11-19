using Base_Asp_Core_MVC_with_Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace MangagerBuyProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderApiController : ControllerBase
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _uid;

        public PurchaseOrderApiController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> uid)
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
                    from tempcustomer in _context.purchaseOrders
                    join temp1 in _context.purchaseContracts
                        on tempcustomer.PurchaseContractId equals temp1.ID.ToString() into tempTable
                    from leftJoinData in tempTable.DefaultIfEmpty()
                    select new
                    {
                        tempcustomer.ID,
                        tempcustomer.PurchaseOrderCode,
                        purchaseContractName = leftJoinData != null ? leftJoinData.PurchaseContractName : string.Empty,
                        tempcustomer.Status
                    };

                // filter theo search box
                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m =>
                        m.PurchaseOrderCode.Contains(searchValue) ||
                        m.purchaseContractName.Contains(searchValue));
                }

                // LUÔN sắp xếp mới → cũ theo mã đơn hàng
                customerData = customerData
                    .OrderByDescending(m => m.PurchaseOrderCode);

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
