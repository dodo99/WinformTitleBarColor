using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformTitleBarColor
{
    public partial class Form1 : Form
    {
        bool useWindowStyle = false;
        bool useDarkMode = false;
        int ncrp;

        public Form1()
        {
            InitializeComponent();
        }

        private void checkBoxDarkmode_Click(object sender, EventArgs e)
        {
            useDarkMode = checkBox1.Checked;

            if (!useDarkMode)
            {
                radioButton1.Checked = true;
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;

                WIN32Interop.UseDarkMode(this.Handle, false);
                ncrp = (int)WIN32Interop.DWMNCRENDERINGPOLICY.DWMNCRP_DISABLED;

                WIN32Interop.SetTitleBkColor(this.Handle, ncrp);
            }
            else
            {
                if (useWindowStyle)
                {
                    useWindowStyle = checkBox2.Checked = false;

                    ncrp = (int)WIN32Interop.DWMNCRENDERINGPOLICY.DWMNCRP_DISABLED;
                    WIN32Interop.SetTitleBkColor(this.Handle, ncrp);
                }
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
            }

        }

        private void DisableDarkmode_Click(object sender, EventArgs e)
        {
            WIN32Interop.UseDarkMode(this.Handle, false);
            ncrp = (int)WIN32Interop.DWMNCRENDERINGPOLICY.DWMNCRP_DISABLED;

            WIN32Interop.SetTitleBkColor(this.Handle, ncrp);
        }

        private void EnableDarkmode_Click(object sender, EventArgs e)
        {
            WIN32Interop.UseDarkMode(this.Handle, true);
            ncrp = (int)WIN32Interop.DWMNCRENDERINGPOLICY.DWMNCRP_ENABLED;

            WIN32Interop.SetTitleBkColor(this.Handle, ncrp);
        }

        private void checkBoxWinStyle_Click(object sender, EventArgs e)
        {
            useWindowStyle = checkBox2.Checked;

            if (useWindowStyle)
            {
                if (useDarkMode)
                {
                    useDarkMode = checkBox1.Checked = false;
                    radioButton1.Checked = true;
                    radioButton1.Enabled = false;
                    radioButton2.Enabled = false;
                }

                ncrp = (int)WIN32Interop.DWMNCRENDERINGPOLICY.DWMNCRP_USEWINDOWSTYLE;
            }
            else
            {
                ncrp = (int)WIN32Interop.DWMNCRENDERINGPOLICY.DWMNCRP_DISABLED;
            }

            WIN32Interop.SetTitleBkColor(this.Handle, ncrp);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ncrp = (int)WIN32Interop.DWMNCRENDERINGPOLICY.DWMNCRP_DISABLED;

            WIN32Interop.SetTitleBkColor(this.Handle, ncrp);
        }
    }

    public abstract class WIN32Interop
    {
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;


        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("User32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private static bool IsWindows10OrGreater(int build = -1)
        {
            //this only for .NET 5+
            //return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
            return true;
        }


        public static bool UseDarkMode(IntPtr handle, bool enabled = true)
        {
            var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
            int useImmersiveDarkMode = 0;

            if (IsWindows10OrGreater(17763))
            {
                attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                }

                useImmersiveDarkMode = enabled ? 1 : 0;
                return DwmSetWindowAttribute(handle, (int)attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
            }

            return DwmSetWindowAttribute(handle, (int)attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
        }
        public enum DWMNCRENDERINGPOLICY
        {
            DWMNCRP_USEWINDOWSTYLE, // Enable/disable non-client rendering based on window style
            DWMNCRP_DISABLED,       // Disabled non-client rendering; window style is ignored
            DWMNCRP_ENABLED,        // Enabled non-client rendering; window style is ignored
            DWMNCRP_LAST
        };

        [DllImport("User32")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);


        public static void SetTitleBkColor(IntPtr hWnd, int ncrp)
        {
            //int ncrp = (int)DWMNCRENDERINGPOLICY.DWMNCRP_DISABLED;
            //int ncrp = (int)DWMNCRENDERINGPOLICY.DWMNCRP_ENABLED;
            //int ncrp = (int)DWMNCRENDERINGPOLICY.DWMNCRP_USEWINDOWSTYLE;

            //HRESULT hr = DwmSetWindowAttribute(m_hWnd, DWMWA_NCRENDERING_POLICY, &ncrp, sizeof(ncrp));
            //SetWindowPos(0, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOSIZE | SWP_NOMOVE);

            DwmSetWindowAttribute(hWnd, 2, ref ncrp, sizeof(DWMNCRENDERINGPOLICY));
            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, 0x0020 | 0x0001 | 0x0002);

        }
            }
}
