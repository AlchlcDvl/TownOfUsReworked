namespace TownOfUsReworked.PlayerLayers.Roles;

public class Troll : Neutral
{
    public bool Killed { get; set; }
    public DateTime LastInteracted { get; set; }
    public CustomButton InteractButton { get; set; }

    public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Troll : Colors.Neutral;
    public override string Name => "Troll";
    public override LayerEnum Type => LayerEnum.Troll;
    public override Func<string> StartText => () => "Troll Everyone With Your Death";
    public override Func<string> Description => () => "- You can interact with players\n- Your interactions do nothing except spread infection and possibly kill you via touch " +
        "sensitive roles\n- If you are killed, you will also kill your killer";
    public override InspectorResults InspectorResults => InspectorResults.Manipulative;
    public float Timer => ButtonUtils.Timer(Player, LastInteracted, CustomGameOptions.InteractCooldown);

    public Troll(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.NeutralEvil;
        Objectives = () => Killed ? "- You have successfully trolled someone" : "- Get killed";
        InteractButton = new(this, "Placeholder", AbilityTypes.Direct, "ActionSecondary", Interact);
    }

    public void Interact()
    {
        if (Timer != 0f || IsTooFar(Player, InteractButton.TargetPlayer))
            return;

        var interact = Utils.Interact(Player, InteractButton.TargetPlayer);

        if (interact[3] || interact[0])
            LastInteracted = DateTime.UtcNow;
        else if (interact[0])
            LastInteracted = DateTime.UtcNow;
        else if (interact[1])
            LastInteracted.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        InteractButton.Update("INTERACT", Timer, CustomGameOptions.InteractCooldown);
    }
}