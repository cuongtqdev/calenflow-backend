namespace CalenFlowApp.Models
{
    public class NotificationSenddto
    {
        public int? Id { get; set; }
        public string? HiringName { get; set; }

        public string? Title { get; set; }
        public string? CompanyName { get; set; }
        public string? Message { get; set; }

        public string? Type { get; set; }

        public bool? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
