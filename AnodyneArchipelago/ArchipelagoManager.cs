using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using System;

namespace AnodyneArchipelago
{
    internal class ArchipelagoManager
    {
        private static ArchipelagoSession _session;

        public static void Connect(string url, string slotName, string password)
        {
            LoginResult result;
            try
            {
                _session = ArchipelagoSessionFactory.CreateSession(url);
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
        }
    }
}
