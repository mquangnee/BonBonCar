using BonBonCar.Application.Common;
using BonBonCar.Domain.IRepository;
using BonBonCar.Domain.Models.QueryModels.Payments;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Queries.PaymentQuery
{
    public class GetPaymentStatusQuery : IRequest<MethodResult<PaymentStatus>>
    {
        public string TxnRef { get; set; } = default!;
        public GetPaymentStatusQuery() { }
        public GetPaymentStatusQuery(string txnRef) => TxnRef = txnRef;
    }

    public class GetPaymentStatusQueryHandler : IRequestHandler<GetPaymentStatusQuery, MethodResult<PaymentStatus>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPaymentStatusQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<PaymentStatus>> Handle(GetPaymentStatusQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<PaymentStatus>();

            var p = await _unitOfWork.Payments.QueryableAsync().AsNoTracking().FirstOrDefaultAsync(x => x.TxnRef == request.TxnRef, cancellationToken);
            if (p == null) return null;
            methodResult.Result = new PaymentStatus
            {
                Status = p.Status,
                ProviderTransactionNo = p.ProviderTransactionNo,
                ProviderResponseCode = p.ProviderResponseCode,
                PaidAt = p.PaidAt,
                ExpiresAt = p.ExpiresAt
            };
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }

}
