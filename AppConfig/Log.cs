using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Appender;
using System.Windows.Forms;
using AppConfig.Diagnostics;

namespace AppConfig
{
    public static class Log
    {
        private static bool isConfigured = false;
        private static ILog iLog;

        public static void Configure()
        {
            //Need to configure the default appender if no appenders exist
        }

        public static void Configure(log4net.Appender.IAppender appender)
        {
            if (isConfigured)
                return;

            var loggerName = typeof(Log).FullName;

            var logger = (log4net.Repository.Hierarchy.Logger)log4net.LogManager.GetRepository().GetLogger(loggerName);
            var ilogger = log4net.LogManager.GetRepository().GetLogger(loggerName);

            log4net.Config.BasicConfigurator.Configure(appender);

            iLog = LogManager.GetLogger(loggerName);
            isConfigured = true;

            Info("Logging Configured at " + DateTime.Now.ToString("g"));
        }

        public static void Debug(object message) { Configure(); iLog.Debug(message); }
        public static void Debug(object message, Exception exception) { Configure(); RaiseExceptionOccurred(exception, message, LogLevel.Debug); }

        public static void Error(object message) { Configure(); iLog.Error(message); }
        public static void Error(object message, Exception exception) { Configure(); RaiseExceptionOccurred(exception, message, LogLevel.Error); }

        public static void Fatal(object message) { Configure(); iLog.Fatal(message); }
        public static void Fatal(object message, Exception exception) { Configure(); RaiseExceptionOccurred(exception, message, LogLevel.Fatal); }

        public static void Info(object message) { Configure(); iLog.Info(message); }
        public static void Info(object message, Exception exception) { Configure(); RaiseExceptionOccurred(exception, message, LogLevel.Info); }

        public static void Warn(object message) { Configure(); iLog.Warn(message); }
        public static void Warn(object message, Exception exception) { Configure(); RaiseExceptionOccurred(exception, message, LogLevel.Warn); }

        public static event EventHandler<ExceptionOccurredEventArgs> ExceptionOccurred;
        private static void RaiseExceptionOccurred(Exception ex, object message, LogLevel logLevel)
        {
            Configure();

            var e = new ExceptionOccurredEventArgs(ex);

            var strMessage = Convert.ToString(message);
            if (!string.IsNullOrEmpty(strMessage))
                e.UserInfoMessage = strMessage;
            //else if (ex is IUserFriendlyExceptionMessage)
            //    e.UserInfoMessage = ((IUserFriendlyExceptionMessage)ex).UserFriendlyMessage;

            if (ExceptionOccurred != null)
                ExceptionOccurred.Invoke(null, e);

            if (!e.LogException)
                return;

            switch (logLevel)
            {
                case LogLevel.Debug:
                    iLog.Debug(e.LogInfoMessage, e.Exception);
                    break;
                case LogLevel.Error:
                    iLog.Error(e.LogInfoMessage, e.Exception);
                    break;
                case LogLevel.Fatal:
                    iLog.Fatal(e.LogInfoMessage, e.Exception);
                    break;
                case LogLevel.Info:
                    iLog.Info(e.LogInfoMessage, e.Exception);
                    break;
                case LogLevel.Warn:
                    iLog.Warn(e.LogInfoMessage, e.Exception);
                    break;
            }

            //if (e.NotifyUserExceptionWasLogged && ExceptionLogged != null)
            //    //Run this code on the STA thread so updates to the UI are allowed.
            //    System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke((MethodInvoker)delegate
            //    {
            //        ExceptionLogged.Invoke(null, new ExceptionLoggedEventArgs(e.UserInfoMessage));
            //    });
        }


        private enum LogLevel
        {
            Debug,
            Error,
            Fatal,
            Info,
            Warn
        }

    }
}
