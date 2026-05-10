namespace CalenFlowApp.Models
{
    public class SendRequestMeetingDto
    {
        public int CompanyId { get; set; }
        public int HiringAvailableId { get; set; }
        public string MeetingType { get; set; }
        public string PositionRole { get; set; }
        public string AdditionalMessage { get; set; }
    }
}
