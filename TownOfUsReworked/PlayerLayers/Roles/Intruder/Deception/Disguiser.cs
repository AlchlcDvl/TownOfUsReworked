namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Disguiser)]
public sealed class Disguiser : Intruder
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number DisguiseCd = 25;

    [NumberOption(2.5f, 15f, 2.5f, Format.Time)]
    public static Number DisguiseDelay = 5;

    [NumberOption(5f, 30f, 2.5f, Format.Time)]
    public static Number DisguiseDur = 10;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number MeasureCd = 25;

    [ToggleOption]
    public static bool DisgCooldownsLinked = false;

    [StringOption<DisguiserTargets>]
    public static DisguiserTargets DisguiseTarget = DisguiserTargets.Everyone;

    private CustomButton DisguiseButton { get; set; }
    private CustomButton MeasureButton { get; set; }
    private PlayerControl MeasuredPlayer { get; set; }
    private PlayerControl CopiedPlayer { get; set; }
    private PlayerControl DisguisedPlayer { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Disguiser : FactionColor;
    public override LayerEnum Type => LayerEnum.Disguiser;
    public override Func<string> StartText => () => "Disguise The <#8CFFFFFF>Crew</color> To Frame Them";
    public override Func<string> Description => () => $"- You can disguise a player into someone else's appearance\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Deception;
        MeasureButton ??= new(this, new SpriteName("Measure"), AbilityTypes.Player, KeybindType.Tertiary, (OnClickPlayer)Measure, new Cooldown(MeasureCd), "MEASURE",
            (PlayerBodyExclusion)Exception2);
        DisguiseButton ??= new(this, new SpriteName("Disguise"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)HitDisguise, new Cooldown(DisguiseCd), (EffectEndVoid)UnDisguise,
            new Duration(DisguiseDur), (EffectVoid)Disguise, new Delay(DisguiseDelay), (PlayerBodyExclusion)Exception1, (UsableFunc)Usable, (EndFunc)EndEffect, "DISGUISE");
        DisguisedPlayer = null;
        MeasuredPlayer = null;
        CopiedPlayer = null;
    }

    public override void Reset(bool meeting, bool start) => MeasuredPlayer = DisguisedPlayer = CopiedPlayer = null;

    private void Disguise() => Morph(DisguisedPlayer, CopiedPlayer);

    private void UnDisguise()
    {
        DefaultOutfit(DisguisedPlayer);
        DisguisedPlayer = null;
        CopiedPlayer = null;
    }

    private void HitDisguise(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            CopiedPlayer = MeasuredPlayer;
            DisguisedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, DisguiseButton, CopiedPlayer, DisguisedPlayer);
            DisguiseButton.Begin();
        }
        else
            DisguiseButton.StartCooldown(cooldown);

        if (DisgCooldownsLinked)
            MeasureButton.StartCooldown(cooldown);
    }

    private void Measure(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            MeasuredPlayer = target;

        MeasureButton.StartCooldown(cooldown);

        if (DisgCooldownsLinked)
            DisguiseButton.StartCooldown(cooldown);
    }

    private bool Exception1(PlayerControl player) => Exception2(player) || (((player.Is(Faction) && DisguiseTarget == DisguiserTargets.NonIntruders) || (!player.Is(Faction) && DisguiseTarget
        == DisguiserTargets.Intruders)) && Faction is Faction.Intruder or Faction.Syndicate);

    private bool Exception2(PlayerControl player) => player == MeasuredPlayer;

    private bool Usable() => MeasuredPlayer;

    private bool EndEffect() => (DisguisedPlayer && DisguisedPlayer.HasDied()) || Dead;

    public override void ReadRPC(MessageReader reader)
    {
        CopiedPlayer = reader.ReadPlayer();
        DisguisedPlayer = reader.ReadPlayer();
    }
}