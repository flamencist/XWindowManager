using System;

namespace X11
{
    public class XWindowInfo
    {
        public IntPtr Id { get; set; }
        public WmClass WmClass { get; set; }
        public string WmName { get; set; }
        public ulong WmPid { get; set; }
        public string WmClientMachine { get; set; }
    }
}