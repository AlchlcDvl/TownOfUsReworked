namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Seer)]
public sealed class Seer : Crew
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number SeerCd = 25;

    private static bool ChangedDead => !GetLayers<Role>().Any(x => !x.Alive && (x.Handler.History.Any() || x is Amnesiac or Thief or Actor or Godfather or Guesser or Rebel or Mystic or Executioner
        or GuardianAngel or BountyHunter || x.Handler.CurrentDisposition is Traitor or Fanatic));
    private CustomButton SeerButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Seer;
    public override LayerEnum Type => LayerEnum.Seer;
    public override Func<string> StartText { get; } = () => "You Can See People's Histories";
    public override Func<string> Description => () => "- You can investigate players to see if their roles have changed\n- If all players whose roles changed have died, you will become a " +
        "<#FFCC80FF>Sheriff</color>";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Investigative;
        SeerButton ??= new(this, "ENVISION", new SpriteName("Seer"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)See, new Cooldown(SeerCd));
    }

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