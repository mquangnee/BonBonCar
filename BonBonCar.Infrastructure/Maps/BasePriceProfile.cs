using AutoMapper;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Models.EntityModels;

namespace BonBonCar.Infrastructure.Maps
{
    public class BasePriceProfile : Profile 
    {
        public BasePriceProfile()
        {
            CreateMap<BasePrices, BasePriceModel>();
        }
    }
}
