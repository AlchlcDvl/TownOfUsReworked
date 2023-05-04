using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Objects;
using UnityEngine;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Detective : CrewRole
    {
        public DateTime LastExamined;
        public CustomButton ExamineButton;
        private static float Time2;

        public Detective(PlayerControl player) : base(player)
        {
            Name = "Detective";
            StartText = "Examine Players To Find Bloody Hands";
            AbilitiesText = "- You can examine players to see if they have killed recently\n- Your screen will flash red if your target has killed in the " +
                $"last {CustomGameOptions.RecentKill}s\n- You can view everyone's footprints to see where they go or where they came from";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Detective : Colors.Crew;
            RoleType = RoleEnum.Detective;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            InspectorResults = InspectorResults.GainsInfo;
            Type = LayerEnum.Detective;
            ExamineButton = new(this, "Examine", AbilityTypes.Direct, "ActionSecondary", Examine);
        }

        public float ExamineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastExamined;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ExamineCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public override void OnMeetingStart(MeetingHud __instance)
        {
            base.OnMeetingStart(__instance);
            Footprint.DestroyAll(this);
        }

        private static Vector2 Position(PlayerControl player) => player.GetTruePosition() + new Vector2(0, 0.366667f);

        public void Examine()
        {
            if (ExamineTimer() != 0f || Utils.IsTooFar(Player, ExamineButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, ExamineButton.TargetPlayer);

            if (interact[3])
            {
                var hasKilled = ExamineButton.TargetPlayer.IsFramed();

                foreach (var player in Murder.KilledPlayers)
                {
                    if (player.KillerId == ExamineButton.TargetPlayer.PlayerId && (DateTime.UtcNow - player.KillTime).TotalSeconds <= CustomGameOptions.RecentKill)
                        hasKilled = true;
                }

                if (hasKilled)
                    Utils.Flash(new Color32(255, 0, 0, 255));
                else
                    Utils.Flash(new Color32(0, 255, 0, 255));
            }

            if (interact[0])
                LastExamined = DateTime.UtcNow;
            else if (interact[1])
                LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            ExamineButton.Update("EXAMINE", ExamineTimer(), CustomGameOptions.ExamineCd);

            if (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                Time2 += Time.deltaTime;

                if (Time2 >= CustomGameOptions.FootprintInterval)
                {
                    Time2 -= CustomGameOptions.FootprintInterval;

                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player?.Data.IsDead == true || player == PlayerControl.LocalPlayer)
                            continue;

                        if (!AllPrints.Any(print => Vector3.Distance(print.Position, Position(player)) < 0.5f && print.Color.a > 0.5 && print.PlayerId == player.PlayerId))
                            _ = new Footprint(player, this);
                    }

                    for (var i = 0; i < AllPrints.Count; i++)
                    {
                        try
                        {
                            var footprint = AllPrints[i];

                            if (footprint.Update())
                                i--;
                        } catch { /*Assume footprint value is null and allow the loop to continue*/ }
                    }
                }
            }
            else
                OnLobby();
        }
    }
}