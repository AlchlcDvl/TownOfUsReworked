namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Ambusher)]
public sealed class Ambusher : IKilling
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number AmbushCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number AmbushDur = 10;

    [ToggleOption]
    private static bool AmbushMates = false;

    public PlayerControl AmbushedPlayer;
    public CustomButton AmbushButton;

    protected override UColor MainColor => CustomColorManager.Ambusher;
    public override Layer Type => Layer.Ambusher;
    public override string StartText => "Spook The <#8CFFFFFF>Crew</color>";
    public override string Description => $"- You can ambush players\n- Ambushed players will be forced to be on alert and kill whoever interacts with them\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        AmbushedPlayer = null;
        AmbushButton ??= new(this, new SpriteName("Ambush"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Ambush, new Cooldown(AmbushCd), (EndFunc)EndEffect, "AMBUSH",
            new Duration(AmbushDur), (EffectEndVoid)UnAmbush, (PlayerBodyExclusion)Exception1);
    }

    public override void Reset(bool meeting, bool start) => AmbushedPlayer = null;

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (AmbushedPlayer == player)
            name += " <#2BD29CFF>人</color>";
    }

    private void UnAmbush() => AmbushedPlayer = null;

    private void Ambush(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            AmbushedPlayer = target;
            AmbushButton.TriggerRpcAndBegin(AmbushedPlayer);
        }
        else
            AmbushButton.StartCooldown(cooldown);

        if (IntruderKillingSettings.KillCdLinked)
            KillButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == AmbushedPlayer || (!AmbushMates && Player.IsBuddyWith(player, Handler.CurrentFaction));

    private bool EndEffect() => Dead || (AmbushedPlayer && AmbushedPlayer.HasDied());

    public override void ReadRPC(RpcReader reader) => AmbushedPlayer = reader.ReadPlayer();
}