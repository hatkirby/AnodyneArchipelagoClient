using BepInEx;
using BepInEx.NET.Common;
using HarmonyLib;
using HarmonyLib.Tools;
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

    [HarmonyPatch(typeof(AnodyneSharp.States.PlayState), nameof(AnodyneSharp.States.PlayState.Create))]
    class PlayStateCreatePatch
    {
        static void Prefix()
        {
            Plugin.Instance.Log.LogInfo("Connecting to Archipelago!");

            ArchipelagoManager.Connect("localhost:38281", "Anodyne", "");
        }
    }
}
