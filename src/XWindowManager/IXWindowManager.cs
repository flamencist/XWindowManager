using System;
using System.Collections.Generic;

namespace X11
{
    public interface IXWindowManager : IDisposable
    {
        void Open(string displayName);
        void Close();
        bool TryGetXWindows(out List<XWindowInfo> windows);
    }
}