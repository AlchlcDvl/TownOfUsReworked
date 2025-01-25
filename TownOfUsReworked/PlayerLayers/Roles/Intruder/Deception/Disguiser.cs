namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Disguiser : Intruder
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

    public CustomButton DisguiseButton { get; set; }
    public CustomButton MeasureButton { get; set; }
    public PlayerControl MeasuredPlayer { get; set; }
    public PlayerControl CopiedPlayer { get; set; }
    public PlayerControl DisguisedPlayer { get; set; }
    public bool Disguised => DisguiseButton.EffectActive;

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Disguiser : FactionColor;
    public override LayerEnum Type => LayerEnum.Disguiser;
    public override Func<string> StartText => () => "Disguise The <#8CFFFFFF>Crew</color> To Frame Them";
    public override Func<string> Description => () => $"- You can disguise a player into someone else's appearance\n{CommonAbilities}";

    public override void Init()
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

    public void Disguise() => Morph(DisguisedPlayer, CopiedPlayer);

    public void UnDisguise()
    {
        DefaultOutfit(DisguisedPlayer);
        DisguisedPlayer = null;
        CopiedPlayer = null;
    }

    public void HitDisguise(PlayerControl target)
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

    public void Measure(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            MeasuredPlayer = target;

        MeasureButton.StartCooldown(cooldown);

        if (DisgCooldownsLinked)
            DisguiseButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => Exception2(player) || (((player.Is(Faction) && DisguiseTarget == DisguiserTargets.NonIntruders) || (!player.Is(Faction) && DisguiseTarget
        == DisguiserTargets.Intruders)) && Faction is Faction.Intruder or Faction.Syndicate);

    public bool Exception2(PlayerControl player) => player == MeasuredPlayer;

    public bool Usable() => MeasuredPlayer;

    public bool EndEffect() => (DisguisedPlayer && DisguisedPlayer.HasDied()) || Dead;

    public override void ReadRPC(MessageReader reader)
    {
        CopiedPlayer = reader.ReadPlayer();
        DisguisedPlayer = reader.ReadPlayer();
    }
}