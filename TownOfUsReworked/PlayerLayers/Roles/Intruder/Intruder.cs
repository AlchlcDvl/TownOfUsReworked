namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Intruder : Role
{
    public CustomButton KillButton { get; set; }
    public string CommonAbilities => "<#FF1919FF>- You can kill players</color>" + (IntruderSettings.IntrudersCanSabotage || (Dead && IntruderSettings.GhostsCanSabotage) ? ("\n- You can " +
        "call sabotages to distract the <#8CFFFFFF>Crew</color>") : "");

    public override UColor Color => CustomColorManager.Intruder;
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        base.Init();
        Faction = Faction.Intruder;
        Objectives = () => IntrudersWinCon;
        KillButton ??= new(this, new SpriteName("IntruderKill"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Kill, new Cooldown(IntruderSettings.IntKillCd), "KILL",
            (PlayerBodyExclusion)Exception, FactionColor);
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();
        team.AddRange(AllPlayers().Where(x => x != Player && x.Is(Faction)));
        return team;
    }

    public virtual void Kill(PlayerControl target) => KillButton.StartCooldown(Interact(Player, target, true));

    public bool Exception(PlayerControl player) => (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||
        Player.IsLinkedTo(player);
}