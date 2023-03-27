using System;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Grenadier : IntruderRole
    {
        public AbilityButton FlashButton;
        public bool Enabled;
        public DateTime LastFlashed;
        public float TimeRemaining;
        private static List<PlayerControl> ClosestPlayers = new();
        private static readonly Color NormalVision = new Color32(212, 212, 212, 0);
        private static readonly Color DimVision = new Color32(212, 212, 212, 51);
        private static readonly Color BlindVision = new Color32(212, 212, 212, 255);
        public List<PlayerControl> FlashedPlayers = new();
        public bool Flashed => TimeRemaining > 0f;

        public Grenadier(PlayerControl player) : base(player)
        {
            Name = "Grenadier";
            StartText = "Blind The <color=#8BFDFDFF>Crew</color> With Your Magnificent Figure";
            AbilitiesText = "- You can drop a flashbang, which blinds players around you.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Grenadier : Colors.Intruder;
            LastFlashed = DateTime.UtcNow;
            RoleType = RoleEnum.Grenadier;
            RoleAlignment = RoleAlignment.IntruderConceal;
            AlignmentName ="Intruder (Concealing)";
            InspectorResults = InspectorResults.DropsItems;
            ClosestPlayers = new();
            FlashedPlayers = new List<PlayerControl>();
        }

        public float FlashTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastFlashed;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.GrenadeCd, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Flash()
        {
            if (!Enabled)
            {
                ClosestPlayers = Utils.GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.FlashRadius);
                FlashedPlayers = ClosestPlayers;
            }

            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0f;

            //To stop the scenario where the flash and sabotage are called at the same time.
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var dummyActive = system.dummy.IsActive;
            var sabActive = system.specials.ToArray().Any(s => s.IsActive);

            if (sabActive || dummyActive)
                return;

            foreach (var player in ClosestPlayers)
            {
                if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
                {
                    HudManager.Instance.FullScreen.enabled = true;
                    HudManager.Instance.FullScreen.gameObject.active = true;

                    if (TimeRemaining > CustomGameOptions.GrenadeDuration - 0.5f)
                    {
                        float fade = (TimeRemaining - CustomGameOptions.GrenadeDuration) * (-2f);

                        if (ShouldPlayerBeBlinded(player))
                            HudManager.Instance.FullScreen.color = Color32.Lerp(NormalVision, BlindVision, fade);
                        else if (ShouldPlayerBeDimmed(player))
                            HudManager.Instance.FullScreen.color = Color32.Lerp(NormalVision, DimVision, fade);
                        else
                            HudManager.Instance.FullScreen.color = NormalVision;
                    }
                    else if (TimeRemaining <= (CustomGameOptions.GrenadeDuration - 0.5f) && TimeRemaining >= 0.5f)
                    {
                        HudManager.Instance.FullScreen.enabled = true;
                        HudManager.Instance.FullScreen.gameObject.active = true;

                        if (ShouldPlayerBeBlinded(player))
                            HudManager.Instance.FullScreen.color = BlindVision;
                        else if (ShouldPlayerBeDimmed(player))
                            HudManager.Instance.FullScreen.color = DimVision;
                        else
                            HudManager.Instance.FullScreen.color = NormalVision;
                    }
                    else if (TimeRemaining < 0.5f)
                    {
                        float fade2 = (TimeRemaining * -2.0f) + 1.0f;

                        if (ShouldPlayerBeBlinded(player))
                            HudManager.Instance.FullScreen.color = Color32.Lerp(BlindVision, NormalVision, fade2);
                        else if (ShouldPlayerBeDimmed(player))
                            HudManager.Instance.FullScreen.color = Color32.Lerp(DimVision, NormalVision, fade2);
                        else
                            HudManager.Instance.FullScreen.color = NormalVision;
                    }
                    else
                    {
                        HudManager.Instance.FullScreen.color = NormalVision;
                        TimeRemaining = 0f;
                    }

                    if (MapBehaviour.Instance)
                        MapBehaviour.Instance.Close();

                    if (Minigame.Instance)
                        Minigame.Instance.Close();
                }
            }
        }

        private static bool ShouldPlayerBeDimmed(PlayerControl player) => (player.Is(Faction.Intruder) || player.Data.IsDead) && !MeetingHud.Instance;

        private static bool ShouldPlayerBeBlinded(PlayerControl player) => !(player.Is(Faction.Intruder) || player.Data.IsDead || MeetingHud.Instance);

        public void UnFlash()
        {
            Enabled = false;
            LastFlashed = DateTime.UtcNow;
            HudManager.Instance.FullScreen.enabled = true;
            HudManager.Instance.FullScreen.color = NormalVision;
            FlashedPlayers.Clear();
        }
    }
}