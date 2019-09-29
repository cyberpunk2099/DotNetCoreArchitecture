namespace DotNetCoreArchitecture.Model
{
    public class UserLogModel
    {
        public UserLogModel
        (
            long userId,
            LogType logType
        )
        {
            UserId = userId;
            LogType = logType;
        }

        public long UserId { get; }

        public LogType LogType { get; }
    }
}
