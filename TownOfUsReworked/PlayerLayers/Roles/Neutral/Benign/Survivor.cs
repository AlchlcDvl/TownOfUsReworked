namespace TownOfUsReworked.PlayerLayers.Roles;

public class Survivor : Neutral
{
    public bool Alive => !Disconnected && !IsDead;
    public CustomButton VestButton { get; set; }

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Survivor : Colors.Neutral;
    public override string Name => "Survivor";
    public override LayerEnum Type => LayerEnum.Survivor;
    public override Func<string> StartText => () => "Do Whatever It Takes To Live";
    public override Func<string> Description => () => "- You can put on a vest, which makes you unkillable for a short duration of time";

    public Survivor(PlayerControl player) : base(player)
    {
        Alignment = Alignment.NeutralBen;
        Objectives = () => "- Live to the end of the game";
        VestButton = new(this, "Vest", AbilityTypes.Targetless, "ActionSecondary", HitVest, CustomGameOptions.VestCd, CustomGameOptions.VestDur, CustomGameOptions.MaxVests);
    }

    public void HitVest()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, VestButton);
        VestButton.Begin();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        VestButton.Update2("VEST");
    }
}