using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.CmdModels.ManageVehicleCmdModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BonBonCar.Application.Commands.ManageVehicleCmdD
{
    public class CreateVehicleCmd : CreateVehicleCmdModel, IRequest<MethodResult<CreateVehicleCmdModel>>
    {
    }

    public class CreateVehicleCmdHandler : IRequestHandler<CreateVehicleCmd, MethodResult<CreateVehicleCmdModel>>
    {   
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateVehicleCmdHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MethodResult<CreateVehicleCmdModel>> Handle(CreateVehicleCmd request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<CreateVehicleCmdModel>();
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
