namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Teleporter : Intruder
    {
        public CustomButton TeleportButton;
        public DateTime LastTeleport;
        public DateTime LastMarked;
        public CustomButton MarkButton;
        public bool CanMark;
        public Vector3 TeleportPoint = Vector3.zero;

        public Teleporter(PlayerControl player) : base(player)
        {
            Name = "Teleporter";
            StartText = () => "X Marks The Spot";
            AbilitiesText = () => $"- You can mark a spot to teleport to later\n{CommonAbilities}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Teleporter : Colors.Intruder;
            RoleType = RoleEnum.Teleporter;
            Type = LayerEnum.Teleporter;
            TeleportPoint = Vector3.zero;
            MarkButton = new(this, "Mark", AbilityTypes.Effect, "Secondary", Mark);
            TeleportButton = new(this, "Teleport", AbilityTypes.Effect, "Secondary", Teleport);
            InspectorResults = InspectorResults.MovesAround;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }

        public float TeleportTimer()
        {
            var timespan = DateTime.UtcNow - LastTeleport;
            var num = Player.GetModifiedCooldown(CustomGameOptions.TeleportCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float MarkTimer()
        {
            var timespan = DateTime.UtcNow - LastMarked;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MarkCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Mark()
        {
            if (!CanMark || MarkTimer() != 0f || TeleportPoint == Player.transform.position)
                return;

            TeleportPoint = Player.transform.position;
            LastMarked = DateTime.UtcNow;

            if (CustomGameOptions.TeleCooldownsLinked)
                LastTeleport = DateTime.UtcNow;
        }

        public void Teleport()
        {
            if (TeleportTimer() != 0f || TeleportPoint == Player.transform.position)
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Teleport);
            writer.Write(PlayerId);
            writer.Write(TeleportPoint);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            LastTeleport = DateTime.UtcNow;
            Utils.Teleport(Player, TeleportPoint);

            if (CustomGameOptions.TeleCooldownsLinked)
                LastMarked = DateTime.UtcNow;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var hits = Physics2D.OverlapBoxAll(Player.transform.position, Utils.GetSize(), 0);
            hits = hits.Where(c => (c.name.Contains("Vent") || !c.isTrigger) && c.gameObject.layer != 8 && c.gameObject.layer != 5).ToArray();
            CanMark = hits.Count == 0 && Player.moveable && !ModCompatibility.GetPlayerElevator(Player).Item1 && TeleportPoint != Player.transform.position;
            MarkButton.Update("MARK", MarkTimer(), CustomGameOptions.MarkCooldown, CanMark);
            TeleportButton.Update("TELEPORT", TeleportTimer(), CustomGameOptions.TeleportCd, true, TeleportPoint != Vector3.zero);
        }
    }
}