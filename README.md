# XWindowManager

[![Build Status](https://travis-ci.org/flamencist/XWindowManager.svg?branch=master)](https://travis-ci.org/flamencist/XWindowManager)
[![NuGet](https://img.shields.io/nuget/v/XWindowManager.svg)](https://www.nuget.org/packages/XWindowManager/)

X11 Window manager for linux on dotnet core.

Sample usage

```cs

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
				var display = args.Length > 0 ? args[0] : null;
                using (var wm = XWindowManager.Instance)
                {
                    wm.Open(dispaly);

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

```

```
chromium	Chromium
gnome-system-monitor	Gnome-system-monitor
```

Contributions and bugs reports are welcome.