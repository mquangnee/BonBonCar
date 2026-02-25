using BonBonCar.Application.Common;
using BonBonCar.Domain.Entities;
using BonBonCar.Domain.Models.QueryModels.Admin;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BonBonCar.Application.Queries.Admin
{
    public class GetUsersQuery : IRequest<MethodResult<ListUsersModel>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, MethodResult<ListUsersModel>>
    {
        private readonly UserManager<User> _userManager;

        public GetUsersQueryHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<MethodResult<ListUsersModel>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<ListUsersModel>();

            var page = request.Page <= 0 ? 1 : request.Page;
            var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 100);

            var query = _userManager.Users.AsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var items = new List<UserItemModel>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                var isLocked = u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.Now;
                items.Add(new UserItemModel
                {
                    UserId = u.Id,
                    Email = u.Email ?? string.Empty,
                    FullName = u.FullName,
                    IsLocked = isLocked,
                    IsVerified = u.IsVerified,
                    CreatedAt = u.CreatedAt,
                    Roles = roles.ToList()
                });
            }

            methodResult.Result = new ListUsersModel
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}

