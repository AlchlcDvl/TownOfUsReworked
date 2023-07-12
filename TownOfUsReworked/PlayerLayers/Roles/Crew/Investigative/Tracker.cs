using static TownOfUsReworked.Languages.Language;
namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Tracker : Crew
    {
        public Dictionary<byte, CustomArrow> TrackerArrows = new();
        public DateTime LastTracked;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public CustomButton TrackButton;

        public Tracker(PlayerControl player) : base(player)
        {
            Name = GetString("Tracker");
            StartText = () => GetString("TrackerStartText");
            AbilitiesText = () => GetString("TrackerAbilitiesText");
            Color = CustomGameOptions.CustomCrewColors ? Colors.Tracker : Colors.Crew;
            RoleType = RoleEnum.Tracker;
            UsesLeft = CustomGameOptions.MaxTracks;
            TrackerArrows = new();
            RoleAlignment = RoleAlignment.CrewInvest;
            InspectorResults = InspectorResults.TracksOthers;
            Type = LayerEnum.Tracker;
            TrackButton = new(this, "Track", AbilityTypes.Direct, "ActionSecondary", Track, Exception, true);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float TrackerTimer()
        {
            var timespan = DateTime.UtcNow - LastTracked;
            var num = Player.GetModifiedCooldown(CustomGameOptions.TrackCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            TrackerArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
            TrackerArrows.Remove(targetPlayerId);
        }

        public override void OnLobby()
        {
            base.OnLobby();
            TrackerArrows.Values.ToList().DestroyAll();
            TrackerArrows.Clear();
        }

        public bool Exception(PlayerControl player) => TrackerArrows.ContainsKey(player.PlayerId);

        public void Track()
        {
            if (Utils.IsTooFar(Player, TrackButton.TargetPlayer) || TrackerTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, TrackButton.TargetPlayer);

            if (interact[3])
            {
                TrackerArrows.Add(TrackButton.TargetPlayer.PlayerId, new(Player, TrackButton.TargetPlayer.GetPlayerColor(), CustomGameOptions.UpdateInterval));
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

            if (IsDead)
                OnLobby();
            else
            {
                foreach (var pair in TrackerArrows)
                {
                    var player = Utils.PlayerById(pair.Key);
                    var body = Utils.BodyById(pair.Key);

                    if (player == null || player.Data.Disconnected || (player.Data.IsDead && body == null))
                    {
                        DestroyArrow(pair.Key);
                        continue;
                    }

                    pair.Value?.Update(player.Data.IsDead ? body.transform.position  : player.transform.position, player.GetPlayerColor());
                }
            }
        }
    }
}