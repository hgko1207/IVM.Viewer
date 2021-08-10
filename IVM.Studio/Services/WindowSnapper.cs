using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Shapes;

namespace IVM.Studio.Services
{
    public class WindowSnapper
    {
        static class WinHelper
        {
            [DllImport("user32.dll")]
            public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

            [DllImport("user32.dll")]
            public static extern IntPtr GetParent(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

            [DllImport("user32.dll")]
            public static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

            [DllImport("user32.dll")]
            public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll")]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

            [DllImport("User32.dll")]
            public static extern Int64 SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            [DllImport("user32.dll")]
            public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

            [DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            public static int GWL_STYLE = -16;

            public static int WS_VISIBLE = 0x10000000;
            public static int WS_CHILD = 0x40000000; // child window
            public static int WS_BORDER = 0x00800000; // window with border
            public static int WS_DLGFRAME = 0x00400000; // window with double border but no title
            public static int WS_CAPTION = WS_BORDER | WS_DLGFRAME; // window with a title bar

            public static uint WM_PAINT = 0x000F;
            public static uint WM_CLOSE = 0x0010;

            public static short SWP_NOMOVE = 0X2;
            public static short SWP_NOSIZE = 1;
            public static short SWP_NOZORDER = 0X4;
            public static int SWP_SHOWWINDOW = 0x0040;

            public static int SW_HIDE = 0;
            public static int SW_SHOWNORMAL = 1;
            public static int SW_SHOWNOACTIVATE = 4;
            public static int SW_SHOW = 5;
        }

        IntPtr ownerHandle = IntPtr.Zero;
        IntPtr childHandle = IntPtr.Zero;
        String childExec;
        String childTitle;
        Window ownerWindow;
        ContentControl placeHolder;

        public WindowSnapper(Window owner, ContentControl holder, String title, String exec)
        {
            ownerWindow = owner;
            placeHolder = holder;
            childExec = exec;
            childTitle = title;
            ownerHandle = new WindowInteropHelper(ownerWindow).Handle;
        }

        public void InvokeProcess()
        {
            //Task.Run(() =>
            //{
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = childExec;
                psi.Arguments = childTitle + " " + childTitle;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true; // muse be useshell-execute disable
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = false;

                Process.Start(psi);
            //});
        }

        public void KillProcess()
        {
            WinHelper.SendMessage(childHandle, WinHelper.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        public void Attach()
        {
            // find child handle
            childHandle = GetWindowHandle(childTitle);
            if (childHandle != IntPtr.Zero)
            {
                // attach
                WinHelper.SetWindowLong(childHandle, WinHelper.GWL_STYLE,
                    WinHelper.GetWindowLong(childHandle, WinHelper.GWL_STYLE) | WinHelper.WS_CHILD);
                WinHelper.SetWindowLong(childHandle, WinHelper.GWL_STYLE, WinHelper.WS_VISIBLE);
                WinHelper.SetParent(childHandle, ownerHandle);

                // adjustment window position.
                ArrangeWindows();
            }
        }

        public void Hide()
        {
            WinHelper.ShowWindow(childHandle, WinHelper.SW_HIDE);
        }

        public void Show()
        {
            WinHelper.ShowWindow(childHandle, WinHelper.SW_SHOW);
        }

        private void ArrangeWindows()
        {
            // Moves the otherWindow on top of childPlaceHolder
            Point topLeft = placeHolder.TransformToAncestor(ownerWindow).Transform(new Point(0, 0));
            Point bottomRight = placeHolder.TransformToAncestor(ownerWindow).Transform(new Point(placeHolder.ActualWidth, placeHolder.ActualHeight));
            
            WinHelper.MoveWindow(childHandle, (int)topLeft.X, (int)topLeft.Y, (int)bottomRight.X - (int)topLeft.X, (int)bottomRight.Y - (int)topLeft.Y, true);

            //WinHelper.ShowWindow(childHandle, WinHelper.SW_SHOWNORMAL);
            //WinHelper.SetForegroundWindow(childHandle);
        }

        private IntPtr GetWindowHandle(string windowTitle)
        {
            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.MainWindowTitle.Contains(windowTitle))
                {
                    return pList.MainWindowHandle;
                }
            }

            return IntPtr.Zero;
        }
    }

}

