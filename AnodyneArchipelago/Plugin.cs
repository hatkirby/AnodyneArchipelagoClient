using AnodyneSharp;
using AnodyneSharp.Entities;
using AnodyneSharp.Entities.Events;
using AnodyneSharp.Entities.Gadget;
using AnodyneSharp.Entities.Gadget.Treasures;
using AnodyneSharp.Entities.Interactive;
using AnodyneSharp.Registry;
using AnodyneSharp.Utilities;
using BepInEx;
using BepInEx.NET.Common;
using HarmonyLib;
using HarmonyLib.Tools;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace AnodyneArchipelago
{
    [BepInPlugin("com.fourisland.plugins.anodyne.archipelago", "Anodyne Archipelago", "1.0.0.0")]
    public class Plugin : BasePlugin
    {
        public static Plugin Instance = null;

        public override void Load()
        {
            Instance = this;

            // Plugin startup logic
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            // Make patches
            HarmonyFileLog.Enabled = true;
            HarmonyFileLog.FileWriterPath = "HarmonyLog.txt";

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(AnodyneGame), "Update")]
    class GameUpdatePatch
    {
        static void Postfix()
        {
            ArchipelagoManager.Update();
        }
    }

    [HarmonyPatch(typeof(AnodyneSharp.States.PlayState), nameof(AnodyneSharp.States.PlayState.Create))]
    class PlayStateCreatePatch
    {
        static void Prefix()
        {
            // Handle Red Grotto stuff.
            GlobalState.events.SetEvent("red_cave_l_ss", 999);
            GlobalState.events.SetEvent("red_cave_n_ss", 999);
            GlobalState.events.SetEvent("red_cave_r_ss", 999);

            // Connect to archipelago.
            Plugin.Instance.Log.LogInfo("Connecting to Archipelago!");

            ArchipelagoManager.Connect("localhost:38281", "Anodyne", "");
        }
    }

    [HarmonyPatch(typeof(TreasureChest), nameof(TreasureChest.PlayerInteraction))]
    class ChestInteractPatch
    {
        static void Prefix(TreasureChest __instance)
        {
            Type chestType = typeof(TreasureChest);
            FieldInfo presetField = chestType.GetField("_preset", BindingFlags.NonPublic | BindingFlags.Instance);
            EntityPreset preset = presetField.GetValue(__instance) as EntityPreset;
            Plugin.Instance.Log.LogInfo($"Touched chest: {preset.EntityID.ToString()}");
        }
    }

    [HarmonyPatch(typeof(TreasureChest), "SetTreasure")]
    class ChestSetTreasurePatch
    {
        static bool Prefix(TreasureChest __instance)
        {
            Type chestType = typeof(TreasureChest);
            FieldInfo presetField = chestType.GetField("_preset", BindingFlags.NonPublic | BindingFlags.Instance);
            EntityPreset preset = presetField.GetValue(__instance) as EntityPreset;

            if (Locations.LocationsByGuid.ContainsKey(preset.EntityID))
            {
                BaseTreasure treasure = new ArchipelagoTreasure(Locations.LocationsByGuid[preset.EntityID], __instance.Position);

                FieldInfo treasureField = chestType.GetField("_treasure", BindingFlags.NonPublic | BindingFlags.Instance);
                treasureField.SetValue(__instance, treasure);

                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Big_Key), nameof(Big_Key.PlayerInteraction))]
    class BigKeyTouchPatch
    {
        // We basically just rewrite this method, because we need to get rid of the part that adds the key to the inventory.
        static bool Prefix(Big_Key __instance, ref bool __result)
        {
            Type keyType = typeof(Big_Key);
            FieldInfo presetField = keyType.GetField("_preset", BindingFlags.NonPublic | BindingFlags.Instance);
            EntityPreset preset = presetField.GetValue(__instance) as EntityPreset;

            MethodInfo statesMethod = keyType.GetMethod("States", BindingFlags.NonPublic | BindingFlags.Instance);

            preset.Alive = false;
            __instance.Solid = false;
            GlobalState.StartCutscene = (System.Collections.Generic.IEnumerator<AnodyneSharp.States.CutsceneState.CutsceneEvent>)statesMethod.Invoke(__instance, new object[] { });
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(Big_Key), "States")]
    class BigKeyStatesPatch
    {
        static void Postfix(Big_Key __instance)
        {
            Type keyType = typeof(Big_Key);
            FieldInfo presetField = keyType.GetField("_preset", BindingFlags.NonPublic | BindingFlags.Instance);
            EntityPreset preset = presetField.GetValue(__instance) as EntityPreset;

            if (preset.Frame == 0)
            {
                ArchipelagoManager.SendLocation("Temple of the Seeing One - Green Key");
            }
            else if (preset.Frame == 1)
            {
                ArchipelagoManager.SendLocation("Red Grotto - Red Key");
            }
            else if (preset.Frame == 2)
            {
                ArchipelagoManager.SendLocation("Mountain Cavern - Blue Key");
            }
        }
    }

    [HarmonyPatch(typeof(HealthCicadaSentinel), nameof(HealthCicadaSentinel.Collided))]
    class HealthCicadaInteractPatch
    {
        static void Prefix(TreasureChest __instance)
        {
            Type hcsType = typeof(HealthCicadaSentinel);
            FieldInfo childField = hcsType.GetField("_child", BindingFlags.NonPublic | BindingFlags.Instance);
            HealthCicada healthCicada = (HealthCicada)childField.GetValue(__instance);

            Type cicadaType = typeof(HealthCicada);
            FieldInfo presetField = cicadaType.GetField("_preset", BindingFlags.NonPublic | BindingFlags.Instance);
            EntityPreset preset = presetField.GetValue(healthCicada) as EntityPreset;
            Plugin.Instance.Log.LogInfo($"Touched cicada: {preset.EntityID.ToString()}");
        }
    }

    [HarmonyPatch(typeof(HealthCicada), nameof(HealthCicada.Update))]
    class HealthCicadaUpdatePatch
    {
        static void Postfix(HealthCicada __instance)
        {
            Type cicadaType = typeof(HealthCicada);
            FieldInfo chirpField = cicadaType.GetField("_chirp", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo sentinelField = cicadaType.GetField("_sentinel", BindingFlags.NonPublic | BindingFlags.Instance);

            HealthCicadaSentinel sentinel = (HealthCicadaSentinel)sentinelField.GetValue(__instance);
            Type hcsType = typeof(HealthCicadaSentinel);
            FieldInfo flyDistanceField = hcsType.GetField("_flyDistance", BindingFlags.NonPublic | BindingFlags.Instance);
            float flyDistance = (float)flyDistanceField.GetValue(sentinel);

            if (__instance.visible && !(bool)chirpField.GetValue(__instance) && flyDistance > 0)
            {
                flyDistanceField.SetValue(sentinel, 0f);

                FieldInfo stateField = cicadaType.GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance);
                stateField.SetValue(__instance, StateLogic(__instance));
            }
        }

        static IEnumerator<string> StateLogic(HealthCicada healthCicada)
        {
            Type cicadaType = typeof(HealthCicada);
            FieldInfo presetField = cicadaType.GetField("_preset", BindingFlags.NonPublic | BindingFlags.Instance);
            EntityPreset preset = presetField.GetValue(healthCicada) as EntityPreset;

            while (!MathUtilities.MoveTo(ref healthCicada.opacity, 0.0f, 0.6f))
                yield return "FadingOut";
            preset.Alive = false;

            FieldInfo sentinelField = cicadaType.GetField("_sentinel", BindingFlags.NonPublic | BindingFlags.Instance);
            HealthCicadaSentinel sentinel = (HealthCicadaSentinel)sentinelField.GetValue(healthCicada);

            sentinel.exists = false;

            if (Locations.LocationsByGuid.ContainsKey(preset.EntityID))
            {
                ArchipelagoManager.SendLocation(Locations.LocationsByGuid[preset.EntityID]);
            }
        }
    }

    [HarmonyPatch(typeof(EntityPreset), nameof(EntityPreset.Create))]
    class EntityPresetCreatePatch
    {
        static void Postfix(EntityPreset __instance, Entity __result)
        {
            if (__instance.Type.FullName == "AnodyneSharp.Entities.Interactive.DungeonStatue")
            {
                __result.Position = __instance.Position;

                string eventName = "StatueMoved_";
                Facing moveDir = Facing.RIGHT;
                if (__instance.Frame == 0)
                {
                    eventName += "Temple";
                    moveDir = Facing.UP;
                }
                else if (__instance.Frame == 1)
                {
                    eventName += "Grotto";
                }
                else if (__instance.Frame == 2)
                {
                    eventName += "Mountain";
                }

                if (GlobalState.events.GetEvent(eventName) == 0)
                {
                    return;
                }

                __result.Position += Entity.FacingDirection(moveDir) * 32f;
            }
            else if (__instance.Type.FullName.StartsWith("AnodyneSharp.Entities.Decorations.RedCave"))
            {
                string side = __instance.Type.FullName.Substring(41);
                int requiredGrottos = 0;
                if (side == "Left")
                {
                    requiredGrottos = 1;
                }
                else if (side == "Right")
                {
                    requiredGrottos = 2;
                }
                else if (side == "North")
                {
                    requiredGrottos = 3;
                }

                if (GlobalState.events.GetEvent("ProgressiveRedGrotto") < requiredGrottos)
                {
                    __result.exists = false;
                    GlobalState.SpawnEntity((Entity)new DoorToggle(__result.Position, __result.width, __result.height));
                }
            }
        }
    }
}
