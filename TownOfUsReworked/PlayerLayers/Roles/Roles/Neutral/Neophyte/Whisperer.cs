using System;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Whisperer : NeutralRole
    {
        public AbilityButton WhisperButton;
        public DateTime LastWhispered;
        public int WhisperCount;
        public int ConversionCount;
        public List<(byte, int)> PlayerConversion;
        public float WhisperConversion;
        public List<byte> Persuaded;

        public Whisperer(PlayerControl player) : base(player)
        {
            Name = "Whisperer";
            Color = Colors.Whisperer;
            RoleType = RoleEnum.Whisperer;
            RoleAlignment = RoleAlignment.NeutralNeo;
            SubFaction = SubFaction.Sect;
            SubFactionColor = Colors.Sect;
            AlignmentName = NN;
            Persuaded = new List<byte>();
            PlayerConversion = new List<(byte, int)>();
            WhisperConversion = CustomGameOptions.InitialWhisperRate;
        }

        public float WhisperTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastWhispered;
            var num = (CustomGameOptions.WhisperCooldown + (CustomGameOptions.WhisperCooldownIncrease * WhisperCount)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public List<(byte, int)> GetPlayers()
        {
            var playerList = new List<(byte, int)>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (Player != player)
                    playerList.Add((player.PlayerId, 100));
            }

            return playerList;
        }
    }
}