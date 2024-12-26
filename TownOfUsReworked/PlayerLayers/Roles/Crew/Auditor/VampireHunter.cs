namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class VampireHunter : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number StakeCd { get; set; } = new(25);

    public static bool VampsDead => !AllPlayers().Any(x => !x.HasDied() && x.Is(SubFaction.Undead));
    private CustomButton StakeButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.VampireHunter : FactionColor;
    public override string Name => "Vampire Hunter";
    public override LayerEnum Type => LayerEnum.VampireHunter;
    public override Func<string> StartText => () => "Stake The <#7B8968FF>Undead</color>";
    public override Func<string> Description => () => "- You can stake players to see if they have been turned\n- When you stake a turned person, or an <#7B8968FF>Undead</color> " +
        "tries to interact with you, you will kill them\n- When all <#7B8968FF>Undead</color> players die, you will become a <#FFFF00FF>Vigilante</color>";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.CrewAudit;
        StakeButton ??= new(this, "STAKE", new SpriteName("Stake"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Stake, new Cooldown(StakeCd));
    }

    public void TurnVigilante()
    {
        var vigi = new Vigilante();
        vigi.RoleUpdate(this);
        vigi.ShootButton.MaxUses = vigi.ShootButton.Uses = TasksDone ? 2 : 1;
    }

    public override void UpdatePlayer()
    {
        if (VampsDead && !Dead)
            TurnVigilante();
    }

    public override void OnKill(PlayerControl victim)
    {
        if (Local && victim.Is(SubFaction.Undead))
            Flash(CustomColorManager.Undead);
    }

    private void Stake(PlayerControl target) => StakeButton.StartCooldown(Interact(Player, target, ShouldKill(target)));

    private bool ShouldKill(PlayerControl player) => (player.Is(SubFaction.Undead) && SubFaction == SubFaction.None) || player.IsFramed() || (!player.Is(SubFaction) && SubFaction !=
        SubFaction.None);
}