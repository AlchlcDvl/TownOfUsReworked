namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Escort : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number EscortCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number EscortDur { get; set; } = new(10);

    public PlayerControl BlockTarget { get; set; }
    public CustomButton BlockButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Escort: FactionColor;
    public override string Name => "Escort";
    public override LayerEnum Type => LayerEnum.Escort;
    public override Func<string> StartText => () => "Roleblock Players From Harming The <#8CFFFFFF>Crew</color>";
    public override Func<string> Description => () => "- You can seduce players\n- Seduction blocks your target from being able to use their abilities for a short while\n- You are immune " +
        "to blocks\n- If you attempt to block a <#336EFFFF>Serial Killer</color>, they will be forced to kill you";
    public override bool RoleBlockImmune => true;

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.CrewSupport;
        BlockTarget = null;
        BlockButton ??= new(this, "ROLEBLOCK", new SpriteName("EscortRoleblock"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClickPlayer)Roleblock, (EffectVoid)Block,
            (EffectEndVoid)UnBlock,  new Cooldown(EscortCd), new Duration(EscortDur), (EndFunc)EndEffect);
    }

    public void UnBlock()
    {
        BlockTarget.GetLayers().ForEach(x => x.IsBlocked = false);
        BlockTarget.GetButtons().ForEach(x => x.BlockExposed = false);

        if (BlockTarget.AmOwner)
            Blocked.BlockExposed = false;

        BlockTarget = null;
    }

    public void Block() => BlockTarget.GetLayers().ForEach(x => x.IsBlocked = !BlockTarget.GetRole().RoleBlockImmune);

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