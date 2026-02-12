using Microsoft.AspNetCore.Http;

namespace BonBonCar.Domain.IService
{
    public interface IFileStorage
    {
        Task SaveAsync(IFormFile file, string folder, CancellationToken cancellationToken);
    }
}
