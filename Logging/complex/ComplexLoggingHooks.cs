using System.Runtime.InteropServices;

namespace Logging.complex;

public class ComplexLoggingHooks
{
    public delegate void KeyboardHookCallback(VKeys key);
    private delegate IntPtr KeyboardHookHandler(int nCode, IntPtr wParam, IntPtr lParam);

    private const int WmKeydown = 0x0100;
    private const int WmSyskeydown = 0x0104;
    private const int WmKeyup = 0x0101;
    private const int WmSyskeyup = 0x0105;
    private const int KeyboardHookId = 13;
    private static KeyboardHookHandler? _hookHandler;
    private GCHandle _gcHandler;
    private IntPtr _hookId = IntPtr.Zero;


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookHandler? lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    public event KeyboardHookCallback? OnKeyDownHandler;
    public event KeyboardHookCallback? OnKeyUpHandler;
    public void Start()
    {
        //https://stackoverflow.com/questions/69102624/executionengineexception-on-lowlevel-keyboard-hook
        _hookHandler = HookFunction; // workaround would crash without this
        _gcHandler = GCHandle.Alloc(_hookHandler);
        _hookId = SetWindowsHookEx(KeyboardHookId, _hookHandler, IntPtr.Zero, 0);
    }

    public void Stop()
    {
        UnhookWindowsHookEx(_hookId);
        _gcHandler.Free();
    }
    private IntPtr HookFunction(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode < 0)
            return CallNextHookEx(_hookId, nCode, wParam, lParam);

        int iwParam = wParam.ToInt32();

        switch (iwParam)
        {
            case WmKeydown or WmSyskeydown:
            {
                OnKeyDownHandler?.Invoke((VKeys)Marshal.ReadInt32(lParam));

                break;
            }
            case WmKeyup or WmSyskeyup:
            {
                OnKeyUpHandler?.Invoke((VKeys)Marshal.ReadInt32(lParam));

                break;
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

}