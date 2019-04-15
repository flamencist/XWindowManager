using System;
using System.Runtime.InteropServices;

namespace X11
{
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
}