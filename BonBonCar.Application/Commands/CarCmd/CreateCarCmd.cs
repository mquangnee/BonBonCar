using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.CmdModels.CarCmdModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BonBonCar.Application.Commands.CarCmd
{
    public class CreateCarCmd : CreateCarCmdModel, IRequest<MethodResult<CreateCarCmdModel>>
    {
    }

    public class CreateVehicleCmdHandler : IRequestHandler<CreateCarCmd, MethodResult<CreateCarCmdModel>>
    {   
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateVehicleCmdHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MethodResult<CreateCarCmdModel>> Handle(CreateCarCmd request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<CreateCarCmdModel>();
        
            //var vehicle = await _unitOfWork.Vehicles.GetByLicensePlate(request.LicensePlate ?? string.Empty);
            //if (!Guid.TryParse(_httpContextAccessor?.HttpContext?.User.FindFirstValue("sub"), out var userId))
            //{
            //    methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.Unauthorized));
            //    return methodResult;
            //}
            //if (vehicle != null
            //{
            //    methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataAlreadyExist), nameof(request.LicensePlate));
            //    return methodResult;
            //}
            //var vehicleModel = new Vehicle
            //{
            //    Brand = request.Brand,
            //    OwnerId = userId,
            //    Model = request.Model,
            //    LicensePlate = request.LicensePlate,
            //    PricePerDay = request.PricePerDay
            //};
            //await _unitOfWork.Vehicles.AddAsync(vehicleModel);
            //_unitOfWork.SaveChanges();
            // map
            ///methodResult.Result = 
            return methodResult;
        }
    }
}
