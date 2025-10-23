using AutoMapper;
using PixelPort.Server.Models;
using PixelPort.Server.Models.Dto;
using PixelPort.Server.Models.DTO;

namespace PixelPort.Server
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<ProductCreateDTO, Product>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now)) // Задаём дату создания
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.Now)) // Задаём дату обновления
                .ForMember(dest => dest.Characteristics, opt => opt.MapFrom(src => src.Characteristics));

            CreateMap<Product, ProductCreateDTO>();

            CreateMap<ProductUpdateDTO, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())  // Не меняем айди
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()); // Не меняем дату создания

            CreateMap<Product, ProductUpdateDTO>();

            CreateMap<ProductUpdateDTO, Product>()
                .ForMember(dest => dest.Characteristics, opt => opt.Ignore()) // Игнорируем характеристики
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.Now)); // Меняем дату обновления

            CreateMap<Product, ProductResponseDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName)) // Берём название категории из другой таблицы
                .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Manufacturer.ManufacturerName)); // Берём название производителя из другой таблицы

            CreateMap<ProductResponseDTO, Product>();


            CreateMap<ProductCharacteristic, ProductCharacteristicCreateDTO>().ReverseMap();
            CreateMap<ProductCharacteristic, ProductCharacteristicResponseDTO>().ReverseMap();

            CreateMap<ProductCharacteristicUpdateDTO, ProductCharacteristic>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Не меняем айди
                .ForMember(dest => dest.ProductId, opt => opt.Ignore()); // Не меняем айди товара

            CreateMap<ProductCharacteristic, ProductCharacteristicUpdateDTO>();
        }
    }
}
