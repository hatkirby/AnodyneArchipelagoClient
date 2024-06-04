namespace AnodyneArchipelago
{
    internal class Functions
    {
        public static string GetClipboard()
        {
            return SDL2.SDL.SDL_GetClipboardText();
        }
    }
}
