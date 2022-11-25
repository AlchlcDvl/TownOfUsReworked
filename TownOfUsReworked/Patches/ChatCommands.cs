using System;
using System.Linq;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class ChatCommands
    {
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
        private static class SendChatPatch
        {
            static bool Prefix(ChatController __instance)
            {
                //Set up dictionaries and list for colours/hats/pets/nameplates/visors
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

                string text = __instance.TextArea.text;
                string inputText = "";
                bool chatHandled = false;

                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                {
                    //Help command - lists commands available
                    if (text.ToLower() == "/help" || text.ToLower() == "/help ")
                    {
                        chatHandled = true;
                        __instance.AddChat(PlayerControl.LocalPlayer,
                        AmongUsClient.Instance.CanBan()
                        ? "Commands available:\n/welcome, /name, /colour or /color, /kick, /ban, /listcolours, /listcolors"
                        : "Commands available:\n/welcome, /name, /colour or /color, /listcolours, /listcolors");
                    }
                    //Display a message (Message Of The Day)
                    else if (text.ToLower().StartsWith("/welcome"))
                    {
                        chatHandled = true;
                        __instance.AddChat(PlayerControl.LocalPlayer, "Welcome to Town Of Us Reworked " + TownOfUsReworked.VersionString + "!");
                    }
                    //Name help                    
                    else if (text.ToLower() == "/name" || text.ToLower() == "/name ")
                    {
                        chatHandled = true;
                        __instance.AddChat(PlayerControl.LocalPlayer, "Usage: /name <name>");
                    }
                    //Change name (Can have multiple players the same name!)
                    else if (text.ToLower().StartsWith("/name "))
                    {
                        chatHandled = true;
                        inputText = text.Substring(6);

                        if (!System.Text.RegularExpressions.Regex.IsMatch(inputText, @"^[a-zA-Z0-9]+$"))
                            __instance.AddChat(PlayerControl.LocalPlayer, "Name contains disallowed characters.");
                        else
                        {
                            PlayerControl.LocalPlayer.RpcSetName(inputText);
                            __instance.AddChat(PlayerControl.LocalPlayer, "Name changed!");
                        }
                    }
                    //Colour help                    
                    else if (text.ToLower() == "/colour" || text.ToLower() == "/color" || text.ToLower() == "/colour " || text.ToLower() == "/color ")
                    {
                        chatHandled = true;
                        __instance.AddChat(PlayerControl.LocalPlayer, "Usage: /colour <colour>\nOr: /color <color>");
                    }
                    //Change colour (Can have multiple players the same colour!)
                    else if (text.ToLower().StartsWith("/color ") || text.ToLower().StartsWith("/colour "))
                    {
                        chatHandled = true;
                        int col;
                        inputText = "";
                        string colourSpelling = text.ToLower().StartsWith("/colour ") ? "colour" : "color";
                        var colorString = "";

                        foreach (var color in coloursDict)
                        {
                            colorString += color + " ";
                        }

                        if (!Int32.TryParse(inputText, out col))
                            __instance.AddChat(PlayerControl.LocalPlayer, inputText + " is an invalid " + colourSpelling + ".\nYou can use: Red, Blue, Green, Pink, Orange, Yellow, Black, White, Purple, Brown, Cyan, Lime, Maroon, Rose, Banana, Gray, Tan, Coral, Watermelon, Chocolate, SkyBlue, Beige, HotPink, Turquoise, Lilac, Olive, Rainbow and Azure");
                        else
                        {
                            col = Math.Clamp(col, 0, Palette.PlayerColors.Length - 1);
                            PlayerControl.LocalPlayer.RpcSetColor ((byte)col);
                            __instance.AddChat(PlayerControl.LocalPlayer, colourSpelling + " changed!");
                        }
                    }
                    //List colours
                    else if (text.ToLower().StartsWith("/listcolours") || text.ToLower().StartsWith("/list colours") || text.ToLower().StartsWith("/listcolors") || text.ToLower().StartsWith("/list colors"))
                    {
                        chatHandled = true;
                        __instance.AddChat(PlayerControl.LocalPlayer, "nYou can use: Red, Blue, Green, Pink, Orange, Yellow, Black, White, Purple, Brown, Cyan, Lime, Maroon, Rose, Banana, Gray, Tan, Coral, Watermelon, Chocolate, SkyBlue, Beige, HotPink, Turquoise, Lilac, Olive, Rainbow and Azure");
                    }
                    //Kick help                    
                    else if (text.ToLower() == "/kick" || text.ToLower() == "/kick ")
                    {
                        chatHandled = true;
                        __instance.AddChat(PlayerControl.LocalPlayer, "Usage: /kick <player name>");
                    }
                    //Kick player (if able to kick, i.e. host command)
                    else if (text.ToLower().StartsWith("/kick "))
                    {
                        chatHandled = true;
                        inputText = text.Substring(6);
                        PlayerControl target = PlayerControl.AllPlayerControls.ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.Equals(inputText));

                        if (target != null && AmongUsClient.Instance != null && AmongUsClient.Instance.CanBan())
                        {
                            var client = AmongUsClient.Instance.GetClient(target.OwnerId);

                            if (client != null)
                            {
                                AmongUsClient.Instance.KickPlayer(client.Id, false);
                                chatHandled = true;
                            }
                        }
                    }
                    //Ban help                    
                    else if (text.ToLower() == "/ban" || text.ToLower() == "/ban ")
                    {
                        chatHandled = true;
                        __instance.AddChat(PlayerControl.LocalPlayer, "Usage: /ban <player name>");
                    }
                    //Ban player (if able to ban, i.e. host command)
                    else if (text.ToLower().StartsWith("/ban "))
                    {
                        chatHandled = true;
                        inputText = text.Substring(5);
                        PlayerControl target = PlayerControl.AllPlayerControls.ToArray().ToList().FirstOrDefault(x => x.Data.PlayerName.Equals(inputText));

                        if (target != null && AmongUsClient.Instance != null && AmongUsClient.Instance.CanBan())
                        {
                            var client = AmongUsClient.Instance.GetClient(target.OwnerId);

                            if (client != null)
                            {
                                AmongUsClient.Instance.KickPlayer(client.Id, true);
                                chatHandled = true;
                            }
                        }
                    }
                }

                if (chatHandled)
                {
                    __instance.TextArea.Clear();
                    __instance.quickChatMenu.ResetGlyphs();
                }
                
                return !chatHandled;
            }
        }
    }
}