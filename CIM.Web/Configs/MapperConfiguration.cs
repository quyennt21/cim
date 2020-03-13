using AutoMapper;
using CIM.Model.Models;
using CIM.Web.Models;

namespace CIM.Web.Configs
{
    public class MapperConfiguration
    {
        // config mapper model and view model
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ApplicationUser, UserViewModel>();
                cfg.CreateMap<ApplicationRole, RoleViewModel>();
                cfg.CreateMap<Location, LocationViewModel>();
                cfg.CreateMap<Area, AreaViewModel>();
                cfg.CreateMap<AssetTypeAttribute, AssetTypeAttributeViewModel>();
                cfg.CreateMap<AssetAttributeValue, AssetAttributeValueViewModel>();
                cfg.CreateMap<AssetType, AssetTypeViewModel>();
                cfg.CreateMap<Asset, AssetViewModel>();
                cfg.CreateMap<Campus, CampusViewModel>();
                cfg.CreateMap<AssetLog, AssetLogViewModel>();
                cfg.CreateMap<MaintenanceDiary, MaintenanceDiaryViewModel>();
                cfg.CreateMap<PIC, PICViewModel>();
                cfg.CreateMap<Rate, RateViewModel>();
                cfg.CreateMap<Report, ReportViewModel>();
                cfg.CreateMap<Warranty, WarrantyViewModel>();
            });
        }
    }
}