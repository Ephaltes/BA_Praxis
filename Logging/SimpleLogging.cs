using System.Runtime.InteropServices;

namespace Logging;

public class SimpleLogging
{
    [DllImport("user32.dll")]
    public static extern ushort GetAsyncKeyState(int i);
}