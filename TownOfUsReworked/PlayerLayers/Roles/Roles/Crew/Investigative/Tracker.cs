using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using UnityEngine;
using TownOfUsReworked.Cosmetics;
using Reactor.Utilities.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Tracker : CrewRole
    {
        public Dictionary<byte, ArrowBehaviour> TrackerArrows;
        public DateTime LastTracked;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public CustomButton TrackButton;
        private static DateTime _time = DateTime.UnixEpoch;

        public Tracker(PlayerControl player) : base(player)
        {
            Name = "Tracker";
            StartText = "Stalk Everyone To Monitor Their Movements";
            AbilitiesText = "- You can track players which creates arrows that update every now and then";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Tracker : Colors.Crew;
            RoleType = RoleEnum.Tracker;
            UsesLeft = CustomGameOptions.MaxTracks;
            TrackerArrows = new Dictionary<byte, ArrowBehaviour>();
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            InspectorResults = InspectorResults.TracksOthers;
            Type = LayerEnum.Tracker;
            TrackButton = new(this, "Track", AbilityTypes.Direct, "ActionSecondary", Track, true);
        }

        public float TrackerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastTracked;
            var num = Player.GetModifiedCooldown(CustomGameOptions.TrackCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = TrackerArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            arrow.Value?.Destroy();
            arrow.Value.gameObject?.Destroy();
            TrackerArrows.Remove(arrow.Key);
        }

        public override void OnLobby()
        {
            TrackerArrows.Values.DestroyAll();
            TrackerArrows.Clear();
        }

        public void Track()
        {
            if (Utils.IsTooFar(Player, TrackButton.TargetPlayer) || TrackerTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, TrackButton.TargetPlayer);

            if (interact[3])
            {
                var target = TrackButton.TargetPlayer;
                var gameObj = new GameObject();
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = AssetManager.GetSprite("Arrow");

                if (DoUndo.IsCamoed)
                    renderer.color = new Color32(128, 128, 128, 255);
                if (ColorUtils.IsRainbow(target.GetDefaultOutfit().ColorId))
                    renderer.color = ColorUtils.Rainbow;
                else if (ColorUtils.IsChroma(target.GetDefaultOutfit().ColorId))
                    renderer.color = ColorUtils.Chroma;
                else if (ColorUtils.IsMonochrome(target.GetDefaultOutfit().ColorId))
                    renderer.color = ColorUtils.Monochrome;
                else if (ColorUtils.IsMantle(target.GetDefaultOutfit().ColorId))
                    renderer.color = ColorUtils.Mantle;
                else if (ColorUtils.IsFire(target.GetDefaultOutfit().ColorId))
                    renderer.color = ColorUtils.Fire;
                else if (ColorUtils.IsGalaxy(target.GetDefaultOutfit().ColorId))
                    renderer.color = ColorUtils.Galaxy;
                else
                    renderer.color = Palette.PlayerColors[target.GetDefaultOutfit().ColorId];

                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = target.transform.position;
                TrackerArrows.Add(target.PlayerId, arrow);
                UsesLeft--;
            }

            if (interact[0])
                LastTracked = DateTime.UtcNow;
            else if (interact[1])
                LastTracked.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var notTracked = PlayerControl.AllPlayerControls.ToArray().Where(x => !TrackerArrows.ContainsKey(x.PlayerId)).ToList();
            TrackButton.Update("Track", TrackerTimer(), CustomGameOptions.TrackCd, UsesLeft, notTracked, ButtonUsable);

            if (PlayerControl.LocalPlayer.Data.IsDead)
                OnLobby();
            else
            {
                foreach (var arrow in TrackerArrows)
                {
                    var player = Utils.PlayerById(arrow.Key);

                    #pragma warning disable
                    if (player == null || player.Data.IsDead || player.Data.Disconnected)
                    {
                        DestroyArrow(arrow.Key);
                        continue;
                    }
                    #pragma warning restore

                    if (DoUndo.IsCamoed)
                        arrow.Value.image.color = new Color32(128, 128, 128, 255);
                    if (ColorUtils.IsRainbow(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Rainbow;
                    else if (ColorUtils.IsChroma(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Chroma;
                    else if (ColorUtils.IsMonochrome(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Monochrome;
                    else if (ColorUtils.IsMantle(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Mantle;
                    else if (ColorUtils.IsFire(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Fire;
                    else if (ColorUtils.IsGalaxy(player.GetDefaultOutfit().ColorId))
                        arrow.Value.image.color = ColorUtils.Galaxy;
                    else
                        arrow.Value.image.color = Palette.PlayerColors[player.GetDefaultOutfit().ColorId];

                    if (_time <= DateTime.UtcNow.AddSeconds(-CustomGameOptions.UpdateInterval))
                        arrow.Value.target = player.transform.position;
                }

                if (_time <= DateTime.UtcNow.AddSeconds(-CustomGameOptions.UpdateInterval))
                    _time = DateTime.UtcNow;
            }
        }
    }
}