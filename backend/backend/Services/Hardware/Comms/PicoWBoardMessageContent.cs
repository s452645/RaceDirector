using System;
using System.Text;

namespace backend.Services.Hardware.Comms
{
    public class PicoWBoardMessageContent
    {
        public static readonly string SEPARATOR = "->";

        public string Param { get; }
        public string Message { get; }

        public static PicoWBoardMessageContent FromBytes(byte[] buffer, int bytesCount)
        {
            var received = Encoding.UTF8.GetString(buffer, 0, bytesCount);
            var splitted = received.Split(SEPARATOR);

            return new PicoWBoardMessageContent(splitted[0], splitted[1]);
        }

        public PicoWBoardMessageContent(string param, string message)
        {
            Param = param;
            Message = message;
        }

        public string AsString()
        {
            return $"{Param}{SEPARATOR}{Message}";
        }

        public byte[] AsBytes()
        {
            return Encoding.UTF8.GetBytes(AsString());
        }
    }
}
