namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Consort)]
public sealed class Consort : ISupport, IBlocker
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ConsortCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number ConsortDur = 10;

    private CustomButton BlockButton;
    public PlayerControl BlockTarget { get; private set; }
    private CustomPlayerMenu BlockMenu;

    protected override UColor MainColor => CustomColorManager.Consort;
    public override LayerEnum Type => LayerEnum.Consort;
    public override string StartText => "Roleblock The <#8CFFFFFF>Crew</color> From Progressing";
    public override string Description => "- You can seduce players\n- Seduction blocks your target from being able to use their abilities for a short while\n- You are " +
        $"immune to blocks\n- If you block a <#336EFFFF>Serial Killer</color>, they will be forced to kill you\n{CommonAbilities}";
    public override bool RoleBlockImmune => true;

    public override void Init()
    {
        base.Init();
        BlockMenu = new(Player, Click, Color, Exception1);
        BlockTarget = null;
        BlockButton ??= new(this, new SpriteName("ConsortRoleblock"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Roleblock, new Cooldown(ConsortCd), (LabelFunc)Label,
            new Duration(ConsortDur), (EffectEndVoid)UnBlock, (EffectStartVoid)BlockStart, (EndFunc)EndEffect);
    }

    public override void Reset(bool meeting, bool start) => BlockTarget = null;

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

    private void Click(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            BlockTarget = player;
        else
            BlockButton.StartCooldown(cooldown);
    }

    private void Roleblock()
    {
        if (!BlockTarget)
            BlockMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, BlockButton, BlockTarget);
            BlockButton.Begin();
        }
    }

    private bool Exception1(PlayerControl player) => player == BlockTarget || player == Player || Player.IsBuddyWith(player, Faction);

    private string Label() => BlockTarget ? "ROLEBLOCK" : "SET TARGET";

    public override void UpdateHud(HudManager __instance)
    {
        if (!KeyboardJoystick.player.GetButtonDown("Delete"))
            return;

        if (BlockTarget && !BlockButton.EffectActive)
            BlockTarget = null;

        Message("Removed a target");
    }

    private bool EndEffect() => (BlockTarget && BlockTarget.HasDied()) || Dead;

    public override void ReadRPC(RpcReader reader)
    {
        BlockTarget = reader.ReadPlayer();

        if (BlockTarget.AmOwner)
            CustomStatsManager.IncrementStat(CustomStatsManager.StatsRoleblocked);
    }
}