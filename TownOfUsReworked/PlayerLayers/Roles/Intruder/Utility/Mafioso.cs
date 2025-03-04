namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Mafioso : Intruder
{
    public Role FormerRole { get; init; }
    public Godfather Godfather { get; init; }
    public bool CanPromote => (Godfather.Dead || Godfather.Disconnected) && !Dead;

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Mafioso : FactionColor;
    public override LayerEnum Type => LayerEnum.Mafioso;
    public override Func<string> StartText => () => "Succeed The <#404C08FF>Godfather</color>";
    public override Func<string> Description => () => "- When the <#404C08FF>Godfather</color> dies, you will become the new <#404C08FF>Godfather</color> with boosted abilities" +
        $" of your former role\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Utility;
    }

    private void TurnGodfather()
    {
        var gf = new PromotedGodfather() { FormerRole = FormerRole is PromotedGodfather pgf ? pgf.FormerRole : FormerRole };
        gf.RoleUpdate(this);
        gf.OnRoleSelected();
    }

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();

        if (CanPromote)
            TurnGodfather();
    }
}