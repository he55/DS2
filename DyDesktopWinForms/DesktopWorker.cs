using System;

namespace DyDesktopWinForms
{
    public static class DesktopWorker
    {
        public static IntPtr GetWorkerWindowHandle()
        {



            // Fetch the Progman window
            IntPtr progman = PInvoke.FindWindow("Progman", null);

            IntPtr result = IntPtr.Zero;

            // Send 0x052C to Progman. This message directs Progman to spawn a 
            // WorkerW behind the desktop icons. If it is already there, nothing 
            // happens.
            PInvoke.SendMessageTimeout(progman,
                                   0x052C,
                                   IntPtr.Zero,
                                   IntPtr.Zero,
                                   0,
                                   1000,
                                   out result);


            //PrintVisibleWindowHandles(2);
            // The output will look something like this
            // .....
            // 0x00010190 "" WorkerW
            //   ...
            //   0x000100EE "" SHELLDLL_DefView
            //     0x000100F0 "FolderView" SysListView32
            // 0x00100B8A "" WorkerW                                   <--- This is the WorkerW instance we are after!
            // 0x000100EC "Program Manager" Progman

            IntPtr workerw = IntPtr.Zero;

            // We enumerate all Windows, until we find one, that has the SHELLDLL_DefView 
            // as a child. 
            // If we found that window, we take its next sibling and assign it to workerw.
            PInvoke.EnumWindows((tophandle, topparamhandle) =>
            {
                IntPtr p = PInvoke.FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            null);

                if (p != IntPtr.Zero)
                {
                    // Gets the WorkerW Window after the current one.
                    workerw = PInvoke.FindWindowEx(IntPtr.Zero,
                                               tophandle,
                                               "WorkerW",
                                               null);
                    return false;
                }

                return true;
            }, IntPtr.Zero);
            return workerw;

        }
    }
}
