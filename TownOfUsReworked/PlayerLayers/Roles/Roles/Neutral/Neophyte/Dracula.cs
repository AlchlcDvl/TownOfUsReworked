using System;
using System.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Dracula : NeutralRole
    {
        public DateTime LastBitten;
        public AbilityButton BiteButton;
        public PlayerControl ClosestPlayer;
        public List<byte> Converted = new();

        public Dracula(PlayerControl player) : base(player)
        {
            Name = "Dracula";
            RoleType = RoleEnum.Dracula;
            StartText = "Lead The <color=#7B8968FF>Undead</color> To Victory";
            AbilitiesText = "- You can convert the <color=#8BFDFDFF>Crew</color> into your own sub faction\n- If the target cannot be converted or the number of alive " +
                $"<color=#7B8968FF>Undead</color> exceeds {CustomGameOptions.AliveVampCount}, you will kill them instead\n- Attempting to convert a <color=#C0C0C0FF>Vampire Hunter" +
                "</color> will force them to kill you";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Dracula : Colors.Neutral;
            Objectives = "- Convert or kill anyone who can oppose the <color=#7B8968FF>Undead</color>";
            SubFaction = SubFaction.Undead;
            RoleAlignment = RoleAlignment.NeutralNeo;
            AlignmentName = NN;
            SubFactionColor = Colors.Undead;
            Converted = new() { Player.PlayerId };
        }

        public float ConvertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBitten;
            var num = CustomGameOptions.BiteCd * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }
    }
}