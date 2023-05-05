using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Controls;


namespace Ultimate_GM_Screen.WindowHost
{
    /// <summary>
    /// Interaction logic for UC_WIndowHost.xaml
    /// </summary>
    public partial class UC_WIndowHost : UserControl
    {
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        private static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        [DllImport("user32")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int GWL_STYLE = -16;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_THICKFRAME = 0x00040000;

        private System.Windows.Forms.Panel _panel;
        private Process _process;

        public UC_WIndowHost()
        {
            InitializeComponent();

            _panel = new System.Windows.Forms.Panel();
            formHost.Child = _panel;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Load("notepad.exe");
        }

        private void UserControl_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            ResizeEmbeddedApp();
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_process != null)
            {
                _process.Refresh();
                _process.Close();
            }
        }

        public void Load(string filename)
        {            
            ProcessStartInfo psi = new ProcessStartInfo(filename);
            _process = Process.Start(psi);
            _process.WaitForInputIdle();
            SetParent(_process.MainWindowHandle, _panel.Handle);

            // remove control box
            int style = GetWindowLong(_process.MainWindowHandle, GWL_STYLE);
            style = style & ~WS_CAPTION & ~WS_THICKFRAME;
            SetWindowLong(_process.MainWindowHandle, GWL_STYLE, style);

            // resize embedded application & refresh
            ResizeEmbeddedApp();
        }

        private void ResizeEmbeddedApp()
        {
            if (_process == null)
                return;

            SetWindowPos(_process.MainWindowHandle, IntPtr.Zero, 0, 0, (int)_panel.ClientSize.Width, (int)_panel.ClientSize.Height, SWP_NOZORDER | SWP_NOACTIVATE);
        }
    }
}
