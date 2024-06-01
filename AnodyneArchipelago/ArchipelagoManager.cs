using AnodyneSharp.Entities;
using AnodyneSharp.Entities.Gadget.Treasures;
using AnodyneSharp.Registry;
using AnodyneSharp.Sounds;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnodyneArchipelago
{
    public enum BigKeyShuffle
    {
        Vanilla = 0,
        Unlocked = 1,
        OwnWorld = 3,
        AnyWorld = 4,
        DifferentWorld = 5,
    }

    public class ArchipelagoManager
    {
        private ArchipelagoSession _session;
        private int _itemIndex = 0;
        private DeathLinkService _deathLinkService;

        private string _seedName;
        private long _endgameCardRequirement = 36;
        private ColorPuzzle _colorPuzzle = new();
        private bool _unlockSmallKeyGates = false;
        private BigKeyShuffle _bigKeyShuffle;
        private bool _vanillaHealthCicadas = false;
        private bool _vanillaRedCave = false;
        private bool _splitWindmill = false;

        private readonly Queue<NetworkItem> _itemsToCollect = new();
        private readonly Queue<string> _messages = new();
        private DeathLink? _pendingDeathLink = null;
        private string _deathLinkReason = null;
        private bool _receiveDeath = false;

        private Task<Dictionary<string, NetworkItem>> _scoutTask;

        public long EndgameCardRequirement => _endgameCardRequirement;
        public ColorPuzzle ColorPuzzle => _colorPuzzle;
        public bool UnlockSmallKeyGates => _unlockSmallKeyGates;
        public BigKeyShuffle BigKeyShuffle => _bigKeyShuffle;
        public bool VanillaHealthCicadas => _vanillaHealthCicadas;
        public bool VanillaRedCave => _vanillaRedCave;
        public bool SplitWindmill => _splitWindmill;

        public bool DeathLinkEnabled => _deathLinkService != null;

        public bool ReceivedDeath
        {
            get { return _receiveDeath; }
            set { _receiveDeath = value; }
        }

        public string DeathLinkReason
        {
            get { return _deathLinkReason; }
            set { _deathLinkReason = value; }
        }

        public async Task<LoginResult> Connect(string url, string slotName, string password)
        {
            LoginResult result;
            try
            {
                _session = ArchipelagoSessionFactory.CreateSession(url);
                _session.MessageLog.OnMessageReceived += OnMessageReceived;

                RoomInfoPacket roomInfoPacket = await _session.ConnectAsync();
                _seedName = roomInfoPacket.SeedName;

                result = await _session.LoginAsync("Anodyne", slotName, ItemsHandlingFlags.AllItems, null, null, null, password == "" ? null : password);
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            _itemIndex = 0;
            _itemsToCollect.Clear();

            LoginSuccessful login = result as LoginSuccessful;
            if (login.SlotData.ContainsKey("endgame_card_requirement"))
            {
                _endgameCardRequirement = (long)login.SlotData["endgame_card_requirement"];
            }

            if (login.SlotData.ContainsKey("seed"))
            {
                Random rand = new Random((int)(long)login.SlotData["seed"]);
                _colorPuzzle.Initialize(rand);
            }

            if (login.SlotData.ContainsKey("unlock_gates"))
            {
                _unlockSmallKeyGates = (bool)login.SlotData["unlock_gates"];
            }
            else
            {
                _unlockSmallKeyGates = false;
            }

            if (login.SlotData.ContainsKey("shuffle_big_gates"))
            {
                _bigKeyShuffle = (BigKeyShuffle)(long)login.SlotData["shuffle_big_gates"];
            }
            else
            {
                _bigKeyShuffle = BigKeyShuffle.AnyWorld;
            }

            if (login.SlotData.ContainsKey("vanilla_health_cicadas"))
            {
                _vanillaHealthCicadas = (bool)login.SlotData["vanilla_health_cicadas"];
            }
            else
            {
                _vanillaHealthCicadas = false;
            }

            if (login.SlotData.ContainsKey("vanilla_red_cave"))
            {
                _vanillaRedCave = (bool)login.SlotData["vanilla_red_cave"];
            }
            else
            {
                _vanillaRedCave = false;
            }

            if (login.SlotData.ContainsKey("split_windmill"))
            {
                _splitWindmill = (bool)login.SlotData["split_windmill"];
            }
            else
            {
                _splitWindmill = false;
            }

            if (login.SlotData.ContainsKey("death_link") && (bool)login.SlotData["death_link"])
            {
                _deathLinkReason = null;
                _receiveDeath = false;

                _deathLinkService = _session.CreateDeathLinkService();
                _deathLinkService.OnDeathLinkReceived += OnDeathLinkReceived;
                _deathLinkService.EnableDeathLink();
            }
            else
            {
                _deathLinkService = null;
            }

            _scoutTask = Task.Run(() => ScoutAllLocations());

            return result;
        }

        ~ArchipelagoManager()
        {
            Disconnect();
        }

        public void Disconnect()
        {
            if (_session == null)
            {
                return;
            }

            _session.Socket.DisconnectAsync();
            _session = null;
        }

        private async Task<Dictionary<string, NetworkItem>> ScoutAllLocations()
        {
            LocationInfoPacket locationInfo = await _session.Locations.ScoutLocationsAsync(_session.Locations.AllLocations.ToArray());

            Dictionary<string, NetworkItem> result = new();
            foreach (NetworkItem networkItem in locationInfo.Locations)
            {
                result[_session.Locations.GetLocationNameFromId(networkItem.Location)] = networkItem;
            }

            return result;
        }

        public NetworkItem? GetScoutedLocation(string locationName)
        {
            if (_scoutTask == null || !_scoutTask.IsCompleted || !_scoutTask.Result.ContainsKey(locationName))
            {
                return null;
            }

            return _scoutTask.Result[locationName];
        }

        public string GetItemName(long id)
        {
            return _session.Items.GetItemName(id);
        }

        public string GetSeed()
        {
            return _seedName;
        }

        public int GetPlayer()
        {
            return _session.ConnectionInfo.Slot;
        }

        public void SendLocation(string location)
        {
            if (_session == null)
            {
                Plugin.Instance.Log.LogError("Attempted to send location while disconnected");
                return;
            }

            _session.Locations.CompleteLocationChecks(_session.Locations.GetLocationIdFromName("Anodyne", location));
        }

        public void Update()
        {
            if (_session == null)
            {
                // We're not connected.
                return;
            }

            if (_session.Items.AllItemsReceived.Count > _itemIndex)
            {
                for (int i = _itemIndex; i < _session.Items.AllItemsReceived.Count; i++)
                {
                    string itemKey = $"ArchipelagoItem-{i}";
                    if (GlobalState.events.GetEvent(itemKey) != 0)
                    {
                        continue;
                    }

                    GlobalState.events.SetEvent(itemKey, 1);

                    NetworkItem item = _session.Items.AllItemsReceived[i];
                    _itemsToCollect.Enqueue(item);
                }

                _itemIndex = _session.Items.AllItemsReceived.Count;
            }

            if ((GlobalState.Dialogue == null || GlobalState.Dialogue == "") &&
                !GlobalState.ScreenTransition &&
                Plugin.Player != null &&
                GlobalState.black_overlay.alpha == 0f &&
                !Plugin.IsGamePaused)
            {
                if (_itemsToCollect.Count > 0)
                {
                    NetworkItem item = _itemsToCollect.Dequeue();
                    HandleItem(item);
                }
                else if (_messages.Count > 0)
                {
                    GlobalState.Dialogue = _messages.Dequeue();
                }
                else if (_pendingDeathLink != null)
                {
                    GlobalState.CUR_HEALTH = 0;

                    string message;
                    if (_pendingDeathLink.Cause == null)
                    {
                        message = $"Received death from {_pendingDeathLink.Source}.";
                    }
                    else
                    {
                        message = $"Received death. Cause: {_pendingDeathLink.Cause}";
                    }

                    _pendingDeathLink = null;
                    _deathLinkReason = message;
                    _receiveDeath = true;
                }
            }
        }

        private static string GetMapNameForDungeon(string dungeon)
        {
            switch (dungeon)
            {
                case "Temple of the Seeing One": return "BEDROOM";
                case "Apartment": return "APARTMENT";
                case "Mountain Cavern": return "CROWD";
                case "Hotel": return "HOTEL";
                case "Red Cave": return "REDCAVE";
                case "Circus": return "CIRCUS";
                default: return "STREET";
            }
        }

        private static int GetCardNumberForName(string name)
        {
            switch (name)
            {
                case "Edward": return 0;
                case "Annoyer": return 1;
                case "Seer": return 2;
                case "Shieldy": return 3;
                case "Slime": return 4;
                case "PewLaser": return 5;
                case "Suburbian": return 6;
                case "Watcher": return 7;
                case "Silverfish": return 8;
                case "Gas Guy": return 9;
                case "Mitra": return 10;
                case "Miao": return 11;
                case "Windmill": return 12;
                case "Mushroom": return 13;
                case "Dog": return 14;
                case "Rock": return 15;
                case "Fisherman": return 16;
                case "Walker": return 17;
                case "Mover": return 18;
                case "Slasher": return 19;
                case "Rogue": return 20;
                case "Chaser": return 21;
                case "Fire Pillar": return 22;
                case "Contorts": return 23;
                case "Lion": return 24;
                case "Arthur and Javiera": return 25;
                case "Frog": return 26;
                case "Person": return 27;
                case "Wall": return 28;
                case "Blue Cube King": return 29;
                case "Orange Cube King": return 30;
                case "Dust Maid": return 31;
                case "Dasher": return 32;
                case "Burst Plant": return 33;
                case "Manager": return 34;
                case "Sage": return 35;
                case "Young": return 36;
                case "Carved Rock": return 37;
                case "City Man": return 38;
                case "Intra": return 39;
                case "Torch": return 40;
                case "Triangle NPC": return 41;
                case "Killer": return 42;
                case "Goldman": return 43;
                case "Broom": return 44;
                case "Rank": return 45;
                case "Follower": return 46;
                case "Rock Creature": return 47;
                case "Null": return 48;
                default: return 0;
            }
        }

        private void HandleItem(NetworkItem item)
        {
            if (item.Player == _session.ConnectionInfo.Slot && item.Location >= 0)
            {
                string itemKey = $"ArchipelagoLocation-{item.Location}";
                if (GlobalState.events.GetEvent(itemKey) > 0)
                {
                    return;
                }

                GlobalState.events.SetEvent(itemKey, 1);
            }

            string itemName = _session.Items.GetItemName(item.Item);

            if (itemName.StartsWith("Small Key"))
            {
                string dungeonName = itemName.Substring(11);
                dungeonName = dungeonName.Substring(0, dungeonName.Length - 1);

                string mapName = GetMapNameForDungeon(dungeonName);
                GlobalState.inventory.AddMapKey(mapName, 1);
            }
            else if (itemName == "Green Key")
            {
                GlobalState.inventory.BigKeyStatus[0] = true;
            }
            else if (itemName == "Blue Key")
            {
                GlobalState.inventory.BigKeyStatus[2] = true;
            }
            else if (itemName == "Red Key")
            {
                GlobalState.inventory.BigKeyStatus[1] = true;
            }
            else if (itemName == "Jump Shoes")
            {
                GlobalState.inventory.CanJump = true;
            }
            else if (itemName == "Health Cicada")
            {
                GlobalState.MAX_HEALTH += 1;
                GlobalState.CUR_HEALTH = GlobalState.MAX_HEALTH;
            }
            else if (itemName == "Heal")
            {
                GlobalState.CUR_HEALTH = GlobalState.MAX_HEALTH;
            }
            else if (itemName == "Broom")
            {
                GlobalState.inventory.HasBroom = true;

                if (GlobalState.inventory.EquippedBroom == BroomType.NONE)
                {
                    GlobalState.inventory.EquippedBroom = BroomType.Normal;
                }
            }
            else if (itemName == "Swap")
            {
                GlobalState.inventory.HasTransformer = true;

                if (GlobalState.inventory.EquippedBroom == BroomType.NONE)
                {
                    GlobalState.inventory.EquippedBroom = BroomType.Transformer;
                }
            }
            else if (itemName == "Extend")
            {
                GlobalState.inventory.HasLengthen = true;

                if (GlobalState.inventory.EquippedBroom == BroomType.NONE)
                {
                    GlobalState.inventory.EquippedBroom = BroomType.Long;
                }
            }
            else if (itemName == "Widen")
            {
                GlobalState.inventory.HasWiden = true;

                if (GlobalState.inventory.EquippedBroom == BroomType.NONE)
                {
                    GlobalState.inventory.EquippedBroom = BroomType.Wide;
                }
            }
            else if (itemName == "Temple of the Seeing One Statue")
            {
                // TODO: This and the other two: move while on the same map.
                GlobalState.events.SetEvent("StatueMoved_Temple", 1);
            }
            else if (itemName == "Mountain Cavern Statue")
            {
                GlobalState.events.SetEvent("StatueMoved_Mountain", 1);
            }
            else if (itemName == "Red Cave Statue")
            {
                GlobalState.events.SetEvent("StatueMoved_Grotto", 1);
            }
            else if (itemName == "Progressive Red Cave")
            {
                GlobalState.events.IncEvent("ProgressiveRedGrotto");
            }
            else if (itemName.StartsWith("Card ("))
            {
                string cardName = itemName.Substring(6);
                cardName = cardName.Substring(0, cardName.Length - 1);

                int cardIndex = GetCardNumberForName(cardName);
                CardTreasure cardTreasure = new(Plugin.Player.Position, cardIndex);
                cardTreasure.GetTreasure();
                GlobalState.SpawnEntity(cardTreasure);
            }
            else if (itemName == "Cardboard Box")
            {
                GlobalState.events.SetEvent("ReceivedCardboardBox", 1);
            }
            else if (itemName == "Biking Shoes")
            {
                GlobalState.events.SetEvent("ReceivedBikingShoes", 1);
            }

            string message;
            if (item.Player == _session.ConnectionInfo.Slot)
            {
                message = $"Found {itemName}!";
            }
            else
            {
                string otherPlayer = _session.Players.GetPlayerAlias(item.Player);
                message = $"Received {itemName} from {otherPlayer}.";
            }

            GlobalState.Dialogue = message;
        }

        public void ActivateGoal()
        {
            var statusUpdatePacket = new StatusUpdatePacket
            {
                Status = ArchipelagoClientState.ClientGoal
            };
            _session.Socket.SendPacket(statusUpdatePacket);
        }

        private void OnMessageReceived(LogMessage message)
        {
            switch (message)
            {
                case ItemSendLogMessage itemSendLogMessage:
                    if (itemSendLogMessage.IsSenderTheActivePlayer && !itemSendLogMessage.IsReceiverTheActivePlayer)
                    {
                        string itemName = _session.Items.GetItemName(itemSendLogMessage.Item.Item);

                        string messageText;
                        string otherPlayer = _session.Players.GetPlayerAlias(itemSendLogMessage.Receiver.Slot);
                        messageText = $"Sent {itemName} to {otherPlayer}.";

                        SoundManager.PlaySoundEffect("gettreasure");
                        _messages.Enqueue(messageText);
                    }
                    break;
            }
        }

        public void SendDeath()
        {
            if (_deathLinkService != null)
            {
                string player = _session.Players.GetPlayerName(_session.ConnectionInfo.Slot);
                string reason = null;
                if (_deathLinkReason != null)
                {
                    reason = $"{player} {_deathLinkReason}";
                }

                _deathLinkService.SendDeathLink(new DeathLink(player, reason));
            }

            _deathLinkReason = null;
        }

        private void OnDeathLinkReceived(DeathLink deathLink)
        {
            _pendingDeathLink = deathLink;
        }
    }
}
