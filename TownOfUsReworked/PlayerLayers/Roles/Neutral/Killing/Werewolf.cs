namespace TownOfUsReworked.PlayerLayers.Roles;

public class Werewolf : Neutral
{
    public bool CanMaul => Rounds is not (0 or 2);
    public CustomButton MaulButton { get; set; }
    public int Rounds { get; set; }

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Werewolf : Colors.Neutral;
    public override string Name => "Werewolf";
    public override LayerEnum Type => LayerEnum.Werewolf;
    public override Func<string> StartText => () => "AWOOOOOOOOOO";
    public override Func<string> Description => () => $"- You kill everyone within {CustomGameOptions.MaulRadius}m";
    public override InspectorResults InspectorResults => InspectorResults.IsAggressive;

    public Werewolf(PlayerControl player) : base(player)
    {
        Objectives = () => "- Maul anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        MaulButton = new(this, "Maul", AbilityTypes.Target, "ActionSecondary", HitMaul, CustomGameOptions.MaulCd, Exception);
    }

    public void Maul()
    {
        foreach (var player in GetClosestPlayers(Player.transform.position, CustomGameOptions.MaulRadius))
        {
            Spread(Player, player);

            if (player.IsVesting() || player.IsProtected() || Player.IsLinkedTo(player) || Player == player || MaulButton.TargetPlayer == player || player.IsShielded() ||
                player.IsRetShielded() || player.IsProtectedMonarch())
            {
                continue;
            }

            if (!player.Is(LayerEnum.Pestilence))
                RpcMurderPlayer(Player, player, DeathReasonEnum.Mauled, false);

            if (player.IsOnAlert() || player.Is(LayerEnum.Pestilence))
                RpcMurderPlayer(player, Player);
            else if (player.IsAmbushed() || player.IsGFAmbushed())
                RpcMurderPlayer(player, Player, DeathReasonEnum.Ambushed);
            else if (player.IsCrusaded() || player.IsRebCrusaded())
            {
                if (player.GetCrusader()?.HoldsDrive == true || player.GetRebCrus()?.HoldsDrive == true)
                    Crusader.RadialCrusade(player);
                else
                    RpcMurderPlayer(player, Player, DeathReasonEnum.Crusaded, true);
            }
        }
    }

    public void HitMaul()
    {
        var interact = Interact(Player, MaulButton.TargetPlayer, true);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
            Maul();

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;
        else if (interact.Vested)
            cooldown = CooldownType.Survivor;

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