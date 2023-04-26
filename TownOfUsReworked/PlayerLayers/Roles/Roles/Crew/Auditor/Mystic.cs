using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;
using System.Linq;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Custom;
using UnityEngine;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Mystic : CrewRole
    {
        public DateTime LastRevealed;
        public static bool ConvertedDead => !PlayerControl.AllPlayerControls.ToArray().Any(x => x?.Data.IsDead == false && !x.Data.Disconnected && !x.Is(SubFaction.None));
        public CustomButton RevealButton;

        public Mystic(PlayerControl player) : base(player)
        {
            Name = "Mystic";
            RoleType = RoleEnum.Mystic;
            Color = CustomGameOptions.CustomCrewColors ? Colors.Mystic : Colors.Crew;
            RoleAlignment = RoleAlignment.CrewAudit;
            AlignmentName = CA;
            StartText = "You Know When Converts Happen";
            AbilitiesText = "- You can investigate players to see if they have been converted\n- Whenever someone has been converted, you will be alerted to it\n- When all converted" +
                " and converters die, you will become a <color=#71368AFF>Seer</color>";
            InspectorResults = InspectorResults.TracksOthers;
            Type = LayerEnum.Mystic;
            RevealButton = new(this, "Reveal", AbilityTypes.Direct, "ActionSecondary", Reveal);
        }

        public float RevealTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastRevealed;
            var num = Player.GetModifiedCooldown(CustomGameOptions.RevealCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void TurnSeer()
        {
            var newRole = new Seer(Player);
            newRole.RoleUpdate(this);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
                Utils.Flash(Colors.Seer);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            RevealButton.Update("REVEAL", RevealTimer(), CustomGameOptions.RevealCooldown);

            if (ConvertedDead && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                TurnSeer();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnSeer);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        public void Reveal()
        {
            if (RevealTimer() != 0f || Utils.IsTooFar(Player, RevealButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, RevealButton.TargetPlayer);

            if (interact[3])
            {
                if ((!RevealButton.TargetPlayer.Is(SubFaction.None) && !RevealButton.TargetPlayer.Is(RoleAlignment.NeutralNeo)) || RevealButton.TargetPlayer.IsFramed())
                    Utils.Flash(new Color32(255, 0, 0, 255));
                else
                    Utils.Flash(new Color32(0, 255, 0, 255));
            }

            if (interact[0])
                LastRevealed = DateTime.UtcNow;
            else if (interact[1])
                LastRevealed.AddSeconds(CustomGameOptions.ProtectKCReset);
        }
    }
}