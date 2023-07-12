using static TownOfUsReworked.Languages.Language;
namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Medium : Crew
    {
        public DateTime LastMediated;
        public Dictionary<byte, CustomArrow> MediateArrows = new();
        public CustomButton MediateButton;
        public CustomButton SeanceButton;
        public List<byte> MediatedPlayers = new();

        public Medium(PlayerControl player) : base(player)
        {
            Name = GetString("Medium");
            StartText = () => GetString("MediumStartText");
            AbilitiesText = () => GetString("MediumAbilitiesText1") + (CustomGameOptions.ShowMediumToDead != ShowMediumToDead.No ?
                GetString("MediumAbilitiesText2") : "");
            Color = CustomGameOptions.CustomCrewColors ? Colors.Medium : Colors.Crew;
            RoleType = RoleEnum.Medium;
            MediatedPlayers = new();
            MediateArrows = new();
            RoleAlignment = RoleAlignment.CrewInvest;
            InspectorResults = InspectorResults.NewLens;
            Type = LayerEnum.Medium;
            MediateButton = new(this, "Mediate", AbilityTypes.Effect, "ActionSecondary", Mediate);
            SeanceButton = new(this, "Seance", AbilityTypes.Effect, "ActionSecondary", Seance, false, true);

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float MediateTimer()
        {
            var timespan = DateTime.UtcNow - LastMediated;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MediateCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        private static void Seance() { /* Currently blank, gonna work on this later */ }

        public override void OnLobby()
        {
            base.OnLobby();
            MediateArrows.Values.ToList().DestroyAll();
            MediateArrows.Clear();
            MediatedPlayers.Clear();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            MediateButton.Update("MEDIATE", MediateTimer(), CustomGameOptions.MediateCooldown);
            SeanceButton.Update("SEANCE", MediateTimer(), CustomGameOptions.MediateCooldown, true, false);

            if (!IsDead)
            {
                foreach (var player in CustomPlayer.AllPlayers)
                {
                    if (MediateArrows.ContainsKey(player.PlayerId))
                    {
                        MediateArrows[player.PlayerId]?.Update(player.transform.position, player.GetPlayerColor(false, CustomGameOptions.ShowMediatePlayer));
                        player.Visible = true;

                        if (!CustomGameOptions.ShowMediatePlayer)
                        {
                            player.SetOutfit(CustomPlayerOutfitType.Camouflage, Utils.BlankOutfit(player));
                            PlayerMaterial.SetColors(UColor.grey, player.MyRend());
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
            var PlayersDead = Utils.KilledPlayers.GetRange(0, Utils.KilledPlayers.Count);

            if (PlayersDead.Count == 0)
                return;

            if (CustomGameOptions.DeadRevealed != DeadRevealed.Random)
            {
                if (CustomGameOptions.DeadRevealed == DeadRevealed.Newest)
                    PlayersDead.Reverse();

                foreach (var dead in PlayersDead)
                {
                    if (Utils.AllBodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
                    {
                        MediateArrows.Add(dead.PlayerId, new(Player, Color));
                        MediatedPlayers.Add(dead.PlayerId);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.Mediate);
                        writer.Write(PlayerId);
                        writer.Write(dead.PlayerId);
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

                if (Utils.AllBodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
                {
                    MediateArrows.Add(dead.PlayerId, new(Player, Color));
                    MediatedPlayers.Add(dead.PlayerId);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Mediate);
                    writer.Write(PlayerId);
                    writer.Write(dead.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}