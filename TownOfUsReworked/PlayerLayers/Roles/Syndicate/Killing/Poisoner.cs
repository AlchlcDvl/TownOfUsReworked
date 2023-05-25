namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Poisoner : SyndicateRole
    {
        public CustomButton PoisonButton;
        public CustomButton GlobalPoisonButton;
        public DateTime LastPoisoned;
        public PlayerControl PoisonedPlayer;
        public float TimeRemaining;
        public bool Enabled;
        public bool Poisoned => TimeRemaining > 0f;
        public CustomMenu PoisonMenu;

        public Poisoner(PlayerControl player) : base(player)
        {
            Name = "Poisoner";
            StartText = "Delay A Kill To Decieve The <color=#8CFFFFFF>Crew</color>";
            AbilitiesText = $"- You can poison players\n- Poisoned players will die after {CustomGameOptions.PoisonDuration}s\n- With the Chaos Drive, you can poison players from anywhere"
                + $"occur concurrently\n{AbilitiesText}";
            Color = CustomGameOptions.CustomSynColors? Colors.Poisoner : Colors.Syndicate;
            RoleType = RoleEnum.Poisoner;
            PoisonedPlayer = null;
            RoleAlignment = RoleAlignment.SyndicateKill;
            PoisonMenu = new(Player, Click, Exception1);
            Type = LayerEnum.Poisoner;
            PoisonButton = new(this, "Poison", AbilityTypes.Direct, "ActionSecondary", HitPoison, Exception1);
            GlobalPoisonButton = new(this, "Poison", AbilityTypes.Effect, "ActionSecondary", HitGlobalPoison);
            InspectorResults = InspectorResults.Unseen;
        }

        public void Poison()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance || PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected || IsDead)
                TimeRemaining = 0f;
        }

        public void PoisonKill()
        {
            if (!(PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected || PoisonedPlayer.Is(RoleEnum.Pestilence)))
                Utils.RpcMurderPlayer(Player, PoisonedPlayer, DeathReasonEnum.Poisoned, false);

            PoisonedPlayer = null;
            Enabled = false;
            LastPoisoned = DateTime.UtcNow;
        }

        public float PoisonTimer()
        {
            var timespan = DateTime.UtcNow - LastPoisoned;
            var num = Player.GetModifiedCooldown(CustomGameOptions.PoisonCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Click(PlayerControl player)
        {
            var interact = Utils.Interact(Player, player);

            if (interact[3] && !player.IsProtected() && !player.IsVesting())
                PoisonedPlayer = player;
            else if (interact[0])
                LastPoisoned = DateTime.UtcNow;
            else if (interact[1] || player.IsProtected())
                LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (player.IsVesting())
                LastPoisoned.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var flag = PoisonedPlayer == null && HoldsDrive;
            GlobalPoisonButton.Update(flag ? "SET TARGET" : "POISON", PoisonTimer(), CustomGameOptions.PoisonCd, Poisoned, TimeRemaining, CustomGameOptions.PoisonDuration, true,
                HoldsDrive);
            PoisonButton.Update("POISON", PoisonTimer(), CustomGameOptions.PoisonCd, Poisoned, TimeRemaining, CustomGameOptions.PoisonDuration, true, !HoldsDrive);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (PoisonedPlayer != null && HoldsDrive && !Poisoned)
                    PoisonedPlayer = null;

                Utils.LogSomething("Removed a target");
            }
        }

        public bool Exception1(PlayerControl player) => player == PoisonedPlayer || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

        public void HitPoison()
        {
            if (PoisonTimer() != 0f || Poisoned || HoldsDrive || Utils.IsTooFar(Player, PoisonButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, PoisonButton.TargetPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Poison);
                writer.Write(Player.PlayerId);
                writer.Write(PoisonButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                PoisonedPlayer = PoisonButton.TargetPlayer;
                TimeRemaining = CustomGameOptions.PoisonDuration;
                Poison();
            }
            else if (interact[0])
                LastPoisoned = DateTime.UtcNow;
            else if (interact[1])
                LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastPoisoned.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public void HitGlobalPoison()
        {
            if (PoisonTimer() != 0f || Poisoned || !HoldsDrive)
                return;

            if (PoisonedPlayer == null)
                PoisonMenu.Open();
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Poison);
                writer.Write(Player.PlayerId);
                writer.Write(PoisonedPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining = CustomGameOptions.PoisonDuration;
                Poison();
            }
        }
    }
}