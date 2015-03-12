using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeyBoardHook
{
    /// <summary>
    /// Event arguments passed as parameter to hoocker events.
    /// </summary>
    public class HoockerEventArgs 
    {
        public KeyEventArgs key;
        public bool isShiftPressed = false;
        public bool isAlphabet=false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="key">Object of KeyEventArgs</param>
        /// <param name="isShiftPressed">boolean value represent the state of shift key</param>
        /// <param name="isAplhabet">Pressed key is alphabet of not</param>
        public HoockerEventArgs(KeyEventArgs key, bool isShiftPressed, bool isAplhabet) {
            this.key = key;
            this.isShiftPressed = isShiftPressed;
            this.isAlphabet = isAplhabet;
        }
    }
}
