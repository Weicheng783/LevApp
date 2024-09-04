using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Text;
using Microsoft.UI;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LevApp
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private string systemInfoPath = @"C:\Users\weicheng\Desktop\ServerSampler\bin\system_info.txt";
        private string instructionFilePath = @"C:\Users\weicheng\Desktop\ServerSampler\bin\instruction.txt";
        private string feedbackFilePath = @"C:\Users\weicheng\Desktop\ServerSampler\bin\feedback.txt";
        int counting_times = 0;
        private Dictionary<string, string> systemInfoValues = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            CheckServerStatus();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            LiveTimeText.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            CheckServerStatus();
        }

        private void CheckServerStatus()
        {
            if (File.Exists(systemInfoPath))
            {
                try {
                    var lines = File.ReadAllLines(systemInfoPath);
                    if (lines.Length > 0)
                    {
                        SystemInfoPanel.Children.Clear(); // Clear previous info
                        systemInfoValues.Clear();

                        string lastServerResponseTime = lines[0];
                        DateTime lastResponseDateTime = DateTime.ParseExact(lastServerResponseTime, "yyMMdd_HHmmss", null);

                        TextBlock timeTextBlock = new TextBlock
                        {
                            Text = "Last Server Response: " + lastResponseDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            FontWeight = FontWeights.Bold,
                            FontSize = 16,
                            Foreground = new SolidColorBrush(Colors.Blue),
                            Margin = new Thickness(0, 0, 0, 10)
                        };
                        SystemInfoPanel.Children.Add(timeTextBlock);

                        TimeSpan timeDifference = DateTime.Now - lastResponseDateTime;

                        if (timeDifference.TotalSeconds > 30)
                        {
                            UpdateStatus("Levitator Not Connected", "C:\\Users\\weicheng\\Desktop\\LevApp\\Assets\\greycross.png");
                        }
                        else
                        {
                            UpdateStatus("Server Connected", "C:\\Users\\weicheng\\Desktop\\LevApp\\Assets\\greentick.png");
                        }

                        // Display and store system info with styling
                        for (int i = 1; i < lines.Length; i++)
                        {
                            // Split the line into key and value parts
                            var parts = lines[i].Split(new[] { ':' }, 2);
                            if (parts.Length == 2)
                            {
                                string key = parts[0].Trim();
                                string value = parts[1].Trim();

                                // Store the key-value pair in the dictionary
                                systemInfoValues[key] = value;

                                if (key == "guiManageMode")
                                {
                                    key = "GUI Connected to Levitation Script";
                                }
                                else if (key == "noOptitrackMode") {
                                    key = "No Optitrack Mode";
                                }
                                else if (key == "noCaptureMode")
                                {
                                    key = "No Capture Mode";
                                }
                                else if (key == "maunalDataCollectionMode")
                                {
                                    key = "Manual Data Collection";
                                }
                                else if (key == "manualInputMode")
                                {
                                    key = "Manual Input Mode";
                                }
                                else if (key == "manualMode")
                                {
                                    key = "Manual Binary Search Mode";
                                }
                                else if (key == "current_velocity")
                                {
                                    key = "Current Velocity";
                                }
                                else if (key == "current_amplitude")
                                {
                                    key = "Current Amplitude";
                                }
                                else if (key == "numAgents")
                                {
                                    key = "Number of Agents (Particles)";
                                }
                                else if (key == "testPattern")
                                {
                                    key = "Test Path Pattern";
                                }
                                else if (key == "round")
                                {
                                    key = "Unique Round Number (#)";
                                }
                                else if (key == "systemVersion")
                                {
                                    key = "System Version String";
                                }
                                else if (key == "solver_algorithm")
                                {
                                    key = "PAT Solver Algorithm Used";
                                }
                                else if (key == "developmentMode")
                                {
                                    key = "Developer/Development Mode";
                                }
                                else if (key == "totalRecordings")
                                {
                                    key = "Current Number of Recording (#)";
                                }
                                else if (key == "lower_bound")
                                {
                                    key = "Current Searching Lower Bound";
                                }
                                else if (key == "upper_bound")
                                {
                                    key = "Current Searching Upper Bound";
                                }

                                if (key != "integratedDir" && key != "fullPathPre") {
                                    // Create a TextBlock for the key with bold styling
                                    TextBlock keyTextBlock = new TextBlock
                                    {
                                        Text = key + ": ",
                                        FontWeight = FontWeights.Bold,
                                        FontSize = 14,
                                        Margin = new Thickness(0, 0, 0, 2)
                                    };

                                    // Create a TextBlock for the value with normal styling
                                    TextBlock valueTextBlock = new TextBlock
                                    {
                                        Text = value,
                                        FontSize = 14,
                                        Margin = new Thickness(5, 0, 0, 5)
                                    };

                                    // Create a StackPanel to hold the key and value together
                                    StackPanel infoPanel = new StackPanel
                                    {
                                        Orientation = Orientation.Horizontal
                                    };
                                    infoPanel.Children.Add(keyTextBlock);
                                    infoPanel.Children.Add(valueTextBlock);

                                    // Add the styled info panel to the SystemInfoPanel
                                    SystemInfoPanel.Children.Add(infoPanel);
                                }
                            }
                        }
                    }
                }
                catch (Exception e) { }
            }
            else
            {
                UpdateStatus("Levitator Not Connected", "C:\\Users\\weicheng\\Desktop\\LevApp\\Assets\\greycross.png");
            }
        }

        private void UpdateStatus(string statusText, string iconPath)
        {
            StatusText.Text = statusText;

            // Update the StatusIcon with the correct URI format using an absolute path
            StatusIcon.Source = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectButton.Content.ToString() == "Connect")
            {
                // Write "0" to instruction.txt to signal connection request
                File.WriteAllText(instructionFilePath, "0");
                int checkMaxTimes = 10;
                int currentTimes = 0;

                // Continuously check for feedback.txt existence and content
                while (true)
                {
                    if (currentTimes >= checkMaxTimes) {
                        break;
                    }

                    if (File.Exists(feedbackFilePath))
                    {
                        var feedback = File.ReadAllLines(feedbackFilePath);
                        if (feedback.Length > 0 && feedback[0] == "0")
                        {
                            ShowToastNotification("Connection Request Received.", true);
                        }
                        else
                        {
                            ShowToastNotification("Connection Request Not Received, Check Server.", false);
                        }

                        // Delete feedback.txt after processing
                        File.Delete(feedbackFilePath);
                        break;
                    }

                    currentTimes++;
                    await System.Threading.Tasks.Task.Delay(1000); // Check every 1 second
                }
            }
            else
            {
                // Implement disconnect logic if needed
                File.WriteAllText(instructionFilePath, "1"); // Example for a disconnect signal
            }

            CheckServerStatus(); // Recheck server status after attempting connection
        }

        private void RunPythonScriptButton_Click(object sender, RoutedEventArgs e)
        {
            // Define the path to the Python interpreter and script
            string pythonInterpreter = @"C:/Python311/python.exe";
            string scriptPath = @"c:/Users/weicheng/Desktop/formal_dataset/visualisation_single.py";
            string argument = systemInfoValues["fullPathPre"] + "sim_output_TargetPosition.csv";

            // Create a ProcessStartInfo object to configure the process
            var processStartInfo = new ProcessStartInfo
            {
                FileName = pythonInterpreter,
                Arguments = $"\"{scriptPath}\" \"{argument}\"",
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                UseShellExecute = true,
                CreateNoWindow = false
            };

            // Create and start the process
            using (var process = new Process { StartInfo = processStartInfo })
            {
                try
                {
                    process.Start();

                    // Read the output and error streams
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    // Handle any errors that occurred during process execution
                }
            }
        }

        private void ShowToastNotification(string message, bool isSuccess)
        {
            NotificationInfoBar.Title = isSuccess ? "Success" : "Error";
            NotificationInfoBar.Message = message;
            NotificationInfoBar.Severity = isSuccess ? InfoBarSeverity.Success : InfoBarSeverity.Error;
            NotificationInfoBar.IsOpen = true;

            // Automatically close the InfoBar after 3 seconds
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += (sender, args) =>
            {
                NotificationInfoBar.IsOpen = false;
                timer.Stop();
            };
            timer.Start();
        }

        private void SaveParticlesButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the input from the TextBox
            string droppedParticles = ParticlesInputBox.Text;

            // Format the current velocity to six digits after the decimal point
            string formattedVelocity = double.Parse(systemInfoValues["current_velocity"]).ToString("F6");

            // Define the path to the file where you want to save the content
            string filePath = $"{systemInfoValues["integratedDir"]}/{(int.Parse(systemInfoValues["totalRecordings"]) - 0)}_{formattedVelocity}_c_pre_process_drop.txt";

            // Initialize the content to write to the file
            //string contentToWrite = $"Dropped Particles: {droppedParticles}\n";
            string contentToWrite = $"";

            try
            {
                // Convert the dropped particles input into a list of integers
                List<int> droppedParticlesList = droppedParticles.Split(' ').Select(int.Parse).ToList();

                // Loop through the number of agents and write whether each particle was dropped
                for (int i = 0; i < int.Parse(systemInfoValues["numAgents"]); i++)
                {
                    string status = droppedParticlesList.Contains(i) ? "y" : "n";
                    contentToWrite += $"{i} {status}\n";
                }

                // Write the content to the file
                File.WriteAllText(filePath, contentToWrite);
                ShowToastNotification("Particle Dropping Information Saved", true);
                File.WriteAllText(instructionFilePath, "4");
            }
            catch (Exception ex){
                ShowToastNotification("Particle Dropping Information Not Saved", false);
            }
        }


        private void ExitPlain_Click(object sender, RoutedEventArgs e)
        {
            // Handle Exit with no training Button Click
            File.WriteAllText(instructionFilePath, "2");
        }

        private void ExitTrainBN_Click(object sender, RoutedEventArgs e)
        {
            // Handle Exit with BN Training Button Click
            File.WriteAllText(instructionFilePath, "22");
        }

        private void ExitTrainBoth_Click(object sender, RoutedEventArgs e)
        {
            // Handle Exit with BN and MLP Training Button Click
            File.WriteAllText(instructionFilePath, "222");
        }

        private void PickingUp_Click(object sender, RoutedEventArgs e)
        {
            // Handle Pick Up Button Click
            File.WriteAllText(instructionFilePath, "3");
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            // Handle Abort Button Click
            File.WriteAllText(instructionFilePath, "4");
        }

        private void GoStart_Click(object sender, RoutedEventArgs e)
        {
            // Handle Start Test Button Click
            File.WriteAllText(instructionFilePath, "5");
        }

        private void SimpleAnalysisFull_Click(object sender, RoutedEventArgs e)
        {
            // Handle Simple Probability Analysis Button Click
            File.WriteAllText(instructionFilePath, "6");
        }

        private void SimpleAnalysisUnknown_Click(object sender, RoutedEventArgs e)
        {
            // Handle Simple Probability Analysis (unknown path) Button Click
            File.WriteAllText(instructionFilePath, "7");
        }

        private void BNFull_Click(object sender, RoutedEventArgs e)
        {
            // Handle Bayesian Network Full Button Click
            File.WriteAllText(instructionFilePath, "8");
        }

        private void BNSimple_Click(object sender, RoutedEventArgs e)
        {
            // Handle Bayesian Network Simple Button Click
            File.WriteAllText(instructionFilePath, "9");
        }

        private void MLP_Click(object sender, RoutedEventArgs e)
        {
            // Handle Multi-Layer Perceptron (MLP) Inference Button Click
            File.WriteAllText(instructionFilePath, "10");
        }

        private void BSHL_Click(object sender, RoutedEventArgs e)
        {
            // Handle Manual Binary Search (to higher the lower bound) Button Click
            File.WriteAllText(instructionFilePath, "11");
        }

        private void BSLU_Click(object sender, RoutedEventArgs e)
        {
            // Handle Manual Binary Search (to lower the upper bound) Button Click
            File.WriteAllText(instructionFilePath, "12");
        }

        private void ManualInput_Click(object sender, RoutedEventArgs e)
        {
            // Handle Manual Drop Result Input Button Click
            File.WriteAllText(instructionFilePath, "13");
        }

        private void ManualDataCollection_Click(object sender, RoutedEventArgs e)
        {
            // Handle Manual Data Collection Button Click
            File.WriteAllText(instructionFilePath, "14");
        }

        private void AmpAdd_Click(object sender, RoutedEventArgs e)
        {
            // Handle Add Amplitude by 1000 Button Click
            File.WriteAllText(instructionFilePath, "15");
        }

        private void AmpReduce_Click(object sender, RoutedEventArgs e)
        {
            // Handle Reduce Amplitude by 1000 Button Click
            File.WriteAllText(instructionFilePath, "16");
        }

        private void AmpAddSmall_Click(object sender, RoutedEventArgs e)
        {
            // Handle Add Amplitude by 500 Button Click
            File.WriteAllText(instructionFilePath, "17");
        }

        private void AmpReduceSmall_Click(object sender, RoutedEventArgs e)
        {
            // Handle Reduce Amplitude by 500 Button Click
            File.WriteAllText(instructionFilePath, "18");
        }

        private void ManualSpeedReduction_Click(object sender, RoutedEventArgs e)
        {
            // Handle Reduce Velocity by 0.05 Button Click
            File.WriteAllText(instructionFilePath, "19");
        }

        private void ManualSpeedIncrease_Click(object sender, RoutedEventArgs e)
        {
            // Handle Add Velocity by 0.05 Button Click
            File.WriteAllText(instructionFilePath, "20");
        }

        private void NoCaptureMode_Click(object sender, RoutedEventArgs e)
        {
            // Handle Capture Mode (Use Cameras) Button Click
            File.WriteAllText(instructionFilePath, "21");
        }


    }

}
