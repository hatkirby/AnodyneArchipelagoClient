using BepInEx;
using BepInEx.NET.Common;

namespace AnodyneArchipelago
{
    [BepInPlugin("com.fourisland.plugins.anodyne.archipelago", "Anodyne Archipelago", "1.0.0.0")]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            // Plugin startup logic
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
