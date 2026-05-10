namespace CalenFlowApp.Models.AI
{
    public class ChatRequestDto
    {
        public List<MessageDto> Context { get; set; }
        public string LatestMessage { get; set; }
    }
}
