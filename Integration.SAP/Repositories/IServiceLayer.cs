using B1SLayer;

namespace Integration.SAP.Repositories;

public interface IServiceLayer
{
    SLConnection Access { get; }
}
