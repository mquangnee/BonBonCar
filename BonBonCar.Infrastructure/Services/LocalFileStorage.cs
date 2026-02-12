using BonBonCar.Domain.IService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BonBonCar.Infrastructure.Services
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LocalFileStorage(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task SaveAsync(IFormFile file, string folder, CancellationToken cancellationToken)
        {
            var uploadsRoot = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            var targetFolder = Path.Combine(uploadsRoot, folder);

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";

            var fullPath = Path.Combine(targetFolder, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream, cancellationToken);
        }
    }
}
