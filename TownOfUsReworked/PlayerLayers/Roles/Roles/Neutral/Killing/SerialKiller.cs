﻿using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class SerialKiller : NeutralRole
    {
        public CustomButton BloodlustButton;
        public CustomButton StabButton;
        public bool Enabled;
        public PlayerControl ClosestPlayer;
        public DateTime LastLusted;
        public DateTime LastKilled;
        public float TimeRemaining;
        public bool Lusted => TimeRemaining > 0f;

        public SerialKiller(PlayerControl player) : base(player)
        {
            Name = "Serial Killer";
            StartText = "You Like To Play With Knives";
            AbilitiesText = "- You can go into bloodlust\n- When in bloodlust, your kill cooldown is very short\n- If and when an <color=#803333FF>Escort</color>, " +
                "<color=#801780FF>Consort</color> or <color=#00FF00FF>Glitch</color> tries to block you, you will immediately kill them, regardless of your cooldown\n- You are " +
                "immune to blocks";
            Objectives = "- Stab anyone who can oppose you";
            Color = CustomGameOptions.CustomNeutColors ? Colors.SerialKiller : Colors.Neutral;
            RoleType = RoleEnum.SerialKiller;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            RoleBlockImmune = true;
            Type = LayerEnum.SerialKiller;
            StabButton = new(this, AssetManager.Stab, AbilityTypes.Direct, "ActionSecondary", Stab);
            BloodlustButton = new(this, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary", Lust);
        }

        public float LustTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastLusted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.BloodlustCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Bloodlust()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Player.Data.IsDead || MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void Unbloodlust()
        {
            Enabled = false;
            LastLusted = DateTime.UtcNow;
        }

        public float StabTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.LustKillCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Lust()
        {
            if (LustTimer() != 0f || Lusted)
                return;

            TimeRemaining = CustomGameOptions.BloodlustDuration;
            Bloodlust();
        }

        public void Stab()
        {
            if (!Lusted || StabTimer() != 0f || Utils.IsTooFar(Player, ClosestPlayer))
                return;

            var interact = Utils.Interact(Player, ClosestPlayer, true);

            if (interact[3] || interact[0])
                LastKilled = DateTime.UtcNow;
            else if (interact[1])
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            StabButton.Update("STAB", StabTimer(), CustomGameOptions.LustKillCd, true, Lusted);
            BloodlustButton.Update("BLOODLUST", LustTimer(), CustomGameOptions.BloodlustCd, Lusted, TimeRemaining, CustomGameOptions.BloodlustDuration);
        }
    }
}
