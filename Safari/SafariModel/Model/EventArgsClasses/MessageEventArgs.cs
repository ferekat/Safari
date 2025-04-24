using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafariModel.Model.EventArgsClasses
{
    public class MessageEventArgs : EventArgs
    {
        private string? message;
        private int x;
        private int y;
        public string? Message { get { return message; } private set { message = value; } }
        public int X { get { return x; } private set { x = value; } }
        public int Y { get { return y; } private set { y = value; } }
        public MessageEventArgs(string message, int x, int y)
        {
            Message = message;
            X = x;
            Y = y;
        }
    }
}
