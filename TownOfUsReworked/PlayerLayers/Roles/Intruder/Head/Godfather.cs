namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Godfather : Intruder
{
    [NumberOption(MultiMenu.LayerSubOptions, 0.25f, 0.9f, 0.05f, Format.Multiplier)]
    public static Number GFPromotionCdDecrease { get; set; } = new(0.75f);

    public bool HasDeclared { get; set; }
    public CustomButton DeclareButton { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Godfather : CustomColorManager.Intruder;
    public override string Name => "Godfather";
    public override LayerEnum Type => LayerEnum.Godfather;
    public override Func<string> StartText => () => "Promote Your Fellow <color=#FF1919FF>Intruder</color> To Do Better";
    public override Func<string> Description => () => "- You can promote a fellow <color=#FF1919FF>Intruder</color> into becoming your successor\n- Promoting an <color=#FF1919FF>" +
        "Intruder</color> turns them into a <color=#6400FFFF>Mafioso</color>\n- If you die, the <color=#6400FFFF>Mafioso</color> will become the new <color=#404C08FF>Godfather</color>"
        + $"\nand inherits better abilities of their former role\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderHead;
        DeclareButton ??= CreateButton(this, "Promote", AbilityType.Alive, KeybindType.Secondary, (OnClick)Declare, (PlayerBodyExclusion)Exception1, "PROMOTE", (UsableFunc)Usable);
    }

    public void Declare(PlayerControl target)
    {
        HasDeclared = true;
        var formerRole = target.GetRole();
        new Mafioso()
        {
            FormerRole = formerRole,
            Godfather = this
        }.RoleUpdate(formerRole, target);
    }

    public void Declare()
    {
        if (Interact(Player, DeclareButton.GetTarget<PlayerControl>()) != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, DeclareButton.GetTarget<PlayerControl>());
            Declare(DeclareButton.GetTarget<PlayerControl>());
        }
    }

    public bool Exception1(PlayerControl player) => player.GetRole() is PromotedGodfather or Mafioso or Godfather || !(player.IsBase(Faction.Intruder) && player.Is(Faction));

    public bool Usable() => !HasDeclared;

    public override void ReadRPC(MessageReader reader) => Declare(reader.ReadPlayer());
}