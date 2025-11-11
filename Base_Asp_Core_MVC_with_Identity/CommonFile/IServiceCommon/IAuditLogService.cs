using Base_Asp_Core_MVC_with_Identity.Data;
using MangagerBuyProduct.Models;

namespace MangagerBuyProduct.CommonFile.IServiceCommon
{
    public interface IAuditLogService
    {
        Task LogAsync(string userId, string action, string tableName, string recordId, string changes);
    }
    public class AuditLogService : IAuditLogService
    {
        private readonly Base_Asp_Core_MVC_with_IdentityContext _context;

        public AuditLogService(Base_Asp_Core_MVC_with_IdentityContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string userId, string action, string tableName, string recordId, string changes)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                TableName = tableName,
                RecordId = recordId,
                Changes = changes,
                Timestamp = DateTime.Now
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
