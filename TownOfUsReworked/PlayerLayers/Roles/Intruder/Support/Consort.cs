namespace TownOfUsReworked.PlayerLayers.Roles;

public class Consort : Intruder
{
    public CustomButton BlockButton { get; set; }
    public PlayerControl BlockTarget { get; set; }
    public CustomMenu BlockMenu { get; set; }

    public override UColor Color => ClientGameOptions.CustomIntColors ? CustomColorManager.Consort : CustomColorManager.Intruder;
    public override string Name => "Consort";
    public override LayerEnum Type => LayerEnum.Consort;
    public override Func<string> StartText => () => "Roleblock The <color=#8CFFFFFF>Crew</color> From Progressing";
    public override Func<string> Description => () => "- You can seduce players\n- Seduction blocks your target from being able to use their abilities for a short while\n- You are " +
        $"immune to blocks\n- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderSupport;
        RoleBlockImmune = true;
        BlockMenu = new(Player, Click, Exception1);
        BlockTarget = null;
        BlockButton = CreateButton(this, new SpriteName("ConsortRoleblock"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)Roleblock, new Cooldown(CustomGameOptions.ConsortCd),
            new Duration(CustomGameOptions.ConsortDur), (EffectVoid)Block, (EffectEndVoid)UnBlock, (LabelFunc)Label);
    }

    public void UnBlock()
    {
        BlockTarget.GetLayers().ForEach(x => x.IsBlocked = false);
        BlockTarget = null;
    }

    public void Block() => BlockTarget.GetLayers().ForEach(x => x.IsBlocked = !BlockTarget.GetRole().RoleBlockImmune);

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
        if (BlockTarget == null)
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

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (BlockTarget && !BlockButton.EffectActive)
                BlockTarget = null;

            LogMessage("Removed a target");
        }
    }

    public bool EndEffect() => (BlockTarget && BlockTarget.HasDied()) || Dead;

    public override void ReadRPC(MessageReader reader) => BlockTarget = reader.ReadPlayer();
}