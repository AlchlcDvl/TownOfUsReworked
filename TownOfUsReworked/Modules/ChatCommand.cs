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

        public static readonly List<ChatCommand> AllCommands = new()
        {
            new("/info", "/i", Info),
            new("/kick", "/k", KickBan),
            new("/ban", "/b", KickBan),
            new("/summary", "/sum", Summary),
            new("/clearlobby", "/cl", Clear),
            new("/setname", "/sn", SetName),
            new("/setcolor", "/sc", SetColor),
            new("/setcolour", "/sc", SetColor),
            new("/whisper", "/w", Whisper),
            new("/testargs", "/targ", TestArgs),
            new("/testargless", "/targless", TestArgless),
            new("/help", "/h", Help),
            new("/rpc", "/rpc", SendRPC)
        };
        public static readonly Dictionary<ChatBubble, (string, Color)> BubbleModifications = new();

        //As much as I hate to do this, people will take advatage of this function so we're better off doing this early
        private static readonly string[] profanities = { "fuck", "bastard", "cunt", "bitch", "ass", "nigg", "nig", "neg", "whore", "negro", "dick", "penis", "yiff", "rape", "rapist" };
        private const string disallowed = "@^[{(_-;:\"'.,\\|)}]+$!#$%^&&*?/";

        public ChatCommand(string command, string shortF)
        {
            Command = command;
            Short = shortF;
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

        private static void Whisper(string[] args, ChatController __instance)
        {
            if (IsLobby)
            {
                __instance.AddChat(CustomPlayer.Local, "Invalid command.");

                if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                    BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Error", UColor.red));

                return;
            }

            if (!CustomGameOptions.Whispers)
            {
                __instance.AddChat(CustomPlayer.Local, "Whispering is not turned on.");

                if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                    BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("No Whispering", UColor.red));

                return;
            }

            if (args.Length < 3 || string.IsNullOrEmpty(args[1]) || string.IsNullOrEmpty(args[2]))
            {
                __instance.AddChat(CustomPlayer.Local, "Usage: /<whisper | w> <meeting number> <message>");

                if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                    BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Whisper Help", UColor.green));

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
                var whispered = PlayerById(id);

                if (whispered == CustomPlayer.Local)
                    __instance.AddChat(CustomPlayer.Local, "Don't whisper to yourself, weirdo.");
                else if (whispered)
                {
                    if (whispered.Data.IsDead || whispered.Data.Disconnected)
                        __instance.AddChat(CustomPlayer.Local, $"{whispered.name} is not in this world anymore.");
                    else
                    {
                        __instance.AddChat(CustomPlayer.Local, $"You whisper to {whispered.name}: {message}");
                        CallRpc(CustomRPC.Misc, MiscRPC.Whisper, CustomPlayer.Local, whispered, message);
                    }
                }
                else
                    __instance.AddChat(CustomPlayer.Local, "Who are you trying to whisper?");
            }
            else
                __instance.AddChat(CustomPlayer.Local, $"{args[1]} is not a valid number.");

            if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Whispers", UColor.blue));
        }

        private static void SetColor(string[] args, ChatController __instance)
        {
            if (!TownOfUsReworked.IsTest || !IsLobby)
            {
                __instance.AddChat(CustomPlayer.Local, "Invalid command.");

                if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                    BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Error", UColor.red));

                return;
            }

            if (args.Length < 2 || string.IsNullOrEmpty(args[1]))
            {
                __instance.AddChat(CustomPlayer.Local, "Usage: /<setcolour | setcolor | sc> <color id>");

                if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                    BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Color Help", UColor.green));

                return;
            }

            var spelling = args[0].Replace("/set", "");
            var changed = false;

            if (!int.TryParse(args[1], out var col))
            {
                __instance.AddChat(CustomPlayer.Local, $"{args[1]} is an invalid {spelling}.\nYou need to use the color ID for the color you want to be. To find out a color's ID, go into "
                    + "the color selection screen and count the number of colors starting from 0 to the position of the color you want to pick, from left to right. The range of colors is "
                    + "from 0 to 49 meaning Red to Rainbow respectively.");
            }
            else if (ColorUtils.OutOfBounds(col))
                __instance.AddChat(CustomPlayer.Local, $"Invalid {spelling} id.");
            else
            {
                CustomPlayer.Local.CmdCheckColor((byte)col);
                __instance.AddChat(CustomPlayer.Local, $"{spelling} changed!");
                changed = true;
            }

            if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Set Color", changed ? UColor.green : UColor.red));
        }

        private static void SetName(string[] args, ChatController __instance)
        {
            if (!TownOfUsReworked.IsTest || !IsLobby)
            {
                __instance.AddChat(CustomPlayer.Local, "Invalid command.");

                if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                    BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Error", UColor.red));

                return;
            }

            if (args.Length < 2 || string.IsNullOrEmpty(args[1]))
            {
                __instance.AddChat(CustomPlayer.Local, "Usage: /<setname | sn> <name>");

                if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                    BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Set Name Help", UColor.green));

                return;
            }

            var arg = "";

            foreach (var arg2 in args[1..])
                arg += $"{arg2} ";

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

            if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Set Name", UColor.green));
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

            if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Summary", UColor.yellow));
        }

        private static void Clear(ChatController __instance)
        {
            if (!IsLobby)
            {
                __instance.AddChat(CustomPlayer.Local, "Invalid command.");

                if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                    BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Error", UColor.red));

                return;
            }

            if (!AmongUsClient.Instance.CanBan() || !AmongUsClient.Instance.AmHost)
            {
                __instance.AddChat(CustomPlayer.Local, "You can't use this command.");

                if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                    BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Error", UColor.red));

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
            if (!IsLobby)
            {
                __instance.AddChat(CustomPlayer.Local, "Invalid command.");

                if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                    BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Error", UColor.red));

                return;
            }

            if (!AmongUsClient.Instance.CanBan() || !AmongUsClient.Instance.AmHost)
            {
                __instance.AddChat(CustomPlayer.Local, "You can't use this command.");

                if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                    BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Error", UColor.red));

                return;
            }

            var ban = args[0].Replace("/", "") is "ban" or "b";

            if (args.Length < 2 || string.IsNullOrEmpty(args[1]))
            {
                __instance.AddChat(CustomPlayer.Local, $"Usage: /{(ban ? "<ban | b>" : "<kick | k>")} <player name>");
                return;
            }

            var arg = "";

            foreach (var arg2 in args[1..])
                arg += $"{arg2} ";

            var target = CustomPlayer.AllPlayers.Find(x => x.Data.PlayerName == arg);

            if (target == null)
            {
                __instance.AddChat(CustomPlayer.Local, $"Could not find {arg}.");
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
                __instance.AddChat(CustomPlayer.Local, $"Could not find {arg}.");
                return;
            }

            AmongUsClient.Instance.KickPlayer(client.Id, ban);
            __instance.AddChat(CustomPlayer.Local, $"{target.name} {(ban ? "Bann" : "Kick")}ed!");

            if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Sucess", UColor.green));
        }

        private static void Help(ChatController __instance)
        {
            var setColor = TownOfUsReworked.IsTest ? "/setcolour or /setcolor, /setname" : "";
            var whisper = CustomGameOptions.Whispers ? "/whisper" : "";
            var comma = setColor?.Length == 0 ? "" : ", ";
            var kickBan = comma + (AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan() ? "/kick, /ban, /clearlobby" : "");
            var test = TownOfUsReworked.IsTest ? ", /testargs, /testargless, /rpc" : "";
            __instance.AddChat(CustomPlayer.Local, $"Commands available all the time:\n/help, /info, /summary{test}\n\nCommands available in lobby:\n{setColor}{kickBan}\n\nCommands " +
                $"available in game:\n{whisper}");
        }

        private static void Info(string[] args, ChatController __instance)
        {
            if ((args.Length == 2 && args[1] is not "controls" and not "ctrl") || args.Length < 2 || string.IsNullOrEmpty(args[1]))
            {
                __instance.AddChat(CustomPlayer.Local, "Usage: /<info | i> <controls | ctrl>");

                if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                    BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Error", UColor.red));

                return;
            }

            var message = args[1].ToLower() switch
            {
                "controls" => "Here are the controls:\nF1 - Start up the MCI control panel (local only)\nF2 - Toggle the visibility of the control panel (local only)\nTab/Backspace - " +
                    "Change pages\nUp/Left Arrow - Go up a page when in a menu\nDown/Right Arrow - Go down a page when in a menu\n1-9 - Jump between setting pages (in lobby)",
                "ctrl" => "Here are the controls:\nF1 - Start up the MCI control panel (local only)\nF2 - Toggle the visibility of the control panel (local only)\nTab/Backspace - Change " +
                    "pages\nUp/Left Arrow - Go up a page when in a menu\nDown/Right Arrow - Go down a page when in a menu\n1-9 - Jump between setting pages (in lobby)",
                _ => "Usage: /<info | i> <controls | ctrl>"
            };

            __instance.AddChat(CustomPlayer.Local, message);

            if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Information", UColor.green));
        }

        private static void TestArgs(string[] args, ChatController __instance)
        {
            var message = "You entered the following params:\n";

            foreach (var arg in args)
                message += $"{arg} ";

            __instance.AddChat(CustomPlayer.Local, message);

            if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Test Command With Arguments", UColor.cyan));
        }

        private static void TestArgless(ChatController __instance)
        {
            __instance.AddChat(CustomPlayer.Local, "Test");

            if (!BubbleModifications.ContainsKey(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>()))
                BubbleModifications.Add(__instance.chatBubblePool.activeChildren[^1].Cast<ChatBubble>(), ("Test Command With No Arguments", UColor.cyan));
        }

        private static void SendRPC(ChatController __instance)
        {
            CallRpc(CustomRPC.Test);
            __instance.AddChat(CustomPlayer.Local, "RPC Sent!");
            LogSomething("RPC Sent!");
        }
    }
}