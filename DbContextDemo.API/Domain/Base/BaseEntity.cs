namespace DbContextDemo.API.Domain.Base;

public class BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
}
