namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Rebel : Syndicate
{
    [NumberOption(MultiMenu.LayerSubOptions, 0.25f, 0.9f, 0.05f, Format.Multiplier)]
    public static Number RebPromotionCdDecrease { get; set; } = new(0.75f);

    public bool HasDeclared { get; set; }
    public CustomButton SidekickButton { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Rebel : CustomColorManager.Syndicate;
    public override string Name => "Rebel";
    public override LayerEnum Type => LayerEnum.Rebel;
    public override Func<string> StartText => () => "Promote Your Fellow <color=#008000FF>Syndicate</color> To Do Better";
    public override Func<string> Description => () => "- You can promote a fellow <color=#008000FF>Syndicate</color> into becoming your successor\n- Promoting a <color=#008000FF>" +
        "Syndicate</color> turns them into a <color=#979C9FFF>Sidekick</color>\n- If you die, the <color=#979C9FFF>Sidekick</color> become the new <color=#FFFCCEFF>Rebel</color>\n" +
        $"and inherits better abilities of their former role\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.SyndicatePower;
        SidekickButton ??= new(this, new SpriteName("Sidekick"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Sidekick, (PlayerBodyExclusion)Exception1, "SIDEKICK", (UsableFunc)Usable);
    }

    public void Sidekick(PlayerControl target)
    {
        HasDeclared = true;
        var formerRole = target.GetLayer<Syndicate>();
        new Sidekick()
        {
            FormerRole = formerRole,
            Rebel = this
        }.RoleUpdate(formerRole, target);
    }

    public void Sidekick()
    {
        if (Interact(Player, SidekickButton.GetTarget<PlayerControl>()) != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, SidekickButton.GetTarget<PlayerControl>());
            Sidekick(SidekickButton.GetTarget<PlayerControl>());
        }
    }

    public bool Exception1(PlayerControl player) => player.GetRole() is PromotedRebel or Roles.Sidekick or Rebel || !(player.IsBase(Faction.Syndicate) && player.Is(Faction));

    public override void ReadRPC(MessageReader reader) => Sidekick(reader.ReadPlayer());

    public bool Usable() => !HasDeclared;
}