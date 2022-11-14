using System;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using System.Collections.Generic;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VigilanteMod;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class VampireHunter : Role
    {
        public bool VHWin;
        public PlayerControl ClosestPlayer;
        public DateTime LastStaked { get; set; }
        public List<byte> Vampires = new List<byte>();
        public int VampsAlive => Vampires.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null &&
            !Utils.PlayerById(x).Data.IsDead && Utils.PlayerById(x).Is(SubFaction.Vampires));
        public bool VampsDead => VampsAlive == 0;

        public VampireHunter(PlayerControl player) : base(player)
        {
            Name = "Vampire Hunter";
            ImpostorText = () => "Stake The <color=#7B8968FF>Vampires</color>";
            TaskText = () => "Stake the <color=#7B8968FF>Vampires</color>";
            Color = CustomGameOptions.CustomCrewColors ? Colors.VampireHunter : Colors.Crew;
            SubFaction = SubFaction.None;
            LastStaked = DateTime.UtcNow;
            RoleType = RoleEnum.VampireHunter;
            Faction = Faction.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewCheck;
            AlignmentName = () => "Crew (Check)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.SurvVHVampVig;
            AddToRoleHistory(RoleType);
        }

        public float StakeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastStaked;
            var num = CustomGameOptions.VigiKillCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public void TurnVigilante()
        {
            RoleDictionary.Remove(Player.PlayerId);
            var role = new Vigilante(Player);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer)
                    role.RegenTask();
            }
        }
    }
}