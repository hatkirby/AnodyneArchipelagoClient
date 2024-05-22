using AnodyneSharp;
using AnodyneSharp.Entities;
using AnodyneSharp.Entities.Gadget;
using AnodyneSharp.Entities.Gadget.Treasures;
using BepInEx;
using BepInEx.NET.Common;
using HarmonyLib;
using HarmonyLib.Tools;
using System;
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
}
