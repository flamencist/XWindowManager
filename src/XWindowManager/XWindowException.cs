using System;

namespace X11
{
    public class XWindowException : Exception
    {
        public XWindowException(string message) : base(message)
        {
        }
    }
}