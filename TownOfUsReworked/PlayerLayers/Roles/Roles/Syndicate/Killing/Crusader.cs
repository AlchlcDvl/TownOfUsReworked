using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using Hazel;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Crusader : SyndicateRole
    {
        public bool Enabled;
        public DateTime LastCrusaded;
        public float TimeRemaining;
        public bool OnCrusade => TimeRemaining > 0f;
        public PlayerControl CrusadedPlayer;
        public PlayerControl ClosestCrusade;
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
            InspectorResults = InspectorResults.SeeksToProtect;
            Type = LayerEnum.Crusader;
            CrusadedPlayer = null;
            CrusadeButton = new(this, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary", HitCrusade);
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

            if (Player.Data.IsDead || CrusadedPlayer.Data.IsDead || CrusadedPlayer.Data.Disconnected || MeetingHud.Instance)
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

                if (player.IsVesting() || player.IsProtected() || player2.IsOtherRival(player))
                    continue;

                if (!player.Is(RoleEnum.Pestilence) && !player.IsOnAlert())
                    Utils.RpcMurderPlayer(player2, player, DeathReasonEnum.Crusaded, false);

                if (player.IsOnAlert() || player.Is(RoleEnum.Pestilence))
                    Utils.RpcMurderPlayer(player, player2);
            }
        }

        public void HitCrusade()
        {
            if (CrusadeTimer() != 0f || Utils.IsTooFar(Player, ClosestCrusade))
                return;

            var interact = Utils.Interact(Player, ClosestCrusade);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Crusade);
                writer.Write(Player.PlayerId);
                writer.Write(ClosestCrusade.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.CrusadeDuration;
                CrusadedPlayer = ClosestCrusade;
                Crusade();
            }
            else if (interact[0])
                LastCrusaded = DateTime.UtcNow;
            else if (interact[1])
                LastCrusaded.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var notCrusaded = PlayerControl.AllPlayerControls.ToArray().Where(x => x != CrusadedPlayer).ToList();
            CrusadeButton.Update("CRUSADE", CrusadeTimer(), CustomGameOptions.CrusadeCooldown, notCrusaded, OnCrusade, TimeRemaining, CustomGameOptions.CrusadeDuration);
        }
    }
}