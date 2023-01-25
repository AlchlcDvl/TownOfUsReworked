using System;
using System.Linq;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod;
using InnerNet;
using TownOfUsReworked.Extensions;
using Reactor.Utilities;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class ChatCommands
    {
        public static System.Collections.Generic.List<string> ChatHistory = new();

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
        private static class SendChatPatch
        {
            static bool Prefix()
            {
                //Set up dictionaries and list for colours
                var coloursDict = new Dictionary<int, string>();
                coloursDict.Add(0, "Red");
                coloursDict.Add(1, "Blue");
                coloursDict.Add(2, "Green");
                coloursDict.Add(3, "Pink");
                coloursDict.Add(4, "Orange");
                coloursDict.Add(5, "Yellow");
                coloursDict.Add(6, "Black");
                coloursDict.Add(7, "White");
                coloursDict.Add(8, "Purple");
                coloursDict.Add(9, "Brown");
                coloursDict.Add(10, "Cyan");
                coloursDict.Add(11, "Lime");
                coloursDict.Add(12, "Maroon");
                coloursDict.Add(13, "Rose");
                coloursDict.Add(14, "Banana");
                coloursDict.Add(15, "Grey");
                coloursDict.Add(16, "Tan");
                coloursDict.Add(17, "Coral");
                coloursDict.Add(18, "Watermelon");
                coloursDict.Add(19, "Chocolate");
                coloursDict.Add(20, "Sky Blue");
                coloursDict.Add(21, "Biege");
                coloursDict.Add(22, "Hot Pink");
                coloursDict.Add(23, "Turquoise");
                coloursDict.Add(24, "Lilac");
                coloursDict.Add(25, "Olive");
                coloursDict.Add(26, "Azure");
                coloursDict.Add(27, "Tomato");
                coloursDict.Add(28, "Backrooms");
                coloursDict.Add(29, "Gold");
                coloursDict.Add(30, "Space");
                coloursDict.Add(31, "Ice");
                coloursDict.Add(32, "Mint");
                coloursDict.Add(33, "BTS");
                coloursDict.Add(34, "Forest Green");
                coloursDict.Add(35, "Donation");
                coloursDict.Add(36, "Cherry");
                coloursDict.Add(37, "Toy");
                coloursDict.Add(38, "Pizzaria");
                coloursDict.Add(39, "Starlight");
                coloursDict.Add(40, "Softball");
                coloursDict.Add(41, "Dark Jester");
                coloursDict.Add(42, "Fresh");
                coloursDict.Add(43, "Goner");
                coloursDict.Add(44, "Psychic Friend");
                coloursDict.Add(45, "Frost");
                coloursDict.Add(46, "Abyss Green");
                coloursDict.Add(47, "Midnight");
                coloursDict.Add(48, "<3");
                coloursDict.Add(49, "Heat From Fire");
                coloursDict.Add(50, "Fire From Heat");
                coloursDict.Add(51, "Determination");
                coloursDict.Add(52, "Patience");
                coloursDict.Add(53, "Bravery");
                coloursDict.Add(54, "Integrity");
                coloursDict.Add(55, "Perserverance");
                coloursDict.Add(56, "Kindness");
                coloursDict.Add(57, "Bravery");
                coloursDict.Add(58, "Purple Plumber");
                coloursDict.Add(59, "Rainbow");

                var hudManager = DestroyableSingleton<HudManager>.Instance.Chat;
                string text = hudManager.TextArea.text;
                string otherText = text;
                text = text.ToLower();
                string inputText = "";
                string chatText = "";
                bool chatHandled = false;
                int EatNeed = CustomGameOptions.CannibalBodyCount >= PlayerControl.AllPlayerControls._size / 2 ?
                    PlayerControl.AllPlayerControls._size / 2 : CustomGameOptions.CannibalBodyCount;
                string getWhat = CustomGameOptions.ConsigInfo == ConsigInfo.Role ? "role" : "faction";
                string setColor = TownOfUsReworked.isDev ? " /setcolour or /setcolor," : "";
                string whisper = CustomGameOptions.Whispers ? " /whisper," : "";
                //TownOfUsReworked.MessageWait.Value = (int)CustomGameOptions.ChatCooldown;

                var player = PlayerControl.LocalPlayer;

                if (ChatHistory.Count == 0 || ChatHistory[^1] != text)
                    ChatHistory.Add(text);

                if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
                {
                    //Help command - lists commands available
                    if (text == "/help" || text.StartsWith("/h"))
                    {
                        chatHandled = true;
                        var message = AmongUsClient.Instance.CanBan()
                        ? $"Commands available:\n/modinfo, /setname,{setColor} /kick, /ban, /roleinfo, /modifierinfo, /abilityinfo, /objectifierinfo, " +
                            "/factioninfo, /alignmentinfo, /quote, /abbreviations, /lookup, /credits, /controls"
                        : $"Commands available:\n/modinfo, /setname,{setColor} /roleinfo, /modifierinfo, /abilityinfo, /objectifierinfo, /factioninfo," +
                            " /alignmentinfo, /quote, /abbreviations, /lookup, /credits, /controls";
                        hudManager.AddChat(player, message);
                    }
                    //Display a message (Information about the mod)
                    else if (text.StartsWith("/modinfo") || text.StartsWith("/mi"))
                    {
                        chatHandled = true;
                        hudManager.AddChat(player, "Welcome to Town Of Us Reworked v" + TownOfUsReworked.versionFinal + "!");
                        hudManager.AddChat(player, "Town Of Us Reworked is essentially a weird mishmash of code from Town Of Us Reactivated and its forks plus some of my own code.");
                        hudManager.AddChat(player, "Credits to the parties have already been given (good luck to those who want to try to cancel me for no reason). This mod has " +
                            "several reworks and additions which I believe fit the mod better. Plus, the more layers there are, the more unique" + 
                            " a player's experience will be each game. If I've missed someone, let me know via Discord.");
                        hudManager.AddChat(player, "Now that you know the basic info, if you want to know more try using the other info commands, visiting the GitHub page at " +
                            "\nhttps://github.com/AlchlcDvl/TownOfUsReworked/ or joining my discord at \nhttps://discord.gg/cd27aDQDY9/. Good luck!");
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
                        inputText = text.StartsWith("/ab ") ? text.Substring(4) : text.Substring(15);
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
                        else if (requiredText == "puppeteer")
                            abbreviation = "pup";
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
                        else if (requiredText == "vampire")
                            abbreviation = "vamp";
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
                            chatText = "The abbreviation for " + tempText + " is " + abbreviation + "!";
                        
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
                        hudManager.AddChat(player, "With Help (And Code) From: Discussions, Det, Oper, -H and twix");
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
                        inputText = otherText.Substring(9);

                        if (!System.Text.RegularExpressions.Regex.IsMatch(inputText, @"^[a-zA-Z0-9]+$"))
                            hudManager.AddChat(player, "Name contains disallowed characters.");
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
                        int col;
                        inputText = text.StartsWith("/colour ") ? text.Substring(7) : text.Substring(6);
                        string colourSpelling = text.StartsWith("/colour ") ? "Colour" : "Color";
                        var colorString = "";

                        foreach (var color in coloursDict)
                            colorString += color + " ";

                        if (!Int32.TryParse(inputText, out col))
                        {
                            hudManager.AddChat(player, inputText + " is an invalid " + colourSpelling + ".\nYou need to use the color ID for the color you want to be. To find out a color's ID, go into the color" +
                                " selection screen and count the number of colors starting from 0 to the position of the color you want to pick. The range of colors is from 0 to 59 meaning Red to Rainbow.");
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
                        inputText = text.Substring(6);
                        PlayerControl target = PlayerControl.AllPlayerControls.ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.Equals(inputText));

                        if (target != null && AmongUsClient.Instance != null && AmongUsClient.Instance.CanBan())
                        {
                            var client = AmongUsClient.Instance.GetClient(target.OwnerId);

                            if (client != null)
                                AmongUsClient.Instance.KickPlayer(client.Id, false);
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
                        inputText = text.Substring(5);
                        PlayerControl target = PlayerControl.AllPlayerControls.ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.Equals(inputText));

                        if (target != null && AmongUsClient.Instance != null && AmongUsClient.Instance.CanBan())
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
                        inputText = text.Substring(10);

                        if (inputText == "agent" || inputText == "ag")
                        {
                            chatText = "The Agent gains more information when on Admin Table. On Admin Table, the Agent can see the colors of every person on the map." +
                                " Be careful when killing anyone, chances are that there is an Agent watching your every move.";
                        }
                        else if (inputText == "altruist" || inputText == "alt")
                        {
                            chatText = "The Altruist is capable of reviving dead players. Upon finding a dead body, the Altruist can hit their revive button, " +
                                "risking sacrificing themselves for the revival of another player. If enabled, the dead body disappears, so only they Altruist's body" +
                                " remains at the scene. After a set period of time, the player will be resurrected, if the revival isn't interrupted. If a revive is " + 
                                "successful, and you were one of the few who aided in the death of the revived player, kill them quick before your identity is revealed.";
                        }
                        else if (inputText == "amnesiac" || inputText == "amne")
                            chatText = "The Amnesiac is essentially roleless and cannot win without remembering a role from someone who has died.";
                        else if (inputText == "anarchist" || inputText == "ana")
                            chatText = "The Anarchist is just a plain Syndicate with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.";
                        else if (inputText == "arsonist" || inputText == "arso")
                        {
                            chatText = "The Arsonist is a slow killer who benefits the longer the game goes on. The Arsonist can douse other players with gasoline. " +
                                "After dousing, the Arsonist can choose to ignite all doused players which kills all doused players at once.";
                        }
                        else if (inputText == "blackmailer" || inputText == "bm")
                        {
                            chatText = "The Blackmailer can silence people in meetings. During each round, the Blackmailer can go up to someone and blackmail them. This prevents " + 
                                "the blackmailed person from speaking during the next meeting.";
                        }
                        else if (inputText == "camouflager" || inputText == "camo")
                        {
                            chatText = "The Camouflager has the ability to disrupt everyone's ability to differentiate others, causing everyone to appear grey and lackluster. " +
                                "This is very advantageous to all evils as they can commit their evil deeds while being unable to tell apart friend from foe.";
                        }
                        else if (inputText == "cannibal" || inputText == "cann")
                        {
                            chatText = $"The Cannibal can eat the body which wipes away the body, like the Janitor. They win when they eat {EatNeed}" +
                                " bodies to win. Do not let them have this chance.";
                        }
                        else if (inputText == "concealer" || inputText == "conc")
                        {
                            chatText = "The Concealer can make everyone invisible for a short while, allowing sneaky kills to be made. If you stop seeing anyone around you, it's" +
                                " probably the work of a Concealer.";
                        }
                        else if (inputText == "consigliere" || inputText == "consig")
                        {
                            chatText = $"The Consigliere is a corrupt Inspector who can find out a player's specific {getWhat}. Using this information wisely " +
                                "is a must because giving too much may give out your role.";
                        }
                        else if (inputText == "consort" || inputText == "cons")
                        {
                            chatText = "The Consort can roleblock players to stop them from using their on screen buttons. If you were blocked in a very crucial moment, blame " +
                                "the Consort.";
                        }
                        else if (inputText == "coroner" || inputText == "cor")
                        {
                            chatText = "The Coroner gets an alert when someone dies. On top of this, the Coroner briefly gets an arrow pointing in the direction of the body. " +
                                "The Coroner also gets a body report from the player they reported. The report will include the cause and time of death, player's faction/role, " +
                                "the killer's faction/role and (according to the settings) the killer's name.";
                        }
                        else if (inputText == "crewmate" || inputText == "crew")
                            chatText = "The Crewmate is just a plain Crew with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.";
                        else if (inputText == "cryomaniac" || inputText == "cryo")
                            chatText = "The Cryomaniac is someone who's obsessed with freezing everyone. Ensure you are not douse because they freeze you!";
                        else if (inputText == "dampyr" || inputText == "damp")
                        {
                            chatText = "The Dampyr is an Undead that is formed from a Dracula converting a killing role. The Dampyr can bite players to kill them." +
                                " For balancing purposes, the Dracula and the Dampyr share cooldowns to avoid them getting a quick majority.";
                        }
                        else if (inputText == "detective" || inputText == "det")
                        {
                            chatText = "The Detective can examine other players for suspicious behavior. If the examined player has killed recently, the Detective " +
                                "will be alerted about it.";
                        }
                        else if (inputText == "disguiser" || inputText == "disg")
                            chatText = "The Disguiser can change another player's appearance. This can be used to frame them or cause the spread of false clears.";
                        else if (inputText == "dracula" || inputText == "drac")
                        {
                            chatText = "The Dracula is the leader of the Undead subfaction. They have the ability to convert other people to their cause. Take them " +
                                "down quick, or else they will overrun you.";
                        }
                        else if (inputText == "engineer" || inputText == "engi")
                        {
                            chatText = "The Engineer can fix a sabotage at any point during the round. They can also vent so beware of them when killing. They " +
                                "could be lurking anywhere at any given point.";
                        }
                        else if (inputText == "mafioso" || inputText == "mafi")
                        {
                            chatText = "The Mafioso is a result of promoting a fellow Intruder via the Godfather. The whole point of the Mafioso is to get stronger off of the death " +
                                "of their leader. The Mafioso in itself is not a bad thing, but should the Godfather die while the Mafioso is still alive, the Intruders will then have " +
                                "a stronger leader";
                        }
                        else if (inputText == "escort" || inputText == "esc")
                        {
                            chatText = "The Escort can roleblock players to stop them from using their on screen buttons. If you were blocked in a very crucial moment, blame " +
                                "the Escort.";
                        }
                        else if (inputText == "executioner" || inputText == "exe")
                        {
                            chatText = "The Executioner is a crazed crewmate who only wants to see their target get ejected. Do not let the target ejected, because " +
                                "the Executioner only targets Crew.";
                        }
                        else if (inputText == "glitch" || inputText == "gli")
                        {
                            chatText = "The Glitch can hack players to prevent them from being able to use their buttons. They can also mimic others so if you see someone" +
                                " kill in front of you, chances are that their appearance is not real.";
                        }
                        else if (inputText == "godfather" || inputText == "gf")
                        {
                            chatText = "The Godfather can only spawn in 3+ Intruder games. They can choose to promote a fellow Intruder to Mafioso. When the Godfather dies, the Mafioso" +
                                " becomes the new Godfather and has lowered cooldowns.";
                        }
                        else if (inputText == "gorgon" || inputText == "gorg")
                        {
                            chatText = "The Gorgon can gaze at players which forces them to stand still. When a meeting is called, all of the frozen players will die. If you start " +
                                "seeing a lot of people standing still, chances are it's the Gorgon's work.";
                        }
                        else if (inputText == "grenadier" || inputText == "gren")
                            chatText = "The Grenadier has flashbangs which can blind players. Blinded players cannot see anything that's happening around them.";
                        else if (inputText == "impostor" || inputText == "imp")
                            chatText = "The Impostor is just a plain Intruder with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.";
                        else if (inputText == "inspector" || inputText == "insp")
                        {
                            chatText = "The Inspector can inspect players and receive a role list of what that player could be. If you claim a role, you better be prepared" +
                                " for an Inspector to check you and call you out.";
                        }
                        else if (inputText == "investigator" || inputText == "inv")
                            chatText = "The Investigator can see the footprints of players. All footprints disappear after a set amount of time and only the Investigator can see them.";
                        else if (inputText == "guardian angel" || inputText == "ga")
                        {
                            chatText = "The Guardian Angel more or less aligns themselves with the faction of their target. The Guardian Angel will win with anyone as long as their " +
                                "target lives to the end of the game, even if their target loses (like in case of a Cannibal Win and their target being an Intruder).";
                        }
                        else if (inputText == "janitor" || inputText == "jani")
                        {
                            chatText = "The Janitor can clean up bodies. Both their Kill and Clean ability have a shared cooldown, meaning they have to choose which one they want to use." +
                                " If the round goes on for too long with no bodies reported, chances are that there is a Janitor cleaning up dead bodies.";
                        }
                        else if (inputText == "jester" || inputText == "jest")
                            chatText = "The Jester has no abilities. They must make themselves appear to be evil and get ejected as a result to win.";
                        else if (inputText == "juggernaut" || inputText == "jugg")
                        {
                            chatText = "The Juggernaut's kill cooldown decreases with every kill they make. When they reach a certain number of kills, the kill cooldown no longer decreases " +
                                "and instead gives them other buffs, like bypassing protections.";
                        }
                        else if (inputText == "mayor")
                        {
                            chatText = "The Mayor can vote multiple times. The Mayor has a Vote Bank, which is the number of times they can vote. They have the option to abstain their vote " +
                                "during a meeting, adding that vote to the Vote Bank. As long as not everyone has voted, the Mayor can use as many votes from their Vote Bank as they please.";
                        }
                        else if (inputText == "medic")
                            chatText = "The Medic can give any player a shield that will make them immortal until the Medic is dead. A shielded player cannot be killed by anyone, unless it's a suicide.";
                        else if (inputText == "medium" || inputText == "med")
                        {
                            chatText = "The Medium can see ghosts. During each round the Medium has an ability called Mediate. If the Medium uses this ability, the Medium and the dead player will be " +
                                "able to see each other and communicate from beyond the grave!";
                        }
                        else if (inputText == "miner" || inputText == "mine")
                        {
                            chatText = "The Miner can create new vents. These vents only connect to each other, forming a new passageway.";
                        }
                        else if (inputText == "morphling" || inputText == "morph")
                        {
                            chatText = "The Morphling can morph into another player. At the beginning of the game and after every meeting, they can choose someone to sample. They can then morph into " +
                                "that person at any time for a limited amount of time.";
                        }
                        else if (inputText == "murderer" || inputText == "murd")
                            chatText = "The Murderer is a simple Neutral Killer with no special abilities.";
                        else if (inputText == "operative" || inputText == "op")
                        {
                            chatText = "The Operative can place bugs around the map. When players enter the range of the bug, they trigger it. In the following meeting, all players who triggered a bug " +
                                "will have their role displayed to the Operative. However, this is done so in a random order, not stating who entered the bug, nor what role a specific player is.";
                        }
                        else if (inputText == "pestilence" || inputText == "pest")
                        {
                            chatText = "Pestilence is always on permanent alert, where anyone who tries to interact with them will die. Pestilence does not spawn in-game and instead gets converted from " +
                                "Plaguebearer after they infect everyone. Pestilence cannot die unless they have been voted out, and they can't be guessed (usually).";
                        }
                        else if (inputText == "plaguebearer" || inputText == "pb")
                        {
                            chatText = "The Plaguebearer can infect other players. Once infected, the infected player can go and infect other players via interacting with them. Once all players are infected," +
                                " the Plaguebearer becomes Pestilence.";
                        }
                        else if (inputText == "poisoner" || inputText == "pois")
                            chatText = "The Poisoner can poison another player instead of killing. When they poison a player, the poisoned player dies either upon the start of the next meeting or after a set duration.";
                        else if (inputText == "puppeteer" || inputText == "pup")
                            chatText = "The Puppeteer can control a player and force them to kill someone.";
                        else if (inputText == "serial killer" || inputText == "sk")
                        {
                            chatText = "Although the Serial Killer has a kill button, they can't use it unless they are in Bloodlust. Once the Serial Killer is in bloodlust they gain the ability to kill. However, " +
                                "unlike most killers, their kill cooldown is really short for the duration of the bloodlust.";
                        }
                        else if (inputText == "shapeshifter" || inputText == "ss")
                            chatText = "The Shapeshifter can randomise everyone's appearances for a short while.";
                        else if (inputText == "sheriff" || inputText == "sher")
                            chatText = "The Sheriff can reveal the alliance of other players. Based on settings, the Sheriff can find out whether a role is Good or Evil. A player's name will change color according to their results.";
                        else if (inputText == "shifter" || inputText == "shift")
                            chatText = "The Shifter can swap roles with someone, as long as they are Crew. If the shift is unsuccessful, the Shifter will die.";
                        else if (inputText == "survivor" || inputText == "surv")
                            chatText = "The Survivor wins by simply surviving. They can vest which makes them immune to death for a short duration.";
                        else if (inputText == "swapper" || inputText == "swap")
                            chatText = "The Swapper can swap the votes on 2 players during a meeting. All the votes for the first player will instead be counted towards the second player and vice versa.";
                        else if (inputText == "teleporter" || inputText == "tele")
                            chatText = "The Teleporter can teleport to a marked positions. Once per round, the Teleporter can mark a location which they can then teleport to later in the round.";
                        else if (inputText == "thief")
                        {
                            chatText = "The Thief can kill players to steal their roles. The player, however, must be a role with the ability to kill otherwise the Thief will kill themselves. After stealing " +
                                "their target's role, the Thief can now win as whatever role they have become.";
                        }
                        else if (inputText == "time lord" || inputText == "tl")
                            chatText = "The Time Lord can rewind time and reverse the positions of all players. If enabled, any players killed during this time will be revived. Nothing but movements and kills are affected.";
                        else if (inputText == "time master" || inputText == "tm")
                        {
                            chatText = "The Time Master can freeze time, causing all non-Intruders (and the Time Lord if a certain setting is on) to freeze in place and unable to move for a short while. Time freeze and " + 
                                "sabotages cannot happen for obvious reasons.";
                        }
                        else if (inputText == "tracker" || inputText == "track")
                            chatText = "The Tracker can track other during a round. Once they track someone, an arrow is continuously pointing to them, which updates in set intervals.";
                        else if (inputText == "transporter" || inputText == "trans")
                            chatText = "The Transporter can swap the locations of two players at will. Players who have been transported are alerted with a blue flash on their screen.";
                        else if (inputText == "troll")
                        {
                            chatText = "The Troll just wants to be killed, but not ejected. Only spawns in Custom Mode. The Troll can fake interact with players. This interaction does nothing, it just triggers any " +
                                "interaction sensitive roles like Veteran and Pestilence.";
                        }
                        else if (inputText == "undertaker" || inputText == "ut")
                            chatText = "The Undertaker can drag, drop bodies and hide them in vents.";
                        else if (inputText == "vampire" || inputText == "vamp")
                            chatText = "The Vampire has no special abilities and just exists to be additional voting power for the Undead subfaction.";
                        else if (inputText == "vampire hunter" || inputText == "vh")
                        {
                            chatText = "The Vampire Hunter only spawns if there are Vampires in the game. They can check players to see if they are a Dracula, Vampire or Dampyr. When the Vampire Hunter finds them, " +
                                " the target is killed. Otherwise they only interact and nothing else happens. When all Undead are dead, the Vampire Hunter turns into a Vigilante.";
                        }
                        else if (inputText == "veteran" || inputText == "vet")
                            chatText = "The Veteran can go on alert. When the Veteran is on alert, anyone, even if Crew, who interacts with the Veteran dies.";
                        else if (inputText == "vigilante" || inputText == "vig")
                            chatText = "The Vigilante is a Crewmate that has the ability to eliminate the Intruder using their kill button. However, if they kill a Crewmate or a Neutral player they can't kill, they instead die themselves.";
                        else if (inputText == "warper" || inputText == "warp")
                            chatText = "The Warper can teleport everyone to a random vent every now and then.";
                        else if (inputText == "wraith")
                            chatText = "The Wraith can temporarily turn invisible.";
                        else if (inputText == "werewolf" || inputText == "ww")
                            chatText = "The Werewolf can kill all players within a certain radius.";
                        else
                            chatText = "Invalid input.";
                        
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding factions
                    else if (text.StartsWith("/factioninfo "))
                    {
                        chatHandled = true;
                        inputText = text.Substring(13);

                        if (inputText == "crew")
                        {
                            chatText = "The Crew is the uninformed majority of the game. They are the \"good guys\". The Crew wins if Intruders, " +
                                "Syndicate, and all Neutral Killers, Neophytes and Proselytes are dead or if they all finish their tasks.";
                        }
                        else if (inputText == "intruder" || inputText == "int")
                        {
                            chatText = "Intruders are the main \"bad guys\" of the game. They are an informed minority of the game. All roles have the " +
                                "capability to kill and sabotage, making them a pain to deal with.";
                        }
                        else if (inputText == "syndicate" || inputText == "syn")
                        {
                            chatText = "Syndicate is an \"evil\" faction that is an informed minority of the game. They have special abilities specifically " +
                                "geared towards slowing down the progress of other or causing chaos. Syndicate members, unless they are Syndicate (Killing), " +
                                "cannot kill by default. Instead they gain the ability to kill by obtaining a powerup called the Chaos Drive. The Chaos Drive " +
                                "not only boosts the holder's abilities but also gives them the ability to kill if they didn't already. If the holder can already " +
                                "kill, their kill power is increased instead.";
                        }
                        else if (inputText == "neutral" || inputText == "neut")
                            chatText = "Neutrals are essentially factionless. They are the uninformed minority of the game and can only win by themselves.";
                        else
                            chatText = "Invalid input.";
                        
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding alignments
                    else if (text.StartsWith("/alignmentinfo "))
                    {
                        chatHandled = true;
                        inputText = text.Substring(15);

                        if (inputText == "crew investigative" || inputText == "ci")
                        {
                            chatText = "Crew (Investigative) roles have the ability to gain information via special methods. Using the acquired info, " +
                                "Crew (Investigative) roles can deduce who is good and who is not.";
                        }
                        else if (inputText == "intruder support" || inputText == "is")
                        {
                            chatText = "Intruder (Support) roles are roles with miscellaneous abilities. These roles can delay players' chances of winning by" +
                                " either gaining enough info to stop them or forcing players to do things they can't.";
                        }
                        else if (inputText == "intruder concealing" || inputText == "ic")
                        {
                            chatText = "Intruder (Concealing) roles are roles that specialise in hiding information from others. If there is no new " +
                                "information, it's probably their work.";
                        }
                        else if (inputText == "neutral benign" || inputText == "nb")
                        {
                            chatText = "Neutral (Benign) roles are special roles that have the capability to win with anyone, as long as a certain " +
                                "condition is fulfilled by the end of the game.";
                        }
                        else if (inputText == "crew protective" || inputText == "cp")
                        {
                            chatText = "Crew (Protective) roles are roles that have have the capability to protect. In doing so, their targets are " +
                                "spared from final death and might even bring useful information from the dead.";
                        }
                        else if (inputText == "syndicate utility" || inputText == "su")
                            chatText = "Syndicate (Utility) roles usually don't have any special abilities and don't even appear under regaular spawn conditions.";
                        else if (inputText == "neutral killing" || inputText == "nk")
                        {
                            chatText = "Neutral (Killing) roles are roles that have the ability to kill and do not side with anyone. Each role has a special way" +
                                " to kill and gain large body counts in one go. Steer clear of them if you don't want to die.";
                        }
                        else if (inputText == "neutral evil" || inputText == "ne")
                        {
                            chatText = "Neutral (Evil) roles are roles whose objectives clash with those of other roles. They have miscellaneous win conditions" +
                                " that end the game. As such, you need to ensure they don't have a chance at winning.";
                        }
                        else if (inputText == "syndicate support" || inputText == "ssu")
                        {
                            chatText = "Syndicate (Support) roles are roles with miscellaneous abilities. They are detrimental to the Syndicate's cause and if" +
                                " used right, can greatly affect how the game continues.";
                        }
                        else if (inputText == "crew support" || inputText == "cs")
                        {
                            chatText = "Crew (Support) roles are roles with miscellaneous abilities. Try not to get lost because if you are not paying " +
                                "attention, your chances of winning will be severely decreased because of them.";
                        }
                        else if (inputText == "crew utility" || inputText == "cu")
                            chatText = "Crew (Utility) roles usually don't have any special abilities and don't even appear under regaular spawn conditions.";
                        else if (inputText == "neutral proselyte" || inputText == "np")
                        {
                            chatText = "Neutral (Proselyte) roles are roles that do not spawn at the start of the game. Instead you are converted into them." +
                                " Neutral (Proselyte) roles exist to help the leader Neutral (Neophyte) role gain a fast majority.";
                        }
                        else if (inputText == "intruder deception" || inputText == "id")
                        {
                            chatText = "Intruder (Deception) roles are roles that are built to spread misinformation. Never trust your eyes, for the killer you " +
                                "see in front of you might not be the one who they seem to be.";
                        }
                        else if (inputText == "neutral neophyte" || inputText == "nn")
                        {
                            chatText = "Neutral (Neophyte) roles are roles that can convert someone to side with them. Be careful of them, as they can easily" +
                                " overrun you with their numbers.";
                        }
                        else if (inputText == "syndicate killing" || inputText == "syk")
                        {
                            chatText = "Syndicate (Killing) roles are roles that specialise in providing body count to the Syndicate. They do not have ways to " +
                                "kill a lot of players at once, but their attacks can be very powerful.";
                        }
                        else if (inputText == "intruder utility" || inputText == "iu")
                            chatText = "Intruder (Utility) roles usually don't have any special abilities and don't even appear under regaular spawn conditions.";
                        else if (inputText == "syndicate disruption" || inputText == "sd")
                            chatText = "Syndicate (Disruption) roles are roles that a designed to change the flow of the game, via changing some mechanic.";
                        else
                            chatText = "Invalid input.";
                        
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding modifiers
                    else if (text.StartsWith("/modifierinfo "))
                    {
                        chatHandled = true;
                        inputText = text.Substring(14);

                        if (inputText == "bait")
                        {
                            chatText = "The Bait is a ticking time bomb for whoever dares to kill them. Killing them will result in the killer self reporting " +
                                "almost instantly.";
                        }
                        else if (inputText == "coward" || inputText == "cow")
                            chatText = "The Coward cannot report anyone's body at all.";
                        else if (inputText == "diseased" || inputText == "dis")
                        {
                            chatText = "The Diseased has a terrible disease that infects their killer upon death. The unfortunate killer's kill cooldown " +
                                "will be tripled for the next kill.";
                        }
                        else if (inputText == "drunk")
                            chatText = "The Drunk's controls are reversed. Traditional control no longer work on them and keybinds are randomised.";
                        else if (inputText == "dwarf")
                            chatText = "The Dwarf is a tiny boi with speed.";
                        else if (inputText == "flincher" || inputText == "flinch")
                        {
                            chatText = "The Flincher will occasioanlly flinch causing them to stop for a moment. This is super unhelpful if you're trying to catch" +
                                " and kill someone or when you are trying to run away from a killer.";
                        }
                        else if (inputText == "giant")
                            chatText = "The Giant is a big boi with lower speed.";
                        else if (inputText == "professional" || inputText == "prof")
                            chatText = "The Professional is so good at avoiding dying via assassination that they are able to dodge one misfire.";
                        else if (inputText == "shy")
                            chatText = "The Shy player cannot call a meeting because they just can't seem to be able to talk in front of a crowd.";
                        else if (inputText == "vip")
                            chatText = "If the VIP is killed, all players will be alerted to their death and the players' screens will flash in the VIP's role's color.";
                        else if (inputText == "volatile" || inputText == "vol")
                            chatText = "The Volatile player player cannot distinguish sounds and hallucinations from reality, causing them to see, hear and feel things at random";
                        else
                            chatText = "Invalid input.";
                        
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding abilities
                    else if (text.StartsWith("/abilityinfo "))
                    {
                        chatHandled = true;
                        inputText = text.Substring(13);

                        if (inputText == "assassin" || inputText == "ass")
                        {
                            chatText = "The Assassin can kill players during a meeting by guessing their target's role. Be careful though, as guessing incorrectly " +
                                "will kill you instead.";
                            
                            if (inputText == "ass")
                                chatText += " I know what you tried to look up. ;)";
                        }
                        else if (inputText == "button barry" || inputText == "bb")
                            chatText = "The Button Barry can call a meeting from anywhere, at the cost of their vision.";
                        else if (inputText == "lighter" || inputText == "light")
                            chatText = "The Lighter has higher vision.";
                        else if (inputText == "multitasker" || inputText == "mt")
                            chatText = "The Multitasker can...well...multitask. When on a task, the task menu will appear to be translucent to allow observing others.";
                        else if (inputText == "radar")
                            chatText = "The Radar always has an arrow pointing at the nearest player, regardless of their position or location.";
                        else if (inputText == "revealer" || inputText == "reveal")
                        {
                            chatText = "The Revealer cannot do anything by themselves while alive. But when the Revealer dies, their tasks reset and they appear to be" +
                                " a faded bean to everyone else. The Revealer must finish their tasks without getting clicked on by evil natured players and on doing so" +
                                " all evils will be revealed to the Crew.";
                        }
                        else if (inputText == "snitch" || inputText == "sni")
                            chatText = "The Snitch needs to finish their tasks in order to have all evil natured players be revealed to them.";
                        else if (inputText == "tiebreaker" || inputText == "tb")
                            chatText = "In the case of a tie between votes, whatever the Tiebreaker voted for will happen.";
                        else if (inputText == "torch")
                            chatText = "The Torch's vision is not affected by the lights sabotage.";
                        else if (inputText == "tunneler" || inputText == "tunn")
                            chatText = "The Tunneler gains the ability to vent when they finish their tasks.";
                        else if (inputText == "underdog" || inputText == "ud")
                        {
                            chatText = "The Underdog has a higher cooldown on all abilities when their team mates are still alive. After the Underdog is alone, all cooldowns" +
                                " will be decreased.";
                        }
                        else
                            chatText = "Invalid input.";
                        
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding objectifiers
                    else if (text.StartsWith("/objectifierinfo "))
                    {
                        chatHandled = true;
                        inputText = text.Substring(17);

                        if (inputText == "fanatic" || inputText == "fan")
                        {
                            chatText = "The Fanatic is a regular crewmate until they are attacked by someone. The attacker's role and identity will by revealed" +
                                " to the Fanatic and the Fanatic now wins with the attacker, regardless of the Fanatic's original role.";
                        }
                        else if (inputText == "lovers" || inputText == "lover")
                        {
                            chatText = "Lovers are a pair of players whose fates are linked together. If one Lover dies and the setting allows, the other Lover" +
                                " will also die, ragardless of their locations. If the Lovers are 2 out of the 3 final players alive, they achieve a special Lovers" +
                                " only win. Intruders cannot be Lovers with other Intruders and same applies for the Syndicate.";
                        }
                        else if (inputText == "phantom" || inputText == "phan")
                        {
                            chatText = "The Phantom cannot win as a Phantom when they are alive. Instead they need to die. Upon death, the Phantom has a faded bean" +
                                " appearance which everyone can see. The Phantom needs to finish their tasks without getting clicked to win.";
                        }
                        else if (inputText == "rivals" || inputText == "rival")
                        {
                            chatText = "Rivals are a pair of player who despise each other. Rivals cannot win together when alive, even if they belong to the same " +
                                "faction. For a Rival to win, they need their other Rival to die not by their hands. A Killing Rival cannot attack their Rival and " +
                                "neither Rivals can vote each other. A Rival wins if they manage to kill the other Rival and live to be 1 of the final 2 players alive.";
                        }
                        else if (inputText == "taskmaster" || inputText == "task")
                            chatText = "The Taskmaster needs to finish their tasks before a certain number of meetings are called in order to win.";
                        else if (inputText == "traitor" || inputText == "trait")
                        {
                            chatText = "The Traitor is a crewmate who needs to finish their tasks to switch sides. Upon finishing their tasks, the Traitor has a 50%" +
                                " chance to side with either the Intruders or the Syndicate and win with them. If they happen to be the last of the factioned evils " +
                                "the Traitor loses all of their current role's abilities and instead gain the ability to kill.";
                        }
                        else
                            chatText = "Invalid input.";
                        
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
                        inputText = text.Substring(7);
                        
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
                            chatText = "I'm way too thirsty nowadays. And Sir Dracula just keeps biting people and not giving me a chance quench my thirst.";
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
                            chatText = "Um, those votes are legitimate. No, I'm not rigging the votes.";
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
                            chatText = "Hey listen man, I mind my own business and you do you. Everyone wins!";
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
                        {
                            hudManager.AddChat(player, role.FactionDescription);
                            hudManager.AddChat(player, role.AlignmentDescription);
                            hudManager.AddChat(player, role.RoleDescription);
                        }
                        
                        if (modifier != null)
                            hudManager.AddChat(player, modifier.ModifierDescription);
                        
                        if (objectifier != null)
                            hudManager.AddChat(player, objectifier.ObjectifierDescription);
                        
                        if (ability != null)
                            hudManager.AddChat(player, ability.AbilityDescription);
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
                        inputText = text.Substring(10);

                        if (inputText == "agent" || inputText == "ag")
                        {
                            chatText = "The Agent gains more information when on Admin Table. On Admin Table, the Agent can see the colors of every person on the map." +
                                " Be careful when killing anyone, chances are that there is an Agent watching your every move.";
                        }
                        else if (inputText == "altruist" || inputText == "alt")
                        {
                            chatText = "The Altruist is capable of reviving dead players. Upon finding a dead body, the Altruist can hit their revive button, " +
                                "risking sacrificing themselves for the revival of another player. If enabled, the dead body disappears, so only they Altruist's body" +
                                " remains at the scene. After a set period of time, the player will be resurrected, if the revival isn't interrupted. If a revive is " + 
                                "successful, and you were one of the few who aided in the death of the revived player, kill them quick before your identity is revealed.";
                        }
                        else if (inputText == "amnesiac" || inputText == "amne")
                            chatText = "The Amnesiac is essentially roleless and cannot win without remembering a role from someone who has died.";
                        else if (inputText == "anarchist" || inputText == "ana")
                            chatText = "The Anarchist is just a plain Syndicate with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.";
                        else if (inputText == "arsonist" || inputText == "arso")
                        {
                            chatText = "The Arsonist is a slow killer who benefits the longer the game goes on. The Arsonist can douse other players with gasoline. " +
                                "After dousing, the Arsonist can choose to ignite all doused players which kills all doused players at once.";
                        }
                        else if (inputText == "blackmailer" || inputText == "bm")
                        {
                            chatText = "The Blackmailer can silence people in meetings. During each round, the Blackmailer can go up to someone and blackmail them. This prevents " + 
                                "the blackmailed person from speaking during the next meeting.";
                        }
                        else if (inputText == "camouflager" || inputText == "camo")
                        {
                            chatText = "The Camouflager has the ability to disrupt everyone's ability to differentiate others, causing everyone to appear grey and lackluster. " +
                                "This is very advantageous to all evils as they can commit their evil deeds while being unable to tell apart friend from foe.";
                        }
                        else if (inputText == "cannibal" || inputText == "cann")
                        {
                            chatText = $"The Cannibal can eat the body which wipes away the body, like the Janitor. They win when they eat {EatNeed}" +
                                " bodies to win. Do not let them have this chance.";
                        }
                        else if (inputText == "concealer" || inputText == "conc")
                        {
                            chatText = "The Concealer can make everyone invisible for a short while, allowing sneaky kills to be made. If you stop seeing anyone around you, it's" +
                                " probably the work of a Concealer.";
                        }
                        else if (inputText == "consigliere" || inputText == "consig")
                        {
                            chatText = $"The Consigliere is a corrupt Inspector who can find out a player's specific {getWhat}. Using this information wisely " +
                                "is a must because giving too much may give out your role.";
                        }
                        else if (inputText == "consort" || inputText == "cons")
                        {
                            chatText = "The Consort can roleblock players to stop them from using their on screen buttons. If you were blocked in a very crucial moment, blame " +
                                "the Consort.";
                        }
                        else if (inputText == "coroner" || inputText == "cor")
                        {
                            chatText = "The Coroner gets an alert when someone dies. On top of this, the Coroner briefly gets an arrow pointing in the direction of the body. " +
                                "The Coroner also gets a body report from the player they reported. The report will include the cause and time of death, player's faction/role, " +
                                "the killer's faction/role and (according to the settings) the killer's name.";
                        }
                        else if (inputText == "crewmate" || inputText == "crew")
                            chatText = "The Crewmate is just a plain Crew with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.";
                        else if (inputText == "cryomaniac" || inputText == "cryo")
                            chatText = "The Cryomaniac is someone who's obsessed with freezing everyone. Ensure you are not douse because they freeze you!";
                        else if (inputText == "dampyr" || inputText == "damp")
                        {
                            chatText = "The Dampyr is an Undead that is formed from a Dracula converting a killing role. The Dampyr can bite players to kill them." +
                                " For balancing purposes, the Dracula and the Dampyr share cooldowns to avoid them getting a quick majority.";
                        }
                        else if (inputText == "detective" || inputText == "det")
                        {
                            chatText = "The Detective can examine other players for suspicious behavior. If the examined player has killed recently, the Detective " +
                                "will be alerted about it.";
                        }
                        else if (inputText == "disguiser" || inputText == "disg")
                            chatText = "The Disguiser can change another player's appearance. This can be used to frame them or cause the spread of false clears.";
                        else if (inputText == "dracula" || inputText == "drac")
                        {
                            chatText = "The Dracula is the leader of the Undead subfaction. They have the ability to convert other people to their cause. Take them " +
                                "down quick, or else they will overrun you.";
                        }
                        else if (inputText == "engineer" || inputText == "engi")
                        {
                            chatText = "The Engineer can fix a sabotage at any point during the round. They can also vent so beware of them when killing. They " +
                                "could be lurking anywhere at any given point.";
                        }
                        else if (inputText == "mafioso" || inputText == "mafi")
                        {
                            chatText = "The Mafioso is a result of promoting a fellow Intruder via the Godfather. The whole point of the Mafioso is to get stronger off of the death " +
                                "of their leader. The Mafioso in itself is not a bad thing, but should the Godfather die while the Mafioso is still alive, the Intruders will then have " +
                                "a stronger leader";
                        }
                        else if (inputText == "escort" || inputText == "esc")
                        {
                            chatText = "The Escort can roleblock players to stop them from using their on screen buttons. If you were blocked in a very crucial moment, blame " +
                                "the Escort.";
                        }
                        else if (inputText == "executioner" || inputText == "exe")
                        {
                            chatText = "The Executioner is a crazed crewmate who only wants to see their target get ejected. Do not let the target ejected, because " +
                                "the Executioner only targets Crew.";
                        }
                        else if (inputText == "glitch" || inputText == "gli")
                        {
                            chatText = "The Glitch can hack players to prevent them from being able to use their buttons. They can also mimic others so if you see someone" +
                                " kill in front of you, chances are that their appearance is not real.";
                        }
                        else if (inputText == "godfather" || inputText == "gf")
                        {
                            chatText = "The Godfather can only spawn in 3+ Intruder games. They can choose to promote a fellow Intruder to Mafioso. When the Godfather dies, the Mafioso" +
                                " becomes the new Godfather and has lowered cooldowns.";
                        }
                        else if (inputText == "gorgon" || inputText == "gorg")
                        {
                            chatText = "The Gorgon can gaze at players which forces them to stand still. When a meeting is called, all of the frozen players will die. If you start " +
                                "seeing a lot of people standing still, chances are it's the Gorgon's work.";
                        }
                        else if (inputText == "grenadier" || inputText == "gren")
                            chatText = "The Grenadier has flashbangs which can blind players. Blinded players cannot see anything that's happening around them.";
                        else if (inputText == "impostor" || inputText == "imp")
                            chatText = "The Impostor is just a plain Intruder with no abilities and only spawns if all the other roles are taken or set to spawn in Custom mode.";
                        else if (inputText == "inspector" || inputText == "insp")
                        {
                            chatText = "The Inspector can inspect players and receive a role list of what that player could be. If you claim a role, you better be prepared" +
                                " for an Inspector to check you and call you out.";
                        }
                        else if (inputText == "investigator" || inputText == "inv")
                            chatText = "The Investigator can see the footprints of players. All footprints disappear after a set amount of time and only the Investigator can see them.";
                        else if (inputText == "guardian angel" || inputText == "ga")
                        {
                            chatText = "The Guardian Angel more or less aligns themselves with the faction of their target. The Guardian Angel will win with anyone as long as their " +
                                "target lives to the end of the game, even if their target loses (like in case of a Cannibal Win and their target being an Intruder).";
                        }
                        else if (inputText == "janitor" || inputText == "jani")
                        {
                            chatText = "The Janitor can clean up bodies. Both their Kill and Clean ability have a shared cooldown, meaning they have to choose which one they want to use." +
                                " If the round goes on for too long with no bodies reported, chances are that there is a Janitor cleaning up dead bodies.";
                        }
                        else if (inputText == "jester" || inputText == "jest")
                            chatText = "The Jester has no abilities. They must make themselves appear to be evil and get ejected as a result to win.";
                        else if (inputText == "juggernaut" || inputText == "jugg")
                        {
                            chatText = "The Juggernaut's kill cooldown decreases with every kill they make. When they reach a certain number of kills, the kill cooldown no longer decreases " +
                                "and instead gives them other buffs, like bypassing protections.";
                        }
                        else if (inputText == "mayor")
                        {
                            chatText = "The Mayor can vote multiple times. The Mayor has a Vote Bank, which is the number of times they can vote. They have the option to abstain their vote " +
                                "during a meeting, adding that vote to the Vote Bank. As long as not everyone has voted, the Mayor can use as many votes from their Vote Bank as they please.";
                        }
                        else if (inputText == "medic")
                            chatText = "The Medic can give any player a shield that will make them immortal until the Medic is dead. A shielded player cannot be killed by anyone, unless it's a suicide.";
                        else if (inputText == "medium" || inputText == "med")
                        {
                            chatText = "The Medium can see ghosts. During each round the Medium has an ability called Mediate. If the Medium uses this ability, the Medium and the dead player will be " +
                                "able to see each other and communicate from beyond the grave!";
                        }
                        else if (inputText == "miner" || inputText == "mine")
                        {
                            chatText = "The Miner can create new vents. These vents only connect to each other, forming a new passageway.";
                        }
                        else if (inputText == "morphling" || inputText == "morph")
                        {
                            chatText = "The Morphling can morph into another player. At the beginning of the game and after every meeting, they can choose someone to sample. They can then morph into " +
                                "that person at any time for a limited amount of time.";
                        }
                        else if (inputText == "murderer" || inputText == "murd")
                            chatText = "The Murderer is a simple Neutral Killer with no special abilities.";
                        else if (inputText == "operative" || inputText == "op")
                        {
                            chatText = "The Operative can place bugs around the map. When players enter the range of the bug, they trigger it. In the following meeting, all players who triggered a bug " +
                                "will have their role displayed to the Operative. However, this is done so in a random order, not stating who entered the bug, nor what role a specific player is.";
                        }
                        else if (inputText == "pestilence" || inputText == "pest")
                        {
                            chatText = "Pestilence is always on permanent alert, where anyone who tries to interact with them will die. Pestilence does not spawn in-game and instead gets converted from " +
                                "Plaguebearer after they infect everyone. Pestilence cannot die unless they have been voted out, and they can't be guessed (usually).";
                        }
                        else if (inputText == "plaguebearer" || inputText == "pb")
                        {
                            chatText = "The Plaguebearer can infect other players. Once infected, the infected player can go and infect other players via interacting with them. Once all players are infected," +
                                " the Plaguebearer becomes Pestilence.";
                        }
                        else if (inputText == "poisoner" || inputText == "pois")
                            chatText = "The Poisoner can poison another player instead of killing. When they poison a player, the poisoned player dies either upon the start of the next meeting or after a set duration.";
                        else if (inputText == "puppeteer" || inputText == "pup")
                            chatText = "The Puppeteer can control a player and force them to kill someone.";
                        else if (inputText == "serial killer" || inputText == "sk")
                        {
                            chatText = "Although the Serial Killer has a kill button, they can't use it unless they are in Bloodlust. Once the Serial Killer is in bloodlust they gain the ability to kill. However, " +
                                "unlike most killers, their kill cooldown is really short for the duration of the bloodlust.";
                        }
                        else if (inputText == "shapeshifter" || inputText == "ss")
                            chatText = "The Shapeshifter can randomise everyone's appearances for a short while.";
                        else if (inputText == "sheriff" || inputText == "sher")
                            chatText = "The Sheriff can reveal the alliance of other players. Based on settings, the Sheriff can find out whether a role is Good or Evil. A player's name will change color according to their results.";
                        else if (inputText == "shifter" || inputText == "shift")
                            chatText = "The Shifter can swap roles with someone, as long as they are Crew. If the shift is unsuccessful, the Shifter will die.";
                        else if (inputText == "survivor" || inputText == "surv")
                            chatText = "The Survivor wins by simply surviving. They can vest which makes them immune to death for a short duration.";
                        else if (inputText == "swapper" || inputText == "swap")
                            chatText = "The Swapper can swap the votes on 2 players during a meeting. All the votes for the first player will instead be counted towards the second player and vice versa.";
                        else if (inputText == "teleporter" || inputText == "tele")
                            chatText = "The Teleporter can teleport to a marked positions. Once per round, the Teleporter can mark a location which they can then teleport to later in the round.";
                        else if (inputText == "thief")
                        {
                            chatText = "The Thief can kill players to steal their roles. The player, however, must be a role with the ability to kill otherwise the Thief will kill themselves. After stealing " +
                                "their target's role, the Thief can now win as whatever role they have become.";
                        }
                        else if (inputText == "time lord" || inputText == "tl")
                            chatText = "The Time Lord can rewind time and reverse the positions of all players. If enabled, any players killed during this time will be revived. Nothing but movements and kills are affected.";
                        else if (inputText == "time master" || inputText == "tm")
                        {
                            chatText = "The Time Master can freeze time, causing all non-Intruders (and the Time Lord if a certain setting is on) to freeze in place and unable to move for a short while. Time freeze and " + 
                                "sabotages cannot happen for obvious reasons.";
                        }
                        else if (inputText == "tracker" || inputText == "track")
                            chatText = "The Tracker can track other during a round. Once they track someone, an arrow is continuously pointing to them, which updates in set intervals.";
                        else if (inputText == "transporter" || inputText == "trans")
                            chatText = "The Transporter can swap the locations of two players at will. Players who have been transported are alerted with a blue flash on their screen.";
                        else if (inputText == "troll")
                        {
                            chatText = "The Troll just wants to be killed, but not ejected. Only spawns in Custom Mode. The Troll can fake interact with players. This interaction does nothing, it just triggers any " +
                                "interaction sensitive roles like Veteran and Pestilence.";
                        }
                        else if (inputText == "undertaker" || inputText == "ut")
                            chatText = "The Undertaker can drag, drop bodies and hide them in vents.";
                        else if (inputText == "vampire" || inputText == "vamp")
                            chatText = "The Vampire has no special abilities and just exists to be additional voting power for the Undead subfaction.";
                        else if (inputText == "vampire hunter" || inputText == "vh")
                        {
                            chatText = "The Vampire Hunter only spawns if there are Vampires in the game. They can check players to see if they are a Dracula, Vampire or Dampyr. When the Vampire Hunter finds them, " +
                                " the target is killed. Otherwise they only interact and nothing else happens. When all Undead are dead, the Vampire Hunter turns into a Vigilante.";
                        }
                        else if (inputText == "veteran" || inputText == "vet")
                            chatText = "The Veteran can go on alert. When the Veteran is on alert, anyone, even if Crew, who interacts with the Veteran dies.";
                        else if (inputText == "vigilante" || inputText == "vig")
                            chatText = "The Vigilante is a Crewmate that has the ability to eliminate the Intruder using their kill button. However, if they kill a Crewmate or a Neutral player they can't kill, they instead die themselves.";
                        else if (inputText == "warper" || inputText == "warp")
                            chatText = "The Warper can teleport everyone to a random vent every now and then.";
                        else if (inputText == "wraith")
                            chatText = "The Wraith can temporarily turn invisible.";
                        else if (inputText == "werewolf" || inputText == "ww")
                            chatText = "The Werewolf can kill all players within a certain radius.";
                        else
                            chatText = "Invalid input.";
                        
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding factions
                    else if (text.StartsWith("/factioninfo "))
                    {
                        chatHandled = true;
                        inputText = text.Substring(13);

                        if (inputText == "crew")
                        {
                            chatText = "The Crew is the uninformed majority of the game. They are the \"good guys\". The Crew wins if Intruders, " +
                                "Syndicate, and all Neutral Killers, Neophytes and Proselytes are dead or if they all finish their tasks.";
                        }
                        else if (inputText == "intruder")
                        {
                            chatText = "Intruders are the main \"bad guys\" of the game. All roles have the capability to kill and sabotage, making " +
                                "them a pain to deal with.";
                        }
                        else if (inputText == "syndicate")
                        {
                            chatText = "Syndicate is an \"evil\" faction that is an informed minority of the game. They have special abilities specifically " +
                                "geared towards slowing down the progress of other or causing chaos. Syndicate members, unless they are Syndicate (Killing), " +
                                "cannot kill by default. Instead they gain the ability to kill by obtaining a powerup called the Chaos Drive. The Chaos Drive " +
                                "not only boosts the holder's abilities but also gives them the ability to kill if they didn't already. If the holder can already " +
                                "kill, their kill power is increased instead.";
                        }
                        else if (inputText == "neutral")
                            chatText = "Neutrals are essentially factionless. They are the uninformed minority of the game and can only win by themselves.";
                        else
                            chatText = "Invalid input.";
                        
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding alignments
                    else if (text.StartsWith("/alignmentinfo "))
                    {
                        chatHandled = true;
                        inputText = text.Substring(15);

                        if (inputText == "crew investigative" || inputText == "ci")
                        {
                            chatText = "Crew (Investigative) roles have the ability to gain information via special methods. Using the acquired info, " +
                                "Crew (Investigative) roles can deduce who is good and who is not.";
                        }
                        else if (inputText == "intruder support" || inputText == "is")
                        {
                            chatText = "Intruder (Support) roles are roles with miscellaneous abilities. These roles can delay players' chances of winning by" +
                                " either gaining enough info to stop them or forcing players to do things they can't.";
                        }
                        else if (inputText == "intruder concealing" || inputText == "ic")
                        {
                            chatText = "Intruder (Concealing) roles are roles that specialise in hiding information from others. If there is no new " +
                                "information, it's probably their work.";
                        }
                        else if (inputText == "neutral benign" || inputText == "nb")
                        {
                            chatText = "Neutral (Benign) roles are special roles that have the capability to win with anyone, as long as a certain " +
                                "condition is fulfilled by the end of the game.";
                        }
                        else if (inputText == "crew protective" || inputText == "cp")
                        {
                            chatText = "Crew (Protective) roles are roles that have have the capability to protect. In doing so, their targets are " +
                                "spared from final death and might even bring useful information from the dead.";
                        }
                        else if (inputText == "syndicate utility" || inputText == "su")
                            chatText = "Syndicate (Utility) roles usually don't have any special abilities and don't even appear under regaular spawn conditions.";
                        else if (inputText == "neutral killing" || inputText == "nk")
                        {
                            chatText = "Neutral (Killing) roles are roles that have the ability to kill and do not side with anyone. Each role has a special way" +
                                " to kill and gain large body counts in one go. Steer clear of them if you don't want to die.";
                        }
                        else if (inputText == "neutral evil" || inputText == "ne")
                        {
                            chatText = "Neutral (Evil) roles are roles whose objectives clash with those of other roles. They have miscellaneous win conditions" +
                                " that end the game. As such, you need to ensure they don't have a chance at winning.";
                        }
                        else if (inputText == "syndicate support" || inputText == "ssu")
                        {
                            chatText = "Syndicate (Support) roles are roles with miscellaneous abilities. They are detrimental to the Syndicate's cause and if" +
                                " used right, can greatly affect how the game continues.";
                        }
                        else if (inputText == "crew support" || inputText == "cs")
                        {
                            chatText = "Crew (Support) roles are roles with miscellaneous abilities. Try not to get lost because if you are not paying " +
                                "attention, your chances of winning will be severely decreased because of them.";
                        }
                        else if (inputText == "crew utility" || inputText == "cu")
                            chatText = "Crew (Utility) roles usually don't have any special abilities and don't even appear under regaular spawn conditions.";
                        else if (inputText == "neutral proselyte" || inputText == "np")
                        {
                            chatText = "Neutral (Proselyte) roles are roles that do not spawn at the start of the game. Instead you are converted into them." +
                                " Neutral (Proselyte) roles exist to help the leader Neutral (Neophyte) role gain a fast majority.";
                        }
                        else if (inputText == "intruder deception" || inputText == "id")
                        {
                            chatText = "Intruder (Deception) roles are roles that are built to spread misinformation. Never trust your eyes, for the killer you " +
                                "see in front of you might not be the one who they seem to be.";
                        }
                        else if (inputText == "neutral neophyte" || inputText == "nn")
                        {
                            chatText = "Neutral (Neophyte) roles are roles that can convert someone to side with them. Be careful of them, as they can easily" +
                                " overrun you with their numbers.";
                        }
                        else if (inputText == "syndicate killing" || inputText == "syk")
                        {
                            chatText = "Syndicate (Killing) roles are roles that specialise in providing body count to the Syndicate. They do not have ways to " +
                                "kill a lot of players at once, but their attacks can be very powerful.";
                        }
                        else if (inputText == "intruder utility" || inputText == "iu")
                            chatText = "Intruder (Utility) roles usually don't have any special abilities and don't even appear under regaular spawn conditions.";
                        else if (inputText == "syndicate disruption" || inputText == "sd")
                            chatText = "Syndicate (Disruption) roles are roles that a designed to change the flow of the game, via changing some mechanic.";
                        else
                            chatText = "Invalid input.";
                        
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding modifiers
                    else if (text.StartsWith("/modifierinfo "))
                    {
                        chatHandled = true;
                        inputText = text.Substring(14);

                        if (inputText == "bait")
                        {
                            chatText = "The Bait is a ticking time bomb for whoever dares to kill them. Killing them will result in the killer self reporting " +
                                "almost instantly.";
                        }
                        else if (inputText == "coward" || inputText == "cow")
                            chatText = "The Coward cannot report anyone's body at all.";
                        else if (inputText == "diseased" || inputText == "dis")
                        {
                            chatText = "The Diseased has a terrible disease that infects their killer upon death. The unfortunate killer's kill cooldown " +
                                "will be tripled for the next kill.";
                        }
                        else if (inputText == "drunk")
                            chatText = "The Drunk's controls are reversed. Traditional control no longer work on them and keybinds are randomised.";
                        else if (inputText == "dwarf")
                            chatText = "The Dwarf is a tiny boi with speed.";
                        else if (inputText == "flincher" || inputText == "flinch")
                        {
                            chatText = "The Flincher will occasioanlly flinch causing them to stop for a moment. This is super unhelpful if you're trying to catch" +
                                " and kill someone or when you are trying to run away from a killer.";
                        }
                        else if (inputText == "giant")
                            chatText = "The Giant is a big boi with lower speed.";
                        else if (inputText == "professional" || inputText == "prof")
                            chatText = "The Professional is so good at avoiding dying via assassination that they are able to dodge one misfire.";
                        else if (inputText == "shy")
                            chatText = "The Shy player cannot call a meeting because they just can't seem to be able to talk in front of a crowd.";
                        else if (inputText == "vip")
                            chatText = "If the VIP is killed, all players will be alerted to their death and the players' screens will flash in the VIP's role's color.";
                        else if (inputText == "volatile" || inputText == "vol")
                            chatText = "The Volatile player player cannot distinguish sounds and hallucinations from reality, causing them to see, hear and feel things at random";
                        else
                            chatText = "Invalid input.";
                        
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding abilities
                    else if (text.StartsWith("/abilityinfo "))
                    {
                        chatHandled = true;
                        inputText = text.Substring(13);

                        if (inputText == "assassin" || inputText == "ass")
                        {
                            chatText = "The Assassin can kill players during a meeting by guessing their target's role. Be careful though, as guessing incorrectly " +
                                "will kill you instead.";
                            
                            if (inputText == "ass")
                                chatText += " I know what you tried to look up. ;)";
                        }
                        else if (inputText == "button barry" || inputText == "bb")
                            chatText = "The Button Barry can call a meeting from anywhere, at the cost of their vision.";
                        else if (inputText == "lighter" || inputText == "light")
                            chatText = "The Lighter has higher vision.";
                        else if (inputText == "multitasker" || inputText == "mt")
                            chatText = "The Multitasker can...well...multitask. When on a task, the task menu will appear to be translucent to allow observing others.";
                        else if (inputText == "radar")
                            chatText = "The Radar always has an arrow pointing at the nearest player, regardless of their position or location.";
                        else if (inputText == "revealer" || inputText == "reveal")
                        {
                            chatText = "The Revealer cannot do anything by themselves while alive. But when the Revealer dies, their tasks reset and they appear to be" +
                                " a faded bean to everyone else. The Revealer must finish their tasks without getting clicked on by evil natured players and on doing so" +
                                " all evils will be revealed to the Crew.";
                        }
                        else if (inputText == "snitch" || inputText == "sni")
                            chatText = "The Snitch needs to finish their tasks in order to have all evil natured players be revealed to them.";
                        else if (inputText == "tiebreaker" || inputText == "tb")
                            chatText = "In the case of a tie between votes, whatever the Tiebreaker voted for will happen.";
                        else if (inputText == "torch")
                            chatText = "The Torch's vision is not affected by the lights sabotage.";
                        else if (inputText == "tunneler" || inputText == "tunn")
                            chatText = "The Tunneler gains the ability to vent when they finish their tasks.";
                        else if (inputText == "underdog" || inputText == "ud")
                        {
                            chatText = "The Underdog has a higher cooldown on all abilities when their team mates are still alive. After the Underdog is alone, all cooldowns" +
                                " will be decreased.";
                        }
                        else
                            chatText = "Invalid input.";
                        
                        hudManager.AddChat(player, chatText);
                    }
                    //Gives information regarding objectifiers
                    else if (text.StartsWith("/objectifierinfo "))
                    {
                        chatHandled = true;
                        inputText = text.Substring(17);

                        if (inputText == "fanatic" || inputText == "fan")
                        {
                            chatText = "The Fanatic is a regular crewmate until they are attacked by someone. The attacker's role and identity will by revealed" +
                                " to the Fanatic and the Fanatic now wins with the attacker, regardless of the Fanatic's original role.";
                        }
                        else if (inputText == "lovers" || inputText == "lover")
                        {
                            chatText = "Lovers are a pair of players whose fates are linked together. If one Lover dies and the setting allows, the other Lover" +
                                " will also die, ragardless of their locations. If the Lovers are 2 out of the 3 final players alive, they achieve a special Lovers" +
                                " only win. Intruders cannot be Lovers with other Intruders and same applies for the Syndicate.";
                        }
                        else if (inputText == "phantom" || inputText == "phan")
                        {
                            chatText = "The Phantom cannot win as a Phantom when they are alive. Instead they need to die. Upon death, the Phantom has a faded bean" +
                                " appearance which everyone can see. The Phantom needs to finish their tasks without getting clicked to win.";
                        }
                        else if (inputText == "rivals" || inputText == "rival")
                        {
                            chatText = "Rivals are a pair of player who despise each other. Rivals cannot win together when alive, even if they belong to the same " +
                                "faction. For a Rival to win, they need their other Rival to die not by their hands. A Killing Rival cannot attack their Rival and " +
                                "neither Rivals can vote each other. A Rival wins if they manage to kill the other Rival and live to be 1 of the final 2 players alive.";
                        }
                        else if (inputText == "taskmaster" || inputText == "task")
                            chatText = "The Taskmaster needs to finish their tasks before a certain number of meetings are called in order to win.";
                        else if (inputText == "traitor" || inputText == "trait")
                        {
                            chatText = "The Traitor is a crewmate who needs to finish their tasks to switch sides. Upon finishing their tasks, the Traitor has a 50%" +
                                " chance to side with either the Intruders or the Syndicate and win with them. If they happen to be the last of the factioned evils " +
                                "the Traitor loses all of their current role's abilities and instead gain the ability to kill.";
                        }
                        else
                            chatText = "Invalid input.";
                        
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
                        hudManager.AddChat(player, "With Help (And Code) From: Discussions, Det, Oper, -H and twix");
                        hudManager.AddChat(player, "Remaining credits are on the GitHub!");
                    }
                    //Quotes
                    else if (text.StartsWith("/quote "))
                    {
                        chatHandled = true;
                        inputText = text.Substring(7);
                        
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
                        inputText = text.StartsWith("/ab ") ? text.Substring(4) : text.Substring(15);
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
                            chatText = "The abbreviation for " + inputText + " is " + abbreviation + "!";
                        
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
                            hudManager.AddChat(player, "Whispering is not turned on.");
                        else
                        {
                            inputText = text.StartsWith("/w ") ? text.Substring(3) : text.Substring(9);
                            var message = text.StartsWith("/w ") ? text.Substring(4) : text.Substring(10);
                            var message2 = text.StartsWith("/w ") ? text.Substring(5) : text.Substring(11);
                            var number = inputText.Replace(message, "");
                            var number2 = inputText.Replace(message2, "");
                            number = number.Replace(" ", "");
                            number2 = number2.Replace(" ", "");
                            PlayerControl whisperer = player;
                            bool correctNumber = false;
                            bool correctNumber2 = false;
                            byte id1 = 100;
                            byte id2 = 100;

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage($"|{number}| = |{message}");
                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage($"|{number2}| = |{message2}");

                            foreach (var player2 in PlayerControl.AllPlayerControls)
                            {
                                if (player2 == player)
                                    whisperer = player2;
                                else if ($"{player.PlayerId}" == number)
                                {
                                    correctNumber = true;
                                    id1 = player2.PlayerId;
                                }
                                else if ($"{player.PlayerId}" == number2)
                                {
                                    correctNumber2 = true;
                                    id2 = player2.PlayerId;
                                }

                                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage($"{player.PlayerId}");
                            }

                            if (correctNumber)
                            {
                                var target = Utils.PlayerById(id1);
                                hudManager.AddChat(player, $"You whisper to {target.name}: {message}");
                                hudManager.AddChat(target, $"{whisperer.name} whispers to you: {message}");

                                foreach (var player2 in PlayerControl.AllPlayerControls)
                                {
                                    if (player2 != whisperer && player2 != target)
                                        hudManager.AddChat(player2, $"{whisperer.name} is whispering to {target.name}!");
                                }
                            }
                            else if (correctNumber2)
                            {
                                var target = Utils.PlayerById(id2);
                                hudManager.AddChat(player, $"You whisper to {target.name}: {message2}");
                                hudManager.AddChat(target, $"{whisperer.name} whispers to you: {message2}");

                                foreach (var player2 in PlayerControl.AllPlayerControls)
                                {
                                    if (player2 != whisperer && player2 != target)
                                        hudManager.AddChat(target, $"{whisperer.name} is whispering to {target.name}!");
                                }
                            }
                            else
                                hudManager.AddChat(player, "Who are you trying to whisper to?");
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
    }
}