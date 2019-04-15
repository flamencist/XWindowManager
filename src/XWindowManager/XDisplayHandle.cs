using System;
using System.Runtime.InteropServices;

namespace X11
{
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