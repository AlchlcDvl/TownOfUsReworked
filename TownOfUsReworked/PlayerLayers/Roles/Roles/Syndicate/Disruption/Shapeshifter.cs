using System;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Shapeshifter : SyndicateRole
    {
        public AbilityButton ShapeshiftButton;
        public bool Enabled;
        public DateTime LastShapeshifted;
        public float TimeRemaining;
        public bool Shapeshifted => TimeRemaining > 0f;
        public PlayerControl ShapeshiftPlayer1;
        public PlayerControl ShapeshiftPlayer2;
        public CustomMenu ShapeshiftMenu1;
        public CustomMenu ShapeshiftMenu2;

        public Shapeshifter(PlayerControl player) : base(player)
        {
            Name = "Shapeshifter";
            StartText = "Change Everyone's Appearances";
            AbilitiesText = $"- You can shuffle everyone's appearances\n{AbilitiesText}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Shapeshifter : Colors.Syndicate;
            RoleType = RoleEnum.Shapeshifter;
            RoleAlignment = RoleAlignment.SyndicateDisruption;
            AlignmentName = SD;
            ShapeshiftPlayer1 = null;
            ShapeshiftPlayer2 = null;
            ShapeshiftMenu1 = new CustomMenu(Player, new CustomMenu.Select(Click1));
            ShapeshiftMenu2 = new CustomMenu(Player, new CustomMenu.Select(Click2));
        }

        public void Shapeshift()
        {
            TimeRemaining -= Time.deltaTime;
            Enabled = true;

            if (!SyndicateHasChaosDrive)
            {
                Utils.Morph(ShapeshiftPlayer1, ShapeshiftPlayer2);
                Utils.Morph(ShapeshiftPlayer2, ShapeshiftPlayer1);
            }

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void UnShapeshift()
        {
            Enabled = false;
            LastShapeshifted = DateTime.UtcNow;

            if (SyndicateHasChaosDrive)
                Utils.DefaultOutfitAll();
            else
            {
                Utils.DefaultOutfit(ShapeshiftPlayer1);
                Utils.DefaultOutfit(ShapeshiftPlayer2);
            }
        }

        public float ShapeshiftTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastShapeshifted;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.ShapeshiftCooldown, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Click1(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                ShapeshiftPlayer1 = player;
            else if (interact[1])
                LastShapeshifted.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void Click2(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3])
                ShapeshiftPlayer2 = player;
            else if (interact[1])
                LastShapeshifted.AddSeconds(CustomGameOptions.ProtectKCReset);
        }
    }
}