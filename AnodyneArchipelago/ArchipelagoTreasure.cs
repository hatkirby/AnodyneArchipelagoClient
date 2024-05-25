using AnodyneSharp.Entities;
using AnodyneSharp.Entities.Gadget.Treasures;
using AnodyneSharp.Registry;
using Microsoft.Xna.Framework;

namespace AnodyneArchipelago
{
    internal class ArchipelagoTreasure : Treasure
    {
        private string _location;

        public ArchipelagoTreasure(string location, Vector2 pos) : base("item_jump_shoes", pos, 0, -1)
        {
            _location = location;
        }

        public override void GetTreasure()
        {
            if (_location == "Street - Broom Chest")
            {
                BroomTreasure broomTreasure = new("broom-icon", this.Position, BroomType.Normal);
                broomTreasure.GetTreasure();
                GlobalState.SpawnEntity(broomTreasure);
            }
            else if (_location == "Street - Key Chest")
            {
                KeyTreasure keyTreasure = new(this.Position);
                keyTreasure.GetTreasure();
                GlobalState.SpawnEntity(keyTreasure);
            }
            else
            {
                base.GetTreasure();
            }
            
            Plugin.ArchipelagoManager.SendLocation(_location);
        }
    }
}
