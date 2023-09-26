namespace TownOfUsReworked.PlayerLayers.Roles;

public class Rebel : Syndicate
{
    public bool HasDeclared { get; set; }
    public CustomButton SidekickButton { get; set; }

    public override Color Color => ClientGameOptions.CustomSynColors ? Colors.Rebel : Colors.Syndicate;
    public override string Name => "Rebel";
    public override LayerEnum Type => LayerEnum.Rebel;
    public override Func<string> StartText => () => "Promote Your Fellow <color=#008000FF>Syndicate</color> To Do Better";
    public override Func<string> Description => () => "- You can promote a fellow <color=#008000FF>Syndicate</color> into becoming your successor\n- Promoting a <color=#008000FF>" +
        "Syndicate</color> turns them into a <color=#979C9FFF>Sidekick</color>\n- If you die, the <color=#979C9FFF>Sidekick</color> become the new <color=#FFFCCEFF>Rebel</color>\n" +
        $"and inherits better abilities of their former role\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.LeadsTheGroup;

    public Rebel(PlayerControl player) : base(player)
    {
        Alignment = Alignment.SyndicateSupport;
        SidekickButton = new(this, "Sidekick", AbilityTypes.Target, "Secondary", Sidekick);
    }

    public void Sidekick(PlayerControl target)
    {
        HasDeclared = true;
        var formerRole = GetRole(target);

        var sidekick = new Sidekick(target)
        {
            FormerRole = formerRole,
            Rebel = this
        };

        sidekick.RoleUpdate(formerRole);
    }

    public void Sidekick()
    {
        var interact = Interact(Player, SidekickButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, SidekickButton.TargetPlayer);
            Sidekick(SidekickButton.TargetPlayer);
        }
    }

    public bool Exception1(PlayerControl player) => !player.Is(Faction) || (!(player.GetRole() is LayerEnum.PromotedRebel or LayerEnum.Sidekick or LayerEnum.Rebel) && player.Is(Faction));

    public override void ReadRPC(MessageReader reader) => Sidekick(reader.ReadPlayer());

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        SidekickButton.Update2("SIDEKICK", !HasDeclared);
    }
}