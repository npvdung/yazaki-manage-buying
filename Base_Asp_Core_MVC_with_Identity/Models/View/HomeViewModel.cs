namespace MangagerBuyProduct.Models.View
{
    public class HomeViewModel
    {
        public int Sales { get; set; }
        public decimal Revenue { get; set; }
        public decimal NewClients { get; set; }
        public List<AuditLog> Logs { get; set; }
    }
}
