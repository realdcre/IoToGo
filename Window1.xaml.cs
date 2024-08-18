using System;
using System.Diagnostics;
using System.IO; // Ensure System.IO is used for file operations
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace IoToGo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public bool ranrufus = false;
        public Window1()
        {
            InitializeComponent();
        }

        private static readonly HttpClient httpClient = new HttpClient();

        public async Task DownloadRunAndCleanupToolAsync(string url, string fileName)
        {
            // Use the Downloads folder within the user's Documents folder
            string downloadsFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Downloads");
            string filePath = System.IO.Path.Combine(downloadsFolder, fileName);

            try
            {
                // Download the file
                await DownloadFileAsync(url, filePath);

                // Execute the file
                Process process = StartProcess(filePath);

                // Wait for the process to exit
                process.WaitForExit();

                // Clean up the file
                CleanupFile(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private async Task DownloadFileAsync(string url, string filePath)
        {
            using (HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                       fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None, 4096, true))
                {
                    await contentStream.CopyToAsync(fileStream);
                }
            }
        }

        private Process StartProcess(string filePath)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true // Allows the file to be executed
            };
            Process process = Process.Start(processStartInfo);

            if (process == null)
            {
                throw new Exception("Failed to start process.");
            }

            return process;
        }

        private void CleanupFile(string filePath)
        {
            // Ensure the file is not in use and then delete it
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    System.IO.File.Delete(filePath);
                }
                catch (IOException ioEx)
                {
                    Console.WriteLine($"IOException occurred while deleting the file: {ioEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while deleting the file: {ex.Message}");
                }
                ranrufus = true;
            }
        }

        private void ok(object sender, RoutedEventArgs e)
        {
            DownloadRunAndCleanupToolAsync("https://github.com/pbatard/rufus/releases/download/v4.5/rufus-4.5.exe", "rufus.exe");
        }

        private void next(object sender, RoutedEventArgs e)
        {
            if (ranrufus)
            {
                Window2 newWindow = new Window2();

                // Show the new window
                newWindow.Show();


                // Close the current window
                this.Close();
            }
            else
            {
                MessageBox.Show("Run Rufus to Continue");
            }
        }
    }
}

