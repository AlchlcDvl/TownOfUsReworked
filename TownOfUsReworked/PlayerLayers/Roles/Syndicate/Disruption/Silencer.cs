namespace TownOfUsReworked.PlayerLayers.Roles;

public class Silencer : Syndicate
{
    public CustomButton SilenceButton { get; set; }
    public PlayerControl SilencedPlayer { get; set; }
    public DateTime LastSilenced { get; set; }
    public bool ShookAlready { get; set; }
    public Sprite PrevOverlay { get; set; }
    public Color PrevColor { get; set; }

    public override Color Color => ClientGameOptions.CustomSynColors ? Colors.Silencer : Colors.Syndicate;
    public override string Name => "Silencer";
    public override LayerEnum Type => LayerEnum.Silencer;
    public override Func<string> StartText => () => "You Are The One Who Hushes";
    public override Func<string> Description => () => "- You can silence players to ensure they cannot hear what others say" + (CustomGameOptions.SilenceRevealed ? "\n- Everyone will"
        + " be alerted at the start of the meeting that someone has been silenced " : "") + (CustomGameOptions.WhispersNotPrivateSilencer ? "\n- You can read whispers during meetings"
        : "") + CommonAbilities;
    public override InspectorResults InspectorResults => InspectorResults.GainsInfo;
    public float Timer => ButtonUtils.Timer(Player, LastSilenced, CustomGameOptions.SilenceCd);

    public Silencer(PlayerControl player) : base(player)
    {
        Alignment = Alignment.SyndicateDisrup;
        SilencedPlayer = null;
        SilenceButton = new(this, "Silence", AbilityTypes.Direct, "Secondary", Silence, Exception1);
    }

    public void Silence()
    {
        if (Timer != 0f || IsTooFar(Player, SilenceButton.TargetPlayer) || SilenceButton.TargetPlayer == SilencedPlayer)
            return;

        var interact = Interact(Player, SilenceButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            SilencedPlayer = SilenceButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.Silence, this, SilencedPlayer);
        }

        if (interact.Reset)
            LastSilenced = DateTime.UtcNow;
        else if (interact.Protected)
            LastSilenced.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public bool Exception1(PlayerControl player) => player == SilencedPlayer || (player.Is(Faction) && Faction != Faction.Crew && CustomGameOptions.SilenceMates) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && CustomGameOptions.SilenceMates);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        SilenceButton.Update("SILENCE", Timer, CustomGameOptions.SilenceCd);
    }
}