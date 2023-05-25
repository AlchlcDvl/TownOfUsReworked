namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Stalker : SyndicateRole
    {
        public Dictionary<byte, CustomArrow> StalkerArrows = new();
        public DateTime LastStalked;
        public CustomButton StalkButton;

        public Stalker(PlayerControl player) : base(player)
        {
            Name = "Stalker";
            StartText = "Stalk Everyone To Monitor Their Movements";
            AbilitiesText = $"- You always know where your targets are\n{AbilitiesText}";
            Color = CustomGameOptions.CustomSynColors ? Colors.Stalker : Colors.Syndicate;
            RoleType = RoleEnum.Stalker;
            StalkerArrows = new();
            RoleAlignment = RoleAlignment.SyndicateSupport;
            InspectorResults = InspectorResults.TracksOthers;
            Type = LayerEnum.Stalker;
            StalkButton = new(this, "Stalk", AbilityTypes.Direct, "ActionSecondary", Stalk, Exception1);
        }

        public float StalkTimer()
        {
            var timespan = DateTime.UtcNow - LastStalked;
            var num = Player.GetModifiedCooldown(CustomGameOptions.StalkCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = StalkerArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            arrow.Value?.Destroy();
            StalkerArrows.Remove(arrow.Key);
        }

        public override void OnLobby()
        {
            base.OnLobby();
            StalkerArrows.Values.ToList().DestroyAll();
            StalkerArrows.Clear();
        }

        public void Stalk()
        {
            if (Utils.IsTooFar(Player, StalkButton.TargetPlayer) || StalkTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, StalkButton.TargetPlayer);

            if (interact[3])
                StalkerArrows.Add(StalkButton.TargetPlayer.PlayerId, new(Player, StalkButton.TargetPlayer.GetPlayerColor(!HoldsDrive)));

            if (interact[0])
                LastStalked = DateTime.UtcNow;
            else if (interact[1])
                LastStalked.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public bool Exception1(PlayerControl player) => StalkerArrows.ContainsKey(player.PlayerId);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            StalkButton.Update("STALK", StalkTimer(), CustomGameOptions.StalkCd, true, !HoldsDrive);

            if (IsDead)
                OnLobby();
            else
            {
                foreach (var pair in StalkerArrows)
                {
                    var player = Utils.PlayerById(pair.Key);
                    var body = Utils.BodyById(pair.Key);

                    if (player.Data.Disconnected || (player.Data.IsDead && !body))
                    {
                        DestroyArrow(pair.Key);
                        continue;
                    }

                    pair.Value.Update(player.Data.IsDead ? player.GetTruePosition() : body.TruePosition, player.GetPlayerColor(!HoldsDrive));
                }

                if (HoldsDrive)
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (!StalkerArrows.ContainsKey(player.PlayerId))
                            StalkerArrows.Add(player.PlayerId, new(Player, player.GetPlayerColor(false)));
                    }
                }
            }
        }
    }
}