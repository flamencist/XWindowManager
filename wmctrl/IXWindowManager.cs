using System;
using System.Collections.Generic;

namespace wmctrl
{
    public interface IXWindowManager : IDisposable
    {
        void Open(string displayName);
        void Close();
        bool TryGetXWindows(out List<XWindowInfo> windows);
    }
}