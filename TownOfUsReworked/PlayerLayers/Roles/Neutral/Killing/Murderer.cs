namespace TownOfUsReworked.PlayerLayers.Roles;

public class Murderer : Neutral
{
    public DateTime LastKilled { get; set; }
    public CustomButton MurderButton { get; set; }

    public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Murderer : Colors.Neutral;
    public override string Name => "Murderer";
    public override LayerEnum Type => LayerEnum.Murderer;
    public override Func<string> StartText => () => "I Got Murder On My Mind";
    public override Func<string> Description => () => "- You can kill";
    public override InspectorResults InspectorResults => InspectorResults.IsBasic;
    public float Timer => ButtonUtils.Timer(Player, LastKilled, CustomGameOptions.MurdKCD);

    public Murderer(PlayerControl player) : base(player)
    {
        Objectives = () => "- Murder anyone who can oppose you";
        RoleAlignment = RoleAlignment.NeutralKill;
        MurderButton = new(this, "Murder", AbilityTypes.Direct, "ActionSecondary", Murder, Exception);
    }

    public void Murder()
    {
        if (IsTooFar(Player, MurderButton.TargetPlayer) || Timer != 0f)
            return;

        var interact = Interact(Player, MurderButton.TargetPlayer, true);

        if (interact[3] || interact[0])
            LastKilled = DateTime.UtcNow;
        else if (interact[1])
            LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (interact[2])
            LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
        || Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        MurderButton.Update("MURDER", Timer, CustomGameOptions.MurdKCD);
    }
}