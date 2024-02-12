namespace TownOfUsReworked.PlayerLayers.Roles;

public class Mafioso : Intruder
{
    public Role FormerRole { get; set; }
    public Godfather Godfather { get; set; }
    public bool CanPromote => (Godfather.IsDead || Godfather.Disconnected) && !IsDead;

    public override UColor Color => ClientGameOptions.CustomIntColors ? CustomColorManager.Mafioso : CustomColorManager.Intruder;
    public override string Name => "Mafioso";
    public override LayerEnum Type => LayerEnum.Mafioso;
    public override Func<string> StartText => () => "Succeed The <color=#404C08FF>Godfather</color>";
    public override Func<string> Description => () => "- When the <color=#404C08FF>Godfather</color> dies, you will become the new <color=#404C08FF>Godfather</color> with boosted abilities" +
        $" of your former role\n{CommonAbilities}";

    public Mafioso() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.IntruderUtil;
        return this;
    }

    public void TurnGodfather()
    {
        new PromotedGodfather()
        {
            FormerRole = FormerRole,
            RoleBlockImmune = FormerRole.RoleBlockImmune
        }.Start<Role>(Player).RoleUpdate(this);
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