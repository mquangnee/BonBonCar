using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.Car;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Queries.CarQueries
{
    public class GetAvailableCarsQuery : IRequest<MethodResult<IList<Car>>>
    {
    }

    public class GetAvailableCarsQueryHandler : IRequestHandler<GetAvailableCarsQuery, MethodResult<IList<Car>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAvailableCarsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<IList<Car>>> Handle(GetAvailableCarsQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<IList<Car>>();
            var cars = await _unitOfWork.Vehicles.QueryableAsync().Where(c => c.Status == EnumCarStatus.Available).ToListAsync();
            if (cars == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist));
                return methodResult;
            }
            methodResult.Result = cars;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
