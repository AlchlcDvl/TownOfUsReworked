namespace TownOfUsReworked.PlayerLayers.Roles;

public class Godfather : Intruder
{
    public bool HasDeclared { get; set; }
    public CustomButton DeclareButton { get; set; }

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Godfather : Colors.Intruder;
    public override string Name => "Godfather";
    public override LayerEnum Type => LayerEnum.Godfather;
    public override Func<string> StartText => () => "Promote Your Fellow <color=#FF0000FF>Intruder</color> To Do Better";
    public override Func<string> Description => () => "- You can promote a fellow <color=#FF0000FF>Intruder</color> into becoming your successor\n- Promoting an <color=#FF0000FF>" +
        "Intruder</color> turns them into a <color=#6400FFFF>Mafioso</color>\n- If you die, the <color=#6400FFFF>Mafioso</color> will become the new <color=#404C08FF>Godfather</color>"
        + $"\nand inherits better abilities of their former role\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.LeadsTheGroup;

    public Godfather(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderSupport;
        DeclareButton = new(this, "Promote", AbilityTypes.Direct, "Secondary", Declare);
    }

    public static void Declare(Godfather gf, PlayerControl target)
    {
        gf.HasDeclared = true;
        var formerRole = GetRole(target);

        var mafioso = new Mafioso(target)
        {
            FormerRole = formerRole,
            Godfather = gf
        };

        mafioso.RoleUpdate(formerRole);

        if (target == CustomPlayer.Local)
            Flash(Colors.Mafioso);

        if (CustomPlayer.Local.Is(LayerEnum.Seer))
            Flash(Colors.Seer);
    }

    public void Declare()
    {
        if (IsTooFar(Player, DeclareButton.TargetPlayer) || HasDeclared)
            return;

        var interact = Interact(Player, DeclareButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.Declare, this, DeclareButton.TargetPlayer);
            Declare(this, DeclareButton.TargetPlayer);
        }
    }

    public bool Exception1(PlayerControl player) => !player.Is(Faction) || (!(player.GetRole() is LayerEnum.PromotedGodfather or LayerEnum.Mafioso or LayerEnum.Godfather) &&
        player.Is(Faction));

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        DeclareButton.Update("PROMOTE", true, !HasDeclared);
    }
}