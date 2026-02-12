using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Enums.ErrorCodes;
using BonBonCar.Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BonBonCar.Application.Queries.ModelQuery
{
    public class GetModelsByBrandIdQuery : IRequest<MethodResult<IList<Model>>>
    {
        public Guid BrandId { get; set; }
    }

    public class GetModelsByBrandIdQueryHandler : IRequestHandler<GetModelsByBrandIdQuery, MethodResult<IList<Model>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetModelsByBrandIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<IList<Model>>> Handle(GetModelsByBrandIdQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<IList<Model>>();
            var listModels = await _unitOfWork.Models.GetByBrandIdAsync(request.BrandId);
            if (listModels == null)
            {
                methodResult.AddErrorBadRequest(nameof(EnumSystemErrorCode.DataNotExist), nameof(request.BrandId));
                return methodResult;
            }
            var modelsActive = listModels.Where(m => m.IsActive == true).ToList();
            methodResult.Result = modelsActive;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
