using AnodyneSharp.Input;
using AnodyneSharp.Sounds;
using AnodyneSharp.States;
using AnodyneSharp.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AnodyneArchipelago.Menu
{
    internal class BaseTextEntry : State
    {
        public delegate void CommitChange(string value);

        private readonly string _header;
        private readonly CommitChange _commitFunc;

        private string _value;

        private UILabel _headerLabel;
        private UILabel _valueLabel;
        private UIEntity _bgBox;

        public BaseTextEntry(string header, string value, CommitChange commitFunc)
        {
            _header = header;
            _value = value;
            _commitFunc = commitFunc;

            _headerLabel = new(new Vector2(20f, 44f), false, _header, new Color(226, 226, 226), AnodyneSharp.Drawing.DrawOrder.TEXT);
            _valueLabel = new(new Vector2(20f, 52f), false, "", new Color(), AnodyneSharp.Drawing.DrawOrder.TEXT);
            _bgBox = new UIEntity(new Vector2(16f, 40f), "pop_menu", 16, 16, AnodyneSharp.Drawing.DrawOrder.TEXTBOX);

            UpdateDisplay();
        }

        protected void OnTextInput(char ch)
        {
            if (ch == '\b')
            {
                if (_value.Length > 0)
                {
                    _value = _value.Substring(0, _value.Length - 1);
                    UpdateDisplay();
                }
            }
            else if (ch == 22)
            {
                _value += Functions.GetClipboard();
                UpdateDisplay();
            }
            else if (!char.IsControl(ch))
            {
                _value += ch;
                UpdateDisplay();
            }
        }

        public override void Update()
        {
            if (KeyInput.JustPressedKey(Keys.Escape) || (KeyInput.ControllerMode && KeyInput.JustPressedRebindableKey(KeyFunctions.Cancel)))
            {
                SoundManager.PlaySoundEffect("menu_select");
                this.Exit = true;
            }
            else if (KeyInput.JustPressedKey(Keys.Enter) || (KeyInput.ControllerMode && KeyInput.JustPressedRebindableKey(KeyFunctions.Accept)))
            {
                SoundManager.PlaySoundEffect("menu_select");
                _commitFunc(_value);
                this.Exit = true;
            }
        }

        public override void DrawUI()
        {
            _bgBox.Draw();
            _headerLabel.Draw();
            _valueLabel.Draw();
        }

        private void UpdateDisplay()
        {
            if (_value.Length == 0)
            {
                _valueLabel.SetText("[empty]");
                _valueLabel.Color = new Color(116, 140, 144);
            }
            else
            {
                string finalText = "";
                string tempText = _value;

                while (tempText.Length > 18)
                {
                    finalText += tempText.Substring(0, 18);
                    finalText += "\n";
                    tempText = tempText.Substring(18);
                }

                finalText += tempText;

                _valueLabel.SetText(finalText);
                _valueLabel.Color = new Color(184, 32, 0);
            }

            float innerHeight = 8f + _valueLabel.Writer.TotalTextHeight();

            _bgBox = new UIEntity(new Vector2(16f, 40f), "pop_menu", 136, (int)innerHeight + 8, AnodyneSharp.Drawing.DrawOrder.TEXTBOX);
        }
    }
}
