namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Medium : CrewRole
    {
        public DateTime LastMediated;
        public Dictionary<byte, ArrowBehaviour> MediatedPlayers = new();
        public CustomButton MediateButton;
        public CustomButton SeanceButton;

        public Medium(PlayerControl player) : base(player)
        {
            Name = "Medium";
            StartText = "Spooky Scary Ghosties Send Shivers Down Your Spine";
            AbilitiesText = "- You can mediate which makes ghosts visible to you" + (CustomGameOptions.ShowMediumToDead ? "\n- When mediating, dead players will be able to see " +
                "you" : "");
            Color = CustomGameOptions.CustomCrewColors ? Colors.Medium : Colors.Crew;
            RoleType = RoleEnum.Medium;
            MediatedPlayers = new();
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            InspectorResults = InspectorResults.NewLens;
            Type = LayerEnum.Medium;
            MediateButton = new(this, "Mediate", AbilityTypes.Effect, "ActionSecondary", Mediate);
            SeanceButton = new(this, "Seance", AbilityTypes.Effect, "ActionSecondary", Seance, false, true);
        }

        public float MediateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMediated;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MediateCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        private static void Seance() { /* Currently blank, gonna work on this later */ }

        public void AddMediatePlayer(byte playerId)
        {
            var gameObj = new GameObject("MediumArrow");
            var arrow = gameObj.AddComponent<ArrowBehaviour>();

            if (Player == PlayerControl.LocalPlayer || CustomGameOptions.ShowMediumToDead)
            {
                gameObj.transform.parent = Player.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = AssetManager.GetSprite("Arrow");
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = Utils.PlayerById(playerId).transform.position;
                Utils.Flash(Color);
            }

            MediatedPlayers.Add(playerId, arrow);
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = MediatedPlayers.FirstOrDefault(x => x.Key == targetPlayerId);
            arrow.Value?.Destroy();
            arrow.Value.gameObject?.Destroy();
            MediatedPlayers.Remove(arrow.Key);
        }

        public override void OnLobby()
        {
            base.OnLobby();
            MediatedPlayers.Values.DestroyAll();
            MediatedPlayers.Clear();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            MediateButton.Update("MEDIATE", MediateTimer(), CustomGameOptions.MediateCooldown);
            SeanceButton.Update("SEANCE", MediateTimer(), CustomGameOptions.MediateCooldown, true, false);

            if (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (MediatedPlayers.ContainsKey(player.PlayerId))
                    {
                        MediatedPlayers.GetValueSafe(player.PlayerId).target = player.transform.position;
                        player.Visible = true;

                        if (!CustomGameOptions.ShowMediatePlayer)
                        {
                            player.SetOutfit(CustomPlayerOutfitType.Camouflage, new GameData.PlayerOutfit()
                            {
                                ColorId = player.GetDefaultOutfit().ColorId,
                                HatId = "",
                                SkinId = "",
                                VisorId = "",
                                PlayerName = " "
                            });

                            PlayerMaterial.SetColors(new Color32(128, 128, 128, 255), player.MyRend());
                        }
                    }
                }
            }
        }

        public void Mediate()
        {
            if (MediateTimer() != 0f)
                return;

            LastMediated = DateTime.UtcNow;
            var PlayersDead = Murder.KilledPlayers.GetRange(0, Murder.KilledPlayers.Count);

            if (PlayersDead.Count == 0)
                return;

            if (CustomGameOptions.DeadRevealed != DeadRevealed.Random)
            {
                if (CustomGameOptions.DeadRevealed == DeadRevealed.Newest)
                    PlayersDead.Reverse();

                foreach (var dead in PlayersDead)
                {
                    if (UObject.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == dead.PlayerId && !MediatedPlayers.ContainsKey(x.ParentId)))
                    {
                        AddMediatePlayer(dead.PlayerId);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.Mediate);
                        writer.Write(dead.PlayerId);
                        writer.Write(PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        if (CustomGameOptions.DeadRevealed != DeadRevealed.All)
                            break;
                    }
                }
            }
            else
            {
                PlayersDead.Shuffle();
                var dead = PlayersDead.Random();

                if (UObject.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == dead.PlayerId && !MediatedPlayers.ContainsKey(x.ParentId)))
                {
                    AddMediatePlayer(dead.PlayerId);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Mediate);
                    writer.Write(dead.PlayerId);
                    writer.Write(PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}