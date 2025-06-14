using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace FoodRestaurant.Services
{
    public class ImageService
    {
        private readonly IWebHostEnvironment _environment;
        // Set fixed dimensions for all cart images
        private const int FixedWidth = 300;
        private const int FixedHeight = 300;

        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string GetWebRootPath()
        {
            return _environment.WebRootPath;
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile, string category)
        {
            if (imageFile == null || imageFile.Length == 0)
                return string.Empty;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", category);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var image = await Image.LoadAsync(imageFile.OpenReadStream()))
            {
                image.Mutate(x => x
                    .Resize(new ResizeOptions
                    {
                        Size = new Size(FixedWidth, FixedHeight),
                        Mode = ResizeMode.Crop,
                        Position = AnchorPositionMode.Center
                    })
                    .AutoOrient());

                await image.SaveAsync(filePath);
            }

            return $"/images/{category}/{uniqueFileName}";
        }
    }
}