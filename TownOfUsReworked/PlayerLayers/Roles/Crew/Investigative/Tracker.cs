namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Tracker : Crew
    {
        public Dictionary<byte, CustomArrow> TrackerArrows { get; set; }
        public DateTime LastTracked { get; set; }
        public int UsesLeft { get; set; }
        public bool ButtonUsable => UsesLeft > 0;
        public CustomButton TrackButton { get; set; }
        public float Timer => ButtonUtils.Timer(Player, LastTracked, CustomGameOptions.TrackCd);

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Tracker : Colors.Crew;
        public override string Name => "Tracker";
        public override LayerEnum Type => LayerEnum.Tracker;
        public override RoleEnum RoleType => RoleEnum.Tracker;
        public override Func<string> StartText => () => "Track Everyone's Movements";
        public override Func<string> Description => () => "- You can track players which creates arrows that update every now and then with the target's position";
        public override InspectorResults InspectorResults => InspectorResults.TracksOthers;

        public Tracker(PlayerControl player) : base(player)
        {
            UsesLeft = CustomGameOptions.MaxTracks;
            TrackerArrows = new();
            RoleAlignment = RoleAlignment.CrewInvest;
            TrackButton = new(this, "Track", AbilityTypes.Direct, "ActionSecondary", Track, Exception, true);
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
            if (IsTooFar(Player, TrackButton.TargetPlayer) || Timer != 0f)
                return;

            var interact = Interact(Player, TrackButton.TargetPlayer);

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
            TrackButton.Update("TRACK", Timer, CustomGameOptions.TrackCd, UsesLeft, ButtonUsable, ButtonUsable);

            if (IsDead)
                OnLobby();
            else
            {
                foreach (var pair in TrackerArrows)
                {
                    var player = PlayerById(pair.Key);
                    var body = BodyById(pair.Key);

                    if (player == null || player.Data.Disconnected || (player.Data.IsDead && body == null))
                        DestroyArrow(pair.Key);
                    else
                        pair.Value?.Update(player.Data.IsDead ? body.transform.position : player.transform.position, player.GetPlayerColor());
                }
            }
        }
    }
}