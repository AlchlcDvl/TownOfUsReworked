namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Godfather : Intruder
{
    [NumberOption(0.25f, 0.9f, 0.05f, Format.Multiplier)]
    public static Number GFPromotionCdDecrease = 0.75f;

    public bool HasDeclared { get; set; }
    public CustomButton DeclareButton { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Godfather : FactionColor;
    public override LayerEnum Type => LayerEnum.Godfather;
    public override Func<string> StartText => () => "Promote Your Fellow <#FF1919FF>Intruder</color> To Do Better";
    public override Func<string> Description => () => "- You can promote a fellow <#FF1919FF>Intruder</color> into becoming your successor\n- Promoting an <#FF1919FF>" +
        "Intruder</color> turns them into a <#6400FFFF>Mafioso</color>\n- If you die, the <#6400FFFF>Mafioso</color> will become the new <#404C08FF>Godfather</color>"
        + $"\nand inherits better abilities of their former role\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Head;
        DeclareButton ??= new(this, new SpriteName("Promote"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Declare, (PlayerBodyExclusion)Exception1, "PROMOTE", (UsableFunc)Usable);
    }

    public void Declare(PlayerControl target)
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
        var formerRole = target.GetRole();
        new Mafioso()
        {
            FormerRole = formerRole,
            Godfather = this
        }.RoleUpdate(formerRole, target);
    }

    public bool Exception1(PlayerControl player) => player.GetRole() is PromotedGodfather or Mafioso or Godfather || !(player.Is<Intruder>() && player.Is(Faction));

    public bool Usable() => !HasDeclared;

    public override void ReadRPC(MessageReader reader) => Declare(reader.ReadPlayer());
}