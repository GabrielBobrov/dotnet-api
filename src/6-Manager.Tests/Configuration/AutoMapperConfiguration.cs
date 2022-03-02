using AutoMapper;
using Manager.API.ViewModels;
using Manager.Domain.Entities;
using Manager.Services.DTO;

namespace Manager.Tests.Configurations.AutoMapper
{
    public static class AutoMapperConfiguration
    {
        public static IMapper GetConfiguration()
        {
            var autoMapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>()
                    .ReverseMap();

                cfg.CreateMap<CreateUserViewModel, UserDto>()
                    .ReverseMap();

                cfg.CreateMap<UpdateUserViewModel, UserDto>()
                    .ReverseMap();
            });

            return autoMapperConfig.CreateMapper();
        }
    }
}