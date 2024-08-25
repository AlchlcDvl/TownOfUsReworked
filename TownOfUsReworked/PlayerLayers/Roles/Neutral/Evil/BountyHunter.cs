namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class BountyHunter : Neutral
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool BountyHunterCanPickTargets { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 1, 15, 1)]
    public static int BountyHunterGuesses { get; set; } = 5;

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float GuessCd { get; set; } = 25f;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool BHVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool VigiKillsBH { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool BHToTroll { get; set; } = true;

    public PlayerControl TargetPlayer { get; set; }
    public bool TargetKilled { get; set; }
    public bool ColorHintGiven { get; set; }
    public bool RoleHintGiven { get; set; }
    public bool TargetFound { get; set; }
    public CustomButton GuessButton { get; set; }
    public CustomButton HuntButton { get; set; }
    public CustomButton RequestButton { get; set; }
    public PlayerControl RequestingPlayer { get; set; }
    public PlayerControl TentativeTarget { get; set; }
    public bool Failed => (!TargetPlayer && Rounds > 2) || (!GuessButton.Usable() && !TargetFound) || (!TargetKilled && TargetPlayer && TargetPlayer.HasDied());
    private int LettersGiven { get; set; }
    private bool LettersExhausted { get; set; }
    private List<string> Letters { get; set; }
    public bool CanHunt => TargetPlayer && ((TargetFound && !TargetPlayer.HasDied()) || (TargetKilled && !NeutralSettings.AvoidNeutralKingmakers));
    public bool CanRequest => RequestingPlayer.HasDied() && !TargetPlayer;
    public bool Assigned { get; set; }
    public int Rounds { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.BountyHunter : CustomColorManager.Neutral;
    public override string Name => "Bounty Hunter";
    public override LayerEnum Type => LayerEnum.BountyHunter;
    public override Func<string> StartText => () => "Find And Kill Your Target";
    public override Func<string> Description => () => !TargetPlayer ? "- You can request a hit from a player to set your bounty" : ("- You can guess a player to be your bounty\n- Upon " +
        "finding the bounty, you can kill them\n- After your bounty has been killed by you, you can kill others as many times as you want\n- If your target dies not by your hands, you will" +
        " become a <color=#678D36FF>Troll</color>");
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;

    public override void Init()
    {
        BaseStart();
        Objectives = () => TargetKilled ? "- You have completed the bounty" : (!TargetPlayer ? "- Recieve a bounty" : "- Find and kill your target");
        Alignment = Alignment.NeutralEvil;
        TargetPlayer = null;
        GuessButton = CreateButton(this, new SpriteName("BHGuess"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Guess, new Cooldown(GuessCd), (UsableFunc)Usable1, "GUESS",
            BountyHunterGuesses);
        HuntButton = CreateButton(this, new SpriteName("Hunt"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Hunt, new Cooldown(GuessCd), "HUNT", (UsableFunc)Usable2);

        if (BountyHunterCanPickTargets)
        {
            RequestButton = CreateButton(this, new SpriteName("Request"), AbilityTypes.Alive, KeybindType.Tertiary, (OnClick)Request, (PlayerBodyExclusion)Exception, "REQUEST HIT",
                (UsableFunc)Usable3);
        }

        Letters = [];
    }

    public bool Exception(PlayerControl player) => player == TargetPlayer || player.IsLinkedTo(Player) || player.GetRole().Requesting || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None);

    public void TurnTroll() => new Troll().Start<Role>(Player).RoleUpdate(this);

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (TargetPlayer.HasDied() || Dead)
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
            something = $"Your target is a {(TargetPlayer.CurrentOutfit.ColorId.IsLighter() ? "lighter" : "darker")} color!";
            ColorHintGiven = true;
        }
        else if (!RoleHintGiven)
        {
            something = $"Your target is a {TargetPlayer.GetRole()}!";
            RoleHintGiven = true;
        }

        if (IsNullEmptyOrWhiteSpace(something))
            return;

        // Ensures only the Bounty Hunter sees this
        if (HUD && something != "")
            Run("<color=#B51E39FF>〖 Bounty Hunt 〗</color>", something);
    }

    public bool Usable1() => TargetPlayer && !CanHunt;

    public bool Usable2() => TargetPlayer && CanHunt;

    public bool Usable3() => Requesting;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Failed && !Dead)
        {
            if (BHToTroll)
            {
                CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this);
                TurnTroll();
            }
            else if (BountyHunterCanPickTargets)
            {
                TargetPlayer = null;
                Rounds = 0;
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, this, 255);
            }
            else
                RpcMurderPlayer(Player);
        }
    }

    public void Request()
    {
        RequestingPlayer = RequestButton.TargetPlayer;
        RequestingPlayer.GetRole().Requesting = true;
        RequestingPlayer.GetRole().Requestor = Player;
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, RequestingPlayer);
    }

    public override void ReadRPC(MessageReader reader)
    {
        var request = reader.ReadPlayer();
        RequestingPlayer = request;
        request.GetRole().Requesting = true;
        request.GetRole().Requestor = Player;
    }

    public void Guess()
    {
        TargetFound = GuessButton.TargetPlayer == TargetPlayer;
        Flash(new(TargetFound ? 0 : 255, TargetFound ? 255 : 0, 0, 255));
        GuessButton.StartCooldown();

        if (TargetFound)
            HuntButton.StartCooldown();
    }

    public void Hunt()
    {
        if (HuntButton.TargetPlayer != TargetPlayer && !TargetKilled)
        {
            Flash(new(255, 0, 0, 255));
            HuntButton.StartCooldown();
        }
        else if (HuntButton.TargetPlayer == TargetPlayer && !TargetKilled)
        {
            TargetKilled = true;
            HuntButton.StartCooldown(Interact(Player, HuntButton.TargetPlayer, true, bypass: true));
            CallRpc(CustomRPC.WinLose, WinLoseRPC.BountyHunterWin, this);
        }
        else
            HuntButton.StartCooldown(Interact(Player, HuntButton.TargetPlayer, true));
    }
}