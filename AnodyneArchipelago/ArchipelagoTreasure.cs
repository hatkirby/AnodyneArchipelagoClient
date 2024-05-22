using AnodyneSharp.Entities.Gadget.Treasures;
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
            base.GetTreasure();
            ArchipelagoManager.SendLocation(_location);
        }
    }
}
