﻿using System;
using X11;

namespace wmctrl
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var wm = new XWindowManager())
                {
                    wm.Open(args.Length > 0 ? args[0] : null);

                    if (wm.TryGetXWindows(out var windows))
                    {
                        Console.WriteLine($"Windows count {windows.Count}\n");
                        windows.ForEach(_ => Console.WriteLine($"0x{_.Id.ToString("x8")}" +
                                                               $" PID:{_.WmPid}" +
                                                               $" Machine:{_.WmClientMachine}" +
                                                               $" {_.WmName}" +
                                                               $" {_.WmClass.InstanceName} {_.WmClass.ClassName}" +
                                                               $" X:{_.Geometry.X} Y:{_.Geometry.Y}"));
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