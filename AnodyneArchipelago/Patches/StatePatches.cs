using AnodyneSharp.Entities;
using AnodyneSharp.Registry;
using AnodyneSharp.States;
using AnodyneSharp;
using HarmonyLib;
using System.Reflection;

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
