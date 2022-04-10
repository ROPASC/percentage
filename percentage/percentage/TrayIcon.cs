using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace percentage
{
    class TrayIcon
    {
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        static extern bool DestroyIcon(IntPtr handle);

        private Color fontColor = Color.Black;

        private NotifyIcon notifyIcon;

        public TrayIcon()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItemExit = new MenuItem();
            MenuItem menuItemFontSet = new MenuItem();

            notifyIcon = new NotifyIcon();

            contextMenu.MenuItems.AddRange(new MenuItem[] { menuItemExit, menuItemFontSet });

            menuItemFontSet.Click += new System.EventHandler(MenuItemFontSetClick);
            menuItemFontSet.Index = 0;
            menuItemFontSet.Text = "F&ontSet";

            menuItemExit.Click += new System.EventHandler(MenuItemExitClick);
            menuItemExit.Index = 1;
            menuItemExit.Text = "E&xit";

            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Visible = true;

            Timer timer = new Timer
            {
                Interval = 1000
            };
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();
        }

        private Bitmap GetTextBitmap(String text, Font font, Color fontColor)
        {
            SizeF imageSize = GetStringImageSize(text, font);
            Bitmap bitmap = new Bitmap((int)imageSize.Width, (int)imageSize.Height);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.FromArgb(0, 0, 0, 0));
                using (Brush brush = new SolidBrush(fontColor))
                {
                    graphics.DrawString(text, font, brush, 0, 0);
                    graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    graphics.Save();
                }
            }
            return bitmap;
        }

        // dark is true, light is false
        private static bool GetWindowsTheme()
        {
            const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            //const string RegistryValueName = "AppsUseLightTheme";
            const string RegistryValueName = "SystemUsesLightTheme";
            // Maybe LocalMachine(HKEY_LOCAL_MACHINE)
            // see "https://www.addictivetips.com/windows-tips/how-to-enable-the-dark-theme-in-windows-10/"
            object registryValueObject = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistryKeyPath)?.GetValue(RegistryValueName);
            if (registryValueObject is null)
                return false;

            return (int)registryValueObject <= 0;
        }

        private static SizeF GetStringImageSize(string text, Font font)
        {
            using (Image image = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(image))
                return graphics.MeasureString(text, font);
        }

        private void MenuItemExitClick(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            Application.Exit();
        }

        private void MenuItemFontSetClick(object sender, EventArgs e)
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                fontDialog.Font = Properties.Settings.Default.font;
                fontDialog.ShowDialog();
                Properties.Settings.Default.font = fontDialog.Font;
            }
            Properties.Settings.Default.Save();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            PowerStatus powerStatus = SystemInformation.PowerStatus;
            string percentage = (powerStatus.BatteryLifePercent * 100).ToString();
            bool isCharging = powerStatus.PowerLineStatus == PowerLineStatus.Online;
            string bitmapText = powerStatus.BatteryLifePercent < 1 ? ((int)(powerStatus.BatteryLifePercent * 100)).ToString("D2") : "00";

            if (isCharging)
            {
                if (fontColor != Color.Green)
                {
                    fontColor = Color.Green;
                }
            }
            else if (powerStatus.BatteryLifePercent <= 0.2)
            {
                if (fontColor != Color.Red)
                {
                    fontColor = Color.Red;
                }
            }
            else if (GetWindowsTheme())
            {
                if (fontColor != Color.White)
                {
                    fontColor = Color.White;
                }
            }
            else
            {
                if (fontColor != Color.Black)
                {
                    fontColor = Color.Black;
                }
            }

            using (Bitmap bitmap = new Bitmap(GetTextBitmap(bitmapText, Properties.Settings.Default.font, fontColor)))
            {
                System.IntPtr intPtr = bitmap.GetHicon();
                try
                {
                    using (Icon icon = Icon.FromHandle(intPtr))
                    {
                        notifyIcon.Icon = icon;
                        //string toolTipText = percentage + "%" + (isCharging ? " Charging" : "");
                        notifyIcon.Text = percentage + "%" + (isCharging ? " Charging" : "");
                    }
                }
                finally
                {
                    DestroyIcon(intPtr);
                }
            }
        }
    }
}
