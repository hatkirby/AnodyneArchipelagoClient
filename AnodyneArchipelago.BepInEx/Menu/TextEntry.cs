using Microsoft.Xna.Framework.Input;

namespace AnodyneArchipelago.Menu
{
    internal class TextEntry : BaseTextEntry
    {
        public TextEntry(string header, string value, CommitChange commitFunc) : base(header, value, commitFunc)
        {
            TextInputEXT.TextInput += OnTextInput;
            TextInputEXT.StartTextInput();
        }

        public override void Update()
        {
            base.Update();

            if (this.Exit)
            {
                TextInputEXT.StopTextInput();
                TextInputEXT.TextInput -= OnTextInput;
            }
        }
    }
}
