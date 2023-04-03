using System;
using System.Linq;
using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod;
using InnerNet;
using TownOfUsReworked.Classes;
using Hazel;
using TownOfUsReworked.Data;
using System.Collections.Generic;
using UnityEngine;
using AmongUs.Data;
using AmongUs.GameOptions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class ChatCommands
    {
        private readonly static List<string> ChatHistory = new();
        private static int LobbyLimit = 127;

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
        public static class SendChatPatch
        {
            public static bool Prefix()
            {
                var hudManager = HudManager.Instance.Chat;
                var text = hudManager.TextArea.text;
                var otherText = text;
                text = text.ToLower();
                var inputText = "";
                var chatText = "";
                var chatHandled = false;
                var EatNeed = CustomGameOptions.CannibalBodyCount >= PlayerControl.AllPlayerControls._size / 2 ? PlayerControl.AllPlayerControls._size / 2 :
                    CustomGameOptions.CannibalBodyCount;
                var getWhat = CustomGameOptions.ConsigInfo == ConsigInfo.Role ? "role" : "faction";
                var setColor = TownOfUsReworked.isTest ? ", /setcolour or /setcolor" : "";
                var whisper = CustomGameOptions.Whispers ? ", /whisper" : "";
                var kickBan = AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan() ? ", /kick, /ban, /clearlobby, /size" : "";

                var player = PlayerControl.LocalPlayer;

                if (ChatHistory.Count == 0 || ChatHistory[^1] != text)
                    ChatHistory.Add(text);

                //Help command - lists commands available
                if (text.StartsWith("/help") || text == "/h" || text == "/h ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Commands available all the time:\n/modinfo /roleinfo, /modifierinfo, /abilityinfo, /objectifierinfo, /factioninfo, /alignmentinfo," +
                        " /quote, /credits, /controls, /lore");
                    hudManager.AddChat(player, $"Commands available in lobby:\n/setname{setColor}{kickBan}");
                    hudManager.AddChat(player, $"Commands available in game:\n/mystate{whisper}");
                }
                //Display a message (Information about the mod)
                else if (text.StartsWith("/modinfo") || text.StartsWith("/mi"))
                {
                    chatHandled = true;
                    hudManager.AddChat(player, $"Welcome to Town Of Us Reworked {TownOfUsReworked.versionFinal}!\nTown Of Us Reworked is essentially a weird mishmash of code from Town " +
                        "Of Us Reactivated and its forks plus some of my own code.\nCredits to the parties have already been given (good luck to those who want to try to cancel me for " +
                        "no reason). This mod has several reworks and additions which I believe fit the mod better. Plus, the more layers there are, the more unique a player's " +
                        "experience will be each game. If I've missed someone, let me know via Discord.\nNow that you know the basic info, if you want to know more try using the other " +
                        "info commands, visiting the GitHub page at \nhttps://github.com/AlchlcDvl/TownOfUsReworked/ or joining my discord at \nhttps://discord.gg/cd27aDQDY9/. Good luck!");
                }
                //Credits
                else if (text.StartsWith("/credits") || text.StartsWith("/cr "))
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "The mod would not have been possible without these people!\nMod Creator: slushiegoose\nContinued By: polus.gg\n" +
                        "Reactivated By: eDonnes (or Donners), Term, MyDragonBreath and -H\nWith Help (And Code) From: Discussions, Det, Oper, -H, twix, xerminator and" +
                        " MyDragonBreath\nRemaining credits are on the GitHub!");
                }
                //Control
                else if (text.StartsWith("/controls") || text.StartsWith("/ctrl"))
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Here are the controls for MCI:-\nF1: Spawn a robot and toggle MCI to active (only in lobby)\nF2/F3: Swap control over the bots (only " +
                        "in game)\nF4: End the game in a stalemate\nF5: Fix a sabotage\nF6: Finish all tasks\nF7: Replay the intro cutscene (only in game)\nF8: Remove all bots and toggle" +
                        " MCI to inactive (only in lobby)\nF9: Remove the latest bot and toggle MCI to inactive if there was only one bot (only in lobby)\nF10: Toggle lobby cap\nF11: " +
                        "Toggle bot persistence");
                }
                //RoleInfo help
                else if (text == "/roleinfo" || text == "/roleinfo " || text == "/ri" || text == "/ri ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<roleinfo or ri> <role full or short name>");
                }
                //AlignmentInfo help
                else if (text == "/alignmentinfo" || text == "/alignmentinfo " || text == "/ai" || text == "/ai ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<alignmentinfo or ai> <alignment name or abbreviation>");
                }
                //ModifierInfo help
                else if (text == "/modifierinfo" || text == "/modifierinfo " || text == "/moi" || text == "/moi ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<modifierinfo or moi> <modifier name or abbreviation>");
                }
                //ObjectifierInfo help
                else if (text == "/objectifierinfo" || text == "/objectifierinfo " || text == "/oi" || text == "/oi ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<objectifierinfo or oi> <objectifier name or abbreviation>");
                }
                //AbilityInfo help
                else if (text == "/abilityinfo" || text == "/abilityinfo " || text == "/abi" || text == "/abi ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<abilityinfo or abi> <ability name or abbreviation>");
                }
                //FactionInfo help
                else if (text == "/factioninfo" || text == "/factioninfo " || text == "/fi" || text == "/fi ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /factioninfo <faction name or abbreviation>");
                }
                //Quote help
                else if (text == "/quote" || text == "/quote " || text == "/q" || text == "/q ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<quote or q> <role name or abbreviation>");
                }
                //Lore help
                else if (text == "/lore" || text == "/lore " || text == "/l" || text == "/l ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<lore or l> <role name or abbreviation>");
                }
                //Gives information regarding roles
                else if (text.StartsWith("/roleinfo ") || text.StartsWith("/ri "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/ri ") ? text[4..] : text[10..];
                    chatText = LayerInfo.AllRoles.FirstOrDefault(x => string.Equals(inputText, x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(inputText, x.Short,
                        StringComparison.OrdinalIgnoreCase), LayerInfo.AllRoles[0]).RoleInfoMessage();
                    hudManager.AddChat(player, chatText);
                }
                //Gives information regarding factions
                else if (text.StartsWith("/factioninfo ") || text.StartsWith("/fi "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/fi ") ? text[4..] : text[13..];
                    chatText = LayerInfo.AllRoles.FirstOrDefault(x => string.Equals(inputText, x.FactionS, StringComparison.OrdinalIgnoreCase) || string.Equals(inputText, x.FactionShort,
                        StringComparison.OrdinalIgnoreCase), LayerInfo.AllRoles[0]).FactionInfoMessage();
                    hudManager.AddChat(player, chatText);
                }
                //Gives information regarding alignments
                else if (text.StartsWith("/alignmentinfo ") || text.StartsWith("/ai "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/ai ") ? text[4..] : text[15..];
                    chatText = LayerInfo.AllRoles.FirstOrDefault(x => string.Equals(x.Alignment, inputText, StringComparison.OrdinalIgnoreCase) || string.Equals(x.AlignmentShort,
                        inputText, StringComparison.OrdinalIgnoreCase), LayerInfo.AllRoles[0]).AlignmentInfoMessage();
                    hudManager.AddChat(player, chatText);
                }
                //Gives information regarding modifiers
                else if (text.StartsWith("/modifierinfo ") || text.StartsWith("/moi "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/moi ") ? text[5..] : text[14..];
                    chatText = LayerInfo.AllModifiers.FirstOrDefault(x => string.Equals(x.Name, inputText, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Short, inputText,
                        StringComparison.OrdinalIgnoreCase), LayerInfo.AllModifiers[0]).InfoMessage();
                    hudManager.AddChat(player, chatText);
                }
                //Gives information regarding abilities
                else if (text.StartsWith("/abilityinfo ") || text.StartsWith("/abi "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/abi ") ? text[5..] : text[13..];
                    chatText = LayerInfo.AllAbilities.FirstOrDefault(x => string.Equals(x.Name, inputText, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Short, inputText,
                        StringComparison.OrdinalIgnoreCase), LayerInfo.AllAbilities[0]).InfoMessage();
                    hudManager.AddChat(player, chatText);
                }
                //Gives information regarding objectifiers
                else if (text.StartsWith("/objectifierinfo ") || text.StartsWith("/oi "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/oi ") ? text[4..] : text[17..];
                    chatText = LayerInfo.AllObjectifiers.FirstOrDefault(x => string.Equals(x.Name, inputText, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Short, inputText,
                        StringComparison.OrdinalIgnoreCase), LayerInfo.AllObjectifiers[0]).InfoMessage();
                    hudManager.AddChat(player, chatText);
                }
                //Gives information regarding objectifiers
                else if (text.StartsWith("/lore ") || text.StartsWith("/l "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/l ") ? text[3..] : text[6..];
                    chatText = LayerInfo.AllLore.FirstOrDefault(x => string.Equals(x.Name, inputText, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Short, inputText,
                        StringComparison.OrdinalIgnoreCase), LayerInfo.AllLore[0]).InfoMessage();
                    hudManager.AddChat(player, chatText);
                }
                //Quotes
                else if (text.StartsWith("/quote ") || text.StartsWith("/q "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/q ") ? text[3..] : text[7..];

                    if (inputText == "det")
                        chatText = "I showed AD how to make ONE ROLE...AND HE FUCKING EXPLODED.";
                    else
                    {
                        chatText = LayerInfo.AllRoles.FirstOrDefault(x => string.Equals(x.Name, inputText, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Short, inputText,
                            StringComparison.OrdinalIgnoreCase), LayerInfo.AllRoles[0]).Quote;
                    }

                    hudManager.AddChat(player, chatText);
                }
                else if (ConstantVariables.IsLobby)
                {
                    //Lobby size help help
                    if (text == "/size" || text == "/size " || text == "/s" || text == "/s ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /<size or s> <number between 1 to 127>");
                    }
                    //Changing lobby size
                    else if (text.StartsWith("/size ") || text.StartsWith("/s "))
                    {
                        chatHandled = true;

                        if (AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan())
                        {
                            if (!int.TryParse(text.StartsWith("/s ") ? text[3..] : text[6..], out LobbyLimit))
                                hudManager.AddChat(player, "Invalid");
                            else
                            {
                                LobbyLimit = Math.Clamp(LobbyLimit, 1, 127);

                                if (LobbyLimit != GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers)
                                {
                                    GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers = LobbyLimit;
                                    GameStartManager.Instance.LastPlayerCount = LobbyLimit;
                                    player.RpcSyncSettings(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameOptionsManager.Instance.currentGameOptions));
                                    //TODO Maybe simpler?? 
                                    hudManager.AddChat(player, $"Lobby size changed to {LobbyLimit} player(s).");
                                }
                                else
                                    hudManager.AddChat(player, $"Lobby size is already {LobbyLimit}.");
                            }
                        }
                        else
                            hudManager.AddChat(player, "You can't do that.");
                    }
                    //Name help
                    else if (text == "/setname" || text == "/setname " || text == "/sn" || text == "/sn ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /<setname or sn> <name>");
                    }
                    //Change name (Can have multiple players the same name!)
                    else if (text.StartsWith("/setname ") || text.StartsWith("/sn "))
                    {
                        chatHandled = true;
                        inputText = text.StartsWith("/sn") ? otherText[4..] : otherText[9..];
                        //As much as I hate to do this, people will take advatage of this function so we're better off doing this early
                        string[] profanities = { "fuck", "bastard", "cunt", "bitch", "ass", "nigg", "whore", "negro", "dick", "penis", "yiff", "rape", "rapist" };
                        const string disallowed = "@^[a-zA-Z0-9]+$";

                        if (inputText.Any(disallowed.Contains))
                            hudManager.AddChat(player, "Name contains disallowed characters.");
                        else if (profanities.Any(x => inputText.ToLower().Contains(x)))
                            hudManager.AddChat(player, "Name contains unaccepted words.");
                        else if (inputText.Length > 20)
                            hudManager.AddChat(player, "Name is too long.");
                        else
                        {
                            player.RpcSetName(inputText);
                            hudManager.AddChat(player, "Name changed!");
                        }
                    }
                    //Colour help
                    else if (text == "/colour" || text == "/color" || text == "/colour " || text == "/color ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /colour <colour> or /color <color>");
                    }
                    //Change colour (Can have multiple players the same colour!)
                    else if (text.StartsWith("/color ") || text.StartsWith("/colour "))
                    {
                        chatHandled = true;
                        inputText = text.StartsWith("/colour ") ? text[7..] : text[6..];
                        var colourSpelling = text.StartsWith("/colour ") ? "Colour" : "Color";

                        if (!int.TryParse(inputText, out var col))
                        {
                            hudManager.AddChat(player, inputText + " is an invalid " + colourSpelling + ".\nYou need to use the color ID for the color you want to be. To find out a" +
                                " color's ID, go into the color selection screen and count the number of colors starting from 0 to the position of the color you want to pick. The range" +
                                " of colors is from 0 to 41 meaning Red to Rainbow.");
                        }
                        else
                        {
                            col = Math.Clamp(col, 0, Palette.PlayerColors.Length - 1);
                            player.RpcSetColor((byte)col);
                            hudManager.AddChat(player, colourSpelling + " changed!");
                        }
                    }
                    //Kick help
                    else if (text == "/kick" || text == "/kick ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /kick <player name>");
                    }
                    //Kick player (if able to kick, i.e. host command)
                    else if (text.StartsWith("/kick "))
                    {
                        chatHandled = true;
                        inputText = text[6..];
                        PlayerControl target = PlayerControl.AllPlayerControls.ToArray().ToList().Find(x => x.Data.PlayerName.Equals(inputText));

                        if (target != null && target != player && AmongUsClient.Instance?.CanBan() == true)
                        {
                            var client = AmongUsClient.Instance.GetClient(target.OwnerId);

                            if (client != null)
                                AmongUsClient.Instance.KickPlayer(client.Id, false);
                        }
                    }
                    //Clear the lobby (if able to kick, i.e. host command)
                    else if (text.StartsWith("/clearlobby"))
                    {
                        chatHandled = true;
                        inputText = text[6..];

                        foreach (var player2 in PlayerControl.AllPlayerControls)
                        {
                            if (player2 != player && AmongUsClient.Instance?.CanBan() == true)
                            {
                                var client = AmongUsClient.Instance.GetClient(player2.OwnerId);

                                if (client != null)
                                    AmongUsClient.Instance.KickPlayer(client.Id, false);
                            }
                        }
                    }
                    //Ban help
                    else if (text == "/ban" || text == "/ban ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /ban <player name>");
                    }
                    //Ban player (if able to ban, i.e. host command)
                    else if (text.StartsWith("/ban "))
                    {
                        chatHandled = true;
                        inputText = text[5..];
                        var target = PlayerControl.AllPlayerControls.ToArray().ToList().Find(x => x.Data.PlayerName.Equals(inputText));

                        if (target != null && AmongUsClient.Instance?.CanBan() == true)
                        {
                            var client = AmongUsClient.Instance.GetClient(target.OwnerId);

                            if (client != null)
                                AmongUsClient.Instance.KickPlayer(client.Id, true);
                        }
                    }
                }
                else if (!ConstantVariables.IsLobby)
                {
                    //This command gives the current status and description of the player
                    if (text.StartsWith("/mystate") || text.StartsWith("/ms"))
                    {
                        chatHandled = true;

                        var role = Role.GetRole(player);
                        var modifier = Modifier.GetModifier(player);
                        var ability = Ability.GetAbility(player);
                        var objectifier = Objectifier.GetObjectifier(player);

                        if (role != null)
                            hudManager.AddChat(player, LayerInfo.AllRoles.FirstOrDefault(x => x.Name == role.Name, LayerInfo.AllRoles[0])?.RoleInfoMessage());

                        if (modifier?.Hidden == false)
                            hudManager.AddChat(player, LayerInfo.AllModifiers.FirstOrDefault(x => x.Name == modifier.Name, LayerInfo.AllModifiers[0])?.InfoMessage());

                        if (objectifier?.Hidden == false)
                            hudManager.AddChat(player, LayerInfo.AllObjectifiers.FirstOrDefault(x => x.Name == objectifier.Name, LayerInfo.AllObjectifiers[0])?.InfoMessage());

                        if (ability?.Hidden == false)
                            hudManager.AddChat(player, LayerInfo.AllAbilities.FirstOrDefault(x => x.Name == ability.Name, LayerInfo.AllAbilities[0])?.InfoMessage());
                    }
                    else if (text == "/whisper" || text == "/w" || text == "/w ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /<whisper or w> <meeting number>");
                    }
                    else if (text.StartsWith("/whisper ") || text.StartsWith("/w "))
                    {
                        chatHandled = true;

                        if (!CustomGameOptions.Whispers)
                            hudManager.AddChat(player, "Whispering is not turned on.");
                        else
                        {
                            inputText = text.StartsWith("/w ") ? text[3..] : text[9..];
                            var message = text.StartsWith("/w ") ? text[4..] : text[10..];
                            var message2 = text.StartsWith("/w ") ? text[5..] : text[11..];
                            var number = inputText.Replace(message, "");
                            var number2 = inputText.Replace(message2, "");
                            number = number.Replace(" ", "");
                            number2 = number2.Replace(" ", "");
                            var id1 = byte.Parse(number);
                            var id2 = byte.Parse(number2);
                            var whispered = Utils.PlayerById(id1);
                            var whispered2 = Utils.PlayerById(id2);

                            if (whispered != null)
                            {
                                if (whispered.Data.IsDead)
                                    hudManager.AddChat(player, $"{whispered.name} is dead.");
                                else if (whispered.Data.Disconnected)
                                    hudManager.AddChat(player, $"{whispered.name} is not in this world anymore.");
                                else
                                {
                                    hudManager.AddChat(player, $"You whisper to {whispered.name}:{message}");
                                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Whisper, SendOption.Reliable);
                                    writer.Write(player.PlayerId);
                                    writer.Write(id1);
                                    writer.Write(message);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                }
                            }
                            else if (whispered2 != null)
                            {
                                if (whispered2.Data.IsDead)
                                    hudManager.AddChat(player, $"{whispered2.name} is dead.");
                                else if (whispered2.Data.Disconnected)
                                    hudManager.AddChat(player, $"{whispered2.name} is not in this world anymore.");
                                else
                                {
                                    hudManager.AddChat(player, $"You whisper to {whispered2.name}:{message2}");
                                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Whisper, SendOption.Reliable);
                                    writer.Write(player.PlayerId);
                                    writer.Write(id1);
                                    writer.Write(message);
                                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                                }
                            }
                            else
                                hudManager.AddChat(player, "Who are you trying to whisper?");
                        }
                    }
                }

                //Incorrect command
                if (text.StartsWith("/") && !chatHandled)
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Invalid command.");
                }

                if (chatHandled)
                {
                    hudManager.TextArea.Clear();
                    hudManager.quickChatMenu.ResetGlyphs();
                }

                return !chatHandled;
            }
        }

        //Thanks to Town Of Host for all this code
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
        public static class ChatControlPatch
        {
            private static int CurrentHistorySelection = -1;

            public static void Postfix(ChatController __instance)
            {
                if (!__instance.TextArea.hasFocus)
                    return;

                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.C))
                    ClipboardHelper.PutClipboardString(__instance.TextArea.text);

                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.V))
                    __instance.TextArea.SetText(__instance.TextArea.text + GUIUtility.systemCopyBuffer);

                if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.X))
                {
                    ClipboardHelper.PutClipboardString(__instance.TextArea.text);
                    __instance.TextArea.SetText("");
                }

                if (Input.GetKeyDown(KeyCode.UpArrow) && ChatHistory.Count > 0)
                {
                    CurrentHistorySelection = Mathf.Clamp(--CurrentHistorySelection, 0, ChatHistory.Count - 1);
                    __instance.TextArea.SetText(ChatHistory[CurrentHistorySelection]);
                }

                if (Input.GetKeyDown(KeyCode.DownArrow) && ChatHistory.Count > 0)
                {
                    CurrentHistorySelection++;

                    if (CurrentHistorySelection < ChatHistory.Count)
                        __instance.TextArea.SetText(ChatHistory[CurrentHistorySelection]);
                    else if (CurrentHistorySelection > ChatHistory.Count)
                        __instance.TextArea.SetText(ChatHistory[0]);
                    else
                        __instance.TextArea.SetText("");
                }
            }
        }

        [HarmonyPatch(typeof(ControllerManager), nameof(ControllerManager.Update))]
        public static class ControllerManagerUpdatePatch
        {
            public static void Postfix()
            {
                if (AmongUsClient.Instance.AmHost)
                {
                    if (Input.GetKeyDown(KeyCode.LeftShift) && ConstantVariables.IsCountDown)
                        GameStartManager.Instance.countDownTimer = 0;

                    if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
                        GameStartManager.Instance.Start();

                    if (Input.GetKeyDown(KeyCode.C) && ConstantVariables.IsCountDown)
                        GameStartManager.Instance.ResetStartState();
                }
            }
        }

        //Thanks to The Other Roles for this code
        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.HostGame))]
        public static class InnerNetClientHostPatch
        {
            public static void Prefix([HarmonyArgument(0)] GameOptionsData settings)
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

                LobbyLimit = maxPlayers;
                settings.MaxPlayers = 127; // Force 127 Player Lobby on Server
                DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
            }

            public static void Postfix([HarmonyArgument(0)] GameOptionsData settings) => settings.MaxPlayers = LobbyLimit;
        }

        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.JoinGame))]
        public static class InnerNetClientJoinPatch
        {
            public static void Prefix() => DataManager.Settings.Multiplayer.ChatMode = QuickChatModes.FreeChatOrQuickChat;
        }

        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
        public static class AmongUsClientOnPlayerJoined
        {
            public static bool Prefix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
            {
                if (LobbyLimit < __instance.allClients.Count)
                {
                    // TODO: Fix this canceling start
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