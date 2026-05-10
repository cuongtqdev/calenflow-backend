namespace CalenFlowApp.Models
{
    public class RescheduleResponseDto
    {
        public int CandidateId { get; set; }
        public int HiringId { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime NewDate { get; set; }
    }
}
