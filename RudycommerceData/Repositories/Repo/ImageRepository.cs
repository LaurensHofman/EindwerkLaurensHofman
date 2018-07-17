using RudycommerceData.Entities.Products.Products;
using RudycommerceData.Repositories.BaseRepo;
using RudycommerceData.Repositories.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace RudycommerceData.Repositories.Repo
{
    public class ImageRepository : BaseRepository<ProductImage>, IImageRepository
    {
        private readonly Account MyAccount;

        public ImageRepository()
        {
            MyAccount = new Account(
                "dhgcdhtlx",
                "874148858628711",
                "N7fTdEIRuW_vflagtsRJnAttx6A");
        }

        public async Task<string> SaveImage(Brand brand)
        {
            string path;

            if (brand.LogoURL != null)
            {
                path = brand.LogoURL;
            }
            else
            {
                if (brand.LocalLogoPath != null)
                {
                    path = brand.LocalLogoPath;
                }
                else
                {
                    return null;
                }
            }

            Cloudinary cloudinary = new Cloudinary(MyAccount);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(path),
                PublicId = $"ID{brand.ID}",
                Overwrite = true,
                Folder = $"Brands/{brand.ID.ToString()}"
            };

            var uploadResult = await Task.FromResult(cloudinary.UploadAsync(uploadParams).Result);

            string url = uploadResult.Uri.ToString();

            return url;
        }

        public Task<string> SaveImage(Product product)
        {
            throw new NotImplementedException();
        }
    }
}
