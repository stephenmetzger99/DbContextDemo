namespace DbContextDemo.API.Persistance.Models.Base;

public class BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
}
