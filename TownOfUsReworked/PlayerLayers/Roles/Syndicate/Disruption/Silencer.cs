namespace TownOfUsReworked.PlayerLayers.Roles;

public class Silencer : Syndicate
{
    public CustomButton SilenceButton { get; set; }
    public PlayerControl SilencedPlayer { get; set; }
    public bool ShookAlready { get; set; }
    public Sprite PrevOverlay { get; set; }
    public Color PrevColor { get; set; }

    public override Color Color => ClientGameOptions.CustomSynColors ? Colors.Silencer : Colors.Syndicate;
    public override string Name => "Silencer";
    public override LayerEnum Type => LayerEnum.Silencer;
    public override Func<string> StartText => () => "You Are The One Who Hushes";
    public override Func<string> Description => () => "- You can silence players to ensure they cannot hear what others say" + (CustomGameOptions.SilenceRevealed ? "\n- Everyone will be"  +
        "alerted at the start of the meeting that someone has been silenced " : "") + (CustomGameOptions.WhispersNotPrivateSilencer ? "\n- You can read whispers during meetings" : "") +
        CommonAbilities;
    public override InspectorResults InspectorResults => InspectorResults.GainsInfo;

    public Silencer(PlayerControl player) : base(player)
    {
        Alignment = Alignment.SyndicateDisrup;
        SilencedPlayer = null;
        SilenceButton = new(this, "Silence", AbilityTypes.Target, "Secondary", Silence, CustomGameOptions.SilenceCd, Exception1);
    }

    public void Silence()
    {
        var interact = Interact(Player, SilenceButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
        {
            SilencedPlayer = SilenceButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, SilencedPlayer);
        }

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        SilenceButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == SilencedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate && !CustomGameOptions.SilenceMates) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !CustomGameOptions.SilenceMates);

    public override void ReadRPC(MessageReader reader) => SilencedPlayer = reader.ReadPlayer();

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        SilenceButton.Update2("SILENCE");
    }
}