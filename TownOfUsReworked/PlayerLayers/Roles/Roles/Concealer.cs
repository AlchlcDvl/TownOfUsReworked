using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Concealer : Role
    {
        public KillButton _concealButton;
        public bool Enabled;
        public DateTime LastConcealed { get; set; }
        public float TimeRemaining;
        public bool Concealed => TimeRemaining > 0f;

        public Concealer(PlayerControl player) : base(player)
        {
            Name = "Concealer";
            ImpostorText = () => "Make The <color=#8BFDFDFF>Crew</color> Invisible For Some Chaos";
            TaskText = () => "Camouflage among everyone and kill in front of them";
            Color = CustomGameOptions.CustomSynColors ? Colors.Concealer : Colors.Syndicate;
            RoleType = RoleEnum.Concealer;
            Faction = Faction.Syndicate;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateSupport;
            AlignmentName = () => "Syndicate (Support)";
            IntroText = "Cause choas and kill your opposition";
            CoronerDeadReport = $"The camouflage suit indicate that this body is a Camouflager!";
            CoronerKillerReport = "There are marks of grey paint on the body. They were killed by a Camouflager!";
            Results = InspResults.Conceal;
            SubFaction = SubFaction.None;
            AddToRoleHistory(RoleType);
        }

        public KillButton ConcealButton
        {
            get => _concealButton;
            set
            {
                _concealButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void Conceal()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.Conceal();
        }

        public void UnConceal()
        {
            Enabled = false;
            LastConcealed = DateTime.UtcNow;
            Utils.UnConceal();
        }

        public float ConcealTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastConcealed;
            var num = CustomGameOptions.CamouflagerCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var synTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Syndicate))
                    synTeam.Add(player);
            }
            __instance.teamToShow = synTeam;
        }
    }
}