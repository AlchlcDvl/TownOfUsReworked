using TownOfUsReworked.Patches;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Phantom : Objectifier
    {
        public bool Caught;
        public bool CompletedTasks;
        public bool HasDied;
        public bool Faded;
        public bool PhantomWins { get; set; }

        public Phantom(PlayerControl player) : base(player)
        {
            Name = "Phantom";
            SymbolName = "Ω";
            TaskText = "You can get revenge from beyond the grave!";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Phantom : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Phantom;
            Hidden = !CustomGameOptions.PhantomKnows;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (!Player.Data.IsDead || Player.Data.Disconnected)
                return true;
                
            if (CompletedTasks)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PhantomWin, SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return true;
        }

        public void Fade()
        {
            Faded = true;
            var color = new Color(1f, 1f, 1f, 0f);

            var maxDistance = ShipStatus.Instance.MaxLightRadius * CustomGameOptions.CrewVision;

            if (PlayerControl.LocalPlayer == null)
                return;

            var distance = (PlayerControl.LocalPlayer.GetTruePosition() - Player.GetTruePosition()).magnitude;

            var distPercent = distance / maxDistance;
            distPercent = Mathf.Max(0, distPercent - 1);

            var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            color.a = 0.07f + velocity / Player.MyPhysics.GhostSpeed * 0.13f;
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

            Player.myRend().color = color;
            Player.nameText().color = Color.clear;
            Player.cosmetics.colorBlindText.color = Color.clear;
        }
    }
}