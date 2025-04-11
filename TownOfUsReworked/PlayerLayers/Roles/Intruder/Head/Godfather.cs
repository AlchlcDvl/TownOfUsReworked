namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Godfather)]
public sealed class Godfather : Intruder
{
    [NumberOption(0.25f, 0.9f, 0.05f, Format.Multiplier)]
    public static Number GfPromotionCdDecrease = 0.75f;

    private bool HasDeclared { get; set; }
    private CustomButton DeclareButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Godfather;
    public override LayerEnum Type { get; } = LayerEnum.Godfather;
    public override Func<string> StartText { get; } = () => "Promote Your Fellow <#FF1919FF>Intruder</color> To Do Better";
    public override Func<string> Description => () => "- You can promote a fellow <#FF1919FF>Intruder</color> into becoming your successor\n- Promoting an <#FF1919FF>" +
        "Intruder</color> turns them into a <#6400FFFF>Mafioso</color>\n- If you die, the <#6400FFFF>Mafioso</color> will become the new <#404C08FF>Godfather</color>"
        + $"\nand inherits better abilities of their former role\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Head;
        DeclareButton ??= new(this, new SpriteName("Promote"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Declare, (PlayerBodyExclusion)Exception1, "PROMOTE", (UsableFunc)Usable);
    }

    private void Declare(PlayerControl target)
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
        var role = target.GetLayer<Intruder>();
        role.IsMafioso = true;
        role.IsPromoted = false;
        role.Promoter = this;
        role.Name = TranslationManager.Translate("Layer.Mafioso");
    }

    private bool Exception1(PlayerControl player) => !player.Is<Intruder>(out var intruder) || intruder.IsMafioso || intruder.IsPromoted || !player.Is(Faction);

    private bool Usable() => !HasDeclared;

    public override void ReadRPC(NetData reader) => Declare(reader.ReadPlayer());
}