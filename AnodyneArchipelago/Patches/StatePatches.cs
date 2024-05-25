using AnodyneSharp.Entities;
using AnodyneSharp.Registry;
using AnodyneSharp.States;
using AnodyneSharp;
using HarmonyLib;
using System.Reflection;
using AnodyneSharp.Drawing.Effects;
using AnodyneSharp.States.MainMenu;
using static AnodyneSharp.AnodyneGame;
using AnodyneSharp.Drawing;

namespace AnodyneArchipelago.Patches
{
    [HarmonyPatch(typeof(AnodyneGame), "Update")]
    class GameUpdatePatch
    {
        static void Postfix()
        {
            ArchipelagoManager.Update();
        }
    }

    [HarmonyPatch(typeof(AnodyneGame), "SetState")]
    class SetStatePatch
    {
        // Pretty much rewrite this whole method, so that we can swap out some states.
        static bool Prefix(AnodyneGame __instance, GameState state)
        {
            foreach (IFullScreenEffect effect in GlobalState.AllEffects)
            {
                effect.Deactivate();
            }

            State new_state = CreateState(__instance, state);

            if (new_state != null)
            {
                new_state.Create();

                MethodInfo setStateMethod = typeof(AnodyneGame).GetMethod("SetState", BindingFlags.NonPublic | BindingFlags.Instance);
                new_state.ChangeStateEvent = (ChangeState)setStateMethod.CreateDelegate(typeof(ChangeState), __instance);
            }

            FieldInfo stateField = typeof(AnodyneGame).GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            stateField.SetValue(__instance, new_state);

            return false;
        }

        static State CreateState(AnodyneGame __instance, GameState state)
        {
            switch (state)
            {
                case GameState.TitleScreen: return new TitleState();
                case GameState.MainMenu: return new Menu.MenuState();
                case GameState.Intro: return new IntroState();
                case GameState.Game:
                    FieldInfo cameraField = typeof(AnodyneGame).GetField("_camera", BindingFlags.NonPublic | BindingFlags.Instance); 
                    return new PlayState((Camera)cameraField.GetValue(__instance));
                case GameState.Credits: return new CreditsState();
                default: return null;
            }
        }
    }

    [HarmonyPatch(typeof(PlayState), nameof(PlayState.Create))]
    class PlayStateCreatePatch
    {
        static void Prefix(PlayState __instance)
        {
            // Get player for later access.
            FieldInfo playerField = typeof(PlayState).GetField("_player", BindingFlags.NonPublic | BindingFlags.Instance);
            Plugin.Player = (Player)playerField.GetValue(__instance);

            // Handle Red Grotto stuff.
            GlobalState.events.SetEvent("red_cave_l_ss", 999);
            GlobalState.events.SetEvent("red_cave_n_ss", 999);
            GlobalState.events.SetEvent("red_cave_r_ss", 999);

            // Connect to archipelago.
            Plugin.Instance.Log.LogInfo("Connecting to Archipelago!");

            ArchipelagoManager.Connect("localhost:38281", "Anodyne", "");
        }
    }
}
