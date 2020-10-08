namespace AppLayer
{
    public class LogAppLayer /*: IAppLayer*/
    {
        public readonly IAppLayer Logged;

        private long _LogMsgId = 0;

        private string LogMsgPrefix => $"[Log:{_LogMsgId++}]";

        public LogAppLayer(IAppLayer logged)
        {
            Logged = logged;
        }
    }
}