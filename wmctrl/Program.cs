using System;
using System.Threading;

namespace wmctrl
{
    class Program
    {
        static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) => cancellationTokenSource.Cancel(false);
            try
            {
//                while (!cancellationTokenSource.IsCancellationRequested)
//                {
                    using (var wm = XWindowManager.Instance)
                    {
                        wm.Open(args.Length > 0 ? args[0] : null);

                        if (wm.TryGetXWindows(out var windows))
                        {
                            windows.ForEach(_ => Console.WriteLine(string.Join("\t", _.WmClass)));
                        }
                    }
//                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(-1);
            }
        }
    }
}