namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Sidekick : Syndicate
{
    public Syndicate FormerRole { get; init; }
    public Rebel Rebel { get; init; }
    private bool CanPromote => (Rebel.Dead || Rebel.Disconnected) && !Dead;

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Sidekick : FactionColor;
    public override LayerEnum Type => LayerEnum.Sidekick;
    public override Func<string> StartText => () => "Succeed The <#FFFCCEFF>Rebel</color>";
    public override Func<string> Description => () => "- When the <#FFFCCEFF>Rebel</color> dies, you will become the new <#FFFCCEFF>Rebel</color> with boosted abilities of your" +
        $" former role\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Utility;
    }

    private void TurnRebel()
    {
        FormerRole.IsPromoted = true;
        var reb = new PromotedRebel() { FormerRole = FormerRole is PromotedRebel preb ? preb.FormerRole : FormerRole };
        reb.RoleUpdate(this);
        reb.OnRoleSelected();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (CanPromote)
        {
            CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this);
            TurnRebel();
        }
    }
}