namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Intruder : Role
{
    public CustomButton KillButton { get; set; }
    public string CommonAbilities => "<color=#FF1919FF>- You can kill players" + (IntruderSettings.IntrudersCanSabotage || (Dead && IntruderSettings.GhostsCanSabotage) ? ("\n- You can " +
        "call sabotages to distract the <color=#8CFFFFFF>Crew</color>") : "") + "</color>";

    public override UColor Color => CustomColorManager.Intruder;
    public override Faction BaseFaction => Faction.Intruder;
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public void BaseStart()
    {
        RoleStart();
        Faction = Faction.Intruder;
        FactionColor = CustomColorManager.Intruder;
        Objectives = () => IntrudersWinCon;
        KillButton = CreateButton(this, new SpriteName("IntruderKill"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)Kill, new Cooldown(IntruderSettings.IntKillCd), "KILL",
            (PlayerBodyExclusion)Exception);
        Player.SetImpostor(true);
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();

        if (IsRecruit)
        {
            var jackal = Player.GetJackal();
            team.Add(jackal.Player);
            team.AddRange(jackal.GetOtherRecruits(Player));
        }

        foreach (var player in AllPlayers())
        {
            if (player.Is(Faction) && player != Player)
                team.Add(player);
        }

        if (Player.Is(LayerEnum.Lovers))
            team.Add(Player.GetOtherLover());
        else if (Player.Is(LayerEnum.Rivals))
            team.Add(Player.GetOtherRival());
        else if (Player.Is(LayerEnum.Mafia))
        {
            foreach (var player in AllPlayers())
            {
                if (player != Player && player.Is(LayerEnum.Mafia))
                    team.Add(player);
            }
        }

        return team;
    }

    public void Kill()
    {
        var cooldown = Interact(Player, KillButton.GetTarget<PlayerControl>(), true);

        if (this is Janitor jani)
        {
            if (Janitor.JaniCooldownsLinked)
                jani.CleanButton.StartCooldown(cooldown);
        }
        else if (this is PromotedGodfather gf)
        {
            if (Janitor.JaniCooldownsLinked && gf.IsJani)
                gf.CleanButton.StartCooldown(cooldown);
        }

        KillButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||
        Player.IsLinkedTo(player);
}