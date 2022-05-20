using System.Runtime.InteropServices;
using System.Text;

namespace Logging;

public class WindowHook
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetWindowText(IntPtr windowHandle, StringBuilder outWindowText, int maxWindowTextLength);

    public string GetWindowText()
    {
        IntPtr windowHandle = GetForegroundWindow();
        StringBuilder windowText = new StringBuilder(256);
        _ = GetWindowText(windowHandle, windowText, windowText.Capacity);

        return windowText.ToString();
    }
}