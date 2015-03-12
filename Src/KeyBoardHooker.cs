using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyBoardHook
{
    /// <summary>
    /// A class than manage global low level keyboard hook.
    /// </summary>
    class globalKeyboardHook
    {

        #region DLL imports
        /// <summary>
        /// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
        /// </summary>
        /// <param name="idHook">The id of the event you want to hook</param>
        /// <param name="callback">The callback.</param>
        /// <param name="hInstance">The handle you want to attach the event to, can be null</param>
        /// <param name="threadId">The thread you want to attach the event to, can be null</param>
        /// <returns>a handle to the desired hook</returns>
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

        /// <summary>
        /// Unhooks the windows hook.
        /// </summary>
        /// <param name="hInstance">The hook handle that was returned from SetWindowsHookEx</param>
        /// <returns>True if successful, false otherwise</returns>
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        /// <summary>
        /// Calls the next hook.
        /// </summary>
        /// <param name="idHook">The hook id</param>
        /// <param name="nCode">The hook code</param>
        /// <param name="wParam">The wparam.</param>
        /// <param name="lParam">The lparam.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);

        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="lpFileName">Name of the library</param>
        /// <returns>A handle to the library</returns>
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
        #endregion

        #region Constant,Structure and Delegates definition

        /// <summary>
        /// define the call back type for hook
        /// </summary>
        public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);

        public struct keyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;
        
        #endregion

        #region Instance variables

        //Collection of keys to watch..
        public List<Keys> HookedKeys = new List<Keys>();

        IntPtr hook = IntPtr.Zero;
        #endregion

        #region Events

        /// <summary>
        /// Occurs when one of the hoocked key down
        /// </summary>
        public event KeyEventHandler KeyDown;

        /// <summary>
        /// Occurs when one of the hooked key up
        /// </summary>
        public event KeyEventHandler KeyUp;

        #endregion

        #region public functions

        /// <summary>
        /// Initiate the public hook
        /// </summary>
        public void HookUp() {

            IntPtr hInstance = LoadLibrary("User32");
            hook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hInstance, 0);
                
        }

        /// <summary>
        /// Disposing the hook
        /// </summary>
        public void HookDown() {
            UnhookWindowsHookEx(hook);
        }

        /// <summary>
        /// Callback for the keyboardhook
        /// </summary>
        /// <param name="code">The hook code, must have to >=0. </param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">Keyhook event information</param>
        /// <returns></returns>
        private int hookProc(int code, int wParam, ref keyboardHookStruct lParam) {
            if (code >= 0) {
                Keys key = (Keys)lParam.vkCode; 
                if (HookedKeys.Contains(key)) {
                    KeyEventArgs EKey = new KeyEventArgs(key); //Initiating new event.
                    if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                    {
                        KeyDown(this, EKey);
                    }
                    else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                    {
                        KeyUp(this, EKey);
                    }
                    if (EKey.Handled)
                        return 1;
                }
            }
            return CallNextHookEx(hook, code, wParam, ref lParam);
        }
        #endregion


    }
}
