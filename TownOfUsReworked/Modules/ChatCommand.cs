namespace TownOfUsReworked.Modules;

public class ChatCommand
{
    private string[] Aliases { get; }
    private ExecuteArgsCommand ExecuteArgs { get; }
    private ExecuteArglessCommand ExecuteArgless { get; }

    private delegate void ExecuteArgsCommand(string[] args);
    private delegate void ExecuteArglessCommand();

    private static readonly List<ChatCommand> AllCommands =
    [
        new(["controls", "ctrl", "mci"], Controls),
        new(["kick", "k", "ban", "b"], KickBan),
        new(["summary", "sum"], Summary),
        new(["clearlobby", "cl", "clear"], Clear),
        new(["setname", "sn", "name"], SetName),
        new(["setcolor", "setcolour", "sc", "color", "colour"], SetColor),
        new(["whisper", "w"], Whisper),
        new(["help", "h"], Help)/*,
        new(["testargs", "targ"], TestArgs),
        new(["testargless", "targless"], TestArgless),
        new(["translate", "trans"], Translate),
        new(["rpc"], SendRPC)*/
    ];

    // As much as I hate to do this, people will take advantage of this function so we're better off doing this early
    private static readonly string[] profanities = ["fuck", "bastard", "cunt", "bitch", "ass", "nigg", "nig", "neg", "whore", "negro", "dick", "penis", "yiff", "rape", "rapist"];
    private const string disallowed = "@^[{(_-;:\"'.,\\|)}]+$!#$%^&&*?/";

    private ChatCommand(string[] aliases) => Aliases = aliases;

    private ChatCommand(string[] aliases, ExecuteArgsCommand executeArgs) : this(aliases)
    {
        ExecuteArgs = executeArgs;
        ExecuteArgless = null;
    }

    private ChatCommand(string[] aliases, ExecuteArglessCommand executeArgless) : this(aliases)
    {
        ExecuteArgless = executeArgless;
        ExecuteArgs = null;
    }

    public static void Execute(ChatCommand command, string[] args)
    {
        if (command == null)
            Run("<color=#FF0000FF>⚠ Invalid Command ⚠</color>", "This command does not exist.");
        else if (command.ExecuteArgs == null)
            command.ExecuteArgless();
        else if (command.ExecuteArgless == null)
            command.ExecuteArgs(args);
        else
            Run("<color=#FFFF00FF>⚠ Huh? ⚠</color>", "Weird...");
    }

    public static ChatCommand Find(string[] args) => AllCommands.Find(x => x.Aliases.Any(x => args[0] == $"/{x}"));

    public static void Run(string title, string text, bool withColor = true, bool hasColor = false, UColor? color = null)
    {
        var pooledBubble = Chat.GetPooledBubble();

        try
        {
            pooledBubble.transform.SetParent(Chat.scroller.Inner);
            pooledBubble.transform.localScale = Vector3.one;
            pooledBubble.SetLeft();
            pooledBubble.SetCosmetics(CustomPlayer.Local.Data);
            pooledBubble.NameText.text = title;
            pooledBubble.NameText.color = UColor.white;
            pooledBubble.NameText.ForceMeshUpdate(true, true);
            pooledBubble.votedMark.enabled = false;
            pooledBubble.Xmark.enabled = false;
            pooledBubble.TextArea.text = text;
            pooledBubble.TextArea.ForceMeshUpdate(true, true);
            pooledBubble.Background.size = new(5.52f, 0.2f + pooledBubble.NameText.GetNotDumbRenderedHeight() + pooledBubble.TextArea.GetNotDumbRenderedHeight());
            pooledBubble.MaskArea.size = pooledBubble.Background.size - new Vector2(0, 0.03f);
            pooledBubble.TextArea.richText = withColor;
            pooledBubble.Background.color = color ?? UColor.white;

            if (withColor && !hasColor)
                pooledBubble.TextArea.text = Info.ColorIt(text);

            pooledBubble.AlignChildren();
            var pos = pooledBubble.NameText.transform.localPosition;
            pos.y += 0.05f;
            pooledBubble.NameText.transform.localPosition = pos;
            Chat.AlignAllBubbles();
            Play("Chat");
        }
        catch (Exception ex)
        {
            LogError(ex);
            Chat.chatBubblePool.Reclaim(pooledBubble);
        }
    }

    private static void Whisper(string[] args)
    {
        if (!CustomGameOptions.Whispers && IsInGame)
        {
            Run("<color=#00FF00FF>⚠ No Whispering ⚠</color>", "Whispering is not turned on.");
            return;
        }

        if (args.Length < 3 || IsNullEmptyOrWhiteSpace(args[1]) || IsNullEmptyOrWhiteSpace(args[2]))
        {
            Run("<color=#00FF00FF>★ Help ★</color>", "Usage: /<whisper | w> <meeting number> <message>");
            return;
        }

        var message = "";
        args[2..].ForEach(arg => message += $"{arg} ");
        message = message.Remove(message.Length - 1);

        if (CustomPlayer.LocalCustom.Dead)
            Run("<color=#FFFF00FF>米 Shhhh 米</color>", "You are dead.");
        else if (CustomPlayer.Local.IsBlackmailed())
            Run("<color=#02A752FF>米 Shhhh 米</color>", "You are blackmailed.");
        else if (CustomPlayer.Local.SilenceActive())
            Run("<color=#AAB43EFF>米 Shhhh 米</color>", "You are silenced.");
        else if (byte.TryParse(args[1], out var id))
        {
            var whispered = PlayerById(id);

            if (whispered == CustomPlayer.Local)
                Run("<color=#FF0000FF>⚠ Whispering Error ⚠</color>", "Don't whisper to yourself, weirdo.");
            else if (whispered)
            {
                if (whispered.HasDied())
                    Run("<color=#FF0000FF>⚠ Whispering Error ⚠</color>", $"{whispered.name} is not in this world anymore.");
                else
                {
                    Run("<color=#4D4DFFFF>「 Whispers 」</color>", $"You whisper to {whispered.name}: {message}");
                    CallRpc(CustomRPC.Misc, MiscRPC.Whisper, CustomPlayer.Local, whispered, message);
                }
            }
            else
                Run("<color=#FF0000FF>⚠ Whispering Error ⚠</color>", "Who are you trying to whisper?");
        }
        else
            Run("<color=#FF0000FF>⚠ Whispering Error ⚠</color>", $"{args[1]} is not a valid number.");
    }

    private static void SetColor(string[] args)
    {
        if (!TownOfUsReworked.IsTest || !IsLobby)
        {
            Run("<color=#FF0000FF>⚠ Invalid Command ⚠</color>", "This command does not exist.");
            return;
        }

        if (args.Length < 2 || IsNullEmptyOrWhiteSpace(args[1]))
        {
            Run("<color=#00FF00FF>★ Help ★</color>", "Usage: /<setcolour | setcolor | sc> <color id>");
            return;
        }

        var spelling = args[0].ToLower().Contains("color") ? "" : "u";

        if (!byte.TryParse(args[1], out var col))
        {
            Run($"<color=#FF0000FF>⚠ Colo{spelling}r Error ⚠</color>", $"{args[1]} is an invalid colo{spelling}r.\nYou need to use the colo{spelling}r ID for the colo{spelling}r you want to"
                + $" be. To find out a colo{spelling}r's ID, go into the colo{spelling}r selection screen and count the number of colo{spelling}rs starting from 0 to the position of the " +
                $"colo{spelling}r you want to pick, from left to right. The range of colo{spelling}rs is from 0 to {CustomColorManager.AllColors.Count - 1} meaning Red to Rainbow " +
                "respectively.");
        }
        else if (CustomColorManager.OutOfBounds(col))
            Run($"<color=#FF0000FF>⚠ Colo{spelling}r Error ⚠</color>", $"Invalid colo{spelling}r id.");
        else
        {
            CustomPlayer.Local.CmdCheckColor(col);
            Run("<color=#B148E2FF>◈ Success ◈</color>", $"Colo{spelling}r changed!");
        }
    }

    private static void SetName(string[] args)
    {
        if (!TownOfUsReworked.IsTest || !IsLobby)
        {
            Run("<color=#FF0000FF>⚠ Invalid Command ⚠</color>", "This command does not exist.");
            return;
        }

        if (args.Length < 2 || IsNullEmptyOrWhiteSpace(args[1]))
        {
            Run("<color=#00FF00FF>★ Help ★</color>", "Usage: /<setname | sn> <name>");
            return;
        }

        var arg = "";
        args[1..].ForEach(arg2 => arg += $"{arg2} ");
        arg = arg.Remove(arg.Length - 1);

        if (arg.Any(disallowed.Contains))
            Run("<color=#FF0000FF>⚠ Name Error ⚠</color>", "Name contains disallowed characters.");
        else if (profanities.Any(arg.Contains))
            Run("<color=#FF0000FF>⚠ Name Error ⚠</color>", "Name contains unaccepted words.");
        else if (arg.Length > 20)
            Run("<color=#FF0000FF>⚠ Name Error ⚠</color>", "Name is too long.");
        else
        {
            CustomPlayer.Local.RpcSetName(arg);
            Run("<color=#B148E2FF>◈ Success ◈</color>", "Name changed!");
        }
    }

    private static void Summary()
    {
        var summary = ReadDiskText("Summary", TownOfUsReworked.Other);

        if (IsNullEmptyOrWhiteSpace(summary))
            Run("<color=#FF0000FF>⚠ Summary Error ⚠</color>", "Summary could not be found.");
        else
            Run("<color=#FF0080FF>个 Previous Game Summary 个</color>", summary, false, true);
    }

    private static void Clear()
    {
        if (!IsLobby)
        {
            Run("<color=#FF0000FF>⚠ Invalid Command ⚠</color>", "This command does not exist.");
            return;
        }

        if (!AmongUsClient.Instance.CanBan() || !AmongUsClient.Instance.AmHost)
        {
            Run("<color=#FF0000FF>⚠ Not Allowed ⚠</color>", "This command is not for you to use.");
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

        Run("<color=#B148E2FF>◈ Success ◈</color>", "Lobby cleared!");
    }

    private static void KickBan(string[] args)
    {
        if (!IsLobby)
        {
            Run("<color=#FF0000FF>⚠ Invalid Command ⚠</color>", "This command does not exist.");
            return;
        }

        if (!AmongUsClient.Instance.CanBan() || !AmongUsClient.Instance.AmHost)
        {
            Run("<color=#FF0000FF>⚠ Not Allowed ⚠</color>", "This command is not for you to use.");
            return;
        }

        var ban = args[0].Replace("/", "") is "ban" or "b";

        if (args.Length < 2 || IsNullEmptyOrWhiteSpace(args[1]))
        {
            Run("<color=#00FF00FF>★ Help ★</color>", $"Usage: /{(ban ? "<ban | b>" : "<kick | k>")} <player name>");
            return;
        }

        var arg = "";
        args[1..].ForEach(arg2 => arg += $"{arg2} ");
        arg = arg.Remove(arg.Length - 1);
        var target = CustomPlayer.AllPlayers.Find(x => x.Data.PlayerName == arg);

        if (target == null)
        {
            Run($"<color=#FF0000FF>⚠ {(ban ? "Ban" : "Kick")} Error ⚠</color>", $"Could not find {arg}.");
            return;
        }

        if (target == CustomPlayer.Local)
        {
            Run($"<color=#FF0000FF>⚠ {(ban ? "Ban" : "Kick")} Error ⚠</color>", $"Don't {(ban ? "ban" : "kick")} yourself.");
            return;
        }

        var client = AmongUsClient.Instance.GetClient(target.OwnerId);

        if (client == null)
        {
            Run($"<color=#FF0000FF>⚠ {(ban ? "Ban" : "Kick")} Error ⚠</color>", $"Could not find {arg}.");
            return;
        }

        AmongUsClient.Instance.KickPlayer(client.Id, ban);
        Run("<color=#B148E2FF>◈ Success ◈</color>", $"{target.name} {(ban ? "Bann" : "Kick")}ed!");
    }

    private static void Help()
    {
        var setColor = TownOfUsReworked.IsTest ? "/setcolour or /setcolor, /setname" : "";
        var comma = setColor.Length == 0 ? "" : ", ";
        var kickBan = comma + (AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan() ? "/kick, /ban, /clearlobby" : "");
        var test = TownOfUsReworked.IsTest ? ", /testargs, /testargless, /rpc" : "";
        var lobby = setColor + kickBan != "" ? $"\n\nCommands available in lobby:\n{setColor}{kickBan}" : "";
        Run("<color=#0000FFFF>✿ Help Menu ✿</color>", $"Commands available all the time:\n/help, /controls, /summary, /whisper{test}\n\nCommands available in game:\n{lobby}");
    }

    private static void Controls() => Run("<color=#6697FFFF>◆ Controls ◆</color>", "Here are the controls:\nF1 - Start up the MCI control panel (local only)\nF2 - Toggle the visibility of "
        + "the control panel (local only)\nTab/Backspace - Change pages\nUp/Left Arrow - Go up a page when in a menu\nDown/Right Arrow - Go down a page when in a menu\n1 - 9 - Jump between "
        + "setting pages (in lobby)");

    /*private static void TestArgs(string[] args)
    {
        var message = "You entered the following params:\n";
        args[1..].ForEach(arg => message += $"{arg}, ");
        message = message.Remove(message.Length - 2);
        Run("<color=#FF00FFFF>⚠ TEST ⚠</color>", message);
    }

    private static void TestArgless() => Run("<color=#FF00FFFF>⚠ TEST ⚠</color>", "Test.");

    private static void SendRPC()
    {
        CallRpc(CustomRPC.Test);
        LogMessage("RPC Sent!");
        Run("<color=#FF00FFFF>⚠ RPC TEST ⚠</color>", "RPC Sent!");
    }

    private static void Translate(string[] args)
    {
        if (args.Length < 2 || IsNullEmptyOrWhiteSpace(args[1]))
            Run("<color=#00FF00FF>★ Help ★</color>", "Usage: /<translate | trans> <text id>");
        else
            Run("<color=#B148E2FF>◈ Success ◈</color>", TranslationManager.Test(args[1]));
    }*/
}