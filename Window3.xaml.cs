using System;
using System.IO;  // Ensure this namespace is being used for System.IO.Path
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace IoToGo
{
    public partial class Window3 : Window
    {
        private string _downloadFolderPath;

        public Window3(string path)
        {
            InitializeComponent();
            _downloadFolderPath = path;  // Assign or initialize the download folder path appropriately
        }

        private async Task DownloadFileAsync(string fileUrl, string fileName)
        {
            if (string.IsNullOrEmpty(fileUrl) || string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(_downloadFolderPath))
            {
                MessageBox.Show("Invalid URL, filename, or download path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string filePath = System.IO.Path.Combine(_downloadFolderPath, fileName);  // Explicitly specify System.IO.Path

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    long totalBytes = response.Content.Headers.ContentLength.GetValueOrDefault(-1L);
                    long totalBytesRead = 0L;

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        var contentStream = await response.Content.ReadAsStreamAsync();
                        byte[] buffer = new byte[8192];
                        int bytesRead;
                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;

                            if (totalBytes > 0)
                            {
                                double progress = (double)totalBytesRead / totalBytes * 100;
                                Dispatcher.Invoke(() => DownloadProgressBar.Value = progress);  // Ensure the ProgressBar control exists in your XAML
                            }
                        }
                    }

                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error downloading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
         private void dl(object sender, RoutedEventArgs e)
        {
            if (c1.IsChecked == true)
            {
                DownloadFileAsync("https://downloadmirror.intel.com/823673/WiFi-23.60.1-Driver64-Win10-Win11.exe\r\n", "intel-wifi.exe");
            }
            if (c2.IsChecked == true)
            {
                DownloadFileAsync("https://downloadmirror.intel.com/825880/BT-23.60.0-64UWD-Win10-Win11.exe\r\n", "intel-bluetooth.exe");
            }
            if (c3.IsChecked == true)
            {
                DownloadFileAsync("https://us.download.nvidia.com/Windows/560.81/560.81-desktop-win10-win11-64bit-international-dch-whql.exe", "nvdia-grd.exe");
            }
            
            if (c6.IsChecked == true)
            {
                DownloadFileAsync("https://downloadmirror.intel.com/829246/Release_29.2.1.zip\r\n", "intel-ethernet.exe");
            }
            if (c7.IsChecked == true)
            {
                DownloadFileAsync("https://drivers.amd.com/drivers/installer/24.10/whql/amd-software-adrenalin-edition-24.7.1-minimalsetup-240805_web.exe\r\n", "amd-driversutil.exe");
            }
            if (c8.IsChecked == true) 
            {
                DownloadFileAsync("https://store.steampowered.com/about/?snr=1_4_4__global-header\r\n", "valve-steam.exe");
            }
            if (c9.IsChecked == true)
            {
                DownloadFileAsync("https://download.scdn.co/SpotifySetup.exe\r\n", "spotify-spotify.exe");
            }
            if (c10.IsChecked == true)
            {
                DownloadFileAsync("https://spotx-official.github.io/run.ps1\r\n", "spotx-spotify.bat");
            }
            if (c11.IsChecked == true)
            {
                DownloadFileAsync("https://launcher-public-service-prod06.ol.epicgames.com/launcher/api/installer/download/EpicGamesLauncherInstaller.msi\r\n", "epicgames-launcher.msi");
            }
            if (c12.IsChecked == true)
            {
                DownloadFileAsync("https://discord.com/api/downloads/distributions/app/installers/latest?channel=stable&platform=win&arch=x64\r\n", "discord-discord.exe");
            }
            if (c13.IsChecked == true)
            {
                DownloadFileAsync("https://github.com/ArmCord/ArmCord/releases/download/v3.2.8/ArmCord.Setup.3.2.8.exe\r\n", "armcord-discord.exe");
            }
            if (c14.IsChecked == true)
            {
                DownloadFileAsync("https://download-chromium.appspot.com/dl/Win_x64?type=snapshots\r\n", "chromiumproject-chromium.zip");
            }
            if (c15.IsChecked == true)
            {
                DownloadFileAsync("https://download01.logi.com/web/ftp/pub/techsupport/gaming/lghub_installer.exe\r\n", "logitech-lghub.exe");
            }
            if (c16.IsChecked == true)
            {
                DownloadFileAsync("https://github.com/Ryochan7/DS4Windows/releases/download/v3.3.3/DS4Windows_3.3.3_x64.zip", "ds4-ds4windows.exe");
            }
            if (c18.IsChecked == true)
            {
                DownloadFileAsync("https://download.msi.com/uti_exe/desktop/MSI-Center.zip\r\n", "msi-msicenter.zip");
            }
            if (c19.IsChecked == true)
            {
                DownloadFileAsync("https://download.cpuid.com/cpu-z/cpu-z_2.10-en.exe\r\n", "cpuid-cpuz.exe");
            }
            if (c21.IsChecked == true)
            {
                DownloadFileAsync("https://download.msi.com/uti_exe/vga/MSIAfterburnerSetup.zip?__token__=exp=1724178010~acl=/*~hmac=1c13a0cf320d2db21b8ba2324cfe320b77cb90204c2612ea114b52181e88c932\r\n", "msi-afterburner.zip");
            }
            if (c22.IsChecked == true)
            {
                DownloadFileAsync("https://www.mozilla.org/de/firefox/download/thanks/\r\n", "mozilla-firefox.exe");
            }
            if (c23.IsChecked == true)
            {
                DownloadFileAsync("https://gitlab.com/api/v4/projects/44042130/packages/generic/librewolf/129.0.1-1/librewolf-129.0.1-1-windows-x86_64-setup.exe", "librewolf-firefox.exe");
            }
            if (c24.IsChecked == true)
            {
                DownloadFileAsync("https://github.com/microsoft/PowerToys/releases/download/v0.83.0/PowerToysUserSetup-0.83.0-x64.exe\r\n", "microsoft-powertoys.exe");
            }
            if (c25.IsChecked == true)
            {
                DownloadFileAsync("https://download.gimp.org/gimp/v2.10/windows/gimp-2.10.38-setup.exe\r\n", "gnuproject-gimp.exe");
            }
            if (c26.IsChecked == true)
            {
                DownloadFileAsync("https://muse-cdn.com/Muse_Hub.exe\r\n", "muse-audacity.exe");
            }
            if (c27.IsChecked == true)
            {
                DownloadFileAsync("https://c2rsetup.officeapps.live.com/c2r/download.aspx?ProductreleaseID=O365ProPlusRetail&platform=x64&language=en-us&version=O16GA\r\n", "microsoft-office.exe");
            }
            if (c28.IsChecked == true)
            {
                DownloadFileAsync("https://www.torproject.org/dist/torbrowser/13.5.2/tor-browser-windows-x86_64-portable-13.5.2.exe\r\n", "torproject-torbrowser.exe");
            }
            if (c29.IsChecked == true)
            {
                DownloadFileAsync("https://get.videolan.org/vlc/3.0.21/win64/vlc-3.0.21-win64.exe\r\n", "videolan-vlcmediaplayer.exe");
            }
            Window4 newWindow = new Window4();
            newWindow.Show();
            this.Close();

        }
        private void skip(object sender, RoutedEventArgs e)
        {
            Window4 newWindow = new Window4();
            newWindow.Show();
            this.Close();
        }





    }
}
