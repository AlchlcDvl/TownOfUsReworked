namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Intruder : Role
{
    public CustomButton KillButton { get; set; }
    public string CommonAbilities => "<color=#FF1919FF>- You can kill players" + (IntruderSettings.IntrudersCanSabotage || (Dead && IntruderSettings.GhostsCanSabotage) ? ("\n- You can " +
        "call sabotages to distract the <color=#8CFFFFFF>Crew</color>") : "") + "</color>";

    public override UColor Color => CustomColorManager.Intruder;
    public override Faction BaseFaction => Faction.Intruder;
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        base.Init();
        Faction = Faction.Intruder;
        Objectives = () => IntrudersWinCon;
        KillButton ??= new(this, new SpriteName("IntruderKill"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Kill, new Cooldown(IntruderSettings.IntKillCd), "KILL",
            (PlayerBodyExclusion)Exception, FactionColor);
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();
        team.AddRange(AllPlayers().Where(x => x != Player && x.Is(Faction)));

        if (Player.Is(LayerEnum.Lovers))
            team.Add(Player.GetOtherLover());
        else if (Player.Is(LayerEnum.Rivals))
            team.Add(Player.GetOtherRival());
        else if (Player.Is(LayerEnum.Mafia))
            team.AddRange(AllPlayers().Where(x => x != Player && x.Is(LayerEnum.Mafia)));

        if (IsRecruit)
        {
            var jackal = Player.GetJackal();
            team.Add(jackal.Player);
            team.AddRange(jackal.GetOtherRecruits(Player));
        }

        return team;
    }

    public virtual void Kill() => KillButton.StartCooldown(Interact(Player, KillButton.GetTarget<PlayerControl>(), true));

    public bool Exception(PlayerControl player) => (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||
        Player.IsLinkedTo(player);
}