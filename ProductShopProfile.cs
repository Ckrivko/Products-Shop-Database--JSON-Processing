using AutoMapper;
using ProductShop.DTOs.Categories_Products;
using ProductShop.DTOs.Category;
using ProductShop.DTOs.Product;
using ProductShop.DTOs.User;
using ProductShop.Models;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {

            this.CreateMap<ImportUserDto, User>();
            this.CreateMap<ImportProductDto, Product>();
            this.CreateMap<ImportCategoryDto, Category>();
            this.CreateMap<ImportCategoryProductDto, CategoryProduct>();
            this.CreateMap<Product, ExportProductsInRange>()
                .ForMember(d => d.SellerFullName, mo => mo.MapFrom(s => $"{s.Seller.FirstName} {s.Seller.LastName}"));

            //Inner Dto 06.
            this.CreateMap<Product, ExportUserSoldProductsDto>()
                .ForMember(x => x.BuyerFirstName, mo => mo.MapFrom(s => s.Buyer.FirstName))
                .ForMember(x => x.BuyerLastName, mo => mo.MapFrom(s => s.Buyer.LastName));

            //Outer Dto 06.
            this.CreateMap<User, ExportUsersWithSoldProductsDto>()
                .ForMember(d => d.SoldProducts, mo => mo.MapFrom(x => x.ProductsSold
                .Where(x => x.BuyerId.HasValue)));



            this.CreateMap<Product, ExportProductsShortInfo>();
            
            this.CreateMap<User, ExportProductsFullInfo>()                
                .ForMember(x => x.Products, mo => mo.MapFrom(x => x.ProductsSold
                .Where(x => x.BuyerId.HasValue)));

            this.CreateMap<User, ExportUserFullInfo>()
                .ForMember(x=>x.SoldProductsInfo,mo=>mo.MapFrom(x=>x));
                





        }
    }
}
