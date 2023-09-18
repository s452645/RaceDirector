namespace backend.Services.Hardware.Comms
{
    public enum PicoWBoardMessageSender
    {
        BACKEND,
        BOARD
    }

    public class PicoWBoardMessage
    {
        public PicoWBoardMessageSender Sender { get; }
        public long? SentTimestamp { get; }
        public long? ReceivedTimestamp { get; }
        public PicoWBoardMessageContent Content { get; }

        public PicoWBoardMessage(PicoWBoardMessageSender sender, long? sentTimestamp, long? receivedTimestamp, PicoWBoardMessageContent content)
        {
            Sender = sender;
            SentTimestamp = sentTimestamp;
            ReceivedTimestamp = receivedTimestamp;
            Content = content;
        }
    }
}
