namespace TownOfUsReworked.PlayerLayers.Roles;

public class Escort : Crew
{
    public PlayerControl BlockTarget { get; set; }
    public CustomButton BlockButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Escort : CustomColorManager.Crew;
    public override string Name => "Escort";
    public override LayerEnum Type => LayerEnum.Escort;
    public override Func<string> StartText => () => "Roleblock Players From Harming The <color=#8CFFFFFF>Crew</color>";
    public override Func<string> Description => () => "- You can seduce players\n- Seduction blocks your target from being able to use their abilities for a short while\n- You are immune " +
        "to blocks\n- If you attempt to block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you";

    public Escort(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewSupport;
        RoleBlockImmune = true;
        BlockTarget = null;
        BlockButton = new(this, "EscortRoleblock", AbilityTypes.Alive, "ActionSecondary", Roleblock, CustomGameOptions.EscortCd, CustomGameOptions.EscortDur, (CustomButton.EffectVoid)Block,
            UnBlock);
    }

    public void UnBlock()
    {
        GetLayers(BlockTarget).ForEach(x => x.IsBlocked = false);
        BlockTarget = null;
    }

    public void Block() => GetLayers(BlockTarget).ForEach(x => x.IsBlocked = !GetRole(BlockTarget).RoleBlockImmune);

    public void Roleblock()
    {
        var cooldown = Interact(Player, BlockButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            BlockTarget = BlockButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, BlockButton, BlockTarget);
            BlockButton.Begin();
        }
        else
            BlockButton.StartCooldown(cooldown);
    }

    public override void ReadRPC(MessageReader reader) => BlockTarget = reader.ReadPlayer();

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        BlockButton.Update2("ROLEBLOCK");
    }

    public override void TryEndEffect() => BlockButton.Update3(IsDead || (BlockTarget != null && BlockTarget.HasDied()));
}