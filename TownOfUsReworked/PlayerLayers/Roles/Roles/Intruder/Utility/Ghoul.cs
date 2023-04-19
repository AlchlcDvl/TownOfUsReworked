using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Custom;
using Hazel;
using System.Linq;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Ghoul : IntruderRole
    {
        public CustomButton MarkButton;
        public bool Caught;
        public bool Faded;
        public DateTime LastMarked;
        public PlayerControl MarkedPlayer;
        public PlayerControl ClosestMark;

        public Ghoul(PlayerControl player) : base(player)
        {
            Name = "Ghoul";
            RoleType = RoleEnum.Ghoul;
            StartText = "BOO!";
            AbilitiesText = "- You can mark a player for death every round\n- Marked players will be announced to all players and will die at the end of the next meeting if you are not" +
                " clicked";
            RoleAlignment = RoleAlignment.IntruderUtil;
            AlignmentName = IU;
            InspectorResults = InspectorResults.Ghostly;
            Color = CustomGameOptions.CustomIntColors ? Colors.Ghoul : Colors.Intruder;
            MarkedPlayer = null;
            Type = LayerEnum.Ghoul;
            MarkButton = new(this, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary", Mark, false, true);
        }

        public float MarkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMarked;
            var num = CustomGameOptions.GhoulMarkCd * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Fade()
        {
            Faded = true;
            Player.Visible = true;
            var color = new Color(1f, 1f, 1f, 0f);

            var maxDistance = ShipStatus.Instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
            var distance = (PlayerControl.LocalPlayer.GetTruePosition() - Player.GetTruePosition()).magnitude;

            var distPercent = distance / maxDistance;
            distPercent = Mathf.Max(0, distPercent - 1);

            var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            color.a = 0.07f + (velocity / Player.MyPhysics.TrueSpeed * 0.13f);
            color.a = Mathf.Lerp(color.a, 0, distPercent);

            if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
            {
                Player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, new GameData.PlayerOutfit()
                {
                    ColorId = Player.GetDefaultOutfit().ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = ""
                });
            }

            Player.MyRend().color = color;
            Player.NameText().color = new(0f, 0f, 0f, 0f);
            Player.cosmetics.colorBlindText.color = new(0f, 0f, 0f, 0f);
        }

        public void Mark()
        {
            if (MarkTimer() != 0f || Utils.IsTooFar(Player, ClosestMark))
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Mark);
            writer.Write(Player.PlayerId);
            writer.Write(ClosestMark.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            MarkedPlayer = ClosestMark;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            KillButton.Disable();
            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder) && x != MarkedPlayer).ToList();
            MarkButton.Update("MARK", MarkTimer(), CustomGameOptions.GhoulMarkCd, notImp, true, MarkedPlayer == null);
        }
    }
}