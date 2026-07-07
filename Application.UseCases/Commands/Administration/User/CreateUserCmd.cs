using Application.DataTransferObjects.Administration.User;
using Application.UseCases.Repositories.Bases;
using DataCipher;
using Domain.Entities.Administration.User.Management;
using Domain.Entities.Administration.User.Role;
using Domain.ValueObjects.Others;
using MediatR;

namespace Application.UseCases.Commands.Transaction.Administration.User;

public record CreateUserCmd(UserDTO User) : ITransactionalRequest<bool>;

public class CrateUserCmdHandler(
    IAppReadRepository appReadRepo,
    IAppCommandRepository appCommandRepo)
    : IRequestHandler<CreateUserCmd, bool>
{
    public async Task<bool> Handle(CreateUserCmd request, CancellationToken cancellationToken)
    {
        string username = request.User.Account.UserName.Value;

        if (await appReadRepo.ExistsAsync<UserDEM>(x => x.Account.UserName.Value.ToLower() == username.ToLower()))
            throw new Exception("Username already in use");
        if (await appReadRepo.ExistsAsync<UserDEM>(x => x.Email.Address.ToLower() == request.User.Email.Address.ToLower()))
            throw new Exception("Email Address already in use");

        request.User.Account.HashedPassword = Encryption.Encrypt(request.User.Account.HashedPassword);

        List<RolePermissionDEM> rolePermissions = await appReadRepo.GetListAsync<RolePermissionDEM>(x => x.RoleId == request.User.Role.Id);

        UserDEM dem = UserDEM.Create(new PersonNameVO(request.User.Name.FirstName,
                                                      request.User.Name.MiddleName,
                                                      request.User.Name.LastName),
                                     new EmailVO(request.User.Email.Address),
                                     new AccountVO(new UserNameVO(username),
                                                   request.User.Account.HashedPassword,
                                                   request.User.Account.LockoutEnabled),
                                     request.User.Company,
                                     request.User.Role.Id,
                                     request.User.PhoneNumber);

        dem.AddPermission([.. rolePermissions.Select(rp => {
                return UserPermissionDEM.Create(rp.ModulePermission, dem.Id);
            })]);

        await appCommandRepo.AddAsync(dem);

        return true;
    }
}
