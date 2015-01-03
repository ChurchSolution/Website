namespace Church.Model
{
    using System;
    using System.Collections.Generic;

    public interface IWeeklyRecord
    {
        string Name { get; }

        string FileUrl { get; }

        IWeeklyRecord Add(IWeeklyRecord record);

        IWeeklyRecord DivideBy(int num);
    }

    public interface ILoggerWriter
    {
        void Write(object sender, LoggerEventArgs e);
    }

    public interface ILogger
    {
        void LogEvent(string type, string description, string ipAddress, string username = null);

        void LogException(Exception exp, string ipAddress, string username = null);

        void LogWarnning(string message, string ipAddress, string username = null);
    }
}
