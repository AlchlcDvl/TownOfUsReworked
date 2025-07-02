namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Seer)]
public sealed class Seer : Investigative
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number SeerCd = 25;

    private static bool ChangedDead => !GetLayers<Role>().Any(x => !x.Alive && (x.Handler.History.Any() || x is Amnesiac or Thief or Actor or Godfather or Rebel or ITargeter ||
        x.Handler.CurrentDisposition is FactionChanger));

    private CustomButton SeerButton;

    protected override UColor MainColor => CustomColorManager.Seer;
    public override Layer Type => Layer.Seer;
    public override string StartText => "You Can See People's Histories";
    public override string Description => "- You can investigate players to see if their roles have changed\n- If all relevant players whose roles changed have died, you will become a " +
        "<#FFCC80FF>Sheriff</color>";

    public override void Init() => SeerButton ??= new(this, "ENVISION", new SpriteName("Seer"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)See, new Cooldown(SeerCd));

    private void TurnSheriff() => new Sheriff().RoleUpdate(this);

    private void See(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash(LayerHandler.Handlers[target.PlayerId].History.Any() || target.IsFramed() ? UColor.red : UColor.green);

        SeerButton.StartCooldown(cooldown);
    }

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();

        if (ChangedDead && !Dead)
            TurnSheriff();
    }
}