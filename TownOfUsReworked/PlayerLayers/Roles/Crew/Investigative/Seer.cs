using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;
using System.Linq;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Custom;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Seer : CrewRole
    {
        public DateTime LastSeered;
        public bool ChangedDead => !AllRoles.Any(x => !x.IsDead && !x.Disconnected && (x.RoleHistory.Count > 0 || x.Is(RoleEnum.Amnesiac) ||
            x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Godfather) || x.Is(RoleEnum.Mafioso) || x.Is(RoleEnum.Thief) || x.Is(RoleEnum.Shifter) || x.Is(RoleEnum.Rebel) ||
            x.Is(RoleEnum.Mystic) || (x.Is(RoleEnum.Seer) && x != this) || x.Is(RoleEnum.Sidekick) || x.Is(RoleEnum.GuardianAngel) || x.Is(RoleEnum.Executioner) ||
            x.Is(RoleEnum.BountyHunter) || x.Is(RoleEnum.Guesser) || x.Player.Is(ObjectifierEnum.Traitor) || x.Player.Is(ObjectifierEnum.Fanatic)));
        public CustomButton SeerButton;

        public Seer(PlayerControl player) : base(player)
        {
            Name = "Seer";
            RoleType = RoleEnum.Seer;
            Color = CustomGameOptions.CustomCrewColors ? Colors.Seer : Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            AbilitiesText = "- You can investigate players to see if their roles have changed\n- If all players whose roles changed have died, you will become a <color=#FFCC80FF>" +
                "Sheriff</color>";
            InspectorResults = InspectorResults.GainsInfo;
            Type = LayerEnum.Seer;
            SeerButton = new(this, "Seer", AbilityTypes.Direct, "ActionSecondary", See);
        }

        public float SeerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastSeered;
            var num = Player.GetModifiedCooldown(CustomGameOptions.SeerCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void TurnSheriff()
        {
            var role = new Sheriff(Player);
            role.RoleUpdate(this);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Color);
        }

        public void See()
        {
            if (SeerTimer() != 0f || Utils.IsTooFar(Player, SeerButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, SeerButton.TargetPlayer);

            if (interact[3])
            {
                if (GetRole(SeerButton.TargetPlayer).RoleHistory.Count > 0 || SeerButton.TargetPlayer.IsFramed())
                    Utils.Flash(new Color32(255, 0, 0, 255));
                else
                    Utils.Flash(new Color32(0, 255, 0, 255));
            }

            if (interact[0])
                LastSeered = DateTime.UtcNow;
            else if (interact[1])
                LastSeered.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            SeerButton.Update("SEE", SeerTimer(), CustomGameOptions.SeerCooldown);
        }
    }
}