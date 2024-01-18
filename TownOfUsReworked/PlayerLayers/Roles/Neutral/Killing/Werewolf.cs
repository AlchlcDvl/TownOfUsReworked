namespace TownOfUsReworked.PlayerLayers.Roles;

public class Werewolf : Neutral
{
    public bool CanMaul => Rounds is not (0 or 2);
    public CustomButton MaulButton { get; set; }
    public int Rounds { get; set; }

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Werewolf : CustomColorManager.Neutral;
    public override string Name => "Werewolf";
    public override LayerEnum Type => LayerEnum.Werewolf;
    public override Func<string> StartText => () => "AWOOOOOOOOOO";
    public override Func<string> Description => () => $"- You kill everyone within {CustomGameOptions.MaulRadius}m";
    public override AttackEnum AttackVal => AttackEnum.Powerful;
    public override DefenseEnum DefenseVal => CanMaul ? DefenseEnum.None : DefenseEnum.Basic;

    public Werewolf(PlayerControl player) : base(player)
    {
        Objectives = () => "- Maul anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        MaulButton = new(this, "Maul", AbilityTypes.Alive, "ActionSecondary", HitMaul, CustomGameOptions.MaulCd, Exception);
        player.Data.Role.IntroSound = GetAudio("WerewolfIntro");
    }

    public void Maul()
    {
        foreach (var player in GetClosestPlayers(Player.transform.position, CustomGameOptions.MaulRadius))
        {
            Spread(Player, player);

            if (CanAttack(AttackVal, player.GetDefenseValue()))
                RpcMurderPlayer(Player, player, DeathReasonEnum.Mauled, false);
        }
    }

    public void HitMaul()
    {
        var cooldown = Interact(Player, MaulButton.TargetPlayer, true);

        if (cooldown != CooldownType.Fail)
            Maul();

        MaulButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        MaulButton.Update2("MAUL", CanMaul);
    }
}