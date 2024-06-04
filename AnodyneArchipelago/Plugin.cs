using AnodyneSharp.Entities;
using HarmonyLib;
using HarmonyLib.Tools;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace AnodyneArchipelago
{
    public class Plugin
    {
        public static Plugin Instance = null;

        public static Game Game = null;
        public static Player Player = null;
        public static ArchipelagoManager ArchipelagoManager = null;
        public static bool IsGamePaused = false;

        public const string Version = "0.2.0";

        public void Load()
        {
            Instance = this;

            // Make patches
            HarmonyFileLog.Enabled = true;
            HarmonyFileLog.FileWriterPath = "HarmonyLog.txt";

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
