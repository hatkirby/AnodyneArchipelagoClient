using AnodyneSharp.Input;
using AnodyneSharp.Resources;
using AnodyneSharp.Sounds;
using AnodyneSharp.States;
using AnodyneSharp.UI;
using AnodyneSharp.UI.Font;
using AnodyneSharp.UI.Text;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Threading.Tasks;

namespace AnodyneArchipelago.Menu
{
    internal class ConnectionState : State
    {
        public delegate void SuccessEvent(ArchipelagoManager archipelagoManager);

        private readonly SuccessEvent _successFunc;

        private Task<LoginResult> _connectionTask;
        private ArchipelagoManager _archipelago = new();

        private TextWriter _textWriter;
        private UIEntity _bgBox;
        private readonly SpriteFont _font;

        private string _text = "Connecting...";

        public ConnectionState(string apServer, string apSlot, string apPassword, SuccessEvent successFunc)
        {
            _successFunc = successFunc;

            _connectionTask = Task.Run(() => _archipelago.Connect(apServer, apSlot, apPassword));

            _font = FontManager.InitFont(new Color(226, 226, 226), true);

            _textWriter = new(20, 44, 128, 100)
            {
                drawLayer = AnodyneSharp.Drawing.DrawOrder.TEXT
            };
            _textWriter.SetSpriteFont(_font, ResourceManager.GetTexture("consoleButtons"));

            _bgBox = new UIEntity(new Vector2(16f, 40f), "pop_menu", 16, 16, AnodyneSharp.Drawing.DrawOrder.TEXTBOX);
            UpdateDisplay();
        }

        public override void Update()
        {
            if (_connectionTask != null && _connectionTask.IsCompleted)
            {
                LoginResult result = _connectionTask.Result;

                if (result.Successful)
                {
                    Exit = true;
                    _successFunc(_archipelago);
                    return;
                }
                else
                {
                    LoginFailure failure = result as LoginFailure;
                    string errorMessage = "";
                    foreach (string error in failure.Errors)
                    {
                        errorMessage += error;
                        errorMessage += "\n";
                    }
                    foreach (ConnectionRefusedError error in failure.ErrorCodes)
                    {
                        errorMessage += error.ToString();
                        errorMessage += "\n";
                    }

                    if (errorMessage.Length > 0)
                    {
                        errorMessage = errorMessage.Substring(0, errorMessage.Length - 1);
                    }
                    else
                    {
                        errorMessage = "Unknown error during connection.";
                    }

                    _text = errorMessage;
                    _connectionTask = null;

                    UpdateDisplay();
                }
            }

            if (KeyInput.JustPressedRebindableKey(KeyFunctions.Accept) || KeyInput.JustPressedRebindableKey(KeyFunctions.Cancel))
            {
                Exit = true;
                SoundManager.PlaySoundEffect("menu_select");
            }
        }

        public override void DrawUI()
        {
            _bgBox.Draw();
            _textWriter.Draw();
        }

        private void UpdateDisplay()
        {
            _textWriter.Text = _text;
            _textWriter.ProgressTextToEnd();

            FieldInfo linesField = typeof(TextWriter).GetField("_line", BindingFlags.NonPublic | BindingFlags.Instance);
            int lineValue = (int)linesField.GetValue(_textWriter);

            int innerHeight = (lineValue + 1) * _font.lineSeparation;

            _bgBox = new UIEntity(new Vector2(16f, 40f), "pop_menu", 136, innerHeight + 8, AnodyneSharp.Drawing.DrawOrder.TEXTBOX);
        }
    }
}
