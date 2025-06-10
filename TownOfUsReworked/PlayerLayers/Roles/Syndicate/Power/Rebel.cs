namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Rebel)]
public sealed class Rebel : Syndicate
{
    [NumberOption(0.25f, 0.9f, 0.05f, Format.Multiplier)]
    public static Number RebPromotionCdDecrease = 0.75f;

    private bool HasDeclared { get; set; }
    private CustomButton SidekickButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Rebel;
    public override LayerEnum Type => LayerEnum.Rebel;
    public override Func<string> StartText { get; } = () => "Promote Your Fellow <#008000FF>Syndicate</color> To Do Better";
    public override Func<string> Description => () => "- You can promote a fellow <#008000FF>Syndicate</color> into becoming your successor\n- Promoting a <#008000FF>" +
        "Syndicate</color> turns them into a <#979C9FFF>Sidekick</color>\n- If you die, the <#979C9FFF>Sidekick</color> become the new <#FFFCCEFF>Rebel</color>\n" +
        $"and inherits better abilities of their former role\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Power;
        SidekickButton ??= new(this, new SpriteName("Sidekick"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Sidekick, (PlayerBodyExclusion)Exception1, "SIDEKICK",
            (UsableFunc)Usable);
    }

    private void Sidekick(PlayerControl target)
    {
        var allow = true;

        if (Local)
        {
            allow = Interact(Player, target) != CooldownType.Fail;

            if (allow)
                CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, target);
        }

        if (!allow)
            return;

        HasDeclared = true;
        var role = target.GetLayer<Syndicate>();
        role.IsUnderling = true;
        role.IsPromoted = false;
        role.Promoter = this;
        role.Name = TranslationManager.Translate("Layer.Sidekick");
        role.Handler.History.Add((role.Type, role.Faction));
    }

    private bool Exception1(PlayerControl player) => !player.Is<Syndicate>(out var syn) || syn.IsPromoted || syn.IsUnderling || !player.Is(Faction);

    public override void ReadRPC(RpcReader reader) => Sidekick(reader.ReadPlayer());

    private bool Usable() => !HasDeclared;
}