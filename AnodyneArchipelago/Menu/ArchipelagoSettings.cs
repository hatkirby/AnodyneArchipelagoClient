using AnodyneSharp.Registry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AnodyneArchipelago.Menu
{
    internal class ConnectionDetails : IEquatable<ConnectionDetails>
    {
        public string ApServer;
        public string ApSlot;
        public string ApPassword;

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ConnectionDetails);
        }

        public bool Equals(ConnectionDetails other)
        {
            return (ApServer == other.ApServer) && (ApSlot == other.ApSlot) && (ApPassword == other.ApPassword);
        }

        public override int GetHashCode()
        {
            return (ApServer, ApSlot, ApPassword).GetHashCode();
        }
    }

    internal class ArchipelagoSettings
    {
        public static JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            IncludeFields = true
        };

        public List<ConnectionDetails> ConnectionDetails = new();

        public static string GetFilePath() => string.Format("{0}Saves/ap_settings.dat", GameConstants.SavePath);

        public static ArchipelagoSettings Load()
        {
            try
            {
                string s = File.ReadAllText(GetFilePath());
                return JsonSerializer.Deserialize<ArchipelagoSettings>(s, serializerOptions);
            } catch (Exception)
            {
                return null;
            }
        }

        public void Save()
        {
            File.WriteAllText(GetFilePath(), JsonSerializer.Serialize(this, serializerOptions));
        }

        public void AddConnection(ConnectionDetails connectionDetails)
        {
            if (ConnectionDetails.Contains(connectionDetails))
            {
                ConnectionDetails.Remove(connectionDetails);
            }

            ConnectionDetails.Insert(0, connectionDetails);

            if (ConnectionDetails.Count > 9)
            {
                ConnectionDetails.RemoveAt(ConnectionDetails.Count - 1);
            }
        }
    }
}
