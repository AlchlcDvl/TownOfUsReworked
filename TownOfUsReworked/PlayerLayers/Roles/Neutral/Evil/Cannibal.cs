namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Cannibal : NeutralRole
    {
        public CustomButton EatButton;
        public int EatNeed;
        public bool Eaten;
        public DateTime LastEaten;
        public Dictionary<byte, CustomArrow> BodyArrows = new();
        public bool EatWin => EatNeed == 0;

        public Cannibal(PlayerControl player) : base(player)
        {
            Name = "Cannibal";
            StartText = () => "Eat The Bodies Of The Dead";
            RoleType = RoleEnum.Cannibal;
            AbilitiesText = () => "- You can consume a body, making it disappear from the game" + (CustomGameOptions.EatArrows ? "\n- When someone dies, you get an arrow pointing to their"
                + " body" : "");
            RoleAlignment = RoleAlignment.NeutralEvil;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Cannibal : Colors.Neutral;
            Objectives = () => $"- Eat {EatNeed} {(EatNeed == 1 ? "body" : "bodies")}";
            BodyArrows = new();
            EatNeed = CustomGameOptions.CannibalBodyCount >= PlayerControl.AllPlayerControls.Count / 2 ? PlayerControl.AllPlayerControls.Count / 2 : CustomGameOptions.CannibalBodyCount;
            Type = LayerEnum.Cannibal;
            EatButton = new(this, "Eat", AbilityTypes.Dead, "ActionSecondary", Eat);
            InspectorResults = InspectorResults.DealsWithDead;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float EatTimer()
        {
            var timespan = DateTime.UtcNow - LastEaten;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CannibalCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            arrow.Value?.Destroy();
            BodyArrows.Remove(arrow.Key);
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
            EatButton.Update("EAT", EatTimer(), CustomGameOptions.CannibalCd);

            if (CustomGameOptions.EatArrows && !IsDead)
            {
                var validBodies = Utils.AllBodies.Where(x => Utils.KilledPlayers.Any(y => y.PlayerId == x.ParentId &&
                    y.KillTime.AddSeconds(CustomGameOptions.EatArrowDelay) < DateTime.UtcNow));

                foreach (var bodyArrow in BodyArrows.Keys)
                {
                    if (!validBodies.Any(x => x.ParentId == bodyArrow))
                        DestroyArrow(bodyArrow);
                }

                foreach (var body in validBodies)
                {
                    if (!BodyArrows.ContainsKey(body.ParentId))
                        BodyArrows.Add(body.ParentId, new(Player, Color));

                    BodyArrows[body.ParentId].Update(body.TruePosition);
                }
            }
            else if (BodyArrows.Count != 0)
                OnLobby();
        }

        public void Eat()
        {
            if (Utils.IsTooFar(Player, EatButton.TargetBody) || EatTimer() != 0f)
                return;

            var player = Utils.PlayerById(EatButton.TargetBody.ParentId);
            Utils.Spread(Player, player);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.FadeBody);
            writer.Write(EatButton.TargetBody.ParentId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            LastEaten = DateTime.UtcNow;
            EatNeed--;
            Coroutines.Start(Utils.FadeBody(EatButton.TargetBody));

            if (EatWin)
            {
                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer2.Write((byte)WinLoseRPC.CannibalWin);
                writer2.Write(PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer2);
                Eaten = true;
            }
        }
    }
}