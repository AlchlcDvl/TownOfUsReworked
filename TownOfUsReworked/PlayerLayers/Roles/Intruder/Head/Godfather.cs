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
        Alignment = Alignment.IntruderHead;
        DeclareButton = new(this, "Promote", AbilityTypes.Target, "Secondary", Declare);
    }

    public void Declare(PlayerControl target)
    {
        HasDeclared = true;
        var formerRole = GetRole(target);

        var mafioso = new Mafioso(target)
        {
            FormerRole = formerRole,
            Godfather = this
        };

        mafioso.RoleUpdate(formerRole);
    }

    public void Declare()
    {
        var interact = Interact(Player, DeclareButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, DeclareButton.TargetPlayer);
            Declare(DeclareButton.TargetPlayer);
        }
    }

    public bool Exception1(PlayerControl player) => !player.Is(Faction) || (!(player.GetRole() is LayerEnum.PromotedGodfather or LayerEnum.Mafioso or LayerEnum.Godfather) &&
        player.Is(Faction.Intruder));

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        DeclareButton.Update2("PROMOTE", !HasDeclared);
    }

    public override void ReadRPC(MessageReader reader) => Declare(reader.ReadPlayer());
}