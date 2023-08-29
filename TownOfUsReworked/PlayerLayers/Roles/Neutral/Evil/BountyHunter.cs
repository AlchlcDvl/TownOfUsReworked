namespace TownOfUsReworked.PlayerLayers.Roles;

public class BountyHunter : Neutral
{
    public PlayerControl TargetPlayer { get; set; }
    public bool TargetKilled { get; set; }
    public bool ColorHintGiven { get; set; }
    public bool RoleHintGiven { get; set; }
    public bool TargetFound { get; set; }
    public DateTime LastChecked { get; set; }
    public CustomButton GuessButton { get; set; }
    public CustomButton HuntButton { get; set; }
    public CustomButton RequestButton { get; set; }
    public PlayerControl RequestingPlayer { get; set; }
    public PlayerControl TentativeTarget { get; set; }
    public bool ButtonUsable => UsesLeft > 0;
    public bool Failed => (UsesLeft <= 0 && !TargetFound) || (!TargetKilled && (TargetPlayer.Data.IsDead || TargetPlayer.Data.Disconnected));
    public int UsesLeft { get; set; }
    private int LettersGiven { get; set; }
    private bool LettersExhausted { get; set; }
    private readonly List<string> Letters = new();
    public bool CanHunt => (TargetFound && !TargetPlayer.Data.IsDead && !TargetPlayer.Data.Disconnected) || (TargetKilled && !CustomGameOptions.AvoidNeutralKingmakers);
    public bool CanRequest => (RequestingPlayer == null || RequestingPlayer.Data.IsDead || RequestingPlayer.Data.Disconnected) && TargetPlayer == null;
    public bool Assigned { get; set; }
    public int Rounds { get; set; }
    public bool TargetFailed => TargetPlayer == null && Rounds > 2;

    public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.BountyHunter : Colors.Neutral;
    public override string Name => "Bounty Hunter";
    public override LayerEnum Type => LayerEnum.BountyHunter;
    public override Func<string> StartText => () => "Find And Kill Your Target";
    public override Func<string> Description => () => TargetPlayer == null ? "- You can request a hit from a player to set your bounty" : ("- You can guess a player to be your " +
        "bounty\n- Upon finding the bounty, you can kill them\n- After your bounty has been killed by you, you can kill others as many times as you want\n- If your target dies not by "
        + "your hands, you will become a <color=#678D36FF>Troll</color>");
    public override InspectorResults InspectorResults => InspectorResults.TracksOthers;
    public float Timer => ButtonUtils.Timer(Player, LastChecked, CustomGameOptions.GuessCd);

    public BountyHunter(PlayerControl player) : base(player)
    {
        Objectives = () => TargetKilled ? "- You have completed the bounty" : (TargetPlayer == null ? "- Recieve a bounty" : "- Find and kill your target");
        RoleAlignment = RoleAlignment.NeutralEvil;
        UsesLeft = CustomGameOptions.BountyHunterGuesses;
        TargetPlayer = null;
        GuessButton = new(this, "BHGuess", AbilityTypes.Direct, "Secondary", Guess, true);
        HuntButton = new(this, "Hunt", AbilityTypes.Direct, "ActionSecondary", Hunt);
        RequestButton = new(this, "Request", AbilityTypes.Direct, "Tertiary", Request, Exception);
    }

    public bool Exception(PlayerControl player) => player == TargetPlayer || player.IsLinkedTo(Player) || GetRole(player).Requesting || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None);

    public void TurnTroll()
    {
        var newRole = new Troll(Player);
        newRole.RoleUpdate(this);

        if (Local)
            Flash(Colors.Troll);

        if (CustomPlayer.Local.Is(LayerEnum.Seer))
            Flash(Colors.Seer);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (TargetPlayer == null)
            return;

        var targetName = TargetPlayer.name;
        var something = "";

        if (!LettersExhausted)
        {
            var random = URandom.RandomRangeInt(0, targetName.Length);
            var random2 = URandom.RandomRangeInt(0, targetName.Length);
            var random3 = URandom.RandomRangeInt(0, targetName.Length);

            if (LettersGiven <= targetName.Length - 3)
            {
                while (random == random2 || random2 == random3 || random == random3 || Letters.Contains($"{targetName[random]}") || Letters.Contains($"{targetName[random2]}") ||
                    Letters.Contains($"{targetName[random3]}"))
                {
                    if (random == random2 || Letters.Contains($"{targetName[random2]}"))
                        random2 = URandom.RandomRangeInt(0, targetName.Length);

                    if (random2 == random3 || Letters.Contains($"{targetName[random3]}"))
                        random3 = URandom.RandomRangeInt(0, targetName.Length);

                    if (random == random3 || Letters.Contains($"{targetName[random]}"))
                        random = URandom.RandomRangeInt(0, targetName.Length);
                }

                something = $"Your target's name has the Letters {targetName[random]}, {targetName[random2]} and {targetName[random3]} in it!";
            }
            else if (LettersGiven == targetName.Length - 2)
            {
                while (random == random2 || Letters.Contains($"{targetName[random]}") || Letters.Contains($"{targetName[random2]}"))
                {
                    if (Letters.Contains($"{targetName[random2]}"))
                        random2 = URandom.RandomRangeInt(0, targetName.Length);

                    if (Letters.Contains($"{targetName[random]}"))
                        random = URandom.RandomRangeInt(0, targetName.Length);

                    if (random == random2)
                        random = URandom.RandomRangeInt(0, targetName.Length);
                }

                something = $"Your target's name has the Letters {targetName[random]} and {targetName[random2]} in it!";
            }
            else if (LettersGiven == targetName.Length - 1)
            {
                while (Letters.Contains($"{targetName[random]}"))
                    random = URandom.RandomRangeInt(0, targetName.Length);

                something = $"Your target's name has the letter {targetName[random]} in it!";
            }
            else if (LettersGiven == targetName.Length && !LettersExhausted)
                LettersExhausted = true;

            if (!LettersExhausted)
            {
                if (LettersGiven <= targetName.Length - 3)
                {
                    Letters.Add($"{targetName[random]}");
                    Letters.Add($"{targetName[random2]}");
                    Letters.Add($"{targetName[random3]}");
                    LettersGiven += 3;
                }
                else if (LettersGiven == targetName.Length - 2)
                {
                    Letters.Add($"{targetName[random]}");
                    Letters.Add($"{targetName[random2]}");
                    LettersGiven += 2;
                }
                else if (LettersGiven == targetName.Length - 1)
                {
                    Letters.Add($"{targetName[random]}");
                    LettersGiven++;
                }
            }
        }
        else if (!ColorHintGiven)
        {
            something = $"Your target is a {ColorUtils.LightDarkColors[TargetPlayer.CurrentOutfit.ColorId].ToLower()} color!";
            ColorHintGiven = true;
        }
        else if (!RoleHintGiven)
        {
            something = $"Your target is the {GetRole(TargetPlayer)}!";
            RoleHintGiven = true;
        }

        if (string.IsNullOrEmpty(something))
            return;

        //Ensures only the Bounty Hunter sees this
        if (HUD && something != "")
            Run(HUD.Chat, "<color=#B51E39FF>〖 Bounty Hunt 〗</color>", something);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        GuessButton.Update("GUESS", Timer, CustomGameOptions.GuessCd, UsesLeft, true, !TargetFound && TargetPlayer != null);
        HuntButton.Update("HUNT", Timer, CustomGameOptions.GuessCd, true, TargetPlayer != null && CanHunt);
        RequestButton.Update("REQUEST HIT", true, CanRequest);

        if ((TargetFailed || (TargetPlayer != null && Failed)) && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnTroll, this);
            TurnTroll();
        }
    }

    public void Request()
    {
        if (IsTooFar(Player, RequestButton.TargetPlayer))
            return;

        RequestingPlayer = RequestButton.TargetPlayer;
        GetRole(RequestingPlayer).Requesting = true;
        GetRole(RequestingPlayer).Requestor = Player;
        CallRpc(CustomRPC.Action, ActionsRPC.RequestHit, this, RequestingPlayer);
    }

    public void Guess()
    {
        if (IsTooFar(Player, GuessButton.TargetPlayer) || Timer != 0f)
            return;

        if (GuessButton.TargetPlayer != TargetPlayer)
        {
            Flash(new(255, 0, 0, 255));
            UsesLeft--;
        }
        else
        {
            TargetFound = true;
            Flash(new(0, 255, 0, 255));
        }

        LastChecked = DateTime.UtcNow;
    }

    public void Hunt()
    {
        if (IsTooFar(Player, HuntButton.TargetPlayer) || Timer != 0f || !TargetFound)
            return;

        if (HuntButton.TargetPlayer != TargetPlayer && !TargetKilled)
        {
            Flash(new(255, 0, 0, 255));
            LastChecked = DateTime.UtcNow;
        }
        else if (HuntButton.TargetPlayer == TargetPlayer && !TargetKilled)
        {
            var interact = Interact(Player, HuntButton.TargetPlayer, true);

            if (!interact[3])
                RpcMurderPlayer(Player, HuntButton.TargetPlayer);

            TargetKilled = true;
            LastChecked = DateTime.UtcNow;
            CallRpc(CustomRPC.WinLose, WinLoseRPC.BountyHunterWin, this);
        }
        else
        {
            var interact = Interact(Player, HuntButton.TargetPlayer, true);

            if (interact[0] || interact[3])
                LastChecked = DateTime.UtcNow;
            else if (interact[1])
                LastChecked.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastChecked.AddSeconds(CustomGameOptions.VestKCReset);
        }
    }
}