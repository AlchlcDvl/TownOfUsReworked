namespace TownOfUsReworked.PlayerLayers.Roles;

public class Medic : Crew
{
    public bool UsedAbility => ShieldedPlayer != null || ExShielded != null;
    public PlayerControl ShieldedPlayer { get; set; }
    public PlayerControl ExShielded { get; set; }
    public CustomButton ShieldButton { get; set; }

    public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Medic : Colors.Crew;
    public override string Name => "Medic";
    public override LayerEnum Type => LayerEnum.Medic;
    public override Func<string> StartText => () => "Shield A Player To Protect Them";
    public override Func<string> Description => () => "- You can shield a player to prevent them from dying to others\n- If your target is attacked, you will be notified of it by " +
        "default\n- Your shield does not save your target from suicides or ejections";
    public override InspectorResults InspectorResults => InspectorResults.PreservesLife;

    public Medic(PlayerControl player) : base(player)
    {
        ShieldedPlayer = null;
        ExShielded = null;
        RoleAlignment = RoleAlignment.CrewProt;
        ShieldButton = new(this, "Shield", AbilityTypes.Direct, "ActionSecondary", Protect, Exception);
    }

    public void Protect()
    {
        if (IsTooFar(Player, ShieldButton.TargetPlayer))
            return;

        var interact = Interact(Player, ShieldButton.TargetPlayer);

        if (interact[3])
        {
            ShieldedPlayer = ShieldButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.Protect, this, ShieldedPlayer);
        }
    }

    public bool Exception(PlayerControl player) => player == ShieldedPlayer || (player.Is(LayerEnum.Mayor) && GetRole<Mayor>(player).Revealed) || (player.Is(LayerEnum.Dictator) &&
        GetRole<Dictator>(player).Revealed);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ShieldButton.Update("SHIELD", !UsedAbility, !UsedAbility);
    }
}