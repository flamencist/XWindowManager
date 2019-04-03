using System;

namespace wmctrl
{
    public class XWindowException : Exception
    {
        public XWindowException(string message) : base(message)
        {
        }
    }
}