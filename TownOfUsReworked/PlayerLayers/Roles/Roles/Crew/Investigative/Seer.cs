using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;
using Reactor.Utilities;
using System.Linq;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Seer : CrewRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastSeered;
        public bool ChangedDead => !AllRoles.Any(x => !(x.Player.Data.IsDead || x.Player.Data.Disconnected) && (x.RoleHistory.Count > 0 || x.Is(RoleEnum.Amnesiac) ||
            x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Godfather) || x.Is(RoleEnum.Mafioso) || x.Is(RoleEnum.Thief) || x.Is(RoleEnum.Shifter) || x.Is(RoleEnum.Rebel) ||
            x.Is(RoleEnum.Mystic) || (x.Is(RoleEnum.Seer) && x.Player != Player) || x.Is(RoleEnum.Sidekick) || x.Is(RoleEnum.GuardianAngel) || x.Is(RoleEnum.Executioner) ||
            x.Is(RoleEnum.BountyHunter) || x.Is(RoleEnum.Guesser)));
        public AbilityButton SeerButton;

        public Seer(PlayerControl player) : base(player)
        {
            Name = "Seer";
            Type = RoleEnum.Seer;
            Color = CustomGameOptions.CustomCrewColors ? Colors.Seer : Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            AbilitiesText = "- You can investigate players to see if their roles have changed.\n- If all players whose roles changed have died, you will become a <color=#FFCC80FF>" +
                "Sheriff</color>.";
            InspectorResults = InspectorResults.TouchesPeople;
        }

        public float SeerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastSeered;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.SeerCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void TurnSheriff()
        {
            SeerButton.gameObject.SetActive(false);
            var role = new Sheriff(Player);
            role.RoleUpdate(this);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Color, "Someone has changed their identity!");
        }
    }
}