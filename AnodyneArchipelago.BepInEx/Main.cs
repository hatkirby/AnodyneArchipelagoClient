using BepInEx;
using BepInEx.NET.Common;

namespace AnodyneArchipelago.BepInEx
{
    [BepInPlugin("com.fourisland.plugins.anodyne.archipelago", "Anodyne Archipelago", Plugin.Version)]
    public class Main : BasePlugin
    {
        public override void Load()
        {
            Plugin plugin = new();
            plugin.Load();

            // Plugin startup logic
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
