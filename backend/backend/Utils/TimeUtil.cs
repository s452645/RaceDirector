namespace backend.Utils
{
    public static class TimeUtil
    {
        public static long GetCurrentTimestamp()
        {
            return ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds();
        }
    }
}
