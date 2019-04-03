using System;
using System.Runtime.InteropServices;

namespace X11
{
    internal static class Native
    {
        [DllImport("X11")]
        internal static extern IntPtr XOpenDisplay(string displayName);

        [DllImport("X11")]
        internal static extern void XCloseDisplay(SafeHandle display);

        [DllImport("X11")]
        internal static extern ulong XInternAtom(SafeHandle display, string atomName, bool onlyIfExists);


        /**
         * int XGetWindowProperty(display, w, property, long_offset, long_length, delete, req_type, 
                        actual_type_return, actual_format_return, nitems_return, bytes_after_return, 
                        prop_return)
          Display *display;
          Window w;
          Atom property;
          long long_offset, long_length;
          Bool delete;
          Atom req_type; 
          Atom *actual_type_return;
          int *actual_format_return;
          unsigned long *nitems_return;
          unsigned long *bytes_after_return;
          unsigned char **prop_return;
         */
        [DllImport("X11")]
        internal static extern int XGetWindowProperty(SafeHandle display, IntPtr window, ulong atom, long offset,
            long length,
            bool delete, ulong reqType, out ulong actualTypeReturn, out int actualFormatReturn, out ulong nItemsReturn,
            out ulong bytesAfterReturn, out IntPtr propReturn);

        [DllImport("X11")]
        internal static extern int XFree(SafeHandle data);

        [DllImport("X11")]
        internal static extern IntPtr XDefaultRootWindow(SafeHandle display);

        internal enum XAtom : ulong
        {
            XA_WINDOW = 33,
            XA_CARDINAL = 6,
            XA_STRING = 31
        }
    }
}