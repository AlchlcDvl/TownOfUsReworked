namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Medium : Crew
    {
        public DateTime LastMediated;
        public Dictionary<byte, CustomArrow> MediateArrows = new();
        public CustomButton MediateButton;
        //public CustomButton SeanceButton;
        public List<byte> MediatedPlayers = new();

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Medium : Colors.Crew;
        public override string Name => "Medium";
        public override LayerEnum Type => LayerEnum.Medium;
        public override RoleEnum RoleType => RoleEnum.Medium;
        public override Func<string> StartText => () => "<size=80%>Spooky Scary Ghosties Send Shivers Down Your Spine</size>";
        public override Func<string> AbilitiesText => () => "- You can mediate which makes ghosts visible to you" + (CustomGameOptions.ShowMediumToDead == ShowMediumToDead.No ? "" : "\n- "
            + "When mediating, dead players will be able to see you");
        public override InspectorResults InspectorResults => InspectorResults.NewLens;

        public Medium(PlayerControl player) : base(player)
        {
            MediatedPlayers = new();
            MediateArrows = new();
            RoleAlignment = RoleAlignment.CrewInvest;
            MediateButton = new(this, "Mediate", AbilityTypes.Effect, "ActionSecondary", Mediate);
            //SeanceButton = new(this, "Seance", AbilityTypes.Effect, "ActionSecondary", Seance, false, true);
        }

        public float MediateTimer()
        {
            var timespan = DateTime.UtcNow - LastMediated;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MediateCooldown) * 1000f;
            var time = num - (float)timespan.TotalMilliseconds;
            var flag2 = time < 0f;
            return (flag2 ? 0f : time) / 1000f;
        }

        //private static void Seance() { Currently blank, gonna work on this later }

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
            //SeanceButton.Update("SEANCE", MediateTimer(), CustomGameOptions.MediateCooldown, true, false);

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
                            player.SetOutfit(CustomPlayerOutfitType.Camouflage, BlankOutfit(player));
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
            var PlayersDead = KilledPlayers.GetRange(0, KilledPlayers.Count);

            if (PlayersDead.Count == 0)
                return;

            if (CustomGameOptions.DeadRevealed != DeadRevealed.Random)
            {
                if (CustomGameOptions.DeadRevealed == DeadRevealed.Newest)
                    PlayersDead.Reverse();

                foreach (var dead in PlayersDead)
                {
                    if (AllBodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
                    {
                        MediateArrows.Add(dead.PlayerId, new(Player, Color));
                        MediatedPlayers.Add(dead.PlayerId);
                        CallRpc(CustomRPC.Action, ActionsRPC.Mediate, this, dead.PlayerId);

                        if (CustomGameOptions.DeadRevealed != DeadRevealed.All)
                            break;
                    }
                }
            }
            else
            {
                var dead = PlayersDead.Random();

                if (AllBodies.Any(x => x.ParentId == dead.PlayerId && !MediateArrows.ContainsKey(x.ParentId)))
                {
                    MediateArrows.Add(dead.PlayerId, new(Player, Color));
                    MediatedPlayers.Add(dead.PlayerId);
                    CallRpc(CustomRPC.Action, ActionsRPC.Mediate, PlayerId, dead.PlayerId);
                }
            }
        }
    }
}