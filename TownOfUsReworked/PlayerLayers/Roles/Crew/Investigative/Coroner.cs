namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Coroner : Crew
    {
        public Dictionary<byte, CustomArrow> BodyArrows { get; set; }
        public List<byte> Reported { get; set; }
        public CustomButton CompareButton { get; set; }
        public List<DeadPlayer> ReferenceBodies { get; set; }
        public DateTime LastCompared { get; set; }
        public DateTime LastAutopsied { get; set; }
        public CustomButton AutopsyButton { get; set; }
        public float AutopsyTimer => ButtonUtils.Timer(Player, LastAutopsied, CustomGameOptions.AutopsyCooldown);
        public float CompareTimer => ButtonUtils.Timer(Player, LastCompared, CustomGameOptions.CompareCooldown);

        public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Coroner : Colors.Crew;
        public override string Name => "Coroner";
        public override LayerEnum Type => LayerEnum.Coroner;
        public override RoleEnum RoleType => RoleEnum.Coroner;
        public override Func<string> StartText => () => "Examine The Dead For Information";
        public override Func<string> Description => () => "- You know when players die and will be notified to as to where their body is for a brief period of time\n- You will get a " +
            "report when you report a body\n- You can perform an autopsy on bodies, to get a reference\n- You can compare the autopsy reference with players to see if they killed the body "
            + "you examined";
        public override InspectorResults InspectorResults => InspectorResults.DealsWithDead;

        public Coroner(PlayerControl player) : base(player)
        {
            RoleAlignment = RoleAlignment.CrewInvest;
            BodyArrows = new();
            Reported = new();
            ReferenceBodies = new();
            AutopsyButton = new(this, "Autopsy", AbilityTypes.Dead, "ActionSecondary", Autopsy);
            CompareButton = new(this, "Compare", AbilityTypes.Direct, "Secondary", Compare);
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
            BodyArrows.Remove(targetPlayerId);
        }

        public override void OnLobby()
        {
            base.OnLobby();
            BodyArrows.Values.ToList().DestroyAll();
            BodyArrows.Clear();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            AutopsyButton.Update("AUTOPSY", AutopsyTimer, CustomGameOptions.AutopsyCooldown);
            CompareButton.Update("COMPARE", CompareTimer, CustomGameOptions.CompareCooldown, true, ReferenceBodies.Count > 0);

            if (!CustomPlayer.LocalCustom.IsDead)
            {
                var validBodies = AllBodies.Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && DateTime.UtcNow < y.KillTime.AddSeconds(CustomGameOptions.CoronerArrowDuration)));

                foreach (var bodyArrow in BodyArrows.Keys)
                {
                    if (!validBodies.Any(x => x.ParentId == bodyArrow))
                        DestroyArrow(bodyArrow);
                }

                foreach (var body in validBodies)
                {
                    if (!BodyArrows.ContainsKey(body.ParentId))
                        BodyArrows.Add(body.ParentId, new(Player, Color));

                    BodyArrows[body.ParentId]?.Update(body.TruePosition);
                }
            }
            else if (BodyArrows.Count != 0)
                OnLobby();
        }

        public void Autopsy()
        {
            if (IsTooFar(Player, AutopsyButton.TargetBody) || AutopsyTimer != 0f)
                return;

            var playerId = AutopsyButton.TargetBody.ParentId;
            var player = PlayerById(playerId);
            Spread(Player, player);
            var matches = KilledPlayers.Where(x => x.PlayerId == playerId).ToArray();
            DeadPlayer killed = null;

            if (matches.Length > 0)
                killed = matches[0];

            if (killed == null)
                Flash(new(255, 0, 0, 255));
            else
            {
                ReferenceBodies.Add(killed);
                LastAutopsied = DateTime.UtcNow;
                Flash(Color);
            }
        }

        public void Compare()
        {
            if (ReferenceBodies.Count == 0 || IsTooFar(Player, CompareButton.TargetPlayer) || CompareTimer != 0f)
                return;

            var interact = Interact(Player, CompareButton.TargetPlayer);

            if (interact[3])
            {
                if (ReferenceBodies.Any(x => CompareButton.TargetPlayer.PlayerId == x.KillerId) || CompareButton.TargetPlayer.IsFramed())
                    Flash(new(255, 0, 0, 255));
                else
                    Flash(new(0, 255, 0, 255));
            }

            if (interact[0])
                LastCompared = DateTime.UtcNow;
            else if (interact[1])
                LastCompared.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public override void OnBodyReport(GameData.PlayerInfo info)
        {
            base.OnBodyReport(info);

            if (info == null || !Local)
                return;

            var body = KilledPlayers.Find(x => x.PlayerId == info.PlayerId);

            if (body == null)
                return;

            Reported.Add(info.PlayerId);
            body.Reporter = Player;
            body.KillAge = (float)(DateTime.UtcNow - body.KillTime).TotalMilliseconds;
            var reportMsg = body.ParseBodyReport();

            if (string.IsNullOrWhiteSpace(reportMsg))
                return;

            //Only Coroner can see this
            if (HUD)
                HUD.Chat.AddChat(CustomPlayer.Local, reportMsg);
        }
    }
}