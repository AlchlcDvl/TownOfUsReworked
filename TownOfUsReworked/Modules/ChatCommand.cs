namespace TownOfUsReworked.Modules;

public class ChatCommand
{
    private string[] Aliases { get; }
    private ExecuteArgsCommand ExecuteArgs { get; }
    private ExecuteArglessCommand ExecuteArgless { get; }
    private ExecuteArgsMessageCommand ExecuteArgsMessage { get; }

    private delegate void ExecuteArglessCommand();
    private delegate void ExecuteArgsCommand(string[] args);
    private delegate void ExecuteArgsMessageCommand(string[] args, string arg);

    private static readonly List<ChatCommand> AllCommands =
    [
        new([ "controls", "ctrl", "mci" ], Controls),
        new([ "kick", "k", "ban", "b" ], KickBan),
        new([ "summary", "sum" ], Summary),
        new([ "clearlobby", "cl", "clear" ], Clear),
        new([ "setname", "sn", "name" ], SetName),
        new([ "whisper", "w"] , Whisper),
        new([ "ignore", "i", "unignore", "ui" ], ToggleIgnore),
        new([ "help", "h" ], Help),
        // new([ "testargs", "targ" ], TestArgs),
        // new([ "testargless", "targless" ], TestArgless),
        // new([ "testargmessage", "targmess" ], TestArgsMessage),
        // new([ "translate", "trans", "t" ], Translate),
        // new([ "rpc" ], SendRPCArgless),
        // new([ "rpca" ], SendRPCArgs),
    ];

    private ChatCommand(string[] aliases) => Aliases = aliases;

    private ChatCommand(string[] aliases, ExecuteArgsCommand executeArgs) : this(aliases)
    {
        ExecuteArgs = executeArgs;
        ExecuteArgless = null;
        ExecuteArgsMessage = null;
    }

    private ChatCommand(string[] aliases, ExecuteArglessCommand executeArgless) : this(aliases)
    {
        ExecuteArgs = null;
        ExecuteArgless = executeArgless;
        ExecuteArgsMessage = null;
    }

    private ChatCommand(string[] aliases, ExecuteArgsMessageCommand executeArgsMessage) : this(aliases)
    {
        ExecuteArgs = null;
        ExecuteArgless = null;
        ExecuteArgsMessage = executeArgsMessage;
    }

    public static void Execute(ChatCommand command, string[] args, string message)
    {
        if (command == null)
            Run("<#FF0000FF>⚠ Invalid Command ⚠</color>", "This command does not exist.");
        else if (command.ExecuteArgless != null)
            command.ExecuteArgless();
        else if (command.ExecuteArgs != null)
            command.ExecuteArgs(args);
        else if (command.ExecuteArgsMessage != null)
            command.ExecuteArgsMessage(args, message);
        else
            Run("<#FFFF00FF>⚠ Huh? ⚠</color>", "Weird...");
    }

    public static ChatCommand Find(string[] args) => AllCommands.Find(x => x.Aliases.Any(x => args[0] == $"/{x}"));

    public static void Run(string title, string text, bool withColor = true, bool hasColor = false, UColor? color = null)
    {
        var chat = Chat();
        var pooledBubble = chat.GetPooledBubble();

        try
        {
            pooledBubble.transform.SetParent(chat.scroller.Inner);
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
            chat.AlignAllBubbles();
            chat.chatNotification.SetUp(CustomPlayer.Local, text);

            if (!chat.IsOpenOrOpening && chat.notificationRoutine == null)
                chat.notificationRoutine = chat.StartCoroutine(chat.BounceDot());

            Play("Chat");
        }
        catch (Exception ex)
        {
            Error(ex);
            chat.chatBubblePool.Reclaim(pooledBubble);
        }
    }

    private static void Whisper(string[] args, string arg)
    {
        if (!GameModifiers.Whispers && IsInGame())
        {
            Run("<#00FF00FF>⚠ No Whispering ⚠</color>", "Whispering is not turned on.");
            return;
        }

        var args2 = arg.Split([ "(", ")" ], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (args.Length < 3 || IsNullEmptyOrWhiteSpace(args[1]) || IsNullEmptyOrWhiteSpace(args[2]))
        {
            Run("<#00FF00FF>★ Help ★</color>", "Usage: /<whisper | w> <meeting number | (player name in ())> <message>");
            return;
        }

        var message = "";
        PlayerControl whispered = null;

        if (CustomPlayer.LocalCustom.Dead)
            Run("<#FFFF00FF>米 Shhhh 米</color>", "You are dead.");
        else if (CustomPlayer.Local.IsBlackmailed())
            Run("<#02A752FF>米 Shhhh 米</color>", "You are blackmailed.");
        else if (CustomPlayer.Local.SilenceActive())
            Run("<#AAB43EFF>米 Shhhh 米</color>", "You are silenced.");
        else if (byte.TryParse(args[1], out var id))
        {
            whispered = PlayerById(id);
            args[2..].ForEach(arg2 => message += $"{arg2} ");
            message = message.Remove(message.Length - 1);
        }
        else if (AllPlayers().TryFinding(x => x.Data.PlayerName == args2[1], out whispered))
            message = args2[2][1..];

        if (!whispered)
            Run("<#FF0000FF>⚠ Whispering Error ⚠</color>", $"Who are you trying to whisper? {arg} is invalid.");
        else if (whispered.AmOwner)
            Run("<#FF0000FF>⚠ Whispering Error ⚠</color>", "Don't whisper to yourself, weirdo.");
        else if (whispered.HasDied() && !CustomPlayer.Local.HasDied())
            Run("<#FF0000FF>⚠ Whispering Error ⚠</color>", $"{whispered.name} is not in this world anymore.");
        else if (!whispered.HasDied() && CustomPlayer.Local.HasDied())
            Run("<#FF0000FF>⚠ Whispering Error ⚠</color>", $"{whispered.name} is not a real Medium!");
        else
        {
            Run("<#4D4DFFFF>「 Whispers 」</color>", $"You whisper to {whispered.name}: {message}");
            CallRpc(CustomRPC.Misc, MiscRPC.Whisper, CustomPlayer.Local, whispered, message);
        }
    }

    private static void ToggleIgnore(string[] args, string arg)
    {
        if (IsLobby())
        {
            Run("<#FF0000FF>⚠ Invalid Command ⚠</color>", "This command does not exist.");
            return;
        }

        if (args.Length < 2)
        {
            Run("<#00FF00FF>★ Help ★</color>", "Usage: /<ignore | i | unignore | ui> <meeting number | (player name in ||)>");
            return;
        }

        var args2 = arg.Split([ "(", ")" ], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (byte.TryParse(args[1], out var id))
        {
            if (arg.Contains("unignore"))
                ChatChannels.Ignored.RemoveAll(x => x == id);
            else
                ChatChannels.Ignored.Add(id);

            Run("<#99007FFF>《 Ignoring 》</color>", $"Toggled ignore for {id}");
        }
        else if (AllPlayers().TryFinding(x => x.Data.PlayerName == args2[1], out var player))
        {
            if (arg.Contains("unignore"))
                ChatChannels.Ignored.RemoveAll(x => x == player.PlayerId);
            else
                ChatChannels.Ignored.Add(player.PlayerId);

            Run("<#99007FFF>《 Ignoring 》</color>", $"Toggled ignore for {args2[1]}");
        }
        else
            Run("<#FF0000FF>⚠ Ignoring Error ⚠</color>", $"Who are you trying to ignore/unignore? {arg} is invalid.");
    }

    private static void SetName(string[] args)
    {
        if (!TownOfUsReworked.IsTest || !IsLobby())
        {
            Run("<#FF0000FF>⚠ Invalid Command ⚠</color>", "This command does not exist.");
            return;
        }

        if (args.Length < 2 || IsNullEmptyOrWhiteSpace(args[1]))
        {
            Run("<#00FF00FF>★ Help ★</color>", "Usage: /<setname | sn> <name>");
            return;
        }

        var arg = "";
        args[1..].ForEach(arg2 => arg += $"{arg2} ");
        arg = arg.Remove(arg.Length - 1);

        if (arg.Any(Disallowed.Contains))
            Run("<#FF0000FF>⚠ Name Error ⚠</color>", "Name contains disallowed characters.");
        else if (Profanities.Any(arg.Contains))
            Run("<#FF0000FF>⚠ Name Error ⚠</color>", "Name contains unaccepted words.");
        else if (arg.Length > 20)
            Run("<#FF0000FF>⚠ Name Error ⚠</color>", "Name is too long.");
        else
        {
            CustomPlayer.Local.RpcSetName(arg);
            Run("<#B148E2FF>◈ Success ◈</color>", "Name changed!");
        }
    }

    private static void Summary()
    {
        var summary = ReadDiskText("Summary", TownOfUsReworked.Other);

        if (IsNullEmptyOrWhiteSpace(summary))
            Run("<#FF0000FF>⚠ Summary Error ⚠</color>", "Summary could not be found.");
        else
            Run("<#FF0080FF>个 Previous Game Summary 个</color>", summary, false, true);
    }

    private static void Clear()
    {
        if (!IsLobby())
        {
            Run("<#FF0000FF>⚠ Invalid Command ⚠</color>", "This command does not exist.");
            return;
        }

        if (!AmongUsClient.Instance.CanBan() || !AmongUsClient.Instance.AmHost)
        {
            Run("<#FF0000FF>⚠ Not Allowed ⚠</color>", "This command is not for you to use.");
            return;
        }

        foreach (var player2 in AllPlayers())
        {
            if (!player2.AmOwner)
            {
                var client = AmongUsClient.Instance.GetClient(player2.OwnerId);

                if (client != null)
                    AmongUsClient.Instance.KickPlayer(client.Id, false);
            }
        }

        Run("<#B148E2FF>◈ Success ◈</color>", "Lobby cleared!");
    }

    private static void KickBan(string[] args)
    {
        if (!IsLobby())
        {
            Run("<#FF0000FF>⚠ Invalid Command ⚠</color>", "This command does not exist.");
            return;
        }

        if (!AmongUsClient.Instance.CanBan() || !AmongUsClient.Instance.AmHost)
        {
            Run("<#FF0000FF>⚠ Not Allowed ⚠</color>", "This command is not for you to use.");
            return;
        }

        var ban = args[0].Replace("/", "") is "ban" or "b";

        if (args.Length < 2 || IsNullEmptyOrWhiteSpace(args[1]))
        {
            Run("<#00FF00FF>★ Help ★</color>", $"Usage: /{(ban ? "<ban | b>" : "<kick | k>")} <player name>");
            return;
        }

        var arg = "";
        args[1..].ForEach(arg2 => arg += $"{arg2} ");
        arg = arg.Remove(arg.Length - 1);

        if (!AllPlayers().TryFinding(x => x.Data.PlayerName == arg, out var target))
        {
            Run($"<#FF0000FF>⚠ {(ban ? "Ban" : "Kick")} Error ⚠</color>", $"Could not find {arg}.");
            return;
        }

        if (target.AmOwner)
        {
            Run($"<#FF0000FF>⚠ {(ban ? "Ban" : "Kick")} Error ⚠</color>", $"Don't {(ban ? "ban" : "kick")} yourself.");
            return;
        }

        var client = AmongUsClient.Instance.GetClient(target.OwnerId);

        if (client == null)
        {
            Run($"<#FF0000FF>⚠ {(ban ? "Ban" : "Kick")} Error ⚠</color>", $"Could not find {arg}.");
            return;
        }

        AmongUsClient.Instance.KickPlayer(client.Id, ban);
        Run("<#B148E2FF>◈ Success ◈</color>", $"{target.name} {(ban ? "Bann" : "Kick")}ed!");
    }

    private static void Help()
    {
        var setColor = TownOfUsReworked.IsTest ? "/setname" : "";
        var comma = setColor.Length == 0 ? "" : ", ";
        var kickBan = comma + (AmongUsClient.Instance.AmHost && AmongUsClient.Instance.CanBan() ? "/kick, /ban, /clearlobby" : "");
        var test = TownOfUsReworked.IsTest ? ", /testargs, /testargless, /rpc" : "";
        var lobby = setColor + (!IsNullEmptyOrWhiteSpace(kickBan) ? $"\n\nCommands available in lobby:\n{setColor}{kickBan}" : "");
        Run("<#0000FFFF>✿ Help Menu ✿</color>", $"Commands available all the time:\n/help, /controls, /summary, /whisper{test}\n\nCommands available in game:\n/ignore{lobby}");
    }

    private static void Controls() => Run("<#6697FFFF>◆ Controls ◆</color>", "Here are the controls:\nF1 - Toggle the visibility of the MCI control panel (local only)\nUp/Left Arrow - " +
        "Go up a page when in a menu\nDown/Right Arrow - Go down a page when in a menu");

    // private static void TestArgs(string[] args)
    // {
    //     var message = "You entered the following params:\n";
    //     args[1..].ForEach(arg => message += $"{arg}, ");
    //     message = message.Remove(message.Length - 2);
    //     Run("<#FF00FFFF>⚠ TEST ⚠</color>", message);
    // }

    // private static void TestArgsMessage(string[] args, string arg)
    // {
    //     var message = "You entered the following params:\n";
    //     args[1..].ForEach(arg => message += $"{arg}, ");
    //     message = message.Remove(message.Length - 2);
    //     message += $"\nAnd this is the while thing you sent:\n{arg}";
    //     Run("<#FF00FFFF>⚠ TEST ⚠</color>", message);
    // }

    // private static void TestArgless() => Run("<#FF00FFFF>⚠ TEST ⚠</color>", "Test.");

    // private static void SendRPCArgless()
    // {
    //     CallRpc(CustomRPC.Test, TestRPC.Argless);
    //     Message("RPC Sent!");
    //     Run("<#FF00FFFF>⚠ RPC TEST ⚠</color>", "RPC Sent!");
    // }

    // private static void SendRPCArgs(string[] args)
    // {
    //     var message = "You entered the following params:\n";
    //     var writer = CallOpenRpc(CustomRPC.Test, TestRPC.Args);

    //     foreach (var arg in args[1..])
    //     {
    //         writer.Write(arg);
    //         message += $"{arg} ";
    //     }

    //     writer.EndRpc();
    //     Message("RPC Sent!");
    //     Run("<#FF00FFFF>⚠ RPC TEST ⚠</color>", $"RPC Sent!\nWith the following message: {message}");
    // }

    // private static void Translate(string[] args)
    // {
    //     if (args.Length < 2 || IsNullEmptyOrWhiteSpace(args[1]))
    //         Run("<#00FF00FF>★ Help ★</color>", "Usage: /<translate | trans> <text id>");
    //     else
    //         Run("<#B148E2FF>◈ Success ◈</color>", TranslationManager.Test(args[1]));
    // }
}