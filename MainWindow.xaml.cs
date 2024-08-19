using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
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
        public string path;

        public MainWindow()
        {
            InitializeComponent();
            path = init_folder();  // Initialize the 'path' variable
        }

        public string init_folder()
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
                        
                    }
                    else
                    {
                        
                    }

                    return newFolderPath;  // Return the created folder path
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }

            return null;  // Default return value if dialog is canceled
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
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            File.Delete(filePath);
                        }
                    catch {
                            
                        }
                        }

                           

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
            //path = init_folder();  // Initialize path here if it hasn't been already
            _cancellationTokenSource = new CancellationTokenSource();
            CancelButton.IsEnabled = true;

            string newFolderPath = Path.Combine(_downloadFolderPath, "IoToGo");

            await DownloadFileToFolderAsync("https://drive.massgrave.dev/en-us_windows_10_iot_enterprise_ltsc_2021_x64_dvd_257ad90f.iso", newFolderPath, _cancellationTokenSource.Token);
            CancelButton.IsEnabled = false;
            skip();
        }

        private void skip()
        {
            Window1 newWindow = new Window1(path);
            newWindow.Show();
            this.Close();
        }

        private void isoskip(object sender, RoutedEventArgs e)
        {
            skip();
        }
    }
}
