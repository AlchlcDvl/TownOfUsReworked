using System;
using System.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Whisperer : NeutralRole
    {
        public AbilityButton WhisperButton;
        public DateTime LastWhispered;
        public int WhisperCount;
        public int ConversionCount;
        public List<(byte, int)> PlayerConversion = new();
        public float WhisperConversion;
        public List<byte> Persuaded = new();

        public Whisperer(PlayerControl player) : base(player)
        {
            Name = "Whisperer";
            Color = Colors.Whisperer;
            AbilitiesText = "- You can whisper to players around, slowly bending them to your ideals\n- When a player reaches 100% conversion, they will defect and join the " +
                "<color=#F995FCFF>Sect</color>";
            Objectives = "- Persuade or kill anyone who can oppose the <color=#F995FCFF>Sect</color>";
            RoleType = RoleEnum.Whisperer;
            RoleAlignment = RoleAlignment.NeutralNeo;
            SubFaction = SubFaction.Sect;
            SubFactionColor = Colors.Sect;
            AlignmentName = NN;
            PlayerConversion = new();
            WhisperConversion = CustomGameOptions.InitialWhisperRate;
            Persuaded = new()
            {
                Player.PlayerId
            };
        }

        public float WhisperTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastWhispered;
            var num = (CustomGameOptions.WhisperCooldown + (CustomGameOptions.WhisperCooldownIncrease * WhisperCount)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
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

        public void Whisper()
        {
            var truePosition = Player.GetTruePosition();
            var closestPlayers = Utils.GetClosestPlayers(truePosition, CustomGameOptions.WhisperRadius);
            closestPlayers.Remove(Player);

            if (PlayerConversion.Count == 0)
                PlayerConversion = GetPlayers();

            var oldStats = PlayerConversion;
            PlayerConversion = new();

            foreach (var conversionRate in oldStats)
            {
                var player = conversionRate.Item1;
                var stats = conversionRate.Item2;

                if (closestPlayers.Contains(Utils.PlayerById(player)))
                    stats -= (int)WhisperConversion;

                if (!Utils.PlayerById(player).Data.IsDead)
                    PlayerConversion.Add((player, stats));
            }

            WhisperCount++;
            var removals = new List<(byte, int)>();

            foreach (var playerConversion in PlayerConversion)
            {
                if (playerConversion.Item2 <= 0)
                {
                    ConversionCount++;

                    if (CustomGameOptions.WhisperRateDecreases)
                        WhisperConversion -= CustomGameOptions.WhisperRateDecrease;

                    if (WhisperConversion < 2.5f)
                        WhisperConversion = 2.5f;

                    if (Utils.PlayerById(playerConversion.Item1).Is(SubFaction.None))
                    {
                        removals.Add(playerConversion);
                        var targetRole = GetRole(Utils.PlayerById(playerConversion.Item1));
                        targetRole.SubFaction = SubFaction.Sect;
                        targetRole.IsPersuaded = true;
                        Persuaded.Add(playerConversion.Item1);

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                            Utils.Flash(Colors.Mystic, "Someone has changed their allegience!");
                    }
                    else if (!Utils.PlayerById(playerConversion.Item1).Is(SubFaction.Sect))
                        Utils.RpcMurderPlayer(Player, Utils.PlayerById(playerConversion.Item1), false);
                    else  if (Utils.PlayerById(playerConversion.Item1).Is(SubFaction.Sect))
                    {
                        if (Utils.PlayerById(playerConversion.Item1).Is(RoleEnum.Whisperer))
                        {
                            var targetRole = GetRole<Whisperer>(Utils.PlayerById(playerConversion.Item1));
                            Persuaded.AddRange(targetRole.Persuaded);
                            targetRole.Persuaded.AddRange(Persuaded);
                        }
                        else
                            Persuaded.Add(playerConversion.Item1);
                    }
                }
            }

            foreach (var removal in removals)
                PlayerConversion.Remove(removal);
        }
    }
}