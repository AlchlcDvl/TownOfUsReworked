using System;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Dracula : NeutralRole
    {
        public DateTime LastBitten;
        public AbilityButton BiteButton;
        public PlayerControl ClosestPlayer;
        public List<byte> Converted;

        public Dracula(PlayerControl player) : base(player)
        {
            Name = "Dracula";
            RoleType = RoleEnum.Dracula;
            StartText = "Lead The <color=#7B8968FF>Undead</color> To Victory";
            AbilitiesText = "- You can convert the <color=#8BFDFDFF>Crew</color> into your own sub faction.\n- If the target is a killing role, they are converted to " +
                "<color=#DF7AE8FF>Dampyr</color> otherwise they convert into a <color=#2BD29CFF>Vampire</color>.\n- If the target cannot be converted, they will be attacked instead." +
                "\n- There is a chance that there is a <color=#C0C0C0FF>Vampire Hunter</color>\non the loose. Attempting to convert them will make them kill you.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Dracula : Colors.Neutral;
            SubFaction = SubFaction.Undead;
            RoleAlignment = RoleAlignment.NeutralNeo;
            AlignmentName = NN;
            Converted = new List<byte>();
            RoleDescription = "You are a Dracula! You are the leader of the Undead who drain blood from their enemies. Convert people to your side and " +
                "gain a quick majority.";
            SubFactionColor = Colors.Undead;
        }

        public float ConvertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBitten;
            var num = CustomGameOptions.BiteCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}