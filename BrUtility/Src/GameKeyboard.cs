using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BrUtility
{
    public class GameKeyboard
    {
        private int[] keyHeldCounts;
        private KeyboardState currentState, lastState;

        public static bool preventInput;

        public GameKeyboard()
        {
            keyHeldCounts = new int[256];   //idk how big this should be to be honest. Might need to be increased.
        }

        public void PreUpdate()
        {
            currentState = Keyboard.GetState();
        }

        public void Update()
        {
            for (int i = 0; i < keyHeldCounts.Length; i++)
            {
                if (keyHeldCounts[i] < 0)
                    keyHeldCounts[i] = 0;
                else keyHeldCounts[i]--;
            }
        }

        public void PostUpdate()
        {
            lastState = currentState;
        }

        public Keys[] GetCurrentPressedKeys()
        {
            return currentState.GetPressedKeys();
        }

        public Keys[] GetLastPressedKeys()
        {
            return lastState.GetPressedKeys();
        }

        public bool KeyPressed(Keys key, bool overrideNoInput = false)
        {
            if (preventInput && !overrideNoInput)
                return false;
            Keys[] current = GetCurrentPressedKeys();
            Keys[] last = GetLastPressedKeys();

            return current.Contains(key) && !last.Contains(key);
        }

        public bool KeyHeld(Keys key, bool overrideNoInput = false)
        {
            if (preventInput && !overrideNoInput)
                return false;

            Keys[] current = GetCurrentPressedKeys();

            return current.Contains(key);
        }

        public bool KeyHeldAfterTime(Keys key, int time, bool overrideNoInput = false)
        {
            if (preventInput && !overrideNoInput)
                return false;

            if (KeyHeld(key))
                keyHeldCounts[(int)key]+=2;
            else keyHeldCounts[(int)key] = 0;

            if (keyHeldCounts[(int)key] > time)
                return true;
            return false;
        }

        public bool KeyModifierPressed(Keys key, Keys modifier, bool overrideNoInput = false)
        {
            if (preventInput && !overrideNoInput)
                return false;

            if (KeyHeld(key) && KeyPressed(modifier) || KeyHeld(modifier) && KeyPressed(key))
                return true;
            return false;
        }

        public bool KeyModifierHeldAfterTime(Keys key, Keys modifier, int time, bool overrideNoInput = false)
        {
            if (preventInput && !overrideNoInput)
                return false;

            if (KeyHeldAfterTime(key, time, overrideNoInput) && KeyPressed(modifier) || KeyHeldAfterTime(modifier, time, overrideNoInput) && KeyPressed(key))
                return true;
            return false;
        }

        public bool KeysHeld(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (!KeyHeld(key))
                    return false;
            }

            return true;
        }

        public char[] GetKeyboardInput()
        {
            char[] outkeys = new char[4];
            Keys[] keys = GetCurrentPressedKeys();
            bool shift = KeyHeld(Keys.LeftShift, true) || KeyHeld(Keys.RightShift, true);

            bool setany = false;
            foreach (Keys key in keys)
            {
                if (key == Keys.Back || key == Keys.Enter || key == Keys.LeftShift || key == Keys.RightShift || key == Keys.LeftAlt || key == Keys.RightAlt ||
                    key == Keys.LeftControl || key == Keys.RightControl || key == Keys.LeftWindows || key == Keys.RightWindows)
                    continue;
                if (KeyPressed(key, true))
                {
                    char k = '\0';
                    switch (key)
                    {
                        case Keys.A: if (shift) { k = 'A'; } else { k = 'a'; } break;
                        case Keys.B: if (shift) { k = 'B'; } else { k = 'b'; } break;
                        case Keys.C: if (shift) { k = 'C'; } else { k = 'c'; } break;
                        case Keys.D: if (shift) { k = 'D'; } else { k = 'd'; } break;
                        case Keys.E: if (shift) { k = 'E'; } else { k = 'e'; } break;
                        case Keys.F: if (shift) { k = 'F'; } else { k = 'f'; } break;
                        case Keys.G: if (shift) { k = 'G'; } else { k = 'g'; } break;
                        case Keys.H: if (shift) { k = 'H'; } else { k = 'h'; } break;
                        case Keys.I: if (shift) { k = 'I'; } else { k = 'i'; } break;
                        case Keys.J: if (shift) { k = 'J'; } else { k = 'j'; } break;
                        case Keys.K: if (shift) { k = 'K'; } else { k = 'k'; } break;
                        case Keys.L: if (shift) { k = 'L'; } else { k = 'l'; } break;
                        case Keys.M: if (shift) { k = 'M'; } else { k = 'm'; } break;
                        case Keys.N: if (shift) { k = 'N'; } else { k = 'n'; } break;
                        case Keys.O: if (shift) { k = 'O'; } else { k = 'o'; } break;
                        case Keys.P: if (shift) { k = 'P'; } else { k = 'p'; } break;
                        case Keys.Q: if (shift) { k = 'Q'; } else { k = 'q'; } break;
                        case Keys.R: if (shift) { k = 'R'; } else { k = 'r'; } break;
                        case Keys.S: if (shift) { k = 'S'; } else { k = 's'; } break;
                        case Keys.T: if (shift) { k = 'T'; } else { k = 't'; } break;
                        case Keys.U: if (shift) { k = 'U'; } else { k = 'u'; } break;
                        case Keys.V: if (shift) { k = 'V'; } else { k = 'v'; } break;
                        case Keys.W: if (shift) { k = 'W'; } else { k = 'w'; } break;
                        case Keys.X: if (shift) { k = 'X'; } else { k = 'x'; } break;
                        case Keys.Y: if (shift) { k = 'Y'; } else { k = 'y'; } break;
                        case Keys.Z: if (shift) { k = 'Z'; } else { k = 'z'; } break;

                        //Decimal keys
                        case Keys.D0: if (shift) { k = ')'; } else { k = '0'; } break;
                        case Keys.D1: if (shift) { k = '!'; } else { k = '1'; } break;
                        case Keys.D2: if (shift) { k = '@'; } else { k = '2'; } break;
                        case Keys.D3: if (shift) { k = '#'; } else { k = '3'; } break;
                        case Keys.D4: if (shift) { k = '$'; } else { k = '4'; } break;
                        case Keys.D5: if (shift) { k = '%'; } else { k = '5'; } break;
                        case Keys.D6: if (shift) { k = '^'; } else { k = '6'; } break;
                        case Keys.D7: if (shift) { k = '&'; } else { k = '7'; } break;
                        case Keys.D8: if (shift) { k = '*'; } else { k = '8'; } break;
                        case Keys.D9: if (shift) { k = '('; } else { k = '9'; } break;

                        //Decimal numpad keys
                        case Keys.NumPad0: k = '0'; break;
                        case Keys.NumPad1: k = '1'; break;
                        case Keys.NumPad2: k = '2'; break;
                        case Keys.NumPad3: k = '3'; break;
                        case Keys.NumPad4: k = '4'; break;
                        case Keys.NumPad5: k = '5'; break;
                        case Keys.NumPad6: k = '6'; break;
                        case Keys.NumPad7: k = '7'; break;
                        case Keys.NumPad8: k = '8'; break;
                        case Keys.NumPad9: k = '9'; break;

                        //Special keys
                        case Keys.OemTilde: if (shift) { k = '~'; } else { k = '`'; } break;
                        case Keys.OemSemicolon: if (shift) { k = ':'; } else { k = ';'; } break;
                        case Keys.OemQuotes: if (shift) { k = '"'; } else { k = '\''; } break;
                        case Keys.OemQuestion: if (shift) { k = '?'; } else { k = '/'; } break;
                        case Keys.OemPlus: if (shift) { k = '+'; } else { k = '='; } break;
                        case Keys.OemPipe: if (shift) { k = '|'; } else { k = '\\'; } break;
                        case Keys.OemPeriod: if (shift) { k = '>'; } else { k = '.'; } break;
                        case Keys.OemOpenBrackets: if (shift) { k = '{'; } else { k = '['; } break;
                        case Keys.OemCloseBrackets: if (shift) { k = '}'; } else { k = ']'; } break;
                        case Keys.OemMinus: if (shift) { k = '_'; } else { k = '-'; } break;
                        case Keys.OemComma: if (shift) { k = '<'; } else { k = ','; } break;
                        case Keys.Space: k = ' '; break;
                    }

                    for (int i = 0; i < outkeys.Length; i++)
                    {
                        if (outkeys[i] == '\0')
                        {
                            outkeys[i] = k;
                            break;
                        }

                    }
                    setany = true;
                }
            }

            if (!setany)
                return null;
            return outkeys;
        }

        public T Add<T>(T[] array, T addto)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                {
                    array[i] = addto;
                    return addto;
                }
            }

            return addto;
        }
    }
}
