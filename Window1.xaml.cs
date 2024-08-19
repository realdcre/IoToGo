using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace IoToGo
{
    public partial class Window1 : Window
    {
        public bool ranrufus = false;
        private string path;

        public Window1(string path)
        {
            InitializeComponent();
            this.path = path;  // Store the passed path value
        }

        private static readonly HttpClient httpClient = new HttpClient();

        public async Task DownloadRunAndCleanupToolAsync(string url, string fileName)
        {
            string downloadsFolder = path;  // Use the path from MainWindow
            string filePath = Path.Combine(downloadsFolder, fileName);

            try
            {
                // Download the file
                await DownloadFileAsync(url, filePath);

                // Execute the file asynchronously and wait for it to finish
                await Task.Run(() =>
                {
                    Process process = StartProcess(filePath);

                    if (process != null)
                    {
                        process.WaitForExit();  // Wait for the process to exit
                    }
                });

                // Clean up the file
                CleanupFile(filePath);

                // Set the flag to true
                ranrufus = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DownloadFileAsync(string url, string filePath)
        {
            try
            {
                using (HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                        fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                    {
                        await contentStream.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during download: {ex.Message}", "Download Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;  // Re-throw the exception to handle it in the calling method
            }
        }

        private Process StartProcess(string filePath)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true  // Allows the file to be executed
                };
                return Process.Start(processStartInfo);
            }
            catch (Exception)
            {
                // Log the error if needed, but don't throw an exception
                return null;
            }
        }

        private void CleanupFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (IOException ioEx)
                {
                    MessageBox.Show($"IOException occurred while deleting the file: {ioEx.Message}", "Cleanup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the file: {ex.Message}", "Cleanup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ok(object sender, RoutedEventArgs e)
        {
            await DownloadRunAndCleanupToolAsync("https://github.com/pbatard/rufus/releases/download/v4.5/rufus-4.5.exe", "rufus.exe");
        }

        private void next(object sender, RoutedEventArgs e)
        {
            if (ranrufus)
            {
                Window2 newWindow = new Window2(path);
                newWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Run Rufus first!");
            }
        }
    }
}
