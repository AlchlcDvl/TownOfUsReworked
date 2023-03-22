using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Concealer : SyndicateRole
    {
        public AbilityButton ConcealButton;
        public bool Enabled;
        public DateTime LastConcealed;
        public float TimeRemaining;
        public bool Concealed => TimeRemaining > 0f;

        public Concealer(PlayerControl player) : base(player)
        {
            Name = "Concealer";
            StartText = "Make The <color=#8BFDFDFF>Crew</color> Invisible For Some Chaos";
            AbilitiesText = "- You can turn off player's ability to see things properly, making all other players appear invisible.";
            Color = CustomGameOptions.CustomSynColors ? Colors.Concealer : Colors.Syndicate;
            RoleType = RoleEnum.Concealer;
            RoleAlignment = RoleAlignment.SyndicateDisruption;
            AlignmentName = SD;
            InspectorResults = InspectorResults.Unseen;
        }

        public void Conceal()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void UnConceal()
        {
            Enabled = false;
            LastConcealed = DateTime.UtcNow;
            Utils.DefaultOutfitAll();
        }

        public float ConcealTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastConcealed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ConcealCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}