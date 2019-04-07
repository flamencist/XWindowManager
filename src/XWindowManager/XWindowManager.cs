using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace X11
{
    public class XWindowManager : IXWindowManager
    {
        private SafeHandle _display;

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

        public void Dispose()
        {
            Close();
        }

        private void ThrowIfNotOpened()
        {
            if (_display.IsInvalid)
            {
                throw new XWindowException(
                    "Display is not defined. Before call Open method before invoke the method.");
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

                for (var i = 0; i < (int) clientListSize; i++)
                {
                    var win = Marshal.ReadIntPtr(clientList.DangerousGetHandle(), i * IntPtr.Size);
                    var xWindowClass = GetXWindowClass(display, win);
                    var classes = ParseXWindowClass(xWindowClass);
                    var windowTitle = GetWindowTitle(display,win);
                    var pid = GetPid(display, win);
                    var clientMachine = GetClientMachine(display, win);
                    windows.Add(new XWindowInfo
                    {
                        Id = win,
                        WmClass = new WmClass{InstanceName = classes[0], ClassName = classes[1]},
                        WmName = windowTitle,
                        WmPid = pid,
                        WmClientMachine = clientMachine
                    });
                }
            }

            return true;
        }

        private static string[] ParseXWindowClass(string xWindowClass)
        {
            return xWindowClass
                .Split('\0')
                .Where(_ => !string.IsNullOrWhiteSpace(_))
                .ToArray();
        }

        private static string GetXWindowClass(SafeHandle display, IntPtr win)
        {
            using (var wmClass = GetProperty(display, win, Native.XAtom.XA_STRING, "WM_CLASS", out var size))
            {
                return wmClass.IsInvalid ? string.Empty:GetString(wmClass, size);
            }
        }

        private static string GetClientMachine(SafeHandle display, IntPtr win)
        {
            using (var rawClientMachine = GetProperty(display, win, Native.XAtom.XA_STRING, "WM_CLIENT_MACHINE", out var size))
            {
                return GetString(rawClientMachine, size);
            }
        }

        private static string GetString(SafeHandle handle, ulong size)
        {
            return handle.IsInvalid?null:Marshal.PtrToStringAnsi(handle.DangerousGetHandle(), (int) size );
        }

        private static string GetWindowTitle (SafeHandle display, IntPtr win) {
            string netWmName;
            using (var rawNetWmName = GetProperty(display, win, Native.XInternAtom(display, "UTF8_STRING", false), "_NET_WM_NAME", out var size))
            {
                netWmName = GetString(rawNetWmName, size);
            }

            string wmName;
            using (var rawWmName = GetProperty(display, win, Native.XAtom.XA_STRING, "WM_NAME", out var sizeWmName))
            {
                wmName = GetString(rawWmName, sizeWmName);
            }

            return netWmName ?? wmName;
        }

        private static ulong GetPid(SafeHandle display, IntPtr win)
        {
            using (var rawPid = GetProperty(display, win, Native.XAtom.XA_CARDINAL, "_NET_WM_PID", out _))
            {
                return Marshal.PtrToStructure<ulong>(rawPid.DangerousGetHandle());
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
            Native.XAtom xaPropType, string propName, out ulong size) =>
            GetProperty(display, win, (ulong)xaPropType, propName, out size);

        private static SafeHandle GetProperty(SafeHandle display, IntPtr win, ulong xaPropType, string propName, out ulong size)
        {
            size = 0;

            var xaPropName = Native.XInternAtom(display, propName, false);

            if (Native.XGetWindowProperty(display, win, xaPropName, 0,
                    4096 / 4, false,  xaPropType, out var actualTypeReturn, out var actualFormatReturn,
                    out var nItemsReturn, out var bytesAfterReturn, out var propReturn) != 0)
            {
                return new XPropertyHandle(IntPtr.Zero, false);
            }

            if (actualTypeReturn != xaPropType)
            {
                return new XPropertyHandle(IntPtr.Zero, false);
            }

//            size = (ulong) actualFormatReturn / (32 / sizeof(long)) * nItemsReturn;
            size = nItemsReturn;
            return new XPropertyHandle(propReturn, false);
        }
    }
}
