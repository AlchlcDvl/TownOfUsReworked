using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Poisoner : SyndicateRole
    {
        public AbilityButton PoisonButton;
        public DateTime LastPoisoned;
        public PlayerControl PoisonedPlayer;
        public PlayerControl ClosestPoison;
        public float TimeRemaining;
        public bool Enabled;
        public bool Poisoned => TimeRemaining > 0f;
        public CustomMenu PoisonMenu;

        public Poisoner(PlayerControl player) : base(player)
        {
            Name = "Poisoner";
            StartText = "Delay A Kill To Decieve The <color=#8BFDFDFF>Crew</color>";
            AbilitiesText = $"- You can poison players\n- Poisoned players will die after {CustomGameOptions.PoisonDuration}s\n- With the Chaos Drive, your poison delay and cooldown" +
                $"occur concurrently\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors? Colors.Poisoner : Colors.Syndicate;
            RoleType = RoleEnum.Poisoner;
            PoisonedPlayer = null;
            RoleAlignment = RoleAlignment.SyndicateKill;
            AlignmentName = SyK;
            PoisonMenu = new CustomMenu(Player, new CustomMenu.Select(Click));
        }

        public void Poison()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance || PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected || Player.Data.IsDead)
                TimeRemaining = 0f;
        }

        public void PoisonKill()
        {
            if (!(PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected || PoisonedPlayer.Is(RoleEnum.Pestilence)))
            {
                Utils.RpcMurderPlayer(Player, PoisonedPlayer, DeathReasonEnum.Poisoned, false);

                if (!PoisonedPlayer.Data.IsDead)
                {
                    try
                    {
                        SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 1f);
                    } catch {}
                }
            }

            PoisonedPlayer = null;
            Enabled = false;
            LastPoisoned = DateTime.UtcNow;
        }

        public float PoisonTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastPoisoned;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.PoisonCd, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Click(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                PoisonedPlayer = player;
            else if (interact[1])
                LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
        }
    }
}