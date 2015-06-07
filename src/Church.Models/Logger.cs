namespace Church.Models
{
    using System;
    using System.Collections.Generic;

    public enum LogType { Exception = 0, Warning, Audit, Debug };

    public class LoggerEventArgs : EventArgs
    {
        public string Type { get; internal set; }
        public string Description { get; internal set; }
        public string IPAddress { get; internal set; }
        public string Username { get; internal set; }
    }

    public class Logger : ILogger, IDisposable
    {
        protected event EventHandler<LoggerEventArgs> _writers;
        protected List<IDisposable> _disposableWriters;

        public Logger()
        {
            this._disposableWriters = new List<IDisposable>();
        }

        public Logger(params ILoggerWriter[] writers)
            : this()
        {
            this.Add(writers);
        }

        public void Add(params ILoggerWriter[] writers)
        {
            if (null != writers)
            {
                foreach (var w in writers)
                {
                    this._writers += w.Write;
                    if (w is IDisposable)
                    {
                        this._disposableWriters.Add(w as IDisposable);
                    }
                }
            }
        }

        public void LogEvent(string type, string description, string ipAddress, string username)
        {
            if (null != this._writers)
            {
                var evt = new LoggerEventArgs
                {
                    Type = type,
                    Description = description,
                    IPAddress = ipAddress,
                    Username = username,
                };
                this._writers(this, evt);
            }
        }

        public void LogWarnning(string message, string ipAddress, string username = null)
        {
            this.LogEvent(LogType.Warning.ToString(), message, ipAddress, username);
        }

        public void LogException(Exception exp, string ipAddress, string username = null)
        {
            if (null != exp.InnerException)
            {
                this.LogException(exp.InnerException, ipAddress, username);
            }

            this.LogEvent(LogType.Exception.ToString(), string.Format("Message: {1}{0}Stack trace: {2}",
                Environment.NewLine,
                exp.Message,
                exp.StackTrace),
                ipAddress,
                username);
        }

        public void Dispose()
        {
            this._disposableWriters.ForEach(w => w.Dispose());
        }
    }
}
