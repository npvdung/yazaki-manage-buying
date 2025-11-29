using Base_Asp_Core_MVC_with_Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;

namespace MangagerBuyProduct.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnAuthorizationApiController : ControllerBase
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;
        private readonly UserManager<UserSystemIdentity> _uid;

        public ReturnAuthorizationApiController(Base_Asp_Core_MVC_with_IdentityContext context, UserManager<UserSystemIdentity> uid)
        {
            _uid = uid;
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                var draw = Request.Query["draw"].FirstOrDefault();
                var start = Request.Query["start"].FirstOrDefault();
                var length = Request.Query["length"].FirstOrDefault();
                var sortColumn = Request.Query["columns[" + Request.Query["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Query["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Query["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // join ReturnAuthorization -> PurchaseOrder -> PurchaseContract
                var customerData =
                    from ra in _context.returnAuthorizations
                    join po in _context.purchaseOrders
                        on ra.PurchaseOrderId equals po.ID.ToString() into poTable
                    from poLeft in poTable.DefaultIfEmpty()
                    join pc in _context.purchaseContracts
                        on poLeft.PurchaseContractId equals pc.ID.ToString() into pcTable
                    from pcLeft in pcTable.DefaultIfEmpty()
                    select new
                    {
                        ra.ID,
                        ra.ReturnAuthorizationCode,
                        // Mã đặt hàng
                        nameOrder = poLeft != null ? poLeft.PurchaseOrderCode : null,
                        // Tên đơn hàng (tên hợp đồng mua)
                        orderName = pcLeft != null ? pcLeft.PurchaseContractName : null,
                        ra.AmountReturn,
                        // Ngày trả
                        ra.DateShip,
                        // Lý do (map từ Note, nhưng để tên field là Description cho khớp JS)
                        Description = ra.Note
                    };

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    customerData = customerData.OrderBy(sortColumn + " " + sortColumnDirection);
                }

                if (!string.IsNullOrEmpty(searchValue))
                {
                    customerData = customerData.Where(m =>
                        m.ReturnAuthorizationCode.Contains(searchValue) ||
                        (!string.IsNullOrEmpty(m.nameOrder) && m.nameOrder.Contains(searchValue)) ||
                        (!string.IsNullOrEmpty(m.orderName) && m.orderName.Contains(searchValue)) ||
                        (!string.IsNullOrEmpty(m.Description) && m.Description.Contains(searchValue))
                    );
                }

                recordsTotal = customerData.Count();
                var data = customerData.Skip(skip).Take(pageSize).ToList();
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
