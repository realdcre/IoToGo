using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace IoToGo
{
    public partial class Window2 : Window
    {
        private string path;

        public Window2(string path)
        {
            InitializeComponent();
            this.path = path;  // Store the passed path value
        }

        private async Task DownloadFileToFolderAsync(string fileUrl, string downloadFolderPath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(fileUrl) || string.IsNullOrEmpty(downloadFolderPath))
            {
                MessageBox.Show("Invalid file URL or download path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
            string filePath = Path.Combine(downloadFolderPath, fileName);

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                    response.EnsureSuccessStatusCode();

                    long totalBytes = response.Content.Headers.ContentLength.GetValueOrDefault(-1L);
                    long totalBytesRead = 0L;

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        var contentStream = await response.Content.ReadAsStreamAsync();

                        byte[] buffer = new byte[8192];
                        int bytesRead;
                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                            totalBytesRead += bytesRead;

                            if (totalBytes > 0)
                            {
                                double progress = (double)totalBytesRead / totalBytes;
                                Dispatcher.Invoke(() => DownloadProgressBar.Value = progress * 100);
                            }

                            if (cancellationToken.IsCancellationRequested)
                            {
                                MessageBox.Show("Download canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
                                break;
                            }
                        }
                    }

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        
                    }
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("Download canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error downloading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void download(object sender, RoutedEventArgs e)
        {
            string newFolderPath = Path.Combine(path);

            if (!Directory.Exists(newFolderPath))
            {
                Directory.CreateDirectory(newFolderPath);
            }

            var cts = new CancellationTokenSource();
            await DownloadFileToFolderAsync("https://github.com/kr0tchet/LTSC-Add-MicrosoftStore-2021/archive/refs/heads/master.zip", newFolderPath, cts.Token);
            Window3 newWindow = new Window3(path);
            newWindow.Show();
            this.Close();
        }
        private void skip(object sender, RoutedEventArgs e)
        {
            Window3 newWindow = new Window3(path);
            newWindow.Show();
            this.Close();
            
        }

    }
}
