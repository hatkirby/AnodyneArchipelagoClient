using AnodyneSharp.Entities;
using AnodyneSharp.Entities.Gadget.Treasures;
using AnodyneSharp.Registry;
using Archipelago.MultiClient.Net.Models;
using Microsoft.Xna.Framework;

namespace AnodyneArchipelago
{
    internal class ArchipelagoTreasure : Treasure
    {
        private string _location;

        private static (string, int) GetSprite(string location)
        {
            NetworkItem? item = Plugin.ArchipelagoManager.GetScoutedLocation(location);
            if (item == null)
            {
                return ("archipelago", 0);
            }

            if (item?.Player != Plugin.ArchipelagoManager.GetPlayer())
            {
                return ("archipelago", 0);
            }

            string itemName = Plugin.ArchipelagoManager.GetItemName(item?.Item ?? 0);
            if (itemName.StartsWith("Small Key"))
            {
                return ("key", 0);
            }
            else if (itemName == "Green Key")
            {
                return ("key_green", 0);
            }
            else if (itemName == "Blue Key")
            {
                return ("key_green", 4);
            }
            else if (itemName == "Red Key")
            {
                return ("key_green", 2);
            }
            else if (itemName == "Jump Shoes")
            {
                return ("item_jump_shoes", 0);
            }
            else if (itemName == "Health Cicada")
            {
                return ("life_cicada", 0);
            }
            else if (itemName == "Heal")
            {
                return ("small_health_pickup", 0);
            }
            else if (itemName == "Broom")
            {
                return ("broom-icon", 0);
            }
            else if (itemName == "Swap")
            {
                return ("item_tranformer", 0);
            }
            else if (itemName == "Extend")
            {
                return ("item_long_attack", 0);
            }
            else if (itemName == "Widen")
            {
                return ("item_wide_attack", 0);
            }
            else if (itemName == "Cardboard Box")
            {
                return ("fields_npcs", 31);
            }
            else if (itemName == "Biking Shoes")
            {
                return ("item_jump_shoes", 0);
            }

            return ("archipelago", 0);
        }

        public static ArchipelagoTreasure Create(string location, Vector2 pos)
        {
            (string textureName, int frame) = GetSprite(location);

            return new(location, pos, textureName, frame);
        }

        private ArchipelagoTreasure(string location, Vector2 pos, string textureName, int frame) : base(textureName, pos, frame, -1)
        {
            _location = location;
        }

        public override void GetTreasure()
        {
            base.GetTreasure();

            Plugin.ArchipelagoManager.SendLocation(_location);
        }
    }
}
