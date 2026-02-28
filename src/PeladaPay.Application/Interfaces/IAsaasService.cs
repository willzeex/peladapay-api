using PeladaPay.Application.Models.Asaas;

namespace PeladaPay.Application.Interfaces;

public interface IAsaasService
{
    Task<AsaasCreateAccountResponse> CreateSubaccountAsync(AsaasCreateAccountRequest request, CancellationToken cancellationToken);
}
