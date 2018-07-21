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

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            string url = uploadResult.Uri.ToString();

            return url;
        }

        public async Task<string> SaveImage(ProductImage img, int productID)
        {
            string path;

            if (img.ImageURL != null)
            {
                path = img.ImageURL;
            }
            else
            {
                if (img.FileLocation != null)
                {
                    path = img.FileLocation;
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
                PublicId = $"ID{productID}Ord{img.Order}",
                Overwrite = true,
                Folder = $"Products/{productID}"
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            string url = uploadResult.Uri.ToString();

            return url;
        }
    }
}
