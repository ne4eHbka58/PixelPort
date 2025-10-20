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
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Characteristics, opt => opt.MapFrom(src => src.Characteristics));

            CreateMap<Product, ProductCreateDTO>();

            CreateMap<Product, ProductUpdateDTO>();

            CreateMap<ProductUpdateDTO, Product>()
                .ForMember(dest => dest.Characteristics, opt => opt.Ignore()) // Игнорируем характеристики
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<Product, ProductResponseDTO>().ReverseMap();

            CreateMap<ProductCharacteristic, ProductCharacteristicCreateDTO>().ReverseMap();
            CreateMap<ProductCharacteristic, ProductCharacteristicResponseDTO>().ReverseMap();

        }
    }
}
