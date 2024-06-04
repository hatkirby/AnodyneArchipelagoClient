using AnodyneArchipelago.Menu;
using AnodyneSharp.Registry;
using AnodyneSharp.States.MainMenu;
using HarmonyLib;

namespace AnodyneArchipelago.MonoGame
{
    [HarmonyPatch(typeof(MainMenuState), nameof(MainMenuState.Update))]
    class MainMenuUpdatePatch
    {
        static bool Prefix()
        {
            GlobalState.GameState.SetState<MenuState>();
            return false;
        }
    }
}
