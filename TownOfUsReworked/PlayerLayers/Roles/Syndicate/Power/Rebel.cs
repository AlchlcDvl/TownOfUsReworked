namespace TownOfUsReworked.PlayerLayers.Roles;

public class Rebel : Syndicate
{
    public bool HasDeclared { get; set; }
    public CustomButton SidekickButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomSynColors ? CustomColorManager.Rebel : CustomColorManager.Syndicate;
    public override string Name => "Rebel";
    public override LayerEnum Type => LayerEnum.Rebel;
    public override Func<string> StartText => () => "Promote Your Fellow <color=#008000FF>Syndicate</color> To Do Better";
    public override Func<string> Description => () => "- You can promote a fellow <color=#008000FF>Syndicate</color> into becoming your successor\n- Promoting a <color=#008000FF>" +
        "Syndicate</color> turns them into a <color=#979C9FFF>Sidekick</color>\n- If you die, the <color=#979C9FFF>Sidekick</color> become the new <color=#FFFCCEFF>Rebel</color>\n" +
        $"and inherits better abilities of their former role\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicatePower;
        SidekickButton = CreateButton(this, new SpriteName("Sidekick"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Sidekick, (PlayerBodyExclusion)Exception1, "SIDEKICK",
            (UsableFunc)Usable);
    }

    public void Sidekick(PlayerControl target)
    {
        HasDeclared = true;
        var formerRole = target.GetLayer<Syndicate>();
        new Sidekick()
        {
            FormerRole = formerRole,
            Rebel = this
        }.Start<Role>(target).RoleUpdate(formerRole);
    }

    public void Sidekick()
    {
        if (Interact(Player, SidekickButton.TargetPlayer) != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, SidekickButton.TargetPlayer);
            Sidekick(SidekickButton.TargetPlayer);
        }
    }

    public bool Exception1(PlayerControl player) => player.GetRole() is PromotedRebel or Roles.Sidekick or Rebel || !(player.IsBase(Faction.Syndicate) && player.Is(Faction));

    public override void ReadRPC(MessageReader reader) => Sidekick(reader.ReadPlayer());

    public bool Usable() => !HasDeclared;
}