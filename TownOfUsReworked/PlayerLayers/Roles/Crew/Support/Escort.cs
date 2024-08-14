namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Escort : Crew
{
    [NumberOption(MultiMenu2.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float EscortCd { get; set; } = 25f;

    [NumberOption(MultiMenu2.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static float EscortDur { get; set; } = 10f;

    public PlayerControl BlockTarget { get; set; }
    public CustomButton BlockButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Escort : CustomColorManager.Crew;
    public override string Name => "Escort";
    public override LayerEnum Type => LayerEnum.Escort;
    public override Func<string> StartText => () => "Roleblock Players From Harming The <color=#8CFFFFFF>Crew</color>";
    public override Func<string> Description => () => "- You can seduce players\n- Seduction blocks your target from being able to use their abilities for a short while\n- You are immune " +
        "to blocks\n- If you attempt to block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewSupport;
        RoleBlockImmune = true;
        BlockTarget = null;
        BlockButton = CreateButton(this, "ROLEBLOCK", new SpriteName("EscortRoleblock"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Roleblock, (EffectVoid)Block,
            new Cooldown(CustomGameOptions.EscortCd), new Duration(CustomGameOptions.EscortDur), (EffectEndVoid)UnBlock, (EndFunc)EndEffect);
    }

    public void UnBlock()
    {
        BlockTarget.GetLayers().ForEach(x => x.IsBlocked = false);
        BlockTarget = null;
    }

    public void Block() => BlockTarget.GetLayers().ForEach(x => x.IsBlocked = !BlockTarget.GetRole().RoleBlockImmune);

    public void Roleblock()
    {
        var cooldown = Interact(Player, BlockButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            BlockTarget = BlockButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, BlockButton, BlockTarget);
            BlockButton.Begin();
        }
        else
            BlockButton.StartCooldown(cooldown);
    }

    public override void ReadRPC(MessageReader reader) => BlockTarget = reader.ReadPlayer();

    public bool EndEffect() => Dead || (BlockTarget && BlockTarget.HasDied());
}