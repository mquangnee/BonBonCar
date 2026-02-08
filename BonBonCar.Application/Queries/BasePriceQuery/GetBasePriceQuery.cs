using AutoMapper;
using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.EntityModels;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BonBonCar.Application.Queries.BasePriceQuery
{
    public class GetBasePriceQuery : IRequest<MethodResult<IList<BasePriceModel>>>
    {
        public EnumCarType CarType;
    }

    public class GetBasePriceQueryHandler : IRequestHandler<GetBasePriceQuery, MethodResult<IList<BasePriceModel>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetBasePriceQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<IList<BasePriceModel>>> Handle(GetBasePriceQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<IList<BasePriceModel>>();
            var listBasePrice = await _unitOfWork.BasePrices.GetBasePriceByCarTypeAsync(request.CarType);
            if (listBasePrice == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(request.CarType));
                return methodResult;
            }
            var basePrices = listBasePrice.Where(p => p.IsActive == true).OrderBy(p => p.RentalDuration).ToList();
            methodResult.Result = _mapper.Map<IList<BasePriceModel>>(basePrices);
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
