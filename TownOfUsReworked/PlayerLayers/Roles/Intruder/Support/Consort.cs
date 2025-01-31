namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Consort : Intruder, IBlocker
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number ConsortCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number ConsortDur = 10;

    public CustomButton BlockButton { get; set; }
    public PlayerControl BlockTarget { get; set; }
    public CustomPlayerMenu BlockMenu { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Consort : FactionColor;
    public override LayerEnum Type => LayerEnum.Consort;
    public override Func<string> StartText => () => "Roleblock The <#8CFFFFFF>Crew</color> From Progressing";
    public override Func<string> Description => () => "- You can seduce players\n- Seduction blocks your target from being able to use their abilities for a short while\n- You are " +
        $"immune to blocks\n- If you block a <#336EFFFF>Serial Killer</color>, they will be forced to kill you\n{CommonAbilities}";
    public override bool RoleBlockImmune => true;

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Support;
        BlockMenu = new(Player, Click, Exception1);
        BlockTarget = null;
        BlockButton ??= new(this, new SpriteName("ConsortRoleblock"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Roleblock, new Cooldown(ConsortCd), (LabelFunc)Label,
            new Duration(ConsortDur), (EffectEndVoid)UnBlock, (EffectStartVoid)BlockStart, (EndFunc)EndEffect);
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

    public void Click(PlayerControl player)
    {
        var cooldown = Interact(Player, player);

        if (cooldown != CooldownType.Fail)
            BlockTarget = player;
        else
            BlockButton.StartCooldown(cooldown);
    }

    public void Roleblock()
    {
        if (!BlockTarget)
            BlockMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, BlockButton, BlockTarget);
            BlockButton.Begin();
        }
    }

    public bool Exception1(PlayerControl player) => player == BlockTarget || player == Player || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public string Label() => BlockTarget ? "ROLEBLOCK" : "SET TARGET";

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (KeyboardJoystick.player.GetButtonDown("Delete"))
        {
            if (BlockTarget && !BlockButton.EffectActive)
                BlockTarget = null;

            Message("Removed a target");
        }
    }

    public bool EndEffect() => (BlockTarget && BlockTarget.HasDied()) || Dead;

    public override void ReadRPC(MessageReader reader) => BlockTarget = reader.ReadPlayer();
}