namespace TaskAuthenticationAuthorization.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? RoleId { get; set; }        
        public Role Role { get; set; }
        public BuyerType Type { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
    }

    public enum BuyerType { None, Regular, Golden, Wholesale }
}
