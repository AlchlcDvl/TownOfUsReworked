namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Camouflager : IntruderRole
    {
        public CustomButton CamouflageButton;
        public bool Enabled;
        public DateTime LastCamouflaged;
        public float TimeRemaining;
        public bool Camouflaged => TimeRemaining > 0f;

        public Camouflager(PlayerControl player) : base(player)
        {
            Name = "Camouflager";
            StartText = "Hinder The <color=#8CFFFFFF>Crew</color>'s Recognition";
            AbilitiesText = "- You can disrupt everyone's vision, causing them to be unable to tell players apart\n- When camouflaged, everyone will appear grey with fluctuating names " +
                $"and no cosmetics{(CustomGameOptions.MeetingColourblind ? "\n- This effect carries over into the meeting if a meeting is called during a camouflage" : "")}\n" +
                $"{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Camouflager : Colors.Intruder;
            RoleType = RoleEnum.Camouflager;
            RoleAlignment = RoleAlignment.IntruderConceal;
            InspectorResults = InspectorResults.BringsChaos;
            Type = LayerEnum.Camouflager;
            CamouflageButton = new(this, "Camouflage", AbilityTypes.Effect, "Secondary", HitCamouflage);
        }

        public void Camouflage()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            Utils.Camouflage();

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void UnCamouflage()
        {
            Enabled = false;
            LastCamouflaged = DateTime.UtcNow;
            Utils.DefaultOutfitAll();
        }

        public float CamouflageTimer()
        {
            var timespan = DateTime.UtcNow - LastCamouflaged;
            var num = Player.GetModifiedCooldown(CustomGameOptions.CamouflagerCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void HitCamouflage()
        {
            if (CamouflageTimer() != 0f || DoUndo.IsCamoed)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Camouflage);
            writer.Write(PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            TimeRemaining = CustomGameOptions.CamouflagerDuration;
            Camouflage();
            Utils.Camouflage();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            CamouflageButton.Update("CAMOUFLAGE", CamouflageTimer(), CustomGameOptions.CamouflagerCd, Camouflaged, TimeRemaining, CustomGameOptions.CamouflagerDuration, !DoUndo.IsCamoed);
        }
    }
}