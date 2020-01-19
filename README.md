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
                using (var wm = new XWindowManager())
                {
                    wm.Open(args.Length > 0 ? args[0] : null);

                    if (wm.TryGetXWindows(out var windows))
                    {
                        windows.ForEach(_=>Console.WriteLine($"{_.WmClass.InstanceName} {_.WmClass.ClassName}"));
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

### License

This software is distributed under the terms of the MIT License (MIT).

### Authors

Alexander Chermyanin / [LinkedIn](https://www.linkedin.com/in/alexander-chermyanin)



Contributions and bugs reports are welcome.
