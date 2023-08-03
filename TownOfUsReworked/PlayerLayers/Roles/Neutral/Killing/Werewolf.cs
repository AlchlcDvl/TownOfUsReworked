namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Werewolf : Neutral
    {
        public DateTime LastMauled { get; set; }
        public bool CanMaul => Rounds % 2 == 1 || Rounds > 3;
        public CustomButton MaulButton { get; set; }
        public int Rounds { get; set; }

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Werewolf : Colors.Neutral;
        public override string Name => "Werewolf";
        public override LayerEnum Type => LayerEnum.Werewolf;
        public override RoleEnum RoleType => RoleEnum.Werewolf;
        public override Func<string> StartText => () => "AWOOOOOOOOOO";
        public override Func<string> Description => () => $"- You kill everyone within {CustomGameOptions.MaulRadius}m";
        public override InspectorResults InspectorResults => InspectorResults.IsAggressive;
        public float Timer => ButtonUtils.Timer(Player, LastMauled, CustomGameOptions.MaulCooldown);

        public Werewolf(PlayerControl player) : base(player)
        {
            Objectives = () => "- Maul anyone who can oppose you";
            RoleAlignment = RoleAlignment.NeutralKill;
            MaulButton = new(this, "Maul", AbilityTypes.Direct, "ActionSecondary", HitMaul, Exception);
        }

        public void Maul()
        {
            foreach (var player in GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.MaulRadius))
            {
                Spread(Player, player);

                if (player.IsVesting() || player.IsProtected() || Player.IsLinkedTo(player) || Player == player || MaulButton.TargetPlayer == player || player.IsShielded() ||
                    player.IsRetShielded() || player.IsProtectedMonarch())
                {
                    continue;
                }

                if (!player.Is(RoleEnum.Pestilence))
                    RpcMurderPlayer(Player, player, DeathReasonEnum.Mauled, false);

                if (player.IsOnAlert() || player.Is(RoleEnum.Pestilence))
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
            if (Timer != 0f || IsTooFar(Player, MaulButton.TargetPlayer))
                return;

            var interact = Interact(Player, MaulButton.TargetPlayer, true);

            if (interact[3])
                Maul();

            if (interact[0])
                LastMauled = DateTime.UtcNow;
            else if (interact[1])
                LastMauled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastMauled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
            || Player.IsLinkedTo(player);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            MaulButton.Update("MAUL", Timer, CustomGameOptions.MaulCooldown, CanMaul);
        }
    }
}