namespace TownOfUsReworked.PlayerLayers.Roles;

public class Consort : Intruder
{
    public CustomButton BlockButton { get; set; }
    public PlayerControl BlockTarget { get; set; }
    public CustomMenu BlockMenu { get; set; }

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Consort : Colors.Intruder;
    public override string Name => "Consort";
    public override LayerEnum Type => LayerEnum.Consort;
    public override Func<string> StartText => () => "Roleblock The <color=#8CFFFFFF>Crew</color> From Progressing";
    public override Func<string> Description => () => "- You can seduce players\n- Seduction blocks your target from being able to use their abilities for a short while\n- You are " +
        $"immune to blocks\n- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you\n{CommonAbilities}";

    public Consort(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderSupport;
        RoleBlockImmune = true;
        BlockMenu = new(Player, Click, Exception1);
        BlockTarget = null;
        BlockButton = new(this, "ConsortRoleblock", AbilityTypes.Targetless, "Secondary", Roleblock, CustomGameOptions.ConsortCd, CustomGameOptions.ConsortDur, (CustomButton.EffectVoid)Block,
            UnBlock);
    }

    public void UnBlock()
    {
        GetLayers(BlockTarget).ForEach(x => x.IsBlocked = false);
        BlockTarget = null;
    }

    public void Block() => GetLayers(BlockTarget).ForEach(x => x.IsBlocked = !GetRole(BlockTarget).RoleBlockImmune);

    public void Click(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact.AbilityUsed)
            BlockTarget = player;
        else if (interact.Reset)
            BlockButton.StartCooldown();
        else if (interact.Protected)
            BlockButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void Roleblock()
    {
        if (BlockTarget == null)
            BlockMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, BlockButton, BlockTarget);
            BlockButton.Begin();
        }
    }

    public bool Exception1(PlayerControl player) => player == BlockTarget || player == Player || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        BlockButton.Update2(BlockTarget == null ? "SET TARGET" : "ROLEBLOCK");

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (BlockTarget != null && !BlockButton.EffectActive)
                BlockTarget = null;

            LogInfo("Removed a target");
        }
    }

    public override void TryEndEffect() => BlockButton.Update3((BlockTarget != null && BlockTarget.HasDied()) || IsDead);

    public override void ReadRPC(MessageReader reader) => BlockTarget = reader.ReadPlayer();
}