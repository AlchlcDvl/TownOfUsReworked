namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Intruder : Role
{
    public CustomButton KillButton { get; set; }
    public string CommonAbilities => "<color=#FF1919FF>- You can kill players" + (CustomGameOptions.IntrudersCanSabotage || (IsDead && CustomGameOptions.GhostsCanSabotage) ? ("\n- You can " +
        "call sabotages to distract the <color=#8CFFFFFF>Crew</color>") : "") + "</color>";

    public override UColor Color => CustomColorManager.Intruder;
    public override Faction BaseFaction => Faction.Intruder;
    public override AttackEnum AttackVal => AttackEnum.Basic;

    protected Intruder(PlayerControl player) : base(player)
    {
        Faction = Faction.Intruder;
        FactionColor = CustomColorManager.Intruder;
        Objectives = () => IntrudersWinCon;
        KillButton = new(this, "IntruderKill", AbilityTypes.Alive, "ActionSecondary", Kill, CustomGameOptions.IntKillCd, Exception);
        Data.SetImpostor(true);
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();

        if (IsRecruit)
        {
            var jackal = Player.GetJackal();
            team.Add(jackal.Player);
            team.Add(jackal.GoodRecruit);
        }

        foreach (var player in CustomPlayer.AllPlayers)
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
            foreach (var player in CustomPlayer.AllPlayers)
            {
                if (player != Player && player.Is(LayerEnum.Mafia))
                    team.Add(player);
            }
        }

        return team;
    }

    public void Kill()
    {
        var cooldown = Interact(Player, KillButton.TargetPlayer, true);

        if (this is Janitor jani)
        {
            if (CustomGameOptions.JaniCooldownsLinked)
                jani.CleanButton.StartCooldown(cooldown);
        }
        else if (this is PromotedGodfather gf)
        {
            if (CustomGameOptions.JaniCooldownsLinked && gf.IsJani)
                gf.CleanButton.StartCooldown(cooldown);
        }

        KillButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) =>  (player.Is(Faction) && Faction != Faction.Crew) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||  Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        KillButton.Update2("KILL");
    }
}