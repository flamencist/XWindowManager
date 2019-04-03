using System;
using X11;

namespace wmctrl
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var wm = XWindowManager.Instance)
                {
                    wm.Open(args.Length > 0 ? args[0] : null);

                    if (wm.TryGetXWindows(out var windows))
                    {
                        windows.ForEach(_ => Console.WriteLine(string.Join("\t", _.WmClass)));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(-1);
            }
        }
    }
}