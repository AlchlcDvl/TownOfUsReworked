namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Werewolf : Neutral
    {
        public DateTime LastMauled;
        public bool CanMaul;
        public CustomButton MaulButton;

        public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Werewolf : Colors.Neutral;
        public override string Name => "Werewolf";
        public override LayerEnum Type => LayerEnum.Werewolf;
        public override RoleEnum RoleType => RoleEnum.Werewolf;
        public override Func<string> StartText => () => "AWOOOOOOOOOO";
        public override Func<string> AbilitiesText => () => $"- You kill everyone within {CustomGameOptions.MaulRadius}m";
        public override InspectorResults InspectorResults => InspectorResults.IsAggressive;

        public Werewolf(PlayerControl player) : base(player)
        {
            Objectives = () => "- Maul anyone who can oppose you";
            RoleAlignment = RoleAlignment.NeutralKill;
            MaulButton = new(this, "Maul", AbilityTypes.Direct, "ActionSecondary", HitMaul, Exception);
        }

        public float MaulTimer()
        {
            var timespan = DateTime.UtcNow - LastMauled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MaulCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
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
            if (MaulTimer() != 0f || IsTooFar(Player, MaulButton.TargetPlayer))
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
            MaulButton.Update("MAUL", MaulTimer(), CustomGameOptions.MaulCooldown);
        }
    }
}