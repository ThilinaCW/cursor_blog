namespace BlogApi.Core.DTOs
{
    public class ApproveUserDto
    {
        public Guid UserId { get; set; }
        public Guid AdminId { get; set; }
    }

    public class RejectUserDto
    {
        public Guid UserId { get; set; }
        public Guid AdminId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class ApprovePostDto
    {
        public Guid PostId { get; set; }
        public Guid AdminId { get; set; }
    }

    public class RejectPostDto
    {
        public Guid PostId { get; set; }
        public Guid AdminId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class AdminDashboardDto
    {
        public int PendingUsers { get; set; }
        public int PendingPosts { get; set; }
        public int TotalUsers { get; set; }
        public int TotalPosts { get; set; }
    }
} 