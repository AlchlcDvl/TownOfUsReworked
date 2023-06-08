namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class ChatCommands
    {
        public readonly static List<string> ChatHistory = new();

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
                var setColor = TownOfUsReworked.IsTest ? ", /setcolour or /setcolor, /setname" : "";
                var whisper = CustomGameOptions.Whispers ? ", /whisper" : "";
                var kickBan = AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan() ? ", /kick, /ban, /clearlobby" : "";
                var player = PlayerControl.LocalPlayer;

                if (ChatHistory.Count == 0 || ChatHistory[^1] != text)
                    ChatHistory.Add(text);

                //Help command - lists commands available
                if (text.StartsWith("/help") || text == "/h" || text.StartsWith("/h "))
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Commands available all the time:\n/modinfo /roleinfo, /modifierinfo, /abilityinfo, /objectifierinfo, /factioninfo, /alignmentinfo, " +
                        $"/subfactioninfo, /otherinfo, /quote, /controls, /lore\n\nCommands available in lobby:\n/summary{setColor}{kickBan}\n\nCommands available in game:\n/mystate" +
                        whisper);
                }
                //Display a message (Information about the mod)
                else if (text.StartsWith("/modinfo") || text.StartsWith("/mi"))
                {
                    chatHandled = true;
                    hudManager.AddChat(player, $"Welcome to Town Of Us Reworked {TownOfUsReworked.VersionFinal}!\n\nTown Of Us Reworked is essentially a weird mishmash of code from Town " +
                        "Of Us Reactivated and its forks plus some of my own code. Credits to the parties have already been given. This mod has several reworks and additions which I " +
                        "believe fit the mod better. Plus, the more layers there are, the more unique a player's experience will be each game. If I've missed someone, let me know via " +
                        "Discord.\n\nNow that you know the basic info, if you want to know more try using the other info commands, visiting the GitHub page or joining my discord. Happy " + "ejections!");
                }
                //Control
                else if (text.StartsWith("/controls") || text.StartsWith("/ctrl"))
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Here are the controls:\nF1 - Start up the MCI control panel (local only)\nF2 - Toggle the visibility of the control panel (local only)\n" +
                        "Tab - Change pages\nUp/Left Arrow - Go up a page when in a meny\nDown/Right Arrow - Go down a page when in a menu");
                }
                //RoleInfo help
                else if (text is "/roleinfo" or "/roleinfo " or "/ri" or "/ri ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<roleinfo or ri> <role full or short name>");
                }
                //AlignmentInfo help
                else if (text is "/alignmentinfo" or "/alignmentinfo " or "/ai" or "/ai ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<alignmentinfo or ai> <alignment name or abbreviation>");
                }
                //ModifierInfo help
                else if (text is "/modifierinfo" or "/modifierinfo " or "/moi" or "/moi ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<modifierinfo or moi> <modifier name or abbreviation>");
                }
                //ObjectifierInfo help
                else if (text is "/objectifierinfo" or "/objectifierinfo " or "/oi" or "/oi ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<objectifierinfo or oi> <objectifier name or abbreviation>");
                }
                //AbilityInfo help
                else if (text is "/abilityinfo" or "/abilityinfo " or "/abi" or "/abi ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<abilityinfo or abi> <ability name or abbreviation>");
                }
                //FactionInfo help
                else if (text is "/factioninfo" or "/factioninfo " or "/fi" or "/fi ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<factioninfo or fi> <faction name or abbreviation>");
                }
                //SubFactionInfo help
                else if (text is "/subfactioninfo" or "/subfactioninfo " or "/si" or "/si ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<subfactioninfo or si> <subfaction name or abbreviation>");
                }
                //OtherInfo help
                else if (text is "/otherinfo" or "/otherinfo " or "/oti" or "/oti ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<otherinfo or oti> <name or abbreviation>");
                }
                //Quote help
                else if (text is "/quote" or "/quote " or "/q" or "/q ")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Usage: /<quote or q> <role name or abbreviation>");
                }
                //Lore help
                else if (text is "/lore" or "/lore " or "/l" or "/l ")
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
                        StringComparison.OrdinalIgnoreCase), LayerInfo.AllRoles[0]).ToString();
                    hudManager.AddChat(player, chatText);
                }
                //Gives information regarding factions
                else if (text.StartsWith("/factioninfo ") || text.StartsWith("/fi "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/fi ") ? text[4..] : text[13..];
                    chatText = LayerInfo.AllFactions.FirstOrDefault(x => string.Equals(inputText, x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(inputText, x.Short,
                        StringComparison.OrdinalIgnoreCase), LayerInfo.AllFactions[0]).ToString();
                    hudManager.AddChat(player, chatText);
                }
                //Gives information regarding subfactions
                else if (text.StartsWith("/subfactioninfo ") || text.StartsWith("/si "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/si ") ? text[4..] : text[16..];
                    chatText = LayerInfo.AllSubFactions.FirstOrDefault(x => string.Equals(inputText, x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(inputText, x.Short,
                        StringComparison.OrdinalIgnoreCase), LayerInfo.AllSubFactions[0]).ToString();
                    hudManager.AddChat(player, chatText);
                }
                //Gives information regarding alignments
                else if (text.StartsWith("/alignmentinfo ") || text.StartsWith("/ai "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/ai ") ? text[4..] : text[15..];
                    chatText = LayerInfo.AllAlignments.FirstOrDefault(x => string.Equals(x.Name, inputText, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Short, inputText,
                        StringComparison.OrdinalIgnoreCase), LayerInfo.AllAlignments[0]).ToString();
                    hudManager.AddChat(player, chatText);
                }
                //Gives information regarding modifiers
                else if (text.StartsWith("/modifierinfo ") || text.StartsWith("/moi "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/moi ") ? text[5..] : text[14..];
                    chatText = LayerInfo.AllModifiers.FirstOrDefault(x => string.Equals(x.Name, inputText, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Short, inputText,
                        StringComparison.OrdinalIgnoreCase), LayerInfo.AllModifiers[0]).ToString();
                    hudManager.AddChat(player, chatText);
                }
                //Gives information regarding abilities
                else if (text.StartsWith("/abilityinfo ") || text.StartsWith("/abi "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/abi ") ? text[5..] : text[13..];
                    chatText = LayerInfo.AllAbilities.FirstOrDefault(x => string.Equals(x.Name, inputText, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Short, inputText,
                        StringComparison.OrdinalIgnoreCase), LayerInfo.AllAbilities[0]).ToString();
                    hudManager.AddChat(player, chatText);
                }
                //Gives information regarding objectifiers
                else if (text.StartsWith("/objectifierinfo ") || text.StartsWith("/oi "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/oi ") ? text[4..] : text[17..];
                    chatText = LayerInfo.AllObjectifiers.FirstOrDefault(x => string.Equals(x.Name, inputText, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Short, inputText,
                        StringComparison.OrdinalIgnoreCase) || string.Equals(x.Symbol, inputText, StringComparison.OrdinalIgnoreCase), LayerInfo.AllObjectifiers[0]).ToString();
                    hudManager.AddChat(player, chatText);
                }
                //Gives information regarding objectifiers
                else if (text.StartsWith("/lore ") || text.StartsWith("/l "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/l ") ? text[3..] : text[6..];
                    chatText = LayerInfo.AllLore.FirstOrDefault(x => string.Equals(x.Name, inputText, StringComparison.OrdinalIgnoreCase) || string.Equals(x.Short, inputText,
                        StringComparison.OrdinalIgnoreCase), LayerInfo.AllLore[0]).ToString();
                    hudManager.AddChat(player, chatText);
                }
                //Gives information regarding other things
                else if (text.StartsWith("/otherinfo ") || text.StartsWith("/oti "))
                {
                    chatHandled = true;
                    inputText = text.StartsWith("/oti ") ? text[5..] : text[10..];
                    chatText = LayerInfo.AllOthers.FirstOrDefault(x => string.Equals(inputText, x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(inputText, x.Short,
                        StringComparison.OrdinalIgnoreCase), LayerInfo.AllOthers[0]).ToString();
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
                    //Name help
                    if (text is "/setname" or "/setname " or "/sn" or "/sn " && TownOfUsReworked.IsTest)
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /<setname or sn> <name>");
                    }
                    //Change name (Can have multiple players the same name!)
                    else if ((text.StartsWith("/setname ") || text.StartsWith("/sn ")) && TownOfUsReworked.IsTest)
                    {
                        chatHandled = true;
                        inputText = text.StartsWith("/sn") ? otherText[4..] : otherText[9..];
                        //As much as I hate to do this, people will take advatage of this function so we're better off doing this early
                        string[] profanities = { "fuck", "bastard", "cunt", "bitch", "ass", "nigg", "whore", "negro", "dick", "penis", "yiff", "rape", "rapist" };
                        const string disallowed = "@^[{(_-;:\"'.,\\|)}]+$!#$%^&&*?/";

                        if (inputText.ToLower().Any(disallowed.Contains))
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
                    else if (text is "/colour" or "/color" or "/colour " or "/color " && TownOfUsReworked.IsTest)
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /colour <colour> or /color <color>");
                    }
                    //Change colour (Can have multiple players the same colour!)
                    else if ((text.StartsWith("/color ") || text.StartsWith("/colour ")) && TownOfUsReworked.IsTest)
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
                    else if (text is "/ban" or "/ban ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /ban <player name>");
                    }
                    //Kick help
                    else if (text is "/kick" or "/kick ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /kick <player name>");
                    }
                    //Ban player (if able to ban, i.e. host command)
                    else if (text.StartsWith("/ban ") || text.StartsWith("/kick "))
                    {
                        chatHandled = true;
                        inputText = text.StartsWith("/ban ") ? text[5..] : text[6..];
                        var target = PlayerControl.AllPlayerControls.Find(x => x.Data.PlayerName.Equals(inputText));

                        if (target != null && target != player && AmongUsClient.Instance.CanBan())
                        {
                            var client = AmongUsClient.Instance.GetClient(target.OwnerId);

                            if (client != null)
                                AmongUsClient.Instance.KickPlayer(client.Id, text.StartsWith("/ban "));
                        }
                    }
                    //Redisplays the summary for those who missed/skipped it
                    else if (text.StartsWith("/summary") || text.StartsWith("/sum"))
                    {
                        var summary = "";

                        try
                        {
                            summary = File.ReadAllText(Path.Combine(Application.persistentDataPath, "Summary"));
                        }
                        catch
                        {
                            summary = "Summary could not be found.";
                        }

                        hudManager.AddChat(player, summary);
                        chatHandled = true;
                    }
                }
                else if (!ConstantVariables.IsLobby)
                {
                    //This command gives the current status and description of the player
                    if (text.StartsWith("/mystate") || text.StartsWith("/ms"))
                    {
                        chatHandled = true;

                        if (Role.LocalRole)
                            hudManager.AddChat(player, LayerInfo.AllRoles.FirstOrDefault(x => x.Name == Role.LocalRole.Name, LayerInfo.AllRoles[0]).ToString());

                        if (!Modifier.LocalModifier.Hidden)
                            hudManager.AddChat(player, LayerInfo.AllModifiers.FirstOrDefault(x => x.Name == Modifier.LocalModifier.Name, LayerInfo.AllModifiers[0]).ToString());

                        if (!Objectifier.LocalObjectifier.Hidden)
                        {
                            hudManager.AddChat(player, LayerInfo.AllObjectifiers.FirstOrDefault(x => x.Name == Objectifier.LocalObjectifier.Name || x.Symbol ==
                                Objectifier.LocalObjectifier.Symbol, LayerInfo.AllObjectifiers[0]).ToString());
                        }

                        if (!Ability.LocalAbility.Hidden)
                            hudManager.AddChat(player, LayerInfo.AllAbilities.FirstOrDefault(x => x.Name == Ability.LocalAbility.Name, LayerInfo.AllAbilities[0]).ToString());
                    }
                    else if (text is "/whisper" or "/w" or "/w " or "/whisper ")
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
                            if (player.Data.IsDead)
                                hudManager.AddChat(player, "You are dead.");
                            else if (player.IsBlackmailed())
                                hudManager.AddChat(player, "You are blackmailed.");
                            else
                            {
                                inputText = text.StartsWith("/w ") ? text[3..] : text[9..];
                                var args = inputText.Split(' ');
                                var message = inputText.Replace($"{args[0]} ", "");
                                var id = byte.Parse(args[0]);
                                var whispered = Utils.PlayerById(id);

                                if (whispered == player)
                                    hudManager.AddChat(player, "Don't whisper yourself, weirdo.");
                                else if (whispered)
                                {
                                    if (whispered.Data.IsDead)
                                        hudManager.AddChat(player, $"{whispered.name} is dead.");
                                    else if (whispered.Data.Disconnected)
                                        hudManager.AddChat(player, $"{whispered.name} is not in this world anymore.");
                                    else
                                    {
                                        hudManager.AddChat(player, $"You whisper to {whispered.name}: {message}");
                                        var writer = AmongUsClient.Instance.StartRpcImmediately(player.NetId, (byte)CustomRPC.Whisper, SendOption.Reliable);
                                        writer.Write(player.PlayerId);
                                        writer.Write(id);
                                        writer.Write(message);
                                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                                    }
                                }
                                else
                                    hudManager.AddChat(player, "Who are you trying to whisper?");
                            }
                        }
                    }
                }

                //Incorrect command
                if (text.StartsWith("/") && !chatHandled)
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "Invalid command.");
                }
                else if (player.IsBlackmailed() && !chatHandled && text != "i am blackmailed.")
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "You are blackmailed.");
                }
                else if (!player.IsSilenced() && !chatHandled && text != "i am silenced." && PlayerControl.AllPlayerControls.Any(x => x.IsSilenced() && x.GetSilencer().HoldsDrive))
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "You are silenced.");
                }
                else if (MeetingPatches.GivingAnnouncements && !player.Data.IsDead && !chatHandled)
                {
                    chatHandled = true;
                    hudManager.AddChat(player, "You cannot talk right now.");
                }

                if (chatHandled)
                {
                    hudManager.TextArea.Clear();
                    hudManager.quickChatMenu.ResetGlyphs();
                }

                if (!player.Data.IsDead && !chatHandled && !MeetingPatches.GivingAnnouncements && text != "")
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Notify, SendOption.Reliable);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Notify(player.PlayerId);
                }

                return !chatHandled;
            }
        }

        public static void Notify(byte targetPlayerId)
        {
            if (!MeetingHud.Instance)
                return;

            try
            {
                var playerVoteArea = Utils.VoteAreaById(targetPlayerId);
                var rend = UObject.Instantiate(playerVoteArea.Megaphone, playerVoteArea.Megaphone.transform);
                rend.name = "Notification";
                rend.transform.localPosition = new(-2f, 0.1f, -1f);
                rend.sprite = AssetManager.GetSprite("Chat");
                rend.gameObject.SetActive(true);
                HudManager.Instance.StartCoroutine(Effects.Lerp(2, new Action<float>(p =>
                {
                    if (p == 1)
                    {
                        rend.gameObject.SetActive(false);
                        rend.gameObject.Destroy();
                    }
                })));
            }
            catch
            {
                Utils.LogSomething("Notif already exists");
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
                    CurrentHistorySelection--;

                    if (CurrentHistorySelection >= 0)
                        __instance.TextArea.SetText(ChatHistory[CurrentHistorySelection]);
                    else
                    {
                        __instance.TextArea.SetText("");
                        CurrentHistorySelection = ChatHistory.Count - 1;
                    }
                }

                if (Input.GetKeyDown(KeyCode.DownArrow) && ChatHistory.Count > 0)
                {
                    CurrentHistorySelection++;

                    if (CurrentHistorySelection < ChatHistory.Count)
                        __instance.TextArea.SetText(ChatHistory[CurrentHistorySelection]);
                    else if (CurrentHistorySelection == ChatHistory.Count)
                    {
                        __instance.TextArea.SetText("");
                        CurrentHistorySelection = 0;
                    }
                }
            }
        }

        //The code is from The Other Roles; link :- https://github.com/TheOtherRolesAU/TheOtherRoles/blob/main/TheOtherRoles/Modules/DynamicLobbies.cs under GPL v3
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
                if (CustomGameOptions.LobbySize < __instance.allClients.Count)
                {
                    DisconnectPlayer(__instance, client.Id);
                    return false;
                }

                return true;
            }

            private static void DisconnectPlayer(InnerNetClient _this, int clientId)
            {
                if (!_this.AmHost)
                    return;

                var writer = MessageWriter.Get(SendOption.Reliable);
                writer.StartMessage(4);
                writer.Write(_this.GameId);
                writer.WritePacked(clientId);
                writer.Write((byte)DisconnectReasons.GameFull);
                writer.EndMessage();
                _this.SendOrDisconnect(writer);
                writer.Recycle();
            }
        }
    }
}