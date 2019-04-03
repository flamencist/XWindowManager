using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace wmctrl
{
    public class XWindowManager : IXWindowManager
    {
        private SafeHandle _display;

        public static IXWindowManager Instance { get; } = new XWindowManager();

        public void Open(string displayName)
        {
            _display = new XDisplayHandle(Native.XOpenDisplay(displayName), true);
            if (_display.IsInvalid)
            {
                throw new XWindowException($"Could not open display: {displayName}");
            }
        }

        public void Close()
        {
            if (!_display.IsInvalid)
            {
                _display.Dispose();
            }
        }

        public bool TryGetXWindows(out List<XWindowInfo> windows)
        {
            ThrowIfNotOpened();
            return TryGetXWindows(_display, out windows);
        }

        private void ThrowIfNotOpened()
        {
            if (_display.IsInvalid)
            {
                throw new XWindowException(
                    $"Display is not defined. Before call Open method before invoke the method.");
            }
        }

        private static bool TryGetXWindows(SafeHandle display, out List<XWindowInfo> windows)
        {
            windows = new List<XWindowInfo>();

            using (var clientList = GetClientList(display, out var clientListSize))
            {
                if (clientList.IsInvalid)
                {
                    return false;
                }

                for (var i = 0; i < (int) clientListSize / IntPtr.Size; i++)
                {
                    var xWindowClass = GetXWindowClass(display, Marshal.ReadIntPtr(clientList.DangerousGetHandle(), i * IntPtr.Size));
                    var classes = xWindowClass
                        .Split('\0')
                        .Where(_ => !string.IsNullOrWhiteSpace(_))
                        .ToArray();
                    windows.Add(new XWindowInfo {WmClass = classes});
                }
            }

            return true;
        }

        public void Dispose()
        {
            Close();
        }

        private static string GetXWindowClass(SafeHandle display, IntPtr win)
        {
            using (var wmClass = GetProperty(display, win, Native.XAtom.XA_STRING, "WM_CLASS", out var size))
            {
                return wmClass.IsInvalid ? string.Empty:Marshal.PtrToStringUTF8(wmClass.DangerousGetHandle(), (int) size / 2);
            }
        }

        private static SafeHandle GetClientList(SafeHandle display, out ulong size)
        {
            SafeHandle clientList;

            if ((clientList = GetProperty(display, Native.XDefaultRootWindow(display),
                    Native.XAtom.XA_WINDOW, "_NET_CLIENT_LIST", out size)).IsInvalid)
            {
                if ((clientList = GetProperty(display, Native.XDefaultRootWindow(display),
                        Native.XAtom.XA_CARDINAL, "_WIN_CLIENT_LIST", out size)).IsInvalid)
                {
                    return new XPropertyHandle(IntPtr.Zero, false);
                }
            }

            return clientList;
        }


        private static SafeHandle GetProperty(SafeHandle display, IntPtr win,
            Native.XAtom xaPropType, string propName, out ulong size)
        {
            size = 0;

            var xaPropName = Native.XInternAtom(display, propName, false);

            if (Native.XGetWindowProperty(display, win, xaPropName, 0,
                    4096 / 4, false, (ulong) xaPropType, out var actualTypeReturn, out var actualFormatReturn,
                    out var nItemsReturn, out var bytesAfterReturn, out var propReturn) != 0)
            {
                return new XPropertyHandle(IntPtr.Zero, false);
            }

            if (actualTypeReturn != (ulong) xaPropType)
            {
                return new XPropertyHandle(IntPtr.Zero, false);
            }

            size = (ulong) actualFormatReturn / (32 / sizeof(long)) * nItemsReturn;
            return new XPropertyHandle(propReturn, false);
        }
    }

    internal class XPropertyHandle : SafeHandle
    {
        private readonly IntPtr _handle;
        public XPropertyHandle(IntPtr invalidHandleValue, bool ownsHandle) : base(invalidHandleValue, ownsHandle)
        {
            _handle = invalidHandleValue;
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                return Native.XFree(this) == 0;
            }

            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReleaseHandle();
            }
            base.Dispose(disposing);
        }
        
        public override bool IsInvalid => _handle == IntPtr.Zero;
    }
    
    internal class XDisplayHandle : SafeHandle
    {
        private readonly IntPtr _handle;
        public XDisplayHandle(IntPtr invalidHandleValue, bool ownsHandle) : base(invalidHandleValue, ownsHandle)
        {
            _handle = invalidHandleValue;
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReleaseHandle();
            }
            base.Dispose(disposing);
        }

        protected override bool ReleaseHandle()
        {
            try
            {
                Native.XCloseDisplay(this);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public override bool IsInvalid => _handle == IntPtr.Zero;
    }
}