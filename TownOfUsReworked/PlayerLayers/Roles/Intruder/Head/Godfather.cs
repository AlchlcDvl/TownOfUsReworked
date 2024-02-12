namespace TownOfUsReworked.PlayerLayers.Roles;

public class Godfather : Intruder
{
    public bool HasDeclared { get; set; }
    public CustomButton DeclareButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomIntColors ? CustomColorManager.Godfather : CustomColorManager.Intruder;
    public override string Name => "Godfather";
    public override LayerEnum Type => LayerEnum.Godfather;
    public override Func<string> StartText => () => "Promote Your Fellow <color=#FF1919FF>Intruder</color> To Do Better";
    public override Func<string> Description => () => "- You can promote a fellow <color=#FF1919FF>Intruder</color> into becoming your successor\n- Promoting an <color=#FF1919FF>" +
        "Intruder</color> turns them into a <color=#6400FFFF>Mafioso</color>\n- If you die, the <color=#6400FFFF>Mafioso</color> will become the new <color=#404C08FF>Godfather</color>"
        + $"\nand inherits better abilities of their former role\n{CommonAbilities}";

    public Godfather() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.IntruderHead;
        DeclareButton = new(this, "Promote", AbilityTypes.Alive, "Secondary", Declare);
        return this;
    }

    public void Declare(PlayerControl target)
    {
        HasDeclared = true;
        var formerRole = target.GetRole();
        new Mafioso()
        {
            FormerRole = formerRole,
            Godfather = this
        }.Start<Role>(target).RoleUpdate(formerRole);
    }

    public void Declare()
    {
        if (Interact(Player, DeclareButton.TargetPlayer) != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, DeclareButton.TargetPlayer);
            Declare(DeclareButton.TargetPlayer);
        }
    }

    public bool Exception1(PlayerControl player) => !player.Is(Faction) || (!(player.GetRole() is PromotedGodfather or Mafioso or Godfather) && player.IsBase(Faction.Intruder));

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        DeclareButton.Update2("PROMOTE", !HasDeclared);
    }

    public override void ReadRPC(MessageReader reader) => Declare(reader.ReadPlayer());
}