namespace TownOfUsReworked.Modules
{
    public class ChatCommand
    {
        public string Command;
        public string Short;
        public ExecuteArgsCommand ExecuteArgs;
        public ExecuteArglessCommand ExecuteArgless;
        public delegate void ExecuteArgsCommand(string[] args, ChatController __instance);
        public delegate void ExecuteArglessCommand(ChatController __instance);

        public static readonly List<ChatCommand> AllCommands = new();
        public static readonly Dictionary<string, (string, Color)> BubbleModifications = new();

        //As much as I hate to do this, people will take advatage of this function so we're better off doing this early
        private static readonly string[] profanities = { "fuck", "bastard", "cunt", "bitch", "ass", "nigg", "nig", "neg", "whore", "negro", "dick", "penis", "yiff", "rape", "rapist" };
        private const string disallowed = "@^[{(_-;:\"'.,\\|)}]+$!#$%^&&*?/";

        public ChatCommand(string command, string shortF)
        {
            Command = command;
            Short = shortF;
            AllCommands.Add(this);
        }

        public ChatCommand(string command, string shortF, ExecuteArgsCommand executeArgs) : this(command, shortF)
        {
            ExecuteArgs = executeArgs;
            ExecuteArgless = null;
        }

        public ChatCommand(string command, string shortF, ExecuteArglessCommand executeArgless) : this(command, shortF)
        {
            ExecuteArgless = executeArgless;
            ExecuteArgs = null;
        }

        public static void Load()
        {
            AllCommands.Clear();
            BubbleModifications.Clear();
            _ = new ChatCommand("/info", "/i", Info);
            _ = new ChatCommand("/help", "/h", Help);
            _ = new ChatCommand("/mystate", "/ms", MyState);
            _ = new ChatCommand("/kick", "/k", KickBan);
            _ = new ChatCommand("/ban", "/b", KickBan);
            _ = new ChatCommand("/clearlobby", "/cl", Clear);
            _ = new ChatCommand("/summary", "/sum", Summary);
            _ = new ChatCommand("/setname", "/sn", SetName);
            _ = new ChatCommand("/setcolor", "/sc", SetColor);
            _ = new ChatCommand("/setcolour", "/sc", SetColor);
            _ = new ChatCommand("/whisper", "/w", Whisper);
            _ = new ChatCommand("/testargs", "/targ", TestArgs);
            _ = new ChatCommand("/testargless", "/targless", TestArgless);
        }

        private static void Whisper(string[] args, ChatController __instance)
        {
            if (ConstantVariables.IsLobby)
            {
                __instance.AddChat(CustomPlayer.Local, "Invalid command.");

                if (!BubbleModifications.ContainsKey("Invalid command."))
                    BubbleModifications.Add("Invalid command.", ("Error", UColor.red));

                return;
            }

            if (!CustomGameOptions.Whispers)
            {
                __instance.AddChat(CustomPlayer.Local, "Whispering is not turned on.");

                if (!BubbleModifications.ContainsKey("Whispering is not turned on."))
                    BubbleModifications.Add("Whispering is not turned on.", ("No Whispering", UColor.red));

                return;
            }

            if (args.Length < 3 || string.IsNullOrEmpty(args[1]) || string.IsNullOrEmpty(args[2]))
            {
                __instance.AddChat(CustomPlayer.Local, "Usage: /<whisper | w> <meeting number> <message>");

                if (!BubbleModifications.ContainsKey("Usage: /<whisper | w> <meeting number> <message>"))
                    BubbleModifications.Add("Usage: /<whisper | w> <meeting number> <message>", ("Whisper Help", UColor.green));

                return;
            }

            var message = "";

            foreach (var arg in args.Skip(2))
                message += $"{arg} ";

            message = message.Remove(message.Length - 1);

            if (CustomPlayer.Local.Data.IsDead)
                __instance.AddChat(CustomPlayer.Local, "You are dead.");
            else if (CustomPlayer.Local.IsBlackmailed())
                __instance.AddChat(CustomPlayer.Local, "You are blackmailed.");
            else if (CustomPlayer.AllPlayers.Any(x => x.IsSilenced() && x.GetSilencer().HoldsDrive && x != CustomPlayer.Local))
                __instance.AddChat(CustomPlayer.Local, "You are silenced.");
            else if (byte.TryParse(args[1], out var id))
            {
                var whispered = Utils.PlayerById(id);

                if (whispered == CustomPlayer.Local)
                    __instance.AddChat(CustomPlayer.Local, "Don't whisper to yourself, weirdo.");
                else if (whispered)
                {
                    if (whispered.Data.IsDead || whispered.Data.Disconnected)
                        __instance.AddChat(CustomPlayer.Local, $"{whispered.name} is not in this world anymore.");
                    else
                    {
                        __instance.AddChat(CustomPlayer.Local, $"You whisper to {whispered.name}: {message}");
                        var writer = AmongUsClient.Instance.StartRpcImmediately(CustomPlayer.Local.NetId, (byte)CustomRPC.Whisper, SendOption.Reliable);
                        writer.Write(CustomPlayer.Local.PlayerId);
                        writer.Write(id);
                        writer.Write(message);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
                else
                    __instance.AddChat(CustomPlayer.Local, "Who are you trying to whisper?");
            }
            else
                __instance.AddChat(CustomPlayer.Local, $"{args[1]} is not a valid number.");
        }

        private static void SetColor(string[] args, ChatController __instance)
        {
            if (!TownOfUsReworked.IsTest || !ConstantVariables.IsLobby)
            {
                __instance.AddChat(CustomPlayer.Local, "Invalid command.");

                if (!BubbleModifications.ContainsKey("Invalid command."))
                    BubbleModifications.Add("Invalid command.", ("Error", UColor.red));

                return;
            }

            if (args.Length < 2 || string.IsNullOrEmpty(args[1]))
            {
                __instance.AddChat(CustomPlayer.Local, "Usage: /<setcolour | setcolor | sc> <color id>");
                return;
            }

            var arg = args[1];
            var spelling = args[0].Replace("/set", "");

            if (!int.TryParse(arg, out var col))
            {
                __instance.AddChat(CustomPlayer.Local, $"{arg} is an invalid {spelling}.\nYou need to use the color ID for the color you want to be. To find out a color's ID, go into the "
                    + "color selection screen and count the number of colors starting from 0 to the position of the color you want to pick. The range of colors is from 0 to 49 meaning Red"
                    + " to Rainbow.");
            }
            else if (col >= Palette.PlayerColors.Length || col < 0)
                __instance.AddChat(CustomPlayer.Local, $"Invalid {spelling} id.");
            else
            {
                CustomPlayer.Local.CmdCheckColor((byte)col);
                __instance.AddChat(CustomPlayer.Local, $"{spelling} changed!");
            }
        }

        private static void SetName(string[] args, ChatController __instance)
        {
            if (!TownOfUsReworked.IsTest || !ConstantVariables.IsLobby)
            {
                __instance.AddChat(CustomPlayer.Local, "Invalid command.");

                if (!BubbleModifications.ContainsKey("Invalid command."))
                    BubbleModifications.Add("Invalid command.", ("Error", UColor.red));

                return;
            }

            if (args.Length < 2 || string.IsNullOrEmpty(args[1]))
            {
                __instance.AddChat(CustomPlayer.Local, "Usage: /<setname | sn> <name>");
                return;
            }

            var arg = args[1];

            if (arg.Any(disallowed.Contains))
                __instance.AddChat(CustomPlayer.Local, "Name contains disallowed characters.");
            else if (profanities.Any(arg.Contains))
                __instance.AddChat(CustomPlayer.Local, "Name contains unaccepted words.");
            else if (arg.Length > 20)
                __instance.AddChat(CustomPlayer.Local, "Name is too long.");
            else
            {
                CustomPlayer.Local.RpcSetName(arg);
                __instance.AddChat(CustomPlayer.Local, "Name changed!");
            }
        }

        private static void Summary(ChatController __instance)
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

            __instance.AddChat(CustomPlayer.Local, summary);
        }

        private static void Clear(ChatController __instance)
        {
            if (!ConstantVariables.IsLobby)
            {
                __instance.AddChat(CustomPlayer.Local, "Invalid command.");

                if (!BubbleModifications.ContainsKey("Invalid command."))
                    BubbleModifications.Add("Invalid command.", ("Error", UColor.red));

                return;
            }

            if (!AmongUsClient.Instance.CanBan() || !AmongUsClient.Instance.AmHost)
            {
                __instance.AddChat(CustomPlayer.Local, "You can't use this command.");
                return;
            }

            foreach (var player2 in CustomPlayer.AllPlayers)
            {
                if (player2 != CustomPlayer.Local)
                {
                    var client = AmongUsClient.Instance.GetClient(player2.OwnerId);

                    if (client != null)
                        AmongUsClient.Instance.KickPlayer(client.Id, false);
                }
            }

            __instance.AddChat(CustomPlayer.Local, "Lobby cleared!");
        }

        private static void KickBan(string[] args, ChatController __instance)
        {
            if (!ConstantVariables.IsLobby)
            {
                __instance.AddChat(CustomPlayer.Local, "Invalid command.");

                if (!BubbleModifications.ContainsKey("Invalid command."))
                    BubbleModifications.Add("Invalid command.", ("Error", UColor.red));

                return;
            }

            if (!AmongUsClient.Instance.CanBan() || !AmongUsClient.Instance.AmHost)
            {
                __instance.AddChat(CustomPlayer.Local, "You can't use this command.");
                return;
            }

            var ban = args[0].Replace("/", "") is "ban" or "b";

            if (args.Length < 2 || string.IsNullOrEmpty(args[1]))
            {
                __instance.AddChat(CustomPlayer.Local, $"Usage: /{(ban ? "<ban | b>" : "<kick | k>")} <player name>");
                return;
            }

            var target = CustomPlayer.AllPlayers.Find(x => x.Data.PlayerName == args[1]);

            if (target == null)
            {
                __instance.AddChat(CustomPlayer.Local, $"Could not find {args[1]}.");
                return;
            }

            if (target == CustomPlayer.Local)
            {
                __instance.AddChat(CustomPlayer.Local, $"Don't {(ban ? "ban" : "kick")} yourself.");
                return;
            }

            var client = AmongUsClient.Instance.GetClient(target.OwnerId);

            if (client == null)
            {
                __instance.AddChat(CustomPlayer.Local, $"Could not find {args[1]}.");
                return;
            }

            AmongUsClient.Instance.KickPlayer(client.Id, ban);
            __instance.AddChat(CustomPlayer.Local, $"{target.name} {(ban ? "Bann" : "Kick")}ed!");
        }

        private static void MyState(ChatController __instance)
        {
            if (ConstantVariables.IsLobby)
            {
                __instance.AddChat(CustomPlayer.Local, "Invalid command.");

                if (!BubbleModifications.ContainsKey("Invalid command."))
                    BubbleModifications.Add("Invalid command.", ("Error", UColor.red));

                return;
            }

            var message = "";

            if (Role.LocalRole)
                message += LayerInfo.AllRoles.FirstOrDefault(x => x.Name == Role.LocalRole.Name, LayerInfo.AllRoles[0]).ToString();

            if (!Modifier.LocalModifier.Hidden)
                message += '\n' + LayerInfo.AllModifiers.FirstOrDefault(x => x.Name == Modifier.LocalModifier.Name, LayerInfo.AllModifiers[0]).ToString();

            if (!Objectifier.LocalObjectifier.Hidden)
            {
                message += '\n' + LayerInfo.AllObjectifiers.FirstOrDefault(x => x.Name == Objectifier.LocalObjectifier.Name || x.Symbol == Objectifier.LocalObjectifier.Symbol,
                    LayerInfo.AllObjectifiers[0]).ToString();
            }

            if (!Ability.LocalAbility.Hidden)
                message += '\n' + LayerInfo.AllAbilities.FirstOrDefault(x => x.Name == Ability.LocalAbility.Name, LayerInfo.AllAbilities[0]).ToString();

            __instance.AddChat(CustomPlayer.Local, message);
        }

        private static void Help(ChatController __instance)
        {
            var setColor = TownOfUsReworked.IsTest ? "/setcolour or /setcolor, /setname" : "";
            var whisper = CustomGameOptions.Whispers ? ((setColor != "" ? ", " : "") + "/whisper") : "";
            var kickBan = AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan() ? ", /kick, /ban, /clearlobby" : "";
            var test = TownOfUsReworked.IsTest ? ", /testarg, /testargless" : "";
            __instance.AddChat(CustomPlayer.Local, $"Commands available all the time:\n/help, /info, /summary{test}\n\nCommands available in lobby:\n{setColor}{kickBan}\n\nCommands " +
                $"available in game:\n/mystate{whisper}");
        }

        private static void Info(string[] args, ChatController __instance)
        {
            if ((args.Length == 2 && args[1] is not "mod" or "controls" or "ctrl") || args.Length < 2 || string.IsNullOrEmpty(args[1]))
            {
                __instance.AddChat(CustomPlayer.Local, "Usage: /<info | i> <role | modifier | objectifier | ability | faction | subfaction | alignment | mod | lore | controls | other | r |"
                    + " m | obj | ab | f | sf | al | mod | l | ctrl | ot> <name | abbreviation>");
                return;
            }

            var message = args[1].ToLower() switch
            {
                "mod" => Utils.CreateText("ModInfo"),
                "controls" => "Here are the controls:\nF1 - Start up the MCI control panel (local only)\nF2 - Toggle the visibility of the control panel (local only)\nTab - Change pages" +
                    "\nUp/Left Arrow - Go up a page when in a menu\nDown/Right Arrow - Go down a page when in a menu",
                "ctrl" => "Here are the controls:\nF1 - Start up the MCI control panel (local only)\nF2 - Toggle the visibility of the control panel (local only)\nTab - Change pages\nUp/" +
                    "Left Arrow - Go up a page when in a menu\nDown/Right Arrow - Go down a page when in a menu",
                "role" => LayerInfo.AllRoles.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllRoles[0]).ToString(),
                "r" => LayerInfo.AllRoles.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllRoles[0]).ToString(),
                "modifier" => LayerInfo.AllModifiers.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllModifiers[0]).ToString(),
                "m" => LayerInfo.AllModifiers.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllModifiers[0]).ToString(),
                "objectifier" => LayerInfo.AllObjectifiers.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase) || string.Equals(x.Symbol, args[2], StringComparison.OrdinalIgnoreCase), LayerInfo.AllObjectifiers[0]).ToString(),
                "obj" => LayerInfo.AllObjectifiers.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase) || string.Equals(x.Symbol, args[2], StringComparison.OrdinalIgnoreCase), LayerInfo.AllObjectifiers[0]).ToString(),
                "ability" => LayerInfo.AllAbilities.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllAbilities[0]).ToString(),
                "ab" => LayerInfo.AllAbilities.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllAbilities[0]).ToString(),
                "other" => LayerInfo.AllOthers.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllOthers[0]).ToString(),
                "ot" => LayerInfo.AllOthers.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllOthers[0]).ToString(),
                "faction" => LayerInfo.AllFactions.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllFactions[0]).ToString(),
                "f" => LayerInfo.AllFactions.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllFactions[0]).ToString(),
                "subfaction" => LayerInfo.AllSubFactions.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllSubFactions[0]).ToString(),
                "sf" => LayerInfo.AllSubFactions.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllSubFactions[0]).ToString(),
                "alignment" => LayerInfo.AllAlignments.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllAlignments[0]).ToString(),
                "al" => LayerInfo.AllAlignments.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllAlignments[0]).ToString(),
                "lore" => LayerInfo.AllLore.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllLore[0]).ToString(),
                "l" => LayerInfo.AllLore.FirstOrDefault(x => string.Equals(args[2], x.Name, StringComparison.OrdinalIgnoreCase) || string.Equals(args[2], x.Short,
                    StringComparison.OrdinalIgnoreCase), LayerInfo.AllLore[0]).ToString(),
                _ => "Usage: /<info | i> <role | modifier | objectifier | ability | faction | subfaction | alignment | mod | lore | controls | other | r | m | obj | ab | f | sf | al | " +
                    "mod | l | ctrl | ot> <name | abbreviation>"
            };

            __instance.AddChat(CustomPlayer.Local, message);
        }

        private static void TestArgs(string[] args, ChatController __instance)
        {
            var message = "You entered the following params:";

            foreach (var arg in args)
                message += $" {arg}";

            __instance.AddChat(CustomPlayer.Local, message);
        }

        private static void TestArgless(ChatController __instance) => __instance.AddChat(CustomPlayer.Local, "Test");
    }
}