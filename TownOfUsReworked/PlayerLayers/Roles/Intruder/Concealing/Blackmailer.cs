namespace TownOfUsReworked.PlayerLayers.Roles;

public class Blackmailer : Intruder
{
    public CustomButton BlackmailButton { get; set; }
    public PlayerControl BlackmailedPlayer { get; set; }
    public DateTime LastBlackmailed { get; set; }
    public bool ShookAlready { get; set; }
    public Sprite PrevOverlay { get; set; }
    public Color PrevColor { get; set; }

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Blackmailer : Colors.Intruder;
    public override string Name => "Blackmailer";
    public override LayerEnum Type => LayerEnum.Blackmailer;
    public override Func<string> StartText => () => "You Know Their Secrets";
    public override Func<string> Description => () => "- You can silence players to ensure they cannot hear what others say\n" + (CustomGameOptions.BMRevealed ? ("- Everyone will be " +
        "alerted at the start of the meeting that someone has been silenced ") : "") + (CustomGameOptions.WhispersNotPrivate ? "\n- You can read whispers during meetings" : "") +
        $"\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.GainsInfo;
    public float Timer => ButtonUtils.Timer(Player, LastBlackmailed, CustomGameOptions.BlackmailCd);

    public Blackmailer(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderConceal;
        BlackmailedPlayer = null;
        BlackmailButton = new(this, "Blackmail", AbilityTypes.Direct, "Secondary", Blackmail, Exception1);
    }

    public void Blackmail()
    {
        if (Timer != 0f || IsTooFar(Player, BlackmailButton.TargetPlayer) || BlackmailButton.TargetPlayer == BlackmailedPlayer)
            return;

        var interact = Interact(Player, BlackmailButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            BlackmailedPlayer = BlackmailButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.Blackmail, this, BlackmailedPlayer);
        }

        if (interact.Reset)
            LastBlackmailed = DateTime.UtcNow;
        else if (interact.Protected)
            LastBlackmailed.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public bool Exception1(PlayerControl player) => player == BlackmailedPlayer || (player.Is(Faction) && Faction != Faction.Crew && CustomGameOptions.BlackmailMates) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && CustomGameOptions.BlackmailMates);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        BlackmailButton.Update("BLACKMAIL", Timer, CustomGameOptions.BlackmailCd);
    }
}