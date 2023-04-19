using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Werewolf : NeutralRole
    {
        public PlayerControl ClosestPlayer;
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
            MaulButton = new(this, AssetManager.Maul, AbilityTypes.Direct, "ActionSecondary", HitMaul);
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

                if (player.IsVesting() || player.IsProtected() || Player.IsOtherRival(player) || Player == player || ClosestPlayer == player || player.IsShielded() || player.IsRetShielded())
                    continue;

                if (!player.Is(RoleEnum.Pestilence))
                    Utils.RpcMurderPlayer(Player, player, DeathReasonEnum.Mauled, false);

                if (player.IsOnAlert() || player.Is(RoleEnum.Pestilence) || player.IsAmbushed() || player.IsGFAmbushed())
                    Utils.RpcMurderPlayer(player, Player);
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
            if (MaulTimer() != 0f || Utils.IsTooFar(Player, ClosestPlayer))
                return;

            var interact = Utils.Interact(Player, ClosestPlayer, true);

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
            MaulButton.Update("MAUL", MaulTimer(), CustomGameOptions.MaulCooldown);
        }
    }
}