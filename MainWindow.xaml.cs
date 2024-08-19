using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using IoToGo;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private string? _downloadFolderPath = null;
        private CancellationTokenSource? _cancellationTokenSource = null;

        public MainWindow()
        {
            InitializeComponent();
            init_folder();
        }

        private void init_folder()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _downloadFolderPath = dialog.FileName;

                string newFolderName = "IoToGo";
                string newFolderPath = Path.Combine(_downloadFolderPath, newFolderName);

                try
                {
                    if (!Directory.Exists(newFolderPath))
                    {
                        Directory.CreateDirectory(newFolderPath);
                        MessageBox.Show($"Folder created successfully at: {newFolderPath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Folder already exists.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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
                        MessageBox.Show($"File downloaded successfully to: {filePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (OperationCanceledException)
                {
                    skip();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error downloading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        private async void download(object sender, RoutedEventArgs e)
        {
            init_folder();
            _cancellationTokenSource = new CancellationTokenSource();
            CancelButton.IsEnabled = true;

            string newFolderPath = Path.Combine(_downloadFolderPath, "IoToGo");

            await DownloadFileToFolderAsync("https://drive.massgrave.dev/en-us_windows_10_iot_enterprise_ltsc_2021_x64_dvd_257ad90f.iso", newFolderPath, _cancellationTokenSource.Token);
            CancelButton.IsEnabled = false;
            skip();
        }

        private void skip()
        {
            Window1 newWindow = new Window1();
            newWindow.Show();
            this.Close();
        }
    }
}
