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
        private bool _hide = false;

        private string _apServer = "";
        private string _apSlot = "";
        private string _apPassword = "";

        private ArchipelagoSettings _archipelagoSettings;
        private int _curPage;

        private int _selectorIndex = 0;

        private bool _fadingOut = false;
        private bool _isNewGame;

        public override void Create()
        {
            if (Plugin.ArchipelagoManager != null)
            {
                Plugin.ArchipelagoManager.Disconnect();
                Plugin.ArchipelagoManager = null;
            }

            _archipelagoSettings = ArchipelagoSettings.Load();
            if (_archipelagoSettings == null)
            {
                _archipelagoSettings = new();
            }

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

            string[] selectorValues = new string[_archipelagoSettings.ConnectionDetails.Count + 1];
            for (int i= 0; i < selectorValues.Length; i++)
            {
                selectorValues[i] = $"{i+1}/{selectorValues.Length}";
            }

            _connectionSwitcher = new(new Vector2(60f, 95f), 32f, 0, true, selectorValues);
            _connectionSwitcher.noConfirm = true;
            _connectionSwitcher.noLoop = true;
            _connectionSwitcher.ValueChangedEvent = PageValueChanged;

            SetCursorPosition(0);
            SetPage(_archipelagoSettings.ConnectionDetails.Count == 0 ? 0 : 1);
            UpdateLabels();
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            if (_fadingOut)
            {
                GlobalState.black_overlay.ChangeAlpha(0.72f);

                if (GlobalState.black_overlay.alpha == 1.0)
                {
                    ChangeStateEvent(_isNewGame ? AnodyneSharp.AnodyneGame.GameState.Intro : AnodyneSharp.AnodyneGame.GameState.Game);
                }

                return;
            }

            if (_substate != null)
            {
                _substate.Update();

                if (_substate.Exit)
                {
                    _substate = null;
                    _hide = false;
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
            if (!_hide)
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
            }

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
                    case 4:
                        _substate = new ConnectionState(_apServer, _apSlot, _apPassword, OnConnected);
                        break;
                    case 5:
                        _substate = new BoxedConfigState();
                        _hide = true;
                        break;
                    case 6:
                        GlobalState.ClosingGame = true;
                        break;
                    default:
                        // Hi
                        break;
                }
            }
            else if (KeyInput.JustPressedRebindableKey(KeyFunctions.Left))
            {
                if (_selectorIndex < 3 && _curPage > 0)
                {
                    SoundManager.PlaySoundEffect("menu_move");

                    SetPage(_curPage - 1);
                }
            }
            else if (KeyInput.JustPressedRebindableKey(KeyFunctions.Right))
            {
                if (_selectorIndex < 3 && _curPage < _archipelagoSettings.ConnectionDetails.Count)
                {
                    SoundManager.PlaySoundEffect("menu_move");

                    SetPage(_curPage + 1);
                }
            }
        }

        private void SetPage(int index)
        {
            _curPage = index;

            if (index == 0)
            {
                _apServer = "";
                _apSlot = "";
                _apPassword = "";
            }
            else
            {
                ConnectionDetails details = _archipelagoSettings.ConnectionDetails[index - 1];
                _apServer = details.ApServer;
                _apSlot = details.ApSlot;
                _apPassword = details.ApPassword;
            }

            _connectionSwitcher.SetValue(index);
            UpdateLabels();
        }

        private void PageValueChanged(string value, int index)
        {
            SetPage(index);
        }

        private void OnConnected(ArchipelagoManager archipelagoManager)
        {
            _archipelagoSettings.AddConnection(new()
            {
                ApServer = _apServer,
                ApSlot = _apSlot,
                ApPassword = _apPassword
            });
            _archipelagoSettings.Save();

            Plugin.ArchipelagoManager = archipelagoManager;

            GlobalState.Save saveFile = GlobalState.Save.GetSave(string.Format("{0}Saves/Save_zzAP{1}_{2}.dat", GameConstants.SavePath, Plugin.ArchipelagoManager.GetSeed(), Plugin.ArchipelagoManager.GetPlayer()));

            GlobalState.ResetValues();
            if (saveFile != null)
            {
                GlobalState.LoadSave(saveFile);
                _isNewGame = false;
            }
            else
            {
                _isNewGame = true;
            }

            _fadingOut = true;
        }
    }
}
