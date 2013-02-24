using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppConfig.Diagnostics
{
    public class ExceptionOccurredEventArgs : EventArgs
    {
        public ExceptionOccurredEventArgs(Exception Exception)
        {
            this.Exception = Exception;
            this.LogInfoMessage = "An unexpected exception has occurred and was logged for review.";
            this.UserInfoMessage = "An unexpected error has occurred and was logged for review.";
            this.LogException = true;
            this.NotifyUserExceptionWasLogged = true;
        }

        public Exception Exception { get; private set; }
        public string LogInfoMessage { get; set; }
        public string UserInfoMessage { get; set; }
        public bool LogException { get; set; }
        public bool NotifyUserExceptionWasLogged { get; set; }
    }

    public class ExceptionLoggedEventArgs : EventArgs
    {
        public ExceptionLoggedEventArgs(string UserInfoMessage)
        {
            this.UserInfoMessage = UserInfoMessage;
        }

        public string UserInfoMessage { get; private set; }
    }
}
