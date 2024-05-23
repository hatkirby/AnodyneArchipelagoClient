using AnodyneSharp.Entities;
using System;
using System.Reflection;

namespace AnodyneArchipelago.Patches
{
    internal class PatchHelper
    {
        public static EntityPreset GetEntityPreset(Type type, object instance)
        {
            FieldInfo presetField = type.GetField("_preset", BindingFlags.NonPublic | BindingFlags.Instance);
            return presetField.GetValue(instance) as EntityPreset;
        }
    }
}
