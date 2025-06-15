namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Escort)]
public sealed class Escort : CSupport, IBlocker
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number EscortCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number EscortDur = 10;

    public PlayerControl BlockTarget { get; private set; }
    private CustomButton BlockButton;

    protected override UColor MainColor => CustomColorManager.Escort;
    public override Layer Type => Layer.Escort;
    public override string StartText => "Roleblock Players From Harming The <#8CFFFFFF>Crew</color>";
    public override string Description => "- You can seduce players\n- Seduction blocks your target from being able to use their abilities for a short while\n- You are immune " +
        "to blocks\n- If you attempt to block a <#336EFFFF>Serial Killer</color>, they will be forced to kill you";
    public override bool RoleBlockImmune => true;

    public override void Init()
    {
        base.Init();
        BlockTarget = null;
        BlockButton ??= new(this, "ROLEBLOCK", new SpriteName("EscortRoleblock"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Roleblock, (EffectEndVoid)UnBlock,
            new Cooldown(EscortCd), new Duration(EscortDur), (EndFunc)EndEffect, (EffectStartVoid)BlockStart);
    }

    private void UnBlock()
    {
        if (BlockTarget.AmOwner)
            BlockExposed = false;

        BlockTarget = null;
    }

    private void BlockStart()
    {
        if (BlockTarget.AmOwner)
            CustomStatsManager.IncrementStat(CustomStatsManager.StatsRoleblocked);
    }

    private void Roleblock(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            BlockTarget = target;
            BlockButton.TriggerRpcAndBegin(BlockTarget);
        }
        else
            BlockButton.StartCooldown(cooldown);
    }

    public override void ReadRPC(RpcReader reader)
    {
        BlockTarget = reader.ReadPlayer();

        if (BlockTarget.AmOwner)
            CustomStatsManager.IncrementStat(CustomStatsManager.StatsRoleblocked);
    }

    private bool EndEffect() => Dead || (BlockTarget && BlockTarget.HasDied());

    public override void Reset(bool meeting, bool start) => BlockTarget = null;
}