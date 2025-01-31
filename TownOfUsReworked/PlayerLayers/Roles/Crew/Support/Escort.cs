namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Escort : Crew, IBlocker
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number EscortCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number EscortDur = 10;

    public PlayerControl BlockTarget { get; set; }
    public CustomButton BlockButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Escort : FactionColor;
    public override LayerEnum Type => LayerEnum.Escort;
    public override Func<string> StartText => () => "Roleblock Players From Harming The <#8CFFFFFF>Crew</color>";
    public override Func<string> Description => () => "- You can seduce players\n- Seduction blocks your target from being able to use their abilities for a short while\n- You are immune " +
        "to blocks\n- If you attempt to block a <#336EFFFF>Serial Killer</color>, they will be forced to kill you";
    public override bool RoleBlockImmune => true;

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Support;
        BlockTarget = null;
        BlockButton ??= new(this, "ROLEBLOCK", new SpriteName("EscortRoleblock"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Roleblock, (EffectEndVoid)UnBlock,
            new Cooldown(EscortCd), new Duration(EscortDur), (EndFunc)EndEffect, (EffectStartVoid)BlockStart);
    }

    public void UnBlock()
    {
        if (BlockTarget.AmOwner)
            BlockExposed = false;

        BlockTarget = null;
    }

    public void BlockStart()
    {
        if (BlockTarget.AmOwner)
            CustomStatsManager.IncrementStat(CustomStatsManager.StatsRoleblocked);
    }

    public void Roleblock(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            BlockTarget = target;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, BlockButton, BlockTarget);
            BlockButton.Begin();
        }
        else
            BlockButton.StartCooldown(cooldown);
    }

    public override void ReadRPC(MessageReader reader) => BlockTarget = reader.ReadPlayer();

    public bool EndEffect() => Dead || (BlockTarget && BlockTarget.HasDied());
}