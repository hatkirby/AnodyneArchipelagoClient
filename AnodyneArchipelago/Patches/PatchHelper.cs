using AnodyneSharp.Entities;
using AnodyneSharp.MapData;
using AnodyneSharp.Registry;
using System;
using System.Collections.Generic;
using System.Reflection;
using Layer = AnodyneSharp.MapData.Layer;

namespace AnodyneArchipelago.Patches
{
    internal class PatchHelper
    {
        public static EntityPreset GetEntityPreset(Type type, object instance)
        {
            FieldInfo presetField = type.GetField("_preset", BindingFlags.NonPublic | BindingFlags.Instance);
            return presetField.GetValue(instance) as EntityPreset;
        }

        public static void SetMapTile(int x, int y, int value, Layer layer)
        {
            FieldInfo layersField = typeof(Map).GetField("mapLayers", BindingFlags.NonPublic | BindingFlags.Instance);
            TileMap[] mapLayers = (TileMap[])layersField.GetValue(GlobalState.Map);
            TileMap mapLayer = mapLayers[(int)layer];

            FieldInfo tilesField = typeof(TileMap).GetField("tiles", BindingFlags.NonPublic | BindingFlags.Instance);
            List<int> tiles = (List<int>)tilesField.GetValue(mapLayer);

            tiles[x + y * mapLayer.Width] = value;
        }
    }
}
