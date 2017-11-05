namespace Task4
{
    public interface ILogger
    {
        void Log(string key, params object[] args);
    }
}