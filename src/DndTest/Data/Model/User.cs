namespace DndTest.Data.Model;

public class User
{
    public int Id { get; set; }
    
    public string Email { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public int TenantId { get; set; }
}
