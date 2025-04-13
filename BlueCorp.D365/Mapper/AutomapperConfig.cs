using AutoMapper;
using BlueCorp.Common;
using BlueCorp.D365.DTO;

namespace BlueCorp.D365.Mapper
{
    public class AutomapperConfig : Profile
    {
        private Func<string, ContainerType> ConvertToEnumType = (string type) =>
        {
            ContainerType t;
            switch (type.Trim().ToUpper())
            {
                case "20RF":
                    t = ContainerType.RF20;
                    break;
                case "40RF":
                    t = ContainerType.RF40;
                    break;
                case "40HC":
                    t = ContainerType.HC40;
                    break;
                case "20HC":
                    t = ContainerType.HC20;
                    break;
                default:
                    throw new Exception();
            }
            return t;
        };
        public AutomapperConfig()
        {
            CreateMap<DispatchRequest, PayLoad>();
            CreateMap<DTO.Container,Common.Container>()
                .ForMember(d => d.ContainerType, s => s.MapFrom(c => ConvertToEnumType(c.ContainerType)));
            CreateMap<DTO.Item, Common.Item>();
            CreateMap<DTO.DeliveryAddress, Common.DeliveryAddress>();


        }
    }
}
