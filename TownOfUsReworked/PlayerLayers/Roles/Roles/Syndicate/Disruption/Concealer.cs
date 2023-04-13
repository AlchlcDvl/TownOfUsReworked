using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Concealer : SyndicateRole
    {
        public AbilityButton ConcealButton;
        public bool Enabled;
        public DateTime LastConcealed;
        public float TimeRemaining;
        public bool Concealed => TimeRemaining > 0f;
        public PlayerControl ConcealedPlayer;
        public CustomMenu ConcealMenu;

        public Concealer(PlayerControl player) : base(player)
        {
            Name = "Concealer";
            StartText = "Make The <color=#8BFDFDFF>Crew</color> Invisible For Some Chaos";
            AbilitiesText = "- You can make a player invisible\n- With the Chaos Drive, you make everyone invisible";
            Color = CustomGameOptions.CustomSynColors ? Colors.Concealer : Colors.Syndicate;
            RoleType = RoleEnum.Concealer;
            RoleAlignment = RoleAlignment.SyndicateDisruption;
            AlignmentName = SD;
            InspectorResults = InspectorResults.Unseen;
            ConcealMenu = new CustomMenu(Player, new CustomMenu.Select(Click));
            ConcealedPlayer = null;
        }

        public void Conceal()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance || (ConcealedPlayer == null && !SyndicateHasChaosDrive))
                TimeRemaining = 0f;
        }

        public void UnConceal()
        {
            Enabled = false;
            LastConcealed = DateTime.UtcNow;

            if (SyndicateHasChaosDrive)
                Utils.DefaultOutfitAll();
            else
                Utils.DefaultOutfit(ConcealedPlayer);
        }

        public float ConcealTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastConcealed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.ConcealCooldown, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Click(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                ConcealedPlayer = player;
            else if (interact[1])
                LastConcealed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }
    }
}