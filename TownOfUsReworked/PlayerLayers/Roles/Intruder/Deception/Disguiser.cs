namespace TownOfUsReworked.PlayerLayers.Roles;

public class Disguiser : Intruder
{
    public CustomButton DisguiseButton { get; set; }
    public CustomButton MeasureButton { get; set; }
    public PlayerControl MeasuredPlayer { get; set; }
    public PlayerControl CopiedPlayer { get; set; }
    public PlayerControl DisguisedPlayer { get; set; }
    public bool Disguised => DisguiseButton.EffectActive;

    public override UColor Color => ClientGameOptions.CustomIntColors ? CustomColorManager.Disguiser : CustomColorManager.Intruder;
    public override string Name => "Disguiser";
    public override LayerEnum Type => LayerEnum.Disguiser;
    public override Func<string> StartText => () => "Disguise The <color=#8CFFFFFF>Crew</color> To Frame Them";
    public override Func<string> Description => () => $"- You can disguise a player into someone else's appearance\n{CommonAbilities}";

    public Disguiser() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.IntruderDecep;
        MeasureButton = new(this, "Measure", AbilityTypes.Alive, "Tertiary", Measure, CustomGameOptions.MeasureCd, Exception2);
        DisguiseButton = new(this, "Disguise", AbilityTypes.Alive, "Secondary", HitDisguise, CustomGameOptions.DisguiseCd, CustomGameOptions.DisguiseDur, Disguise, UnDisguise,
            CustomGameOptions.DisguiseDelay, Exception1);
        DisguisedPlayer = null;
        MeasuredPlayer = null;
        CopiedPlayer = null;
        return this;
    }

    public void Disguise() => Morph(DisguisedPlayer, CopiedPlayer);

    public void UnDisguise()
    {
        DefaultOutfit(DisguisedPlayer);
        DisguisedPlayer = null;
        CopiedPlayer = null;
    }

    public void HitDisguise()
    {
        var cooldown = Interact(Player, DisguiseButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            CopiedPlayer = MeasuredPlayer;
            DisguisedPlayer = DisguiseButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, DisguiseButton, CopiedPlayer, DisguisedPlayer);
            DisguiseButton.Begin();
        }
        else
            DisguiseButton.StartCooldown(cooldown);

        if (CustomGameOptions.DisgCooldownsLinked)
            DisguiseButton.StartCooldown(cooldown);
    }

    public void Measure()
    {
        var cooldown = Interact(Player, MeasureButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            MeasuredPlayer = MeasureButton.TargetPlayer;

        MeasureButton.StartCooldown(cooldown);

        if (CustomGameOptions.DisgCooldownsLinked)
            DisguiseButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => Exception2(player) || (((player.Is(Faction) && CustomGameOptions.DisguiseTarget == DisguiserTargets.NonIntruders) || (!player.Is(Faction)
        && CustomGameOptions.DisguiseTarget == DisguiserTargets.Intruders)) && Faction is Faction.Intruder or Faction.Syndicate);

    public bool Exception2(PlayerControl player) => player == MeasuredPlayer;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        MeasureButton.Update2("MEASURE");
        DisguiseButton.Update2("DISGUISE", MeasuredPlayer != null);
    }

    public override void TryEndEffect() => DisguiseButton.Update3((DisguisedPlayer != null && DisguisedPlayer.HasDied()) || IsDead);

    public override void ReadRPC(MessageReader reader)
    {
        CopiedPlayer = reader.ReadPlayer();
        DisguisedPlayer = reader.ReadPlayer();
    }
}