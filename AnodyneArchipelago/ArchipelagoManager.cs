using AnodyneSharp.Entities;
using AnodyneSharp.Entities.Enemy.Redcave;
using AnodyneSharp.Entities.Gadget.Treasures;
using AnodyneSharp.Registry;
using AnodyneSharp.Sounds;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnodyneArchipelago
{
    internal class ArchipelagoManager
    {
        private static ArchipelagoSession _session;
        private static int _itemIndex = 0;

        private static readonly Queue<NetworkItem> _itemsToCollect = new();

        public static void Connect(string url, string slotName, string password)
        {
            LoginResult result;
            try
            {
                _session = ArchipelagoSessionFactory.CreateSession(url);
                _session.MessageLog.OnMessageReceived += OnMessageReceived;
                result = _session.TryConnectAndLogin("Anodyne", slotName, ItemsHandlingFlags.AllItems, null, null, null, password == "" ? null : password);
            }
            catch (Exception e)
            {
                result = new LoginFailure(e.GetBaseException().Message);
            }

            if (!result.Successful)
            {
                LoginFailure failure = result as LoginFailure;
                string errorMessage = $"Failed to connect to {url} as {slotName}:";
                foreach (string error in failure.Errors)
                {
                    errorMessage += $"\n    {error}";
                }
                foreach (ConnectionRefusedError error in failure.ErrorCodes)
                {
                    errorMessage += $"\n    {error}";
                }

                Plugin.Instance.Log.LogError(errorMessage);

                return;
            }

            _itemIndex = 0;
            _itemsToCollect.Clear();
        }

        public static void Disconnect()
        {
            if (_session == null)
            {
                return;
            }

            _session.Socket.DisconnectAsync();
            _session = null;
        }

        public static void SendLocation(string location)
        {
            if (_session == null)
            {
                Plugin.Instance.Log.LogError("Attempted to send location while disconnected");
                return;
            }

            _session.Locations.CompleteLocationChecks(_session.Locations.GetLocationIdFromName("Anodyne", location));
        }

        public static void Update()
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
            }

            if (_itemsToCollect.Count > 0 && (GlobalState.Dialogue == null || GlobalState.Dialogue == "") && !GlobalState.ScreenTransition)
            {
                NetworkItem item = _itemsToCollect.Dequeue();
                HandleItem(item);
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
                case "Red Grotto": return "REDCAVE";
                case "Circus": return "CIRCUS";
                default: return "STREET";
            }
        }

        private static void HandleItem(NetworkItem item)
        {
            if (item.Player == _session.ConnectionInfo.Slot)
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
            else if (itemName == "Swap")
            {
                GlobalState.inventory.HasTransformer = true;
            }
            else if (itemName == "Extend")
            {
                GlobalState.inventory.HasLengthen = true;
            }
            else if (itemName == "Widen")
            {
                GlobalState.inventory.HasWiden = true;
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
            else if (itemName == "Red Grotto Statue")
            {
                GlobalState.events.SetEvent("StatueMoved_Grotto", 1);
            }
            else if (itemName == "Progressive Red Grotto")
            {
                GlobalState.events.IncEvent("ProgressiveRedGrotto");
            }
            else if (itemName == "Card")
            {
                CardTreasure cardTreasure = new(Plugin.Player.Position, GlobalState.inventory.CardCount);
                cardTreasure.GetTreasure();
                GlobalState.SpawnEntity(cardTreasure);
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

        private static void OnMessageReceived(LogMessage message)
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
                        GlobalState.Dialogue = messageText;
                    }
                    break;
            }
        }
    }
}
