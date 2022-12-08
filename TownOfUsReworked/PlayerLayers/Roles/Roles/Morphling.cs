using System;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Modifiers;
using UnityEngine;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Morphling : Role, IVisualAlteration
    {
        private KillButton _morphButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastMorphed;
        public PlayerControl MorphedPlayer;
        public PlayerControl SampledPlayer;
        public float TimeRemaining;
        public bool Morphed => TimeRemaining > 0f;

        public Morphling(PlayerControl player) : base(player)
        {
            Name = "Morphling";
            StartText = "Transform Into <color=#8BFDFDFF>Crewmates</color> to frame them";
            AbilitiesText = "Morph into <color=#8BFDFD>Crewmates</color> to frame them";
            Color = CustomGameOptions.CustomImpColors ? Colors.Morphling : Colors.Intruder;
            SubFaction = SubFaction.None;
            LastMorphed = DateTime.UtcNow;
            RoleType = RoleEnum.Morphling;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            RoleAlignment = RoleAlignment.IntruderDecep;
            FactionColor = Colors.Intruder;
            AlignmentName = "Intruder (Deception)";
            IntroText = "Kill those who oppose you";
            Results = InspResults.DisgMorphCamoAgent;
            IntroSound = TownOfUsReworked.MorphlingIntro;
            AddToRoleHistory(RoleType);
        }

        public KillButton MorphButton
        {
            get => _morphButton;
            set
            {
                _morphButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Morph()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Morph(Player, MorphedPlayer);

            if (Player.Data.IsDead)
                TimeRemaining = 0f;
        }

        public void Unmorph()
        {
            MorphedPlayer = null;
            Utils.DefaultOutfitAll();
            LastMorphed = DateTime.UtcNow;
        }

        public float MorphTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMorphed;
            var num = CustomGameOptions.MorphlingCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (Morphed)
            {
                appearance = MorphedPlayer.GetDefaultAppearance();
                var modifier = Modifier.GetModifier(MorphedPlayer);

                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);

                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var intTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Intruders))
                    intTeam.Add(player);
            }
            __instance.teamToShow = intTeam;
        }

        public override void Wins()
        {
            IntruderWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if ((PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Crew) |
                x.Is(RoleAlignment.NeutralKill) | x.Is(Faction.Syndicate) | x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros))) == 0) |
                Utils.Sabotaged())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.IntruderWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}
