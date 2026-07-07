using Application.DataTransferObjects.Administration.User;
using Application.DataTransferObjects.Others;
using Application.DataTransferObjects.Transactions.Commons;
using Application.DataTransferObjects.Transactions.Procurement.Order;
using Domain.Entities.Administration.User.Management;
using Domain.ValueObjects.Others;
using Domain.ValueObjects.Transaction;
using Mapster;

namespace Application.UseCases.Registers;

public class MappingRegistration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        #region DEM to DTO

        #region User Management
        config.NewConfig<UserDEM, UserDataGridDTO>()
            .Map(d => d.Id, s => s.Id)
            .Map(d => d.FullName, s => s.Name.FullName)
            .Map(d => d.Email, s => s.Email.Address)
            .Map(d => d.Phone, s => s.PhoneNumber)
            .Map(d => d.Position, _ => string.Empty)
            .Map(d => d.Active, s => s.Active);
        #endregion User Management

        #endregion DEM to DTO

        #region DTO to VO

        #region System

        #region Common
        config.NewConfig<PersonNameDTO, PersonNameVO>()
            .ConstructUsing(dto => new PersonNameVO(
                dto.FirstName,
                dto.MiddleName,
                dto.LastName));
        #endregion Common

        #region Transactional Documents
        config.NewConfig<SapDocumentReferenceDTO, SapDocumentReferenceVO>()
            .ConstructUsing(dto => new SapDocumentReferenceVO(
                dto.DocEntry,
                dto.DocNum));
        #endregion Transactional Documents

        #endregion System

        #region Administration

        #region User Management

        config.NewConfig<UserNameDTO, UserNameVO>()
            .ConstructUsing(dto => new UserNameVO(dto.Value));
        config.NewConfig<EmailDTO, EmailVO>()
            .ConstructUsing(dto => new EmailVO(dto.Address));
        config.NewConfig<AccountDTO, AccountVO>()
            .ConstructUsing(dto => new AccountVO(
                dto.UserName.Adapt<UserNameVO>(),
                dto.HashedPassword,
                dto.LockoutEnabled,
                dto.Locked));

        #endregion User Management

        #endregion Administration

        #endregion DTO to VO
    }
}

