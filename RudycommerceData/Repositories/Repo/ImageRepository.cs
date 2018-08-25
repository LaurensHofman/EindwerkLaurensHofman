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

        /// <summary>
        /// My Cloudinary credentials
        /// </summary>
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

            // If the LogoURL is already filled in (for example when it's an existing brand coming from the database), use that path as image.
            if (brand.LogoURL != null)
            {
                path = brand.LogoURL;
            }
            else
            {
                // If the image is still locally, use the local path to find the image
                if (brand.LocalLogoPath != null)
                {
                    path = brand.LocalLogoPath;
                }
                else
                {
                    throw new RudycommerceLib.CustomExceptions.ImagePathToSaveNotFound();
                }
            }

            // Creates a cloadinary connection based on my credentials
            Cloudinary cloudinary = new Cloudinary(MyAccount);
            
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(path),
                PublicId = $"ID{brand.ID}",
                Overwrite = true,
                Folder = $"Brands/{brand.ID.ToString()}"
            };

            // Uploads the image
            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            // The uploadResult contains the URL to the image
            string url = uploadResult.Uri.ToString();

            // Returns the URL, so it can be added to the Brand model 
            return url;
        }

        public async Task<string> SaveImage(ProductImage img, int productID)
        {
            string path;

            // If the ImageURL is already filled in (for example when it's an existing ProductImage coming from the database), use that path as image.
            if (img.ImageURL != null)
            {
                path = img.ImageURL;
            }
            else
            {               
                // If the image is still locally, use the local path to find the image
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

            // Return the Image URL so it can be added to the ProductImage model
            return url;
        }
    }
}
