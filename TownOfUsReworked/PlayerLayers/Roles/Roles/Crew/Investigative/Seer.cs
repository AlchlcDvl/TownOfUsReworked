using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;
using Reactor.Utilities;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Seer : CrewRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastSeered;
        public bool ChangedDead => AllRoles.Where(x => !(x.Player.Data.IsDead || x.Player.Data.Disconnected) && (x.RoleHistory.Count > 0 || x.Is(RoleEnum.Amnesiac) ||
            x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Godfather) || x.Is(RoleEnum.Mafioso) || x.Is(RoleEnum.Thief) || x.Is(RoleEnum.Shifter) || x.Is(RoleEnum.Rebel) ||
            x.Is(RoleEnum.Mystic) || (x.Is(RoleEnum.Seer) && x.Player != Player) || x.Is(RoleEnum.Sidekick) || x.Is(RoleEnum.GuardianAngel))).Count() == 0;
        public AbilityButton SeerButton;

        public Seer(PlayerControl player) : base(player)
        {
            Name = "Seer";
            RoleType = RoleEnum.Seer;
            Color = CustomGameOptions.CustomCrewColors ? Colors.Seer : Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            AbilitiesText = "- You can investigate players to see if their roles have changed.\n- If all players whose roles changed have died, you will become a <color=#FFCC80FF>" +
                "Sheriff</color>.";
            RoleDescription = "You are a Seer! You have the power to view a player's background and know if they changed their positions! Use this power to find all unsavoury people!";
            InspectorResults = InspectorResults.TouchesPeople;
        }

        public float SeerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastSeered;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.SeerCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void TurnSheriff()
        {
            var seer = Role.GetRole<Seer>(Player);
            var role = new Sheriff(Player);
            role.RoleUpdate(seer);
            Player.RegenTask();

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Coroutines.Start(Utils.FlashCoroutine(Colors.Seer));
        }
    }
}