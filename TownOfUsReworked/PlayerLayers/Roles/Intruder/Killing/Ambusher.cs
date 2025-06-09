namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Ambusher)]
public sealed class Ambusher : Intruder
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number AmbushCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number AmbushDur = 10;

    [ToggleOption]
    public static bool AmbushMates = false;

    public PlayerControl AmbushedPlayer { get; private set; }
    public CustomButton AmbushButton { get; private set; }

    protected override UColor MainColor => CustomColorManager.Ambusher;
    public override LayerEnum Type => LayerEnum.Ambusher;
    public override Func<string> StartText { get; } = () => "Spook The <#8CFFFFFF>Crew</color>";
    public override Func<string> Description => () => $"- You can ambush players\n- Ambushed players will be forced to be on alert and kill whoever interacts with them\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Killing;
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
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, AmbushButton, AmbushedPlayer);
            AmbushButton.Begin();
        }
        else
            AmbushButton.StartCooldown(cooldown);

        if (IntruderKillingSettings.KillCdLinked)
            KillButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == AmbushedPlayer || (!AmbushMates && Player.IsBuddyWith(player, Faction));

    private bool EndEffect() => Dead || (AmbushedPlayer && AmbushedPlayer.HasDied());

    public override void ReadRPC(RpcReader reader) => AmbushedPlayer = reader.ReadPlayer();
}