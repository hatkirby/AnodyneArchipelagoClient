using AnodyneSharp.Input;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Threading;

namespace AnodyneArchipelago.Menu
{
    internal class InputCharacter
    {
        private readonly string _upper;
        private readonly string _lower;
        private readonly Keys _code;

        public InputCharacter(string upper, string lower, Keys code)
        {
            _upper = upper;
            _lower = lower;
            _code = code;
        }

        public string ReturnCharacter(bool shiftDown)
        {
            return shiftDown ? _upper : _lower;
        }

        public Keys ReturnKey()
        {
            return _code;
        }
    }

    internal class InputHandler
    {
        private static List<InputCharacter> _characters = new();

        static InputHandler()
        {
            _characters.Add(new InputCharacter("A", "a", Keys.A));
            _characters.Add(new InputCharacter("B", "b", Keys.B));
            _characters.Add(new InputCharacter("C", "c", Keys.C));
            _characters.Add(new InputCharacter("D", "d", Keys.D));
            _characters.Add(new InputCharacter("E", "e", Keys.E));
            _characters.Add(new InputCharacter("F", "f", Keys.F));
            _characters.Add(new InputCharacter("G", "g", Keys.G));
            _characters.Add(new InputCharacter("H", "h", Keys.H));
            _characters.Add(new InputCharacter("I", "i", Keys.I));
            _characters.Add(new InputCharacter("J", "j", Keys.J));
            _characters.Add(new InputCharacter("K", "k", Keys.K));
            _characters.Add(new InputCharacter("L", "l", Keys.L));
            _characters.Add(new InputCharacter("M", "m", Keys.M));
            _characters.Add(new InputCharacter("N", "n", Keys.N));
            _characters.Add(new InputCharacter("O", "o", Keys.O));
            _characters.Add(new InputCharacter("P", "p", Keys.P));
            _characters.Add(new InputCharacter("Q", "q", Keys.Q));
            _characters.Add(new InputCharacter("R", "r", Keys.R));
            _characters.Add(new InputCharacter("S", "s", Keys.S));
            _characters.Add(new InputCharacter("T", "t", Keys.T));
            _characters.Add(new InputCharacter("U", "u", Keys.U));
            _characters.Add(new InputCharacter("V", "v", Keys.V));
            _characters.Add(new InputCharacter("W", "w", Keys.W));
            _characters.Add(new InputCharacter("X", "x", Keys.X));
            _characters.Add(new InputCharacter("Y", "y", Keys.Y));
            _characters.Add(new InputCharacter("Z", "z", Keys.Z));

            _characters.Add(new InputCharacter("!", "1", Keys.D1));
            _characters.Add(new InputCharacter("@", "2", Keys.D2));
            _characters.Add(new InputCharacter("#", "3", Keys.D3));
            _characters.Add(new InputCharacter("$", "4", Keys.D4));
            _characters.Add(new InputCharacter("%", "5", Keys.D5));
            _characters.Add(new InputCharacter("^", "6", Keys.D6));
            _characters.Add(new InputCharacter("&", "7", Keys.D7));
            _characters.Add(new InputCharacter("*", "8", Keys.D8));
            _characters.Add(new InputCharacter("(", "9", Keys.D9));
            _characters.Add(new InputCharacter(")", "0", Keys.D0));

            _characters.Add(new InputCharacter(" ", " ", Keys.Space));
            _characters.Add(new InputCharacter("<", ",", Keys.OemComma));
            _characters.Add(new InputCharacter("+", "=", Keys.OemPlus));
            _characters.Add(new InputCharacter("?", "/", Keys.OemQuestion));
            _characters.Add(new InputCharacter(">", ".", Keys.OemPeriod));
            _characters.Add(new InputCharacter("_", "-", Keys.OemMinus));
            _characters.Add(new InputCharacter("{", "[", Keys.OemOpenBrackets));
            _characters.Add(new InputCharacter("}", "]", Keys.OemCloseBrackets));
            _characters.Add(new InputCharacter("|", "\"", Keys.OemBackslash));
            _characters.Add(new InputCharacter(":", ";", Keys.OemSemicolon));
        }

        public static string ReturnCharacter()
        {
            if (KeyInput.JustPressedKey(Keys.V) && (KeyInput.IsKeyPressed(Keys.LeftControl) || KeyInput.IsKeyPressed(Keys.RightControl)))
            {
                string result = "";
                Thread clipboardThread = new(() => result = System.Windows.Forms.Clipboard.GetText());
                clipboardThread.SetApartmentState(ApartmentState.STA);
                clipboardThread.Start();
                clipboardThread.Join();

                return result;
            }

            foreach (InputCharacter inputCharacter in _characters)
            {
                if (KeyInput.JustPressedKey(inputCharacter.ReturnKey()))
                {
                    return inputCharacter.ReturnCharacter(KeyInput.IsKeyPressed(Keys.LeftShift) || KeyInput.IsKeyPressed(Keys.RightShift));
                }
            }

            return "";
        }
    }
}
