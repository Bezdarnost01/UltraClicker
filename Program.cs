using System;
using System.Runtime.InteropServices;
using System.Threading;
using PInvoke;

class Program
{
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll")]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    private const int VK_J = 0x4A;        // Клавиши J
    private const int VK_NUMPAD1 = 0x61;  // Клавиша 1 на нампаде
    private const int VK_NUMPAD2 = 0x62;  // Клавиша 2 на нампаде
    private const int VK_TILDE = 0xC0;    // Клавиша ~
    private const int VK_LBUTTON = 0x01;  // Левая кнопка мыши
    private const int VK_CONTROL = 0x11;  // Клавиша Ctrl
    private const int VK_W = 0x57;        // Клавиша W

    private const uint WM_KEYDOWN = 0x0100;
    private const uint WM_KEYUP = 0x0101;
    private const uint WM_LBUTTONDOWN = 0x0201;
    private const uint WM_LBUTTONUP = 0x0202;

    enum ClickerMode
    {
        Off,
        HoldLButton,
        Continuous
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Наведи курсор на окно и нажми J для выбора окна...");

        IntPtr windowHandle = IntPtr.Zero;
        ClickerMode clickerMode = ClickerMode.Off;
        bool isAutoRunEnabled = false;

        while (true)
        {
            if ((GetAsyncKeyState(VK_J) & 0x8000) != 0)
            {
                windowHandle = User32.GetForegroundWindow();
                if (windowHandle != IntPtr.Zero)
                {
                    Console.WriteLine("Окно выбрано! Нажмите ~ для включения/выключения кликера.");
                    break;
                }
            }
            Thread.Sleep(50);
        }

        while (true)
        {
            if ((GetAsyncKeyState(VK_TILDE) & 0x8000) != 0)
            {
                clickerMode = clickerMode == ClickerMode.Off ? ClickerMode.HoldLButton : ClickerMode.Off;

                if (clickerMode == ClickerMode.Off)
                {
                    Console.WriteLine("Кликер выключен. Нажмите ~ для включения.");
                }
                else
                {
                    Console.WriteLine("Кликер включен в режиме зажима ЛКМ. Нажмите 1 на нампаде для переключения режима.");
                }
                Thread.Sleep(500);
            }

            if ((GetAsyncKeyState(VK_NUMPAD1) & 0x8000) != 0)
            {
                if (clickerMode == ClickerMode.HoldLButton)
                {
                    clickerMode = ClickerMode.Continuous;
                    Console.WriteLine("Режим кликера: постоянный клик. Нажмите 1 на нампаде для переключения режима.");
                }
                else if (clickerMode == ClickerMode.Continuous)
                {
                    clickerMode = ClickerMode.HoldLButton;
                    Console.WriteLine("Режим кликера: зажим ЛКМ. Нажмите 1 на нампаде для переключения режима.");
                }
                Thread.Sleep(500);
            }

            if ((GetAsyncKeyState(VK_NUMPAD2) & 0x8000) != 0)
            {
                isAutoRunEnabled = !isAutoRunEnabled;

                if (isAutoRunEnabled)
                {
                    PostMessage(windowHandle, WM_KEYDOWN, (IntPtr)VK_CONTROL, IntPtr.Zero);
                    PostMessage(windowHandle, WM_KEYDOWN, (IntPtr)VK_W, IntPtr.Zero);
                    Console.WriteLine("Режим автобега включен. Нажмите 2 на нампаде для отключения.");
                }
                else
                {
                    PostMessage(windowHandle, WM_KEYUP, (IntPtr)VK_W, IntPtr.Zero);
                    PostMessage(windowHandle, WM_KEYUP, (IntPtr)VK_CONTROL, IntPtr.Zero);
                    Console.WriteLine("Режим автобега отключен. Нажмите 2 на нампаде для включения.");
                }
                Thread.Sleep(500);
            }

            if (clickerMode == ClickerMode.HoldLButton && (GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0)
            {
                PostMessage(windowHandle, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                PostMessage(windowHandle, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
                Thread.Sleep(100);
            }
            else if (clickerMode == ClickerMode.Continuous)
            {
                PostMessage(windowHandle, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                PostMessage(windowHandle, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
                Thread.Sleep(100);
            }
        }
    }
}
