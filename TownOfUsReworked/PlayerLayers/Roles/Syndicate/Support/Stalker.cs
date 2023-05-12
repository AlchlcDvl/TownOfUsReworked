namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Stalker : SyndicateRole
    {
        public Dictionary<byte, ArrowBehaviour> StalkerArrows = new();
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
            AlignmentName = SSu;
            InspectorResults = InspectorResults.TracksOthers;
            Type = LayerEnum.Stalker;
            StalkButton = new(this, "Stalk", AbilityTypes.Direct, "ActionSecondary", Stalk, Exception1);
        }

        public float StalkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastStalked;
            var num = Player.GetModifiedCooldown(CustomGameOptions.StalkCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = StalkerArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            arrow.Value?.Destroy();
            arrow.Value.gameObject?.Destroy();
            StalkerArrows.Remove(arrow.Key);
        }

        public override void OnLobby()
        {
            base.OnLobby();
            StalkerArrows.Values.DestroyAll();
            StalkerArrows.Clear();
            ClearPoints();
        }

        public void Stalk()
        {
            if (Utils.IsTooFar(Player, StalkButton.TargetPlayer) || StalkTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, StalkButton.TargetPlayer);

            if (interact[3])
            {
                var target = StalkButton.TargetPlayer;
                var gameObj = new GameObject("StalkArrow");
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = AssetManager.GetSprite("Arrow");

                if (DoUndo.IsCamoed && !HoldsDrive)
                    renderer.color = Palette.PlayerColors[6];
                else if (ColorUtils.IsRainbow(target.GetDefaultOutfit().ColorId))
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
                StalkerArrows.Add(target.PlayerId, arrow);
            }

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
                foreach (var arrow in StalkerArrows)
                {
                    var player = Utils.PlayerById(arrow.Key);

                    #pragma warning disable
                    if (player == null || player.Data.IsDead || player.Data.Disconnected)
                    {
                        DestroyArrow(arrow.Key);
                        continue;
                    }
                    #pragma warning restore

                    if (DoUndo.IsCamoed && !HoldsDrive)
                        arrow.Value.image.color = Palette.PlayerColors[6];
                    else if (ColorUtils.IsRainbow(player.GetDefaultOutfit().ColorId))
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

                    arrow.Value.target = player.transform.position;
                }

                if (HoldsDrive)
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (!StalkerArrows.ContainsKey(player.PlayerId))
                        {
                            var target = player;
                            var gameObj = new GameObject("StalkArrow");
                            var arrow = gameObj.AddComponent<ArrowBehaviour>();
                            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                            var renderer = gameObj.AddComponent<SpriteRenderer>();
                            renderer.sprite = AssetManager.GetSprite("Arrow");

                            if (DoUndo.IsCamoed && !HoldsDrive)
                                renderer.color = Palette.PlayerColors[6];
                            else if (ColorUtils.IsRainbow(target.GetDefaultOutfit().ColorId))
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
                            StalkerArrows.Add(target.PlayerId, arrow);
                        }
                    }
                }
            }
        }

        public override void UpdateMap(MapBehaviour __instance)
        {
            base.UpdateMap(__instance);

            if (IsDead || MeetingHud.Instance)
                return;

            foreach (var pair in StalkerArrows)
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