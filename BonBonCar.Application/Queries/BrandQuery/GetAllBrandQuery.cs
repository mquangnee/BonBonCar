using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BonBonCar.Application.Queries.BrandQuery
{
    public class GetAllBrandQuery : IRequest<MethodResult<IList<Brand>>>
    {
    }

    public class GetAllBrandQueryHandler : IRequestHandler<GetAllBrandQuery, MethodResult<IList<Brand>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllBrandQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<IList<Brand>>> Handle(GetAllBrandQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<IList<Brand>>();
            var listBrands = await _unitOfWork.Brands.GetAllAsync();
            if (listBrands == null || !listBrands.Any())
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist));
                return methodResult;
            }
            var brandsActive = listBrands.Where(b => b.IsActive).ToList();
            methodResult.Result = brandsActive;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
