using System.Runtime.InteropServices;

namespace Logging.complex;

public class ComplexLoggingHooks
{
    public delegate void KeyboardHookCallback(VKeys key);

    private const int WmKeydown = 0x100;
    private const int WmSyskeydown = 0x104;
    private const int WmKeyup = 0x101;
    private const int WmSyskeyup = 0x105;
    private static KeyboardHookHandler? _hookHandler;
    private GCHandle _gcHandler;
    private IntPtr _hookId = IntPtr.Zero;

    /// <summary>
    ///     Hooks into specified hookId and calls a callback handler
    ///     https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowshookexa
    /// </summary>
    /// <param name="idHook"></param>
    /// <param name="lpfn"></param>
    /// <param name="hMod"></param>
    /// <param name="dwThreadId"></param>
    /// <returns></returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookHandler? lpfn, IntPtr hMod, uint dwThreadId);

    /// <summary>
    ///     https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-unhookwindowshookex
    /// </summary>
    /// <param name="hhk"></param>
    /// <returns></returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    public event KeyboardHookCallback? KeyDownEvent;
    public event KeyboardHookCallback? KeyUpEvent;
    public void Start()
    {
        //https://stackoverflow.com/questions/69102624/executionengineexception-on-lowlevel-keyboard-hook
        _hookHandler = HookFunc; // workaround would crash without this
        _gcHandler = GCHandle.Alloc(_hookHandler);
        _hookId = SetWindowsHookEx(13, _hookHandler, IntPtr.Zero, 0);
    }

    public void Stop()
    {
        UnhookWindowsHookEx(_hookId);
        _gcHandler.Free();
    }
    private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode < 0)
            return CallNextHookEx(_hookId, nCode, wParam, lParam);

        int iwParam = wParam.ToInt32();

        switch (iwParam)
        {
            case WmKeydown or WmSyskeydown:
            {
                KeyDownEvent?.Invoke((VKeys)Marshal.ReadInt32(lParam));

                break;
            }
            case WmKeyup or WmSyskeyup:
            {
                KeyUpEvent?.Invoke((VKeys)Marshal.ReadInt32(lParam));

                break;
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private delegate IntPtr KeyboardHookHandler(int nCode, IntPtr wParam, IntPtr lParam);
}