namespace TownOfUsReworked.PlayerLayers.Roles;

public class Sidekick : Syndicate
{
    public Role FormerRole { get; set; }
    public Rebel Rebel { get; set; }
    public bool CanPromote => (Rebel.IsDead || Rebel.Disconnected) && !IsDead;

    public override Color Color => ClientGameOptions.CustomSynColors ? Colors.Sidekick : Colors.Syndicate;
    public override string Name => "Sidekick";
    public override LayerEnum Type => LayerEnum.Sidekick;
    public override Func<string> StartText => () => "Succeed The <color=#FFFCCEFF>Rebel</color>";
    public override Func<string> Description => () => "- When the <color=#FFFCCEFF>Rebel</color> dies, you will become the new <color=#FFFCCEFF>Rebel</color> with boosted abilities of your" +
        $" former role\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.IsCold;

    public Sidekick(PlayerControl player) : base(player) => Alignment = Alignment.SyndicateUtil;

    public void TurnRebel()
    {
        var newRole = new PromotedRebel(Player)
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
            CallRpc(CustomRPC.Change, TurnRPC.TurnRebel, this);
            TurnRebel();
        }
    }
}