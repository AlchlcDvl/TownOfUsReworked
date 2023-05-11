using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Crusader : SyndicateRole
    {
        public bool Enabled;
        public DateTime LastCrusaded;
        public float TimeRemaining;
        public bool OnCrusade => TimeRemaining > 0f;
        public PlayerControl CrusadedPlayer;
        public CustomButton CrusadeButton;

        public Crusader(PlayerControl player) : base(player)
        {
            Name = "Crusader";
            StartText = "Ambush";
            AbilitiesText = $"- You can crusade players\n- Ambushed players will be forced to be on alert, and will kill whoever interacts with then\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Crusader : Colors.Syndicate;
            RoleType = RoleEnum.Crusader;
            RoleAlignment = RoleAlignment.SyndicateKill;
            AlignmentName = SyK;
            InspectorResults = InspectorResults.PreservesLife;
            Type = LayerEnum.Crusader;
            CrusadedPlayer = null;
            CrusadeButton = new(this, "Crusade", AbilityTypes.Direct, "ActionSecondary", HitCrusade, Exception1);
        }

        public float CrusadeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastCrusaded;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CrusadeCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Crusade()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (IsDead || CrusadedPlayer.Data.IsDead || CrusadedPlayer.Data.Disconnected || MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void UnCrusade()
        {
            Enabled = false;
            LastCrusaded = DateTime.UtcNow;
            CrusadedPlayer = null;
        }

        public static void RadialCrusade(PlayerControl player2)
        {
            foreach (var player in Utils.GetClosestPlayers(player2.GetTruePosition(), CustomGameOptions.ChaosDriveCrusadeRadius))
            {
                Utils.Spread(player2, player);

                if (player.IsVesting() || player.IsProtected() || player2.IsOtherRival(player) || player.IsShielded() || player.IsRetShielded() || (player.Is(Faction.Syndicate) &&
                    !CustomGameOptions.CrusadeMates))
                {
                    continue;
                }

                if (!player.Is(RoleEnum.Pestilence))
                    Utils.RpcMurderPlayer(player2, player, DeathReasonEnum.Crusaded, false);

                if (player.IsOnAlert() || player.Is(RoleEnum.Pestilence))
                    Utils.RpcMurderPlayer(player, player2);
                else if (player.IsAmbushed() || player.IsGFAmbushed())
                    Utils.RpcMurderPlayer(player, player2, DeathReasonEnum.Ambushed);
                else if (player.IsCrusaded() || player.IsRebCrusaded())
                    Utils.RpcMurderPlayer(player, player2, DeathReasonEnum.Crusaded);
            }
        }

        public void HitCrusade()
        {
            if (CrusadeTimer() != 0f || Utils.IsTooFar(Player, CrusadeButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, CrusadeButton.TargetPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Crusade);
                writer.Write(PlayerId);
                writer.Write(CrusadeButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.CrusadeDuration;
                CrusadedPlayer = CrusadeButton.TargetPlayer;
                Crusade();
            }
            else if (interact[0])
                LastCrusaded = DateTime.UtcNow;
            else if (interact[1])
                LastCrusaded.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception1(PlayerControl player) => player == CrusadedPlayer || (player.Is(Faction) && !CustomGameOptions.CrusadeMates) || (player.Is(SubFaction) &&
            SubFaction != SubFaction.None);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            CrusadeButton.Update("CRUSADE", CrusadeTimer(), CustomGameOptions.CrusadeCooldown, OnCrusade, TimeRemaining, CustomGameOptions.CrusadeDuration);
        }
    }
}