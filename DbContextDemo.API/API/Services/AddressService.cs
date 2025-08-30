namespace DbContextDemo.API.Services;

public sealed class AddressService(ILogger<AddressService> logger, IAddressRepository repo) : IAddressService
{
    public async Task<AddressDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        => (await repo.GetByIdAsync(id, ct).ConfigureAwait(false)).ToAddressDto();

    public async Task<AddressDto?> GetIfExistsAsync(AddressDto dto, CancellationToken ct = default)
        => (await repo.GetIfExistsAsync(dto.ToAddress(), ct).ConfigureAwait(false))?.ToAddressDto();

    /// <summary>
    /// If an equivalent address exists, return it; else insert a new Address.
    /// Works both inside and outside an ambient UoW.
    /// </summary>
    public async Task<AddressDto> AddAsync(AddressDto dto, CancellationToken ct = default)
    {
        var existing = await GetIfExistsAsync(dto, ct).ConfigureAwait(false);
        if (existing is not null) return existing;

        var dao = dto.ToAddress();
        await repo.AddAsync(dao, ct).ConfigureAwait(false); // ambient-aware Add
        return dao.ToAddressDto();
    }

    public async Task<AddressDto> UpdateAsync(AddressDto dto, CancellationToken ct = default)
    {
        var existing = await GetIfExistsAsync(dto, ct).ConfigureAwait(false);
        if (existing is not null) return existing;

        var dao = dto.ToAddress();
        await repo.UpdateAsync(dao, ct).ConfigureAwait(false);
        return dao.ToAddressDto();
    }
}
