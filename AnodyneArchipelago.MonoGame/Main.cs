namespace AnodyneArchipelago.MonoGame
{
    public class Main : AnodyneSharp.Modding.IMod
    {
        public Main()
        {
            Plugin plugin = new();
            plugin.Load();
        }
    }
}
