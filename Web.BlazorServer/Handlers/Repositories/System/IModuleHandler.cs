using Application.DataTransferObjects.System;

namespace Web.BlazorServer.Handlers.Repositories.System;

public interface IModuleHandler
{
    Task<(IEnumerable<ModuleDataGridVM> Data, int Count)> GetModuleTableDetailsAsync(DataGridIntent intent);
}
