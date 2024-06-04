using Microsoft.Xna.Framework;

namespace AnodyneArchipelago.Menu
{
    internal class TextEntry : BaseTextEntry
    {
        public TextEntry(string header, string value, CommitChange commitFunc) : base(header, value, commitFunc)
        {
            Plugin.Game.Window.TextInput += OnMonoGameTextInput;
        }

        private void OnMonoGameTextInput(object? sender, TextInputEventArgs e)
        {
            OnTextInput(e.Character);
        }

        public override void Update()
        {
            base.Update();

            if (this.Exit)
            {
                Plugin.Game.Window.TextInput -= OnMonoGameTextInput;
            }
        }
    }
}
