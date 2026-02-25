using AutoMapper;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Models.EntityModels;

namespace BonBonCar.Infrastructure.Maps
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserModel>();
        }
    }
}
