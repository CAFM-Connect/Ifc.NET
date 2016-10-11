using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4.EventArgs
{
    public enum MessageLogType
    {
        Info,
        Debug,
        Valid,
        Error,
        Warning
    }

    public class MessageLoggedEventArgs : System.EventArgs
    {
        public MessageLoggedEventArgs()
            : this(new List<string> { }, MessageLogType.Info, false, false)
        {
        }

        public MessageLoggedEventArgs(Exception exception)
            : this((exception != null ? exception.Message : ""))
        {
            Exception = exception;
        }

        public MessageLoggedEventArgs(string message)
            : this(message, MessageLogType.Info, false, false)
        {
        }

        public MessageLoggedEventArgs(string message, MessageLogType type)
            : this(message, type, false, false)
        {
        }

        public MessageLoggedEventArgs(string message, MessageLogType type, bool doEvents, bool clear)
            : base()
        {
            Exception = null;
            Type = type;
            Messages = new List<string>();
            AddMessage(message);
            DoEvents = doEvents;
            Clear = clear;
        }

        public MessageLoggedEventArgs(List<string> messages, MessageLogType type, bool doEvents, bool clear)
            : base()
        {
            Exception = null;
            Type = type;
            Messages = new List<string>();
            if (messages != null)
                Messages.AddRange(messages);
            DoEvents = doEvents;
            Clear = clear;
        }

        public string Message
        {
            get { return String.Join("", Messages.ToArray()); }
        }
        public List<string> Messages { get; private set; }
        public Exception Exception { get; private set; }
        public string Hyperlink { get; private set; }
        public bool DoEvents { get; private set; }
        public bool Clear { get; private set; }
        public MessageLogType Type { get; set; }

        public void AddMessage(string message)
        {
            if (message == null)
                return;

            Messages.Add(FormatMessage(message));
        }

        public static string FormatMessage(string message)
        {
            if (message == null)
                return null;

            return String.Concat("\r\n", DateTime.Now.ToLongTimeString().PadRight(12), message);
        }
    }

}
