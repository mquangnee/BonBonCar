namespace BonBonCar.Domain.Models.QueryModels.Admin
{
    public class UserItemModel
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public bool IsLocked { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class ListUsersModel
    {
        public IList<UserItemModel> Items { get; set; } = new List<UserItemModel>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}

