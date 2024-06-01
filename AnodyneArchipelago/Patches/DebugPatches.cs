using AnodyneSharp.Entities;
using AnodyneSharp.Entities.Gadget;
using AnodyneSharp.Entities.Interactive;
using AnodyneSharp.Registry;
using HarmonyLib;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;

namespace AnodyneArchipelago.Patches
{
    /*
    [HarmonyPatch(typeof(TreasureChest), nameof(TreasureChest.PlayerInteraction))]
    class ChestInteractPatch
    {
        static void Prefix(TreasureChest __instance)
        {
            string entityId = PatchHelper.GetEntityPreset(typeof(TreasureChest), __instance).EntityID.ToString();
            Plugin.Instance.Log.LogInfo($"Touched chest: {entityId}");
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

            EntityPreset preset = PatchHelper.GetEntityPreset(typeof(HealthCicada), healthCicada);
            Plugin.Instance.Log.LogInfo($"Touched cicada: {preset.EntityID.ToString()}");
        }
    }

    [HarmonyPatch(typeof(Player), "Movement")]
    class PlayerMovementPatch
    {
        static void Postfix(Player __instance)
        {
            Point pos = GlobalState.Map.ToMapLoc(__instance.Position);
            Plugin.Instance.Log.LogInfo($"Player pos: {pos}, {GlobalState.Map.GetTile(AnodyneSharp.MapData.Layer.BG, pos)}");
        }
    }
    */
}
