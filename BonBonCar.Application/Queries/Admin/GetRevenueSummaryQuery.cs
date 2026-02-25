using BonBonCar.Application.Common;
using BonBonCar.Domain.Enums.Payment;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.QueryModels.Admin;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Queries.Admin
{
    public class GetRevenueSummaryQuery : IRequest<MethodResult<RevenueSummaryModel>>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetRevenueSummaryQueryHandler : IRequestHandler<GetRevenueSummaryQuery, MethodResult<RevenueSummaryModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRevenueSummaryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<RevenueSummaryModel>> Handle(GetRevenueSummaryQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<RevenueSummaryModel>();

            var from = request.FromDate;
            var to = request.ToDate;

            var query = _unitOfWork.Payments.QueryableAsync().AsNoTracking().Where(p => p.Status == EnumPaymentStatus.Paid);

            if (from.HasValue)
            {
                query = query.Where(p => p.PaidAt >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(p => p.PaidAt <= to.Value);
            }

            var grouped = await query
                .GroupBy(p => p.Purpose)
                .Select(g => new
                {
                    Purpose = g.Key,
                    Amount = g.Sum(x => x.Amount)
                })
                .ToListAsync(cancellationToken);

            decimal totalDeposit = grouped
                .Where(x => x.Purpose == EnumPaymentPurpose.Deposit)
                .Sum(x => x.Amount);

            decimal totalRentalFee = grouped
                .Where(x => x.Purpose == EnumPaymentPurpose.RentalFee)
                .Sum(x => x.Amount);

            var byPurpose = grouped
                .Select(x => new RevenueSummaryItem
                {
                    Purpose = x.Purpose,
                    Amount = x.Amount
                })
                .ToList();

            methodResult.Result = new RevenueSummaryModel
            {
                FromDate = from,
                ToDate = to,
                TotalDeposit = totalDeposit,
                TotalRentalFee = totalRentalFee,
                ByPurpose = byPurpose
            };
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}

