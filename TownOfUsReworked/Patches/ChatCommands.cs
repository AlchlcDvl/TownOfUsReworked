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
using TownOfUsReworked.Enums;
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
                //Set up dictionaries and list for colours
                var coloursDict = new Dictionary<int, string>
                {
                    { 0, "Red" },
                    { 1, "Blue" },
                    { 2, "Green" },
                    { 3, "Pink" },
                    { 4, "Orange" },
                    { 5, "Yellow" },
                    { 6, "Black" },
                    { 7, "White" },
                    { 8, "Purple" },
                    { 9, "Brown" },
                    { 10, "Cyan" },
                    { 11, "Lime" },
                    { 12, "Maroon" },
                    { 13, "Rose" },
                    { 14, "Banana" },
                    { 15, "Grey" },
                    { 16, "Tan" },
                    { 17, "Coral" },
                    { 18, "Watermelon" },
                    { 19, "Chocolate" },
                    { 20, "Sky Blue" },
                    { 21, "Biege" },
                    { 22, "Hot Pink" },
                    { 23, "Turquoise" },
                    { 24, "Lilac" },
                    { 25, "Olive" },
                    { 26, "Azure" },
                    { 27, "Plum" },
                    { 28, "Jungle" },
                    { 29, "Mint" },
                    { 30, "Chartreuse" },
                    { 31, "Macau" },
                    { 32, "Tawny" },
                    { 33, "Gold" },
                    { 34, "Chroma" },
                    { 35, "Rainbow" }
                };

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
                var setColor = TownOfUsReworked.isTest ? " /setcolour or /setcolor," : "";
                var whisper = CustomGameOptions.Whispers ? " /whisper," : "";
                var kickBan = AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan() ? " /kick, /ban, /clearlobby, /size," : "";

                var player = PlayerControl.LocalPlayer;

                if (ChatHistory.Count == 0 || ChatHistory[^1] != text)
                    ChatHistory.Add(text);

                if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
                {
                    //Help command - lists commands available
                    if (text == "/help" || text.StartsWith("/h"))
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, $"Commands available:\n/modinfo, /setname,{setColor}{kickBan} /roleinfo, /modifierinfo, /abilityinfo, /objectifierinfo, " +
                            "/factioninfo, /alignmentinfo, /quote, /abbreviations, /lookup, /credits, /controls");
                    }
                    //Display a message (Information about the mod)
                    else if (text.StartsWith("/modinfo") || text.StartsWith("/mi"))
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, $"Welcome to Town Of Us Reworked {TownOfUsReworked.versionFinal}!");
                        hudManager.AddChat(player, "Town Of Us Reworked is essentially a weird mishmash of code from Town Of Us Reactivated and its forks plus some of my own code.");
                        hudManager.AddChat(player, "Credits to the parties have already been given (good luck to those who want to try to cancel me for no reason). This mod has " +
                            "several reworks and additions which I believe fit the mod better. Plus, the more layers there are, the more unique a player's experience will be each" +
                            " game. If I've missed someone, let me know via Discord.");
                        hudManager.AddChat(player, "Now that you know the basic info, if you want to know more try using the other info commands, visiting the GitHub page at " +
                            "\nhttps://github.com/AlchlcDvl/TownOfUsReworked/ or joining my discord at \nhttps://discord.gg/cd27aDQDY9/. Good luck!");
                    }
                    else if (text.StartsWith("/size "))
                    {
                        chatHandled = true;

                        if (AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan())
                        {
                            if (!int.TryParse(text.AsSpan(6), out LobbyLimit))
                            {
                                hudManager.AddChat(PlayerControl.LocalPlayer, "Invalid Size\nUsage: /size <amount>");
                            }
                            else
                            {
                                LobbyLimit = Math.Clamp(LobbyLimit, 1, 127);

                                if (LobbyLimit != GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers)
                                {
                                    GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers = LobbyLimit;
                                    GameStartManager.Instance.LastPlayerCount = LobbyLimit;
                                    PlayerControl.LocalPlayer.RpcSyncSettings(GameOptionsManager.Instance.gameOptionsFactory.ToBytes(GameOptionsManager.Instance.currentGameOptions));
                                    //TODO Maybe simpler?? 
                                    hudManager.AddChat(PlayerControl.LocalPlayer, $"Lobby size changed to {LobbyLimit} players.");
                                }
                                else
                                {
                                    hudManager.AddChat(PlayerControl.LocalPlayer, $"Lobby size is already {LobbyLimit}.");
                                }
                            }
                        }
                        else
                        {
                            hudManager.AddChat(PlayerControl.LocalPlayer, "You can't do that.");
                        }
                    }
                    //Abbreviations help
                    else if (text == "/abbreviations" || text == "/abbreviations " || text == "/ab" || text == "/ab ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /abbreviations <name>");
                    }
                    //Display a message (Information about the mod)
                    else if (text.StartsWith("/abbreviations ") || text.StartsWith("/ab "))
                    {
                        chatHandled = true;
                        inputText = text.StartsWith("/ab ") ? text[4..] : text[15..];
                        var tempText = inputText;
                        inputText = inputText.ToLower();
                        var requiredText = inputText.Replace("(", "");
                        requiredText = requiredText.Replace(")", "");
                        requiredText = requiredText.Replace("/", "");
                        requiredText = requiredText.Replace(" ", "");
                        var abbreviation = "";

                        if (requiredText == "abilityinfo")
                            abbreviation = "ai";
                        else if (requiredText == "setcolor" || requiredText == "setcolour")
                            abbreviation = "sc";
                        else if (requiredText == "fanatic")
                            abbreviation = "fan";
                        else if (requiredText == "lovers")
                            abbreviation = "lover";
                        else if (requiredText == "phantom")
                            abbreviation = "phan";
                        else if (requiredText == "rivals")
                            abbreviation = "rival";
                        else if (requiredText == "taskmaster")
                            abbreviation = "task";
                        else if (requiredText == "traitor")
                            abbreviation = "trait";
                        else if (requiredText == "abbreviations" || requiredText == "abbreviation")
                            abbreviation = "ab";
                        else if (requiredText == "setname")
                            abbreviation = "sn";
                        else if (requiredText == "quote")
                            abbreviation = "q";
                        else if (requiredText == "ban")
                            abbreviation = "b";
                        else if (requiredText == "kick")
                            abbreviation = "k";
                        else if (requiredText == "credits" || requiredText == "credit")
                            abbreviation = "cr";
                        else if (requiredText == "modinfo")
                            abbreviation = "mi";
                        else if (requiredText == "help")
                            abbreviation = "h";
                        else if (requiredText == "roleinfo")
                            abbreviation = "ri";
                        else if (requiredText == "modifierinfo")
                            abbreviation = "modi";
                        else if (requiredText == "objectifierinfo")
                            abbreviation = "oi";
                        else if (requiredText == "factioninfo")
                            abbreviation = "fi";
                        else if (requiredText == "alignmentinfo")
                            abbreviation = "ali";
                        else if (requiredText == "lookup")
                            abbreviation = "lu";
                        else if (requiredText == "agent")
                            abbreviation = "ag";
                        else if (requiredText == "altruist")
                            abbreviation = "alt";
                        else if (requiredText == "amnesiac")
                            abbreviation = "amne";
                        else if (requiredText == "anarchist")
                            abbreviation = "ana";
                        else if (requiredText == "arsonist")
                            abbreviation = "arso";
                        else if (requiredText == "blackmailer")
                            abbreviation = "bm";
                        else if (requiredText == "camouflager")
                            abbreviation = "camo";
                        else if (requiredText == "cannibal")
                            abbreviation = "cann";
                        else if (requiredText == "concealer")
                            abbreviation = "conc";
                        else if (requiredText == "consigliere")
                            abbreviation = "consig";
                        else if (requiredText == "consort")
                            abbreviation = "cons";
                        else if (requiredText == "coroner")
                            abbreviation = "cor";
                        else if (requiredText == "crewmate")
                            abbreviation = "crew";
                        else if (requiredText == "cryomaniac")
                            abbreviation = "cryo";
                        else if (requiredText == "detective")
                            abbreviation = "det";
                        else if (requiredText == "disguiser")
                            abbreviation = "disg";
                        else if (requiredText == "dracula")
                            abbreviation = "drac";
                        else if (requiredText == "engineer")
                            abbreviation = "engi";
                        else if (requiredText == "mafioso")
                            abbreviation = "mafi";
                        else if (requiredText == "escort")
                            abbreviation = "esc";
                        else if (requiredText == "executioner")
                            abbreviation = "exe";
                        else if (requiredText == "glitch")
                            abbreviation = "gli";
                        else if (requiredText == "godfather")
                            abbreviation = "gf";
                        else if (requiredText == "gorgon")
                            abbreviation = "gorg";
                        else if (requiredText == "grenadier")
                            abbreviation = "gren";
                        else if (requiredText == "impostor")
                            abbreviation = "imp";
                        else if (requiredText == "inspector")
                            abbreviation = "insp";
                        else if (requiredText == "investigator")
                            abbreviation = "inv";
                        else if (requiredText == "guardianangel")
                            abbreviation = "ga";
                        else if (requiredText == "janitor")
                            abbreviation = "jani";
                        else if (requiredText == "jester")
                            abbreviation = "jest";
                        else if (requiredText == "juggernaut")
                            abbreviation = "jugg";
                        else if (requiredText == "mayor")
                            abbreviation = "mayor";
                        else if (requiredText == "medic")
                            abbreviation = "medic";
                        else if (requiredText == "medium")
                            abbreviation = "med";
                        else if (requiredText == "miner")
                            abbreviation = "mine";
                        else if (requiredText == "morphling")
                            abbreviation = "morph";
                        else if (requiredText == "murderer")
                            abbreviation = "murd";
                        else if (requiredText == "operative")
                            abbreviation = "op";
                        else if (requiredText == "pestilence")
                            abbreviation = "pest";
                        else if (requiredText == "plaguebearer")
                            abbreviation = "pb";
                        else if (requiredText == "poisoner")
                            abbreviation = "pois";
                        else if (requiredText == "jackal")
                            abbreviation = "jack";
                        else if (requiredText == "serialkiller")
                            abbreviation = "sk";
                        else if (requiredText == "shapeshifter")
                            abbreviation = "ss";
                        else if (requiredText == "sheriff")
                            abbreviation = "sher";
                        else if (requiredText == "shifter")
                            abbreviation = "shift";
                        else if (requiredText == "survivor")
                            abbreviation = "surv";
                        else if (requiredText == "swapper")
                            abbreviation = "swap";
                        else if (requiredText == "teleporter")
                            abbreviation = "tele";
                        else if (requiredText == "thief")
                            abbreviation = "thief";
                        else if (requiredText == "timelord")
                            abbreviation = "tl";
                        else if (requiredText == "time master")
                            abbreviation = "tm";
                        else if (requiredText == "tracker")
                            abbreviation = "track";
                        else if (requiredText == "transporter")
                            abbreviation = "trans";
                        else if (requiredText == "troll")
                            abbreviation = "troll";
                        else if (requiredText == "undertaker")
                            abbreviation = "ut";
                        else if (requiredText == "vampire hunter")
                            abbreviation = "vh";
                        else if (requiredText == "veteran")
                            abbreviation = "vet";
                        else if (requiredText == "vigilante")
                            abbreviation = "vig";
                        else if (requiredText == "warper")
                            abbreviation = "warp";
                        else if (requiredText == "wraith")
                            abbreviation = "wraith";
                        else if (requiredText == "werewolf")
                            abbreviation = "ww";
                        else if (requiredText == "crew")
                            abbreviation = "crew";
                        else if (requiredText == "intruder")
                            abbreviation = "int";
                        else if (requiredText == "syndicate")
                            abbreviation = "syn";
                        else if (requiredText == "neutral")
                            abbreviation = "neut";
                        else if (requiredText == "crewinvestigative")
                            abbreviation = "ci";
                        else if (requiredText == "intrudersupport")
                            abbreviation = "is";
                        else if (requiredText == "intruderconcealing")
                            abbreviation = "ic";
                        else if (requiredText == "neutralbenign")
                            abbreviation = "nb";
                        else if (requiredText == "crewprotective")
                            abbreviation = "cp";
                        else if (requiredText == "syndicateutility")
                            abbreviation = "su";
                        else if (requiredText == "neutralkilling")
                            abbreviation = "nk";
                        else if (requiredText == "neutralevil")
                            abbreviation = "ne";
                        else if (requiredText == "syndicatesupport")
                            abbreviation = "ssu";
                        else if (requiredText == "crewsupport")
                            abbreviation = "cs";
                        else if (requiredText == "crewutility")
                            abbreviation = "cu";
                        else if (requiredText == "neutralproselyte")
                            abbreviation = "np";
                        else if (requiredText == "intruderdeception")
                            abbreviation = "id";
                        else if (requiredText == "neutralneophyte")
                            abbreviation = "nn";
                        else if (requiredText == "syndicatekilling")
                            abbreviation = "syk";
                        else if (requiredText == "intruderutility")
                            abbreviation = "iu";
                        else if (requiredText == "syndicatedisruption")
                            abbreviation = "sd";
                        else if (requiredText == "assassin")
                            abbreviation = "ass (don't get any funny meanings)";
                        else if (requiredText == "buttonbarry")
                            abbreviation = "bb";
                        else if (requiredText == "lighter")
                            abbreviation = "light";
                        else if (requiredText == "multitasker")
                            abbreviation = "mt";
                        else if (requiredText == "radar")
                            abbreviation = "radar";
                        else if (requiredText == "revealer")
                            abbreviation = "reveal";
                        else if (requiredText == "snitch")
                            abbreviation = "sni";
                        else if (requiredText == "tiebreaker")
                            abbreviation = "tb";
                        else if (requiredText == "torch")
                            abbreviation = "torch";
                        else if (requiredText == "tunneler")
                            abbreviation = "tunn";
                        else if (requiredText == "underdog")
                            abbreviation = "ud";
                        else if (requiredText == "bait")
                            abbreviation = "bait";
                        else if (requiredText == "coward")
                            abbreviation = "cow";
                        else if (requiredText == "diseased")
                            abbreviation = "dis";
                        else if (requiredText == "drunk")
                            abbreviation = "drunk";
                        else if (requiredText == "dwarf")
                            abbreviation = "dwarf";
                        else if (requiredText == "flincher")
                            abbreviation = "flinch";
                        else if (requiredText == "giant")
                            abbreviation = "giant";
                        else if (requiredText == "professional")
                            abbreviation = "prof";
                        else if (requiredText == "shy")
                            abbreviation = "shy";
                        else if (requiredText == "vip")
                            abbreviation = "vip";
                        else if (requiredText == "volatile")
                            abbreviation = "vol";
                        else if (requiredText == "controls")
                            abbreviation = "cont";
                        else
                            abbreviation = "Invalid input.";

                        if (abbreviation == "Invalid input.")
                            chatText = abbreviation;
                        else
                            chatText = $"The abbreviation for {tempText} is {abbreviation}!";

                        hudManager.AddChat(player, chatText);
                    }
                    //Credits
                    else if (text.StartsWith("/credits"))
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "The mod would not have been possible without these people!");
                        hudManager.AddChat(player, "Mod Creator: slushiegoose");
                        hudManager.AddChat(player, "Continued By: polus.gg");
                        hudManager.AddChat(player, "Reactivated By: eDonnes (or Donners), Term, MyDragonBreath and -H");
                        hudManager.AddChat(player, "With Help (And Code) From: Discussions, Det, Oper, -H, twix, xerminator and MyDragonBreath");
                        hudManager.AddChat(player, "Remaining credits are on the GitHub!");
                    }
                    //Name help
                    else if (text == "/setname" || text == "/setname ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /setname <name>");
                    }
                    //Change name (Can have multiple players the same name!)
                    else if (text.StartsWith("/setname "))
                    {
                        chatHandled = true;
                        inputText = otherText[9..];
                        //As much as I hate to do this, people will take advatage of this function so we're better off doing this early
                        string[] profanities = { "fuck", "bastard", "cunt", "bitch", "ass", "nigg", "whore", "negro", "dick", "penis", "yiff", "rape", "rapist" };

                        if (!System.Text.RegularExpressions.Regex.IsMatch(inputText, "@^[a-zA-Z0-9]+$"))
                        {
                            hudManager.AddChat(player, "Name contains disallowed characters.");
                        }
                        else if (profanities.Any(x => inputText.ToLower().Contains(x)))
                        {
                            hudManager.AddChat(player, "Name contains unaccepted words.");
                        }
                        else if (inputText.Length > 20)
                        {
                            hudManager.AddChat(player, "Name is too long.");
                        }
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
                            hudManager.AddChat(player, inputText + " is an invalid " + colourSpelling + ".\nYou need to use the color ID for the color you want to be. To find out a color's ID," +
                                " go into the color selection screen and count the number of colors starting from 0 to the position of the color you want to pick. The range of colors is from 0" +
                                " to 41 meaning Red to Rainbow.");
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
                    //RoleInfo help
                    else if (text == "/roleinfo" || text == "/roleinfo ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /roleinfo <role name or role abbreviation>");
                    }
                    //AlignmentInfo help
                    else if (text == "/alignmentinfo" || text == "/alignmentinfo ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /alignmentinfo <faction name> <alignment name> or \n /alignmentinfo <alignment abbreviation>");
                    }
                    //Gives information regarding roles
                    else if (text.StartsWith("/roleinfo "))
                    {
                        chatHandled = true;
                        inputText = text[10..];
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding factions
                    else if (text.StartsWith("/factioninfo "))
                    {
                        chatHandled = true;
                        inputText = text[13..];
                        chatText = LayerInfo.AllRoles.FirstOrDefault(x => string.Equals(inputText, $"{x.Faction}", StringComparison.OrdinalIgnoreCase) || string.Equals(inputText, x.FactionShort, StringComparison.OrdinalIgnoreCase),
                            LayerInfo.AllRoles[0]).FactionInfoMessage();
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding alignments
                    else if (text.StartsWith("/alignmentinfo "))
                    {
                        chatHandled = true;
                        inputText = text[15..];
                        chatText = LayerInfo.AllRoles.FirstOrDefault(x => x.Alignment.ToLower() == inputText || string.Equals(x.AlignmentShort, inputText,
                            StringComparison.OrdinalIgnoreCase), LayerInfo.AllRoles[0])?.AlignmentInfoMessage();
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding modifiers
                    else if (text.StartsWith("/modifierinfo "))
                    {
                        chatHandled = true;
                        inputText = text[14..];
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding abilities
                    else if (text.StartsWith("/abilityinfo "))
                    {
                        chatHandled = true;
                        inputText = text[13..];
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding objectifiers
                    else if (text.StartsWith("/objectifierinfo "))
                    {
                        chatHandled = true;
                        inputText = text[17..];
                        hudManager.AddChat(player, chatText);
                    }
                    //ModifierInfo help
                    else if (text == "/modifierinfo" || text == "/modifierinfo ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /modifierinfo <modifier name>");
                    }
                    //ObjectifierInfo help
                    else if (text == "/objectifierinfo" || text == "/objectifierinfo ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /objectifierinfo <objectifier name>");
                    }
                    //AbilityInfo help
                    else if (text == "/abilityinfo" || text == "/abilityinfo ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /abilityinfo <ability name>");
                    }
                    //FactionInfo help
                    else if (text == "/factioninfo" || text == "/factioninfo ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /factioninfo <faction name>");
                    }
                    //Quote help
                    else if (text == "/quote" || text == "/quote ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /quote <role name or abbreviation>");
                    }
                    //Quotes
                    else if (text.StartsWith("/quote "))
                    {
                        chatHandled = true;
                        inputText = text[7..];

                        if (inputText == "anarchist" || inputText == "ana")
                            chatText = "My job is that I have no job and those things over there are the only things I'm good at breaking.";
                        else if (inputText == "arsonist" || inputText == "arso")
                            chatText = "";
                        else if (inputText == "blackmailer" || inputText == "bm")
                            chatText = "How am I a good Blackmailer? Well for starters, I just tell people that I know their secrets and they believe me right away.";
                        else if (inputText == "camouflager" || inputText == "camo")
                            chatText = "Catch me? Yeah, good luck with that.";
                        else if (inputText == "cannibal" || inputText == "cann")
                            chatText = "";
                        else if (inputText == "concealer" || inputText == "conc")
                            chatText = "I swear I'm turning schizophrenic, people are appearing and disappearing in front of me.";
                        else if (inputText == "consigliere" || inputText == "consig")
                            chatText = "So then, what really are you? *Smacks desk* Want the easy way or the hard way?";
                        else if (inputText == "consort" || inputText == "cons")
                            chatText = "Last night would have been amazing, if it wasn't for the fact that my nightly partner had died.";
                        else if (inputText == "coroner" || inputText == "cor")
                            chatText = "A body? Where? I must perform an autopsy for *ahem* research.";
                        else if (inputText == "crewmate" || inputText == "crew")
                            chatText = "I once made a pencil using erasers. Just like me, it was pointless.";
                        else if (inputText == "cryomaniac" || inputText == "cryo")
                            chatText = "Turn the AC up!";
                        else if (inputText == "detective")
                            chatText = "I am skilled in identifying blood. *Looks at a body* Yup, that's definitely blood.";
                        else if (inputText == "det")
                            chatText = "I showed AD how to make ONE ROLE....AND HE FUCKING EXPLODED.";
                        else if (inputText == "disguiser" || inputText == "disg")
                            chatText = "Here, wear this for me please. I promise I won't do anything to you.";
                        else if (inputText == "dracula" || inputText == "drac")
                            chatText = "I am a great Undead who's power cannot be rivaled. SO WHY DO I KEEP DYING SO MANY TIMES!?";
                        else if (inputText == "engineer" || inputText == "engi")
                            chatText = "There's nothing my 11 PhDs can't solve.";
                        else if (inputText == "mafioso" || inputText == "mafi")
                            chatText = "Yes, boss. Got it, boss.";
                        else if (inputText == "executioner" || inputText == "exe")
                            chatText = "";
                        else if (inputText == "glitch" || inputText == "gli")
                            chatText = "";
                        else if (inputText == "godfather" || inputText == "gf")
                            chatText = "I'm going to make an offer that they can't refuse.";
                        else if (inputText == "gorgon" || inputText == "gorg")
                            chatText = "LOOK AT ME!";
                        else if (inputText == "grenadier" || inputText == "gren")
                            chatText = "All I see is white, but that's fine.";
                        else if (inputText == "impostor" || inputText == "imp")
                            chatText = "They have a better life than I have, all I've got is a knife.";
                        else if (inputText == "investigator" || inputText == "inv")
                            chatText = "I swear I'm not a stalker.";
                        else if (inputText == "guardian angel" || inputText == "ga")
                            chatText = "";
                        else if (inputText == "janitor" || inputText == "jani")
                            chatText = "I'm the guy that cleans up messes....by making even more messes. No need to thank me.";
                        else if (inputText == "jester" || inputText == "jest")
                            chatText = "";
                        else if (inputText == "juggernaut" || inputText == "jugg")
                            chatText = "Strength is the pinnacle of mankind. Gotta get those proteins in.";
                        else if (inputText == "miner" || inputText == "mine")
                            chatText = "Dig, dig, diggin' some rave, the only thing you'll be diggin' is your own grave.";
                        else if (inputText == "morphling" || inputText == "morph")
                            chatText = "*Casually observing the chaos over Green seeing Red kill.* It was me.";
                        else if (inputText == "murderer" || inputText == "murd")
                            chatText = "Ugh, my knife is getting rusty, I guess I've found my whetstone.";
                        else if (inputText == "pestilence" || inputText == "pest")
                            chatText = "You pathetic mortals cannot kill me, the demigod of disease. No...stop. NO. NO. DON'T THROW ME INTO THE LAVA. NOOOOOOOOOOOOOOOOOOO.";
                        else if (inputText == "plaguebearer" || inputText == "pb")
                            chatText = "";
                        else if (inputText == "poisoner" || inputText == "pois")
                            chatText = "So now if you mix these together, you end up creating this...thing.";
                        else if (inputText == "serial killer" || inputText == "sk")
                            chatText = "";
                        else if (inputText == "shapeshifter" || inputText == "ss")
                            chatText = "Everyone! We will be playing dress up! TOGETHER!";
                        else if (inputText == "shifter" || inputText == "shift")
                            chatText = "GET BACK HERE I WANT YOUR ROLE.";
                        else if (inputText == "survivor" || inputText == "surv")
                            chatText = "";
                        else if (inputText == "teleporter" || inputText == "tele")
                            chatText = "He's here, he's there, he's everywhere. Who are ya gonna call? Psychic friend fr-";
                        else if (inputText == "thief")
                            chatText = "";
                        else if (inputText == "time master" || inputText == "tm")
                            chatText = "That darn Time Lord, I will make him pay for taking away my position.";
                        else if (inputText == "troll")
                            chatText = "Kill me.";
                        else if (inputText == "undertaker" || inputText == "ut")
                            chatText = "The Janitor was on a strike so I exist now.";
                        else if (inputText == "warper" || inputText == "warp")
                            chatText = "Wap wap.";
                        else if (inputText == "wraith")
                            chatText = "Now you see me, now you don't.";
                        else if (inputText == "werewolf" || inputText == "ww")
                            chatText = "";
                        else
                            chatText = "Invalid input.";

                        hudManager.AddChat(player, chatText);
                    }
                    //Incorrect command
                    else if (text.StartsWith("/"))
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Invalid command.");
                    }
                }
                else if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                {
                    //Help command - lists commands available
                    if (text.StartsWith("/help"))
                    {
                        chatHandled = true;
                        var message = $"Commands available:\n/mystate,{whisper} /roleinfo, /modifierinfo, /abilityinfo, /objectifierinfo, /factioninfo, /lookup, " +
                            "/abbreviations, /quote, /credits, /controls";
                        hudManager.AddChat(player, message);
                    }
                    //This command gives the current status and description of the player
                    else if (text.StartsWith("/mystate"))
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
                    //RoleInfo help
                    else if (text == "/roleinfo" || text == "/roleinfo ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /roleinfo <role name>");
                    }
                    //ModifierInfo help
                    else if (text == "/modifierinfo" || text == "/modifierinfo ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /modifierinfo <modifier name>");
                    }
                    //ObjectifierInfo help
                    else if (text == "/objectifierinfo" || text == "/objectifierinfo ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /objectifierinfo <objectifier name>");
                    }
                    //AbilityInfo help
                    else if (text == "/abilityinfo" || text == "/abilityinfo ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /abilityinfo <ability name>");
                    }
                    //FactionInfo help
                    else if (text == "/factioninfo" || text == "/factioninfo ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /factioninfo <faction name>");
                    }
                    //AlignmentInfo help
                    else if (text == "/alignmentinfo" || text == "/alignmentinfo ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /alignmentinfo <faction name> <alignment name>");
                    }
                    //Gives information regarding roles
                    else if (text.StartsWith("/roleinfo "))
                    {
                        chatHandled = true;
                        inputText = text[10..];
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding factions
                    else if (text.StartsWith("/factioninfo "))
                    {
                        chatHandled = true;
                        inputText = text[13..];
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding alignments
                    else if (text.StartsWith("/alignmentinfo "))
                    {
                        chatHandled = true;
                        inputText = text[15..];
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding modifiers
                    else if (text.StartsWith("/modifierinfo "))
                    {
                        chatHandled = true;
                        inputText = text[14..];
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding abilities
                    else if (text.StartsWith("/abilityinfo "))
                    {
                        chatHandled = true;
                        inputText = text[13..];
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding objectifiers
                    else if (text.StartsWith("/objectifierinfo "))
                    {
                        chatHandled = true;
                        inputText = text[17..];
                        hudManager.AddChat(player, chatText);
                    }
                    //Credits
                    else if (text.StartsWith("/credits"))
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "The mod would not have been possible without these people!");
                        hudManager.AddChat(player, "Mod Creator: slushiegoose");
                        hudManager.AddChat(player, "Continued By: polus.gg");
                        hudManager.AddChat(player, "Reactivated By: eDonnes (or Donners), Term, MyDragonBreath and -H");
                        hudManager.AddChat(player, "With Help (And Code) From: Discussions, Det, Oper, -H, twix, xerminator and MyDragonBreath");
                        hudManager.AddChat(player, "Remaining credits are on the GitHub!");
                    }
                    //Quotes
                    else if (text.StartsWith("/quote "))
                    {
                        chatHandled = true;
                        inputText = text[7..];

                        if (inputText == "agent" || inputText == "ag")
                            chatText = "Hippity hoppity, your privacy is now my property.";
                        else if (inputText == "altruist" || inputText == "alt")
                            chatText = "I know what I have to do but I don't know if I have the strength to do it.";
                        else if (inputText == "amnesiac" || inputText == "amne")
                            chatText = "I forgor :skull:";
                        else if (inputText == "anarchist" || inputText == "ana")
                            chatText = "My job is that I have no job and those things over there are the only things I'm good at breaking.";
                        else if (inputText == "arsonist" || inputText == "arso")
                            chatText = "I like my meat well done.";
                        else if (inputText == "blackmailer" || inputText == "bm")
                            chatText = "How am I a good Blackmailer? Well for starters, I just tell people that I know their secrets and they believe me right away.";
                        else if (inputText == "camouflager" || inputText == "camo")
                            chatText = "Catch me? Yeah, good luck with that.";
                        else if (inputText == "cannibal" || inputText == "cann")
                            chatText = "How do you survive with no food but with a lot of people? Improvise, adapt, overcome.";
                        else if (inputText == "concealer" || inputText == "conc")
                            chatText = "I swear I'm turning schizophrenic, people are appearing and disappearing in front of me.";
                        else if (inputText == "consigliere" || inputText == "consig")
                            chatText = "So then, what really are you? *Smacks desk* Want the easy way or the hard way?";
                        else if (inputText == "consort" || inputText == "cons")
                            chatText = "Last night would have been amazing, if it wasn't for the fact that my nightly partner had died.";
                        else if (inputText == "coroner" || inputText == "cor")
                            chatText = "A body? Where? I must perform an autopsy for *ahem* research.";
                        else if (inputText == "crewmate" || inputText == "crew")
                            chatText = "Life's great mate, I totally am not jealous at the fact that others are better than me.";
                        else if (inputText == "cryomaniac" || inputText == "cryo")
                            chatText = "Turn the AC up!";
                        else if (inputText == "dampyr" || inputText == "damp")
                            chatText = "I'm way too thirsty nowadays. And Sir Dracula just keeps biting people and not giving me a chance quench it.";
                        else if (inputText == "detective" || inputText == "det")
                            chatText = "I am skilled in identifying blood. *Looks at a body* Yup, that's definitely blood.";
                        else if (inputText == "disguiser" || inputText == "disg")
                            chatText = "Here, wear this for me please. I promise I won't do anything to you.";
                        else if (inputText == "dracula" || inputText == "drac")
                            chatText = "I am a great Undead who's power cannot be rivaled. SO WHY DO I KEEP DYING SO MANY TIMES!?";
                        else if (inputText == "engineer" || inputText == "engi")
                            chatText = "There's nothing my 11 PhDs can't solve.";
                        else if (inputText == "mafioso" || inputText == "mafi")
                            chatText = "Yes, boss. Got it, boss.";
                        else if (inputText == "escort" || inputText == "esc")
                            chatText = "Today, I will make you a man.";
                        else if (inputText == "executioner" || inputText == "exe")
                            chatText = "Source: trust me bro.";
                        else if (inputText == "glitch" || inputText == "gli")
                            chatText = "Hippity hoppity, your code is now my property.";
                        else if (inputText == "godfather" || inputText == "gf")
                            chatText = "I'm going to make an offer that they can't refuse.";
                        else if (inputText == "gorgon" || inputText == "gorg")
                            chatText = "LOOK AT ME!";
                        else if (inputText == "grenadier" || inputText == "gren")
                            chatText = "All I see is white, but that's fine.";
                        else if (inputText == "impostor" || inputText == "imp")
                            chatText = "They have a better life than I have, all I've got is a knife.";
                        else if (inputText == "inspector" || inputText == "insp")
                            chatText = "THAT'S THE GODFATHER! YOU GOTTA BELIEVE ME.";
                        else if (inputText == "investigator" || inputText == "inv")
                            chatText = "I swear I'm not a stalker.";
                        else if (inputText == "guardian angel" || inputText == "ga")
                            chatText = "Hush child...Mama's here.";
                        else if (inputText == "janitor" || inputText == "jani")
                            chatText = "I'm the guy that cleans up messes....by making even more messes. No need to thank me.";
                        else if (inputText == "jester" || inputText == "jest")
                            chatText = "Hehehe I wonder if I do this...";
                        else if (inputText == "juggernaut" || inputText == "jugg")
                            chatText = "Strength is the pinnacle of mankind. Gotta get those proteins in.";
                        else if (inputText == "mayor")
                            chatText = "Yes, those votes are legitimate. No, I'm not rigging the votes.";
                        else if (inputText == "medic")
                            chatText = "Where does it hurt?";
                        else if (inputText == "medium" || inputText == "med")
                            chatText = "The voices...they are telling me that...my breath stinks?";
                        else if (inputText == "miner" || inputText == "mine")
                            chatText = "Dig, dig, diggin' some rave, the only thing you'll be diggin' is your own grave.";
                        else if (inputText == "morphling" || inputText == "morph")
                            chatText = "*Casually observing the chaos over Green seeing Red kill.* It was me.";
                        else if (inputText == "murderer" || inputText == "murd")
                            chatText = "Ugh, my knife is getting rusty, I guess I've found my whetstone.";
                        else if (inputText == "operative" || inputText == "op")
                            chatText = "The only thing you need to find out information is good placement and amazing levels of prediction.";
                        else if (inputText == "pestilence" || inputText == "pest")
                            chatText = "You pathetic mortals cannot kill me, the demigod of disease. No...stop. NO. NO. DON'T THROW ME INTO THE LAVA. NOOOOOOOOOOOOOOOOOOO.";
                        else if (inputText == "plaguebearer" || inputText == "pb")
                            chatText = "*Cough* This should surely work right? *Cough* I sure hope it does.";
                        else if (inputText == "poisoner" || inputText == "pois")
                            chatText = "So now if you mix these together, you end up creating this...thing.";
                        else if (inputText == "puppeteer" || inputText == "pup")
                            chatText = "Are you sure you wanna do that?";
                        else if (inputText == "serial killer" || inputText == "sk")
                            chatText = "My knife, WHERE'S MY KNIFE?!";
                        else if (inputText == "shapeshifter" || inputText == "ss")
                            chatText = "Everyone! We will be playing dress up! TOGETHER!";
                        else if (inputText == "sheriff" || inputText == "sher")
                            chatText = "Guys I promise I'm not an Executioner, I checked Blue and they're sus.";
                        else if (inputText == "shifter" || inputText == "shift")
                            chatText = "GET BACK HERE I WANT YOUR ROLE.";
                        else if (inputText == "survivor" || inputText == "surv")
                            chatText = "Hey listen man, I mind my own business and you mind yours. Everyone wins!";
                        else if (inputText == "swapper" || inputText == "swap")
                            chatText = "Oh no, they totally voted the other guy off. I have no idea why is everyone denying it.";
                        else if (inputText == "teleporter" || inputText == "tele")
                            chatText = "He's here, he's there, he's everywhere. Who are ya gonna call? Psychic friend fr-";
                        else if (inputText == "thief")
                            chatText = "Now it's mine.";
                        else if (inputText == "time lord" || inputText == "tl")
                            chatText = "What's better than an Altruist? An Altruist that dosent suicide!";
                        else if (inputText == "time master" || inputText == "tm")
                            chatText = "That darn Time Lord, I will make him pay for taking away my position.";
                        else if (inputText == "tracker" || inputText == "track")
                            chatText = "I only took up this job because the others were full.";
                        else if (inputText == "transporter" || inputText == "trans")
                            chatText = "You're here and you're there. Where will you go? That's for me to be.";
                        else if (inputText == "troll")
                            chatText = "Kill me.";
                        else if (inputText == "undertaker" || inputText == "ut")
                            chatText = "The Janitor was on a strike so I exist now.";
                        else if (inputText == "vampire" || inputText == "vamp")
                            chatText = "The strength of an Undead is that people will not believe in them. That said, there's someone who is mindlessly chasing us, plese help.";
                        else if (inputText == "vampire hunter" || inputText == "vh")
                            chatText = "The Dracula could be anywhere! He could be you! He could be me! He could even be- *gets voted off*";
                        else if (inputText == "veteran" || inputText == "vet")
                            chatText = "Touch me, I dare you.";
                        else if (inputText == "vigilante" || inputText == "vig")
                            chatText = "I AM THE HAND OF JUSTICE.";
                        else if (inputText == "warper" || inputText == "warp")
                            chatText = "Wap wap.";
                        else if (inputText == "wraith")
                            chatText = "Now you see me, now you don't.";
                        else if (inputText == "werewolf" || inputText == "ww")
                            chatText = "AWOOOOOOOOOOOOOOOOOOOO";
                        else
                            chatText = "Invalid input.";

                        hudManager.AddChat(player, chatText);
                    }
                    //Abbreviations help
                    else if (text == "/abbreviations" || text == "/abbreviations ")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /abbreviations <name>");
                    }
                    //Display a message (Information about the mod)
                    else if (text.StartsWith("/abbreviations ") || text.StartsWith("/ab "))
                    {
                        chatHandled = true;
                        inputText = text.StartsWith("/ab ") ? text[4..] : text[15..];
                        var tempText = inputText;
                        inputText = inputText.ToLower();
                        var requiredText = inputText.Replace("(", "");
                        requiredText = requiredText.Replace(")", "");
                        requiredText = requiredText.Replace("/", "");
                        requiredText = requiredText.Replace(" ", "");
                        var abbreviation = "";

                        if (requiredText == "abilityinfo")
                            abbreviation = "ai";
                        else if (requiredText == "setcolor" || requiredText == "setcolour")
                            abbreviation = "sc";
                        else if (requiredText == "fanatic")
                            abbreviation = "fan";
                        else if (requiredText == "lovers")
                            abbreviation = "lover";
                        else if (requiredText == "phantom")
                            abbreviation = "phan";
                        else if (requiredText == "rivals")
                            abbreviation = "rival";
                        else if (requiredText == "taskmaster")
                            abbreviation = "task";
                        else if (requiredText == "traitor")
                            abbreviation = "trait";
                        else if (requiredText == "abbreviations" || requiredText == "abbreviation")
                            abbreviation = "ab";
                        else if (requiredText == "setname")
                            abbreviation = "sn";
                        else if (requiredText == "quote")
                            abbreviation = "q";
                        else if (requiredText == "ban")
                            abbreviation = "b";
                        else if (requiredText == "kick")
                            abbreviation = "k";
                        else if (requiredText == "credits" || requiredText == "credit")
                            abbreviation = "cr";
                        else if (requiredText == "modinfo")
                            abbreviation = "mi";
                        else if (requiredText == "roleinfo")
                            abbreviation = "ri";
                        else if (requiredText == "modifierinfo")
                            abbreviation = "modi";
                        else if (requiredText == "objectifierinfo")
                            abbreviation = "oi";
                        else if (requiredText == "factioninfo")
                            abbreviation = "fi";
                        else if (requiredText == "alignmentinfo")
                            abbreviation = "ali";
                        else if (requiredText == "lookup")
                            abbreviation = "lu";
                        else if (requiredText == "agent")
                            abbreviation = "ag";
                        else if (requiredText == "altruist")
                            abbreviation = "alt";
                        else if (requiredText == "amnesiac")
                            abbreviation = "amne";
                        else if (requiredText == "anarchist")
                            abbreviation = "ana";
                        else if (requiredText == "arsonist")
                            abbreviation = "arso";
                        else if (requiredText == "blackmailer")
                            abbreviation = "bm";
                        else if (requiredText == "camouflager")
                            abbreviation = "camo";
                        else if (requiredText == "cannibal")
                            abbreviation = "cann";
                        else if (requiredText == "concealer")
                            abbreviation = "conc";
                        else if (requiredText == "consigliere")
                            abbreviation = "consig";
                        else if (requiredText == "consort")
                            abbreviation = "cons";
                        else if (requiredText == "coroner")
                            abbreviation = "cor";
                        else if (requiredText == "crewmate")
                            abbreviation = "crew";
                        else if (requiredText == "cryomaniac")
                            abbreviation = "cryo";
                        else if (requiredText == "dampyr")
                            abbreviation = "damp";
                        else if (requiredText == "detective")
                            abbreviation = "det";
                        else if (requiredText == "disguiser")
                            abbreviation = "disg";
                        else if (requiredText == "dracula")
                            abbreviation = "drac";
                        else if (requiredText == "engineer")
                            abbreviation = "engi";
                        else if (requiredText == "mafioso")
                            abbreviation = "mafi";
                        else if (requiredText == "escort")
                            abbreviation = "esc";
                        else if (requiredText == "executioner")
                            abbreviation = "exe";
                        else if (requiredText == "glitch")
                            abbreviation = "gli";
                        else if (requiredText == "godfather")
                            abbreviation = "gf";
                        else if (requiredText == "gorgon")
                            abbreviation = "gorg";
                        else if (requiredText == "grenadier")
                            abbreviation = "gren";
                        else if (requiredText == "impostor")
                            abbreviation = "imp";
                        else if (requiredText == "inspector")
                            abbreviation = "insp";
                        else if (requiredText == "investigator")
                            abbreviation = "inv";
                        else if (requiredText == "guardian angel")
                            abbreviation = "ga";
                        else if (requiredText == "janitor")
                            abbreviation = "jani";
                        else if (requiredText == "jester")
                            abbreviation = "jest";
                        else if (requiredText == "juggernaut")
                            abbreviation = "jugg";
                        else if (requiredText == "mayor")
                            abbreviation = "mayor";
                        else if (requiredText == "medic")
                            abbreviation = "medic";
                        else if (requiredText == "medium")
                            abbreviation = "med";
                        else if (requiredText == "miner")
                            abbreviation = "mine";
                        else if (requiredText == "morphling")
                            abbreviation = "morph";
                        else if (requiredText == "murderer")
                            abbreviation = "murd";
                        else if (requiredText == "operative")
                            abbreviation = "op";
                        else if (requiredText == "pestilence")
                            abbreviation = "pest";
                        else if (requiredText == "plaguebearer")
                            abbreviation = "pb";
                        else if (requiredText == "poisoner")
                            abbreviation = "pois";
                        else if (requiredText == "puppeteer")
                            abbreviation = "pup";
                        else if (requiredText == "serialkiller")
                            abbreviation = "sk";
                        else if (requiredText == "shapeshifter")
                            abbreviation = "ss";
                        else if (requiredText == "sheriff")
                            abbreviation = "sher";
                        else if (requiredText == "shifter")
                            abbreviation = "shift";
                        else if (requiredText == "survivor")
                            abbreviation = "surv";
                        else if (requiredText == "swapper")
                            abbreviation = "swap";
                        else if (requiredText == "teleporter")
                            abbreviation = "tele";
                        else if (requiredText == "thief")
                            abbreviation = "thief";
                        else if (requiredText == "timelord")
                            abbreviation = "tl";
                        else if (requiredText == "timemaster")
                            abbreviation = "tm";
                        else if (requiredText == "tracker")
                            abbreviation = "track";
                        else if (requiredText == "transporter")
                            abbreviation = "trans";
                        else if (requiredText == "troll")
                            abbreviation = "troll";
                        else if (requiredText == "undertaker")
                            abbreviation = "ut";
                        else if (requiredText == "vampire")
                            abbreviation = "vamp";
                        else if (requiredText == "vampirehunter")
                            abbreviation = "vh";
                        else if (requiredText == "veteran")
                            abbreviation = "vet";
                        else if (requiredText == "vigilante")
                            abbreviation = "vig";
                        else if (requiredText == "warper")
                            abbreviation = "warp";
                        else if (requiredText == "wraith")
                            abbreviation = "wraith";
                        else if (requiredText == "werewolf")
                            abbreviation = "ww";
                        else if (requiredText == "crew")
                            abbreviation = "crew";
                        else if (requiredText == "intruder")
                            abbreviation = "int";
                        else if (requiredText == "syndicate")
                            abbreviation = "syn";
                        else if (requiredText == "neutral")
                            abbreviation = "neut";
                        else if (requiredText == "crewinvestigative")
                            abbreviation = "ci";
                        else if (requiredText == "intrudersupport")
                            abbreviation = "is";
                        else if (requiredText == "intruderconcealing")
                            abbreviation = "ic";
                        else if (requiredText == "neutralbenign")
                            abbreviation = "nb";
                        else if (requiredText == "crewprotective")
                            abbreviation = "cp";
                        else if (requiredText == "syndicateutility")
                            abbreviation = "su";
                        else if (requiredText == "help")
                            abbreviation = "h";
                        else if (requiredText == "neutralkilling")
                            abbreviation = "nk";
                        else if (requiredText == "neutralevil")
                            abbreviation = "ne";
                        else if (requiredText == "syndicatesupport")
                            abbreviation = "ssu";
                        else if (requiredText == "crewsupport")
                            abbreviation = "cs";
                        else if (requiredText == "crewutility")
                            abbreviation = "cu";
                        else if (requiredText == "neutralproselyte")
                            abbreviation = "np";
                        else if (requiredText == "intruderdeception")
                            abbreviation = "id";
                        else if (requiredText == "neutralneophyte")
                            abbreviation = "nn";
                        else if (requiredText == "syndicatekilling")
                            abbreviation = "syk";
                        else if (requiredText == "intruderutility")
                            abbreviation = "iu";
                        else if (requiredText == "syndicatedisruption")
                            abbreviation = "sd";
                        else if (requiredText == "assassin")
                            abbreviation = "ass (don't get any funny meanings)";
                        else if (requiredText == "buttonbarry")
                            abbreviation = "bb";
                        else if (requiredText == "lighter")
                            abbreviation = "light";
                        else if (requiredText == "multitasker")
                            abbreviation = "mt";
                        else if (requiredText == "radar")
                            abbreviation = "radar";
                        else if (requiredText == "revealer")
                            abbreviation = "reveal";
                        else if (requiredText == "snitch")
                            abbreviation = "sni";
                        else if (requiredText == "tiebreaker")
                            abbreviation = "tb";
                        else if (requiredText == "torch")
                            abbreviation = "torch";
                        else if (requiredText == "tunneler")
                            abbreviation = "tunn";
                        else if (requiredText == "underdog")
                            abbreviation = "ud";
                        else if (requiredText == "bait")
                            abbreviation = "bait";
                        else if (requiredText == "coward")
                            abbreviation = "cow";
                        else if (requiredText == "diseased")
                            abbreviation = "dis";
                        else if (requiredText == "drunk")
                            abbreviation = "drunk";
                        else if (requiredText == "dwarf")
                            abbreviation = "dwarf";
                        else if (requiredText == "flincher")
                            abbreviation = "flinch";
                        else if (requiredText == "giant")
                            abbreviation = "giant";
                        else if (requiredText == "professional")
                            abbreviation = "prof";
                        else if (requiredText == "shy")
                            abbreviation = "shy";
                        else if (requiredText == "vip")
                            abbreviation = "vip";
                        else if (requiredText == "volatile")
                            abbreviation = "vol";
                        else if (requiredText == "controls")
                            abbreviation = "cont";
                        else
                            abbreviation = "Invalid input.";

                        if (abbreviation == "Invalid input.")
                            chatText = abbreviation;
                        else
                            chatText = $"The abbreviation for {inputText} is {abbreviation}!";

                        hudManager.AddChat(player, chatText);
                    }
                    else if (text == "/whisper" || text == "/w")
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Usage: /whisper <meeting number>");
                    }
                    else if (text.StartsWith("/whisper ") || text.StartsWith("/w "))
                    {
                        chatHandled = true;

                        if (!CustomGameOptions.Whispers)
                        {
                            hudManager.AddChat(player, "Whispering is not turned on.");
                        }
                        else
                        {
                            inputText = text.StartsWith("/w ") ? text[3..] : text[9..];
                            var message = text.StartsWith("/w ") ? text[4..] : text[10..];
                            var message2 = text.StartsWith("/w ") ? text[5..] : text[11..];
                            var number = inputText.Replace(message, "");
                            var number2 = inputText.Replace(message2, "");
                            number = number.Replace(" ", "");
                            number2 = number2.Replace(" ", "");
                            byte id1 = byte.Parse(number);
                            byte id2 = byte.Parse(number2);

                            var whispered = Utils.PlayerById(id1);
                            var whispered2 = Utils.PlayerById(id2);

                            if (whispered != null)
                            {
                                if (whispered.Data.IsDead)
                                {
                                    hudManager.AddChat(player, $"{whispered.name} is dead.");
                                }
                                else if (whispered.Data.Disconnected)
                                {
                                    hudManager.AddChat(player, $"{whispered.name} is not in this world anymore.");
                                }
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
                                if (whispered.Data.IsDead)
                                {
                                    hudManager.AddChat(player, $"{whispered2.name} is dead.");
                                }
                                else if (whispered.Data.Disconnected)
                                {
                                    hudManager.AddChat(player, $"{whispered2.name} is not in this world anymore.");
                                }
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
                            {
                                hudManager.AddChat(player, "Who are you trying to whisper?");
                            }
                        }
                    }
                    //Incorrect command
                    else if (text.StartsWith("/"))
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Invalid command.");
                    }
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
                    if (Input.GetKeyDown(KeyCode.LeftShift) && Classes.GameStates.IsCountDown)
                        GameStartManager.Instance.countDownTimer = 0;

                    if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S))
                        GameStartManager.Instance.Start();

                    if (Input.GetKeyDown(KeyCode.C) && Classes.GameStates.IsCountDown)
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