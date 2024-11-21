namespace TownOfUsReworked.PlayerLayers.Roles;

public class Mafioso : Intruder
{
    public Role FormerRole { get; set; }
    public Godfather Godfather { get; set; }
    public bool CanPromote => (Godfather.Dead || Godfather.Disconnected) && !Dead;

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Mafioso : CustomColorManager.Intruder;
    public override string Name => "Mafioso";
    public override LayerEnum Type => LayerEnum.Mafioso;
    public override Func<string> StartText => () => "Succeed The <color=#404C08FF>Godfather</color>";
    public override Func<string> Description => () => "- When the <color=#404C08FF>Godfather</color> dies, you will become the new <color=#404C08FF>Godfather</color> with boosted abilities" +
        $" of your former role\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.IntruderUtil;
    }

    public void TurnGodfather()
    {
        var gf = new PromotedGodfather() { FormerRole = FormerRole is PromotedGodfather pgf ? pgf.FormerRole : FormerRole };
        gf.RoleUpdate(this, Player);
        gf.OnRoleSelected();
    }

    public override void UpdatePlayer()
    {
        if (CanPromote)
            TurnGodfather();
    }
}