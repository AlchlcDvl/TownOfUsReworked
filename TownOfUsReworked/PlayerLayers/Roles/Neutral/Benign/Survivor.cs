namespace TownOfUsReworked.PlayerLayers.Roles;

public class Survivor : Neutral
{
    public bool Enabled { get; set; }
    public DateTime LastVested { get; set; }
    public float TimeRemaining { get; set; }
    public int UsesLeft { get; set; }
    public bool ButtonUsable => UsesLeft > 0;
    public bool Vesting => TimeRemaining > 0f;
    public bool Alive => !Disconnected && !IsDead;
    public CustomButton VestButton { get; set; }

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Survivor : Colors.Neutral;
    public override string Name => "Survivor";
    public override LayerEnum Type => LayerEnum.Survivor;
    public override Func<string> StartText => () => "Do Whatever It Takes To Live";
    public override Func<string> Description => () => "- You can put on a vest, which makes you unkillable for a short duration of time";
    public override InspectorResults InspectorResults => InspectorResults.LeadsTheGroup;
    public float Timer => ButtonUtils.Timer(Player, LastVested, CustomGameOptions.VestCd);

    public Survivor(PlayerControl player) : base(player)
    {
        UsesLeft = CustomGameOptions.MaxVests;
        Alignment = Alignment.NeutralBen;
        Objectives = () => "- Live to the end of the game";
        VestButton = new(this, "Vest", AbilityTypes.Effect, "ActionSecondary", HitVest, true);
    }

    public void Vest()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (Meeting)
            TimeRemaining = 0f;
    }

    public void UnVest()
    {
        Enabled = false;
        LastVested = DateTime.UtcNow;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        VestButton.Update("PROTECT", Timer, CustomGameOptions.VestCd, UsesLeft, Vesting, TimeRemaining, CustomGameOptions.VestDur);
    }

    public void HitVest()
    {
        if (!ButtonUsable || Timer != 0f || Vesting)
            return;

        TimeRemaining = CustomGameOptions.VestDur;
        UsesLeft--;
        CallRpc(CustomRPC.Action, ActionsRPC.Vest, this);
    }
}