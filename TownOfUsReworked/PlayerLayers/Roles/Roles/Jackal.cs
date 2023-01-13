using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using System;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Jackal : Role
    {
        public PlayerControl EvilRecruit = null;
        public PlayerControl GoodRecruit = null;
        public PlayerControl BackupRecruit = null;
        public PlayerControl ClosestPlayer;
        public KillButton _recruitButton;
        public bool HasRecruited = false;
        public DateTime LastRecruited { get; set; }

        public Jackal(PlayerControl player) : base(player)
        {
            Name = "Jackal";
            Faction = Faction.Neutral;
            RoleType = RoleEnum.Jackal;
            StartText = "Gain A Majority";
            AbilitiesText = "- You can recruit one player into joining your organisation.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Jackal : Colors.Neutral;
            SubFaction = SubFaction.Cabal;
            SubFactionColor = Colors.Cabal;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralNeo;
            AlignmentName = "Neutral (Neophyte)";
            Results = InspResults.SurvVHVampVig;
        }

        public override void Wins()
        {
            CabalWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }
        
        public float RecruitTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastRecruited;
            var num = CustomGameOptions.RecruitCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Utils.CabalWin())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UndeadWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            
            return false;
        }

        public KillButton RecruitButton
        {
            get => _recruitButton;
            set
            {
                _recruitButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var jackTeam = new List<PlayerControl>();
            jackTeam.Add(PlayerControl.LocalPlayer);
            jackTeam.Add(GoodRecruit);
            jackTeam.Add(EvilRecruit);
            __instance.teamToShow = jackTeam;
        }
    }
}