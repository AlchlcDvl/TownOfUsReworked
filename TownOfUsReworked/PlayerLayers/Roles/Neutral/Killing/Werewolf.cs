using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Werewolf : NeutralRole
    {
        public DateTime LastMauled;
        public bool CanMaul;
        public CustomButton MaulButton;

        public Werewolf(PlayerControl player) : base(player)
        {
            Name = "Werewolf";
            StartText = "AWOOOOOOOOOO";
            AbilitiesText = $"- You kill everyone within {CustomGameOptions.MaulRadius}m";
            Objectives = "- Maul anyone who can oppose you";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Werewolf : Colors.Neutral;
            RoleType = RoleEnum.Werewolf;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            Type = LayerEnum.Werewolf;
            MaulButton = new(this, "Maul", AbilityTypes.Direct, "ActionSecondary", HitMaul);
            InspectorResults = InspectorResults.IsAggressive;
        }

        public float MaulTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMauled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MaulCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Maul()
        {
            foreach (var player in Utils.GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.MaulRadius))
            {
                Utils.Spread(Player, player);

                if (player.IsVesting() || player.IsProtected() || Player.IsOtherRival(player) || Player == player || MaulButton.TargetPlayer == player || player.IsShielded() ||
                    player.IsRetShielded())
                {
                    continue;
                }

                if (!player.Is(RoleEnum.Pestilence))
                    Utils.RpcMurderPlayer(Player, player, DeathReasonEnum.Mauled, false);

                if (player.IsOnAlert() || player.Is(RoleEnum.Pestilence))
                    Utils.RpcMurderPlayer(player, Player);
                else if (player.IsAmbushed() || player.IsGFAmbushed())
                    Utils.RpcMurderPlayer(player, Player, DeathReasonEnum.Ambushed);
                else if (player.IsCrusaded() || player.IsRebCrusaded())
                {
                    if (player.GetCrusader()?.HoldsDrive == true || player.GetRebCrus()?.HoldsDrive == true)
                        Crusader.RadialCrusade(player);
                    else
                        Utils.RpcMurderPlayer(player, Player, DeathReasonEnum.Crusaded, true);
                }
            }
        }

        public void HitMaul()
        {
            if (MaulTimer() != 0f || Utils.IsTooFar(Player, MaulButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, MaulButton.TargetPlayer, true);

            if (interact[3])
                Maul();

            if (interact[0])
                LastMauled = DateTime.UtcNow;
            else if (interact[1])
                LastMauled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastMauled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !(x.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) && !(SubFaction != SubFaction.None &&
                x.GetSubFaction() == SubFaction)).ToList();
            MaulButton.Update("MAUL", MaulTimer(), CustomGameOptions.MaulCooldown, targets);
        }
    }
}