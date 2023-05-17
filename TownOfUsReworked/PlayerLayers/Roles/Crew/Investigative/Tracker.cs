namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Tracker : CrewRole
    {
        public Dictionary<byte, ArrowBehaviour> TrackerArrows = new();
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
            TrackerArrows = new();
            RoleAlignment = RoleAlignment.CrewInvest;
            InspectorResults = InspectorResults.TracksOthers;
            Type = LayerEnum.Tracker;
            _time = DateTime.UnixEpoch;
            TrackButton = new(this, "Track", AbilityTypes.Direct, "ActionSecondary", Track, Exception, true);
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
            base.OnLobby();
            TrackerArrows.Values.DestroyAll();
            TrackerArrows.Clear();
            ClearPoints();
        }

        public bool Exception(PlayerControl player) => TrackerArrows.ContainsKey(player.PlayerId);

        public void Track()
        {
            if (Utils.IsTooFar(Player, TrackButton.TargetPlayer) || TrackerTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, TrackButton.TargetPlayer);

            if (interact[3])
            {
                var target = TrackButton.TargetPlayer;
                var gameObj = new GameObject("TrackArrow");
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = AssetManager.GetSprite("Arrow");
                renderer.color = target.GetPlayerColor();
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
            TrackButton.Update("TRACK", TrackerTimer(), CustomGameOptions.TrackCd, UsesLeft, ButtonUsable, ButtonUsable);

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

                    arrow.Value.image.color = player.GetPlayerColor();

                    if (_time <= DateTime.UtcNow.AddSeconds(-CustomGameOptions.UpdateInterval))
                        arrow.Value.target = player.transform.position;
                }

                if (_time <= DateTime.UtcNow.AddSeconds(-CustomGameOptions.UpdateInterval))
                    _time = DateTime.UtcNow;
            }
        }

        public override void UpdateMap(MapBehaviour __instance)
        {
            base.UpdateMap(__instance);

            foreach (var pair in TrackerArrows)
            {
                var player = Utils.PlayerById(pair.Key);

                if (!player.Data.IsDead)
                {
                    var v = pair.Value.target;
                    v /= ShipStatus.Instance.MapScale;
                    v.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
                    v.z = -1f;

                    if (Points.ContainsKey(player.PlayerId))
                        Points[player.PlayerId].transform.localPosition = v;
                    else
                    {
                        var point = UObject.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent, true);
                        point.transform.localPosition = v;
                        point.enabled = true;
                        player.SetPlayerMaterialColors(point);
                        Points.Add(player.PlayerId, point);
                    }
                }
            }
        }
    }
}