using AmongUs.Data;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using InnerNet;

namespace TownOfUsReworked.Patches
{
    //Thanks to The Other Roles for this code
    [HarmonyPatch]
    public static class DynamicLobbies
    {
        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.HostGame))]
        public static class InnerNetClientHostPatch
        {
            public static void Prefix(InnerNet.InnerNetClient __instance, [HarmonyArgument(0)] GameOptionsData settings)
            {
                int maxPlayers;

                try
                {
                    maxPlayers = GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers;
                }
                catch
                {
                    maxPlayers = 127;
                }

                ChatCommands.LobbyLimit = maxPlayers;
                settings.MaxPlayers = 127; // Force 127 Player Lobby on Server
                DataManager.Settings.Multiplayer.ChatMode = InnerNet.QuickChatModes.FreeChatOrQuickChat;
            }

            public static void Postfix(InnerNet.InnerNetClient __instance, [HarmonyArgument(0)] GameOptionsData settings) => settings.MaxPlayers = ChatCommands.LobbyLimit;
        }

        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.JoinGame))]
        public static class InnerNetClientJoinPatch
        {
            public static void Prefix(InnerNet.InnerNetClient __instance) => DataManager.Settings.Multiplayer.ChatMode = InnerNet.QuickChatModes.FreeChatOrQuickChat;
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
        public static class AmongUsClientOnPlayerJoined
        {
            public static bool Prefix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
            {
                if (ChatCommands.LobbyLimit < __instance.allClients.Count)
                { // TODO: Fix this canceling start
                    DisconnectPlayer(__instance, client.Id);
                    return false;
                }

                return true;
            }

            private static void DisconnectPlayer(InnerNetClient _this, int clientId)
            {
                if (!_this.AmHost)
                    return;

                var messageWriter = MessageWriter.Get(SendOption.Reliable);
                messageWriter.StartMessage(4);
                messageWriter.Write(_this.GameId);
                messageWriter.WritePacked(clientId);
                messageWriter.Write((byte)DisconnectReasons.GameFull);
                messageWriter.EndMessage();
                _this.SendOrDisconnect(messageWriter);
                messageWriter.Recycle();
            }
        }
    }
}