using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AnodyneArchipelago
{
    public class SwapData
    {
        public static List<Rectangle> GetRectanglesForMap(string mapName, bool extendedSwap)
        {
            if (extendedSwap)
            {
                switch (mapName)
                {
                    case "APARTMENT": return new() { new Rectangle(1280, 1120, 161, 161) };
                    case "BEACH": return new() {
                        new Rectangle(336, 160, 48, 48),     // Secret glen
                        new Rectangle(0, 768, 48, 48),       // Left edge, to get out of bounds
                        new Rectangle(688, 1072, 48, 64),    // Bottom edge, to get to secret chest
                    };
                    case "BLANK": return new() {
                        new Rectangle(0, 0, 480, 960),       // Left half
                        new Rectangle(640, 0, 320, 1120),    // Right half
                    };
                    case "CELL": return new() { new Rectangle(160, 320, 160, 160) };
                    case "CIRCUS": return new() { new Rectangle(1120, 0, 161, 161) };
                    case "DRAWER": return new() { new Rectangle(0, 0, 960, 1440) };
                    case "FIELDS": return new() {
                        new Rectangle(208, 192, 176, 112),   // Near terminal secret chest
                        new Rectangle(736, 336, 208, 144),   // Near overworld secret chest
                        new Rectangle(1488, 1120, 256, 160), // Secret glen
                        new Rectangle(1296, 1600, 128, 160), // Blocked river 1
                        new Rectangle(1648, 1488, 112, 96),  // Blocked river 2
                    };
                    case "FOREST": return new() { new Rectangle(0, 0, 800, 1440) };
                    case "GO": return new() {
                        new Rectangle(352, 496, 96, 112),    // Color puzzle
                        new Rectangle(32, 656, 208, 128),    // Secret color puzzle
                    };
                    case "HOTEL": return new() {
                        new Rectangle(1280, 1760, 161, 161), // Post-boss room
                        new Rectangle(480, 80, 160, 48),     // Roof secret
                    };
                    case "OVERWORLD": return new() { new Rectangle(16, 480, 272, 176) };
                    case "REDSEA": return new() { new Rectangle(160, 800, 160, 160) };
                    case "SUBURB": return new() { new Rectangle(320, 640, 160, 160) };
                    case "STREET": return new() { new Rectangle(160, 864, 160, 160) };
                    case "SPACE": return new() { new Rectangle(800, 640, 160, 160) };
                    case "WINDMILL": return new() { new Rectangle(224, 1216, 96, 48) };
                }
            }
            else
            {
                switch (mapName)
                {
                    case "APARTMENT": return new() { new Rectangle(1280, 1120, 161, 161) };
                    case "CIRCUS": return new() { new Rectangle(1120, 0, 161, 161) };
                    case "GO": return new() { new Rectangle(352, 496, 96, 112) };
                    case "HOTEL": return new() { new Rectangle(1280, 1760, 161, 161) };
                }
            }

            return new();
        }
    }
}
