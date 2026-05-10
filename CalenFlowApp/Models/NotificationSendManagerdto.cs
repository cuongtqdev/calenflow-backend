namespace CalenFlowApp.Models
{
    public class NotificationSendManagerdto
    {
        public int? Id { get; set; }
        public string? CandidateName { get; set; }

        public string? Title { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }

        public string? Type { get; set; }

        public bool? Status { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
