using AnodyneSharp.Entities;
using AnodyneSharp.Registry;
using AnodyneSharp.States;
using AnodyneSharp;
using HarmonyLib;
using System.Reflection;
using AnodyneSharp.Resources;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using AnodyneSharp.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace AnodyneArchipelago.Patches
{
    [HarmonyPatch(typeof(AnodyneGame), "Initialize")]
    class AnodyneGameInitializePatch
    {
        static void Prefix(AnodyneGame __instance)
        {
            Plugin.Game = __instance;
        }
    }

    [HarmonyPatch(typeof(AnodyneGame), "Update")]
    class GameUpdatePatch
    {
        static void Postfix()
        {
            if (Plugin.ArchipelagoManager != null)
            {
                Plugin.ArchipelagoManager.Update();
            }
        }
    }

    [HarmonyPatch(typeof(GlobalState.Save), nameof(GlobalState.Save.SaveTo))]
    class SaveToPatch
    {
        static bool Prefix(GlobalState.Save __instance)
        {
            File.WriteAllText(string.Format("{0}Saves/Save_zzAP{1}_{2}.dat", GameConstants.SavePath, Plugin.ArchipelagoManager.GetSeed(), Plugin.ArchipelagoManager.GetPlayer()), __instance.ToString());

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayState), nameof(PlayState.Create))]
    class PlayStateCreatePatch
    {
        static void Prefix(PlayState __instance)
        {
            Plugin.IsGamePaused = false;

            // Get player for later access.
            FieldInfo playerField = typeof(PlayState).GetField("_player", BindingFlags.NonPublic | BindingFlags.Instance);
            Plugin.Player = (Player)playerField.GetValue(__instance);

            // Handle Red Cave stuff.
            if (!Plugin.ArchipelagoManager.VanillaRedCave)
            {
                GlobalState.events.SetEvent("red_cave_l_ss", 999);
                GlobalState.events.SetEvent("red_cave_n_ss", 999);
                GlobalState.events.SetEvent("red_cave_r_ss", 999);
            }

            // Reset death link info.
            Plugin.ArchipelagoManager.DeathLinkReason = null;

            // Pretend we're always in a pre-credits state so that swap is an allowlist, not a denylist.
            GlobalState.events.SetEvent("SeenCredits", 0);
        }
    }

    [HarmonyPatch(typeof(PauseState), MethodType.Constructor, new Type[] { })]
    class CreatePausePatch
    {
        static void Postfix()
        {
            Plugin.IsGamePaused = true;
        }
    }

    [HarmonyPatch(typeof(PauseState), nameof(PauseState.Update))]
    class UpdatePausePatch
    {
        static void Postfix(PauseState __instance)
        {
            if (__instance.Exit)
            {
                Plugin.IsGamePaused = false;
            }
        }
    }

    [HarmonyPatch(typeof(CreditsState), MethodType.Constructor, new Type[] {})]
    static class CreateCreditsPatch
    {
        static void Postfix()
        {
            Plugin.IsGamePaused = true;

            GlobalState.events.SetEvent("DefeatedBriar", 1);
            if (Plugin.ArchipelagoManager.PostgameMode == PostgameMode.Vanilla)
            {
                Plugin.ArchipelagoManager.EnableExtendedSwap();
            }

            if (Plugin.ArchipelagoManager.VictoryCondition == VictoryCondition.DefeatBriar)
            {
                Plugin.ArchipelagoManager.ActivateGoal();
            }
            else if (Plugin.ArchipelagoManager.VictoryCondition == VictoryCondition.AllCards)
            {
                Plugin.ArchipelagoManager.SendLocation("GO - Defeat Briar");
            }
        }
    }

    [HarmonyPatch(typeof(ResourceManager), nameof(ResourceManager.LoadResources))]
    static class LoadResourcesPatch
    {
        static void Postfix()
        {
            FileStream filestream = File.OpenRead($"{AppDomain.CurrentDomain.BaseDirectory}\\Resources\\archipelago.png");
            Texture2D apSprite = Texture2D.FromStream(Plugin.Game.GraphicsDevice, filestream);

            FieldInfo texturesField = typeof(ResourceManager).GetField("_textures", BindingFlags.NonPublic | BindingFlags.Static);
            Dictionary<string, Texture2D> textures = (Dictionary<string, Texture2D>)texturesField.GetValue(null);
            textures["archipelago"] = apSprite;
        }
    }

    [HarmonyPatch(typeof(DeathState), MethodType.Constructor, new Type[] {typeof(Player)})]
    static class DeathStateCtorPatch
    {
        static void Postfix(DeathState __instance)
        {
            if (Plugin.ArchipelagoManager.DeathLinkEnabled)
            {
                if (Plugin.ArchipelagoManager.ReceivedDeath)
                {
                    string message = Plugin.ArchipelagoManager.DeathLinkReason ?? "Received unknown death.";
                    message = Util.WordWrap(message, 20);

                    FieldInfo labelInfo = typeof(DeathState).GetField("_continueLabel", BindingFlags.NonPublic | BindingFlags.Instance);
                    UILabel label = (UILabel)labelInfo.GetValue(__instance);
                    label.SetText(message);
                    label.Position = new Vector2(8, 8);

                    Plugin.ArchipelagoManager.ReceivedDeath = false;
                    Plugin.ArchipelagoManager.DeathLinkReason = null;
                }
                else
                {
                    Plugin.ArchipelagoManager.SendDeath();
                }
            }
        }
    }
}
