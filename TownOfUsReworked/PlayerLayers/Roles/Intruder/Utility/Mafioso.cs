namespace TownOfUsReworked.PlayerLayers.Roles;

public class Mafioso : Intruder
{
    public Role FormerRole { get; set; }
    public Godfather Godfather { get; set; }
    public bool CanPromote => (Godfather.IsDead || Godfather.Disconnected) && !IsDead;

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Mafioso : Colors.Intruder;
    public override string Name => "Mafioso";
    public override LayerEnum Type => LayerEnum.Mafioso;
    public override Func<string> StartText => () => "Succeed The <color=#404C08FF>Godfather</color>";
    public override Func<string> Description => () => "- When the <color=#404C08FF>Godfather</color> dies, you will become the new <color=#404C08FF>Godfather</color> with boosted abilities" +
        $" of your former role\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.IsCold;

    public Mafioso(PlayerControl player) : base(player) => Alignment = Alignment.IntruderUtil;

    public void TurnGodfather()
    {
        var newRole = new PromotedGodfather(Player)
        {
            FormerRole = FormerRole,
            RoleBlockImmune = FormerRole.RoleBlockImmune,
            Alignment = FormerRole.Alignment
        };

        newRole.RoleUpdate(this);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (CanPromote)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnGodfather, this);
            TurnGodfather();
        }
    }
}