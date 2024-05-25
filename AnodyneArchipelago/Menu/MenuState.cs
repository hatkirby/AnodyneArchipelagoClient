using AnodyneSharp.Input;
using AnodyneSharp.Registry;
using AnodyneSharp.Sounds;
using AnodyneSharp.States;
using AnodyneSharp.UI;
using AnodyneSharp.UI.PauseMenu;
using AnodyneSharp.UI.PauseMenu.Config;
using Microsoft.Xna.Framework;

namespace AnodyneArchipelago.Menu
{
    internal class MenuState : AnodyneSharp.States.State
    {
        private MenuSelector _selector;
        private UILabel _versionLabel1;
        private UILabel _versionLabel2;
        private UILabel _serverLabel;
        private UILabel _serverValue;
        private UILabel _slotLabel;
        private UILabel _slotValue;
        private UILabel _passwordLabel;
        private UILabel _passwordValue;
        private TextSelector _connectionSwitcher;
        private UILabel _connectLabel;
        private UILabel _settingsLabel;
        private UILabel _quitLabel;

        private State _substate = null;

        private string _apServer = "";
        private string _apSlot = "";
        private string _apPassword = "";

        private int _selectorIndex = 0;

        public override void Create()
        {
            _selector = new();
            _selector.Play("enabledRight");

            _versionLabel1 = new(new Vector2(10f, 7f), false, "AnodyneArchipelago", new Color(116, 140, 144));
            _versionLabel2 = new(new Vector2(10f, 15f), false, $"v{Plugin.GetVersion()}", new Color(116, 140, 144));
            _serverLabel = new(new Vector2(10f, 31f), false, $"Server:", new Color(226, 226, 226));
            _serverValue = new(new Vector2(18f, 39f), false, "", new Color());
            _slotLabel = new(new Vector2(10f, 51f), false, $"Slot:", new Color(226, 226, 226));
            _slotValue = new(new Vector2(18f, 59f), false, "", new Color());
            _passwordLabel = new(new Vector2(10f, 71f), false, $"Password:", new Color(226, 226, 226));
            _passwordValue = new(new Vector2(18f, 79f), false, "", new Color());
            _connectLabel = new(new Vector2(60f, 115f), false, $"Connect", new Color(116, 140, 144));
            _settingsLabel = new(new Vector2(60f, 131f), false, $"Config", new Color(116, 140, 144));
            _quitLabel = new(new Vector2(60f, 147f), false, $"Quit", new Color(116, 140, 144));

            _connectionSwitcher = new(new Vector2(60f, 95f), 32f, 0, true, new string[1] { "1/1" });
            _connectionSwitcher.noConfirm = true;
            _connectionSwitcher.noLoop = true;
            _connectionSwitcher.ValueChangedEvent = PageValueChanged;

            SetCursorPosition(0);
            UpdateLabels();
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            if (_substate != null)
            {
                _substate.Update();

                if (_substate.Exit)
                {
                    _substate = null;
                }

                return;
            }

            _selector.Update();
            _selector.PostUpdate();

            if (_selectorIndex == 3)
            {
                _connectionSwitcher.Update();
            }

            BrowseInput();
        }

        public override void Draw()
        {
        }

        public override void DrawUI()
        {
            _selector.Draw();
            _versionLabel1.Draw();
            _versionLabel2.Draw();
            _serverLabel.Draw();
            _serverValue.Draw();
            _slotLabel.Draw();
            _slotValue.Draw();
            _passwordLabel.Draw();
            _passwordValue.Draw();
            _connectionSwitcher.Draw();
            _connectLabel.Draw();
            _settingsLabel.Draw();
            _quitLabel.Draw();

            if (_substate != null)
            {
                _substate.DrawUI();
            }
        }

        private void SetCursorPosition(int i)
        {
            _selectorIndex = i;

            if (_selectorIndex == 0)
            {
                _selector.Position = new(2f, 34f);
            }
            else if (_selectorIndex == 1)
            {
                _selector.Position = new(2f, 54f);
            }
            else if (_selectorIndex == 2)
            {
                _selector.Position = new(2f, 74f);
            }
            else if (_selectorIndex == 4)
            {
                _selector.Position = new(52f, 118f);
            }
            else if (_selectorIndex == 5)
            {
                _selector.Position = new(52f, 134f);
            }
            else if (_selectorIndex == 6)
            {
                _selector.Position = new(52f, 150f);
            }

            if (_selectorIndex == 3)
            {
                _selector.visible = false;
                _connectionSwitcher.GetControl();
            }
            else
            {
                _selector.visible = true;
                _connectionSwitcher.LoseControl();
            }
        }

        private void UpdateLabels()
        {
            UpdateLabel(_serverValue, _apServer);
            UpdateLabel(_slotValue, _apSlot);
            UpdateLabel(_passwordValue, _apPassword);
        }

        private void UpdateLabel(UILabel label, string text)
        {
            if (text.Length == 0)
            {
                label.SetText("[empty]");
                label.Color = new Color(116, 140, 144);
            }
            else
            {
                if (text.Length > 20)
                {
                    label.SetText(text.Substring(0, 17) + "...");
                }
                else
                {
                    label.SetText(text);
                }

                label.Color = new Color(184, 32, 0);
            }
        }

        private void BrowseInput()
        {
            if (KeyInput.JustPressedRebindableKey(KeyFunctions.Up))
            {
                SoundManager.PlaySoundEffect("menu_move");
                if (_selectorIndex > 0)
                {
                    SetCursorPosition(_selectorIndex - 1);
                }
            }
            else if (KeyInput.JustPressedRebindableKey(KeyFunctions.Down))
            {
                SoundManager.PlaySoundEffect("menu_move");
                if (_selectorIndex < 6)
                {
                    SetCursorPosition(_selectorIndex + 1);
                }
            }
            else if (KeyInput.JustPressedRebindableKey(KeyFunctions.Accept))
            {
                SoundManager.PlaySoundEffect("menu_select");

                switch (_selectorIndex)
                {
                    case 0:
                        _substate = new TextEntry("Server:", _apServer, (string value) => { _apServer = value; UpdateLabels(); });
                        break;
                    case 1:
                        _substate = new TextEntry("Slot:", _apSlot, (string value) => { _apSlot = value; UpdateLabels(); });
                        break;
                    case 2:
                        _substate = new TextEntry("Password:", _apPassword, (string value) => { _apPassword = value; UpdateLabels(); });
                        break;
                    case 6:
                        GlobalState.ClosingGame = true;
                        break;
                    default:
                        // Hi
                        break;
                }
            }
        }

        private void PageValueChanged(string value, int index)
        {

        }
    }
}
