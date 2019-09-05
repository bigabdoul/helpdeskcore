
using AutoMapper;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Services.Imports;

namespace HelpDeskCore.ViewModels.Mappings
{
  public class ViewModelToEntityMappingProfile : Profile
  {
    public ViewModelToEntityMappingProfile()
    {
      CreateMap<RegistrationViewModel, AppUser>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));
      CreateMap<UserImported, AppUser>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));

      CreateMap<CategoryDetailViewModel, Category>()
        .ForMember(c => c.ForSpecificUsers, map => map.MapFrom(vm => vm.Mode == "SpecificUsers"))
        .ForMember(c => c.ForTechsOnly, map => map.MapFrom(vm => vm.Mode == "TechsOnly"))
        ;
    }
  }
}
