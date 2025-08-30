// using ... your other namespaces (repositories, dtos, etc.)

namespace DbContextDemo.API;

public sealed class UserService
(
    IUserRepository userRepository,
    IAddressService addressService,
    IDbContextFactory<AppDbContext> dbFactory,
    ILogger<UserService> logger
) : IUserService
{
    public async Task<UserDto> AddUserAsync(UserDto user, CancellationToken ct = default)
    {
        var graphUserId = await GetGraphUserIdAsync(user, ct).ConfigureAwait(false);

        var newUser = new UserDto(user.LastName, user.FirstName, user.Email)
        {
            Id = graphUserId,
            PhoneNumber = user.PhoneNumber,
            BranchId = user.BranchId,
            CountyId = user.CountyId,
            UserTypeId = user.UserTypeId,
            ElectedOfficial = user.ElectedOfficial,
            RoleId = user.RoleId,
            SubscriptionIds = user.SubscriptionIds,
            CreatedByUserId = user.CreatedByUserId,
            CreatedDate = user.CreatedDate,
            ModifiedByUserId = user.ModifiedByUserId,
            ModifiedDate = user.ModifiedDate,
            PermissionIds = user.PermissionIds,
            TenantId = user.TenantId,
            Address = user.Address
        };

        await dbFactory.InUowAsync(async () =>
        {
            Guid? addressId = null;

            if (newUser.Address is not null)
            {
                var ensured = await addressService.AddAsync(newUser.Address, ct).ConfigureAwait(false);
                addressId = ensured.Id;
            }

            var userDao = await CreateUserAndPersistAsync(newUser, addressId, ct).ConfigureAwait(false);

            if (newUser.BranchId is not null)
                await UpdateBranchUserFromBranchDaoAsync(userDao, userDao.UserTypeId, newUser.BranchId, isAdd: true, ct)
                    .ConfigureAwait(false);

        }, ct).ConfigureAwait(false);

        return (await userRepository.GetByIdAsync(newUser.Id, ct).ConfigureAwait(false)).ToUserDto();
    }

    private async Task<User> CreateUserAndPersistAsync(UserDto userDto, Guid? addressId, CancellationToken ct = default)
    {
        var userDao = userDto.ToUser();
        var emailDomain = userDto.Email.RightOf('@');
        var branch = (await branchRepository.GetAllThatMatchAsync(x => x.Domain == emailDomain, ct).ConfigureAwait(false)).FirstOrDefault();
        if (branch is not null) userDao.UserTypeId = UserTypes.County.Id;

        await SetRoleDefaultsAsync(userDao, ct).ConfigureAwait(false);
        userDao.Id = userDto.Id;
        userDao.IsFirstSignIn = false;
        userDao.IsActive = true;
        SetUserType(userDao);
        if (userDao.ElectedOfficial is not null)
            _ = await ValidateElectedOfficialAsync(userDao.ElectedOfficial, ct).ConfigureAwait(false);

        userDao.AddressId = addressId;
        userDao.Address = null;

        if (branch is not null)
            await SetUserSubscriptionsByCountyAsync(userDao, branch, ct).ConfigureAwait(false);

        await userRepository.AddAsync(userDao, ct).ConfigureAwait(false); // ambient

        return userDao;
    }

}
