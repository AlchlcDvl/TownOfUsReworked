using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Poisoner : SyndicateRole
    {
        public AbilityButton PoisonButton;
        public DateTime LastPoisoned;
        public PlayerControl PoisonedPlayer;
        public PlayerControl ClosestPoison;
        public float TimeRemaining;
        public bool Enabled = false;
        public bool Poisoned => TimeRemaining > 0f;

        public Poisoner(PlayerControl player) : base(player)
        {
            Name = "Poisoner";
            StartText = "Poison A <color=#8BFDFDFF>Crewmate</color> To Kill Them Later";
            AbilitiesText = "Poison the <color=#8BFDFDFF>Crew</color>";
            Color = CustomGameOptions.CustomIntColors? Colors.Poisoner : Colors.Syndicate;
            LastPoisoned = DateTime.UtcNow;
            RoleType = RoleEnum.Poisoner;
            PoisonedPlayer = null;
            RoleAlignment = RoleAlignment.SyndicateDisruption;
            AlignmentName = SD;
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
                Utils.RpcMurderPlayer(Player, PoisonedPlayer, false);

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
            var timeSpan = utcNow - LastPoisoned;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.PoisonCd, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}