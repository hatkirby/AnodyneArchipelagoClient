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
using System.IO;
using System;
using AnodyneSharp.Resources;
using BepInEx;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using AnodyneSharp.UI;

namespace AnodyneArchipelago.Patches
{
    [HarmonyPatch(typeof(AnodyneGame), "LoadContent")]
    class AnodyneLoadContentPatch
    {
        static void Prefix(AnodyneGame __instance)
        {
            FieldInfo gdmField = typeof(AnodyneGame).GetField("graphics", BindingFlags.NonPublic | BindingFlags.Instance);
            Plugin.GraphicsDevice = ((GraphicsDeviceManager)gdmField.GetValue(__instance)).GraphicsDevice;
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

            Plugin.ArchipelagoManager.DeathLinkReason = null;
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

            if (Plugin.ArchipelagoManager.VictoryCondition == VictoryCondition.AllBosses)
            {
                Plugin.ArchipelagoManager.ActivateGoal();
            }
        }
    }

    [HarmonyPatch(typeof(ResourceManager), nameof(ResourceManager.LoadResources))]
    static class LoadResourcesPatch
    {
        static void Postfix()
        {
            FileStream filestream = File.OpenRead($"{Paths.GameRootPath}\\Resources\\archipelago.png");
            Texture2D apSprite = Texture2D.FromStream(Plugin.GraphicsDevice, filestream);

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
