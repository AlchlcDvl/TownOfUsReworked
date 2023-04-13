using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Cosmetics.CustomColors;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers;
using TownOfUsReworked.Classes;
using HarmonyLib;

namespace TownOfUsReworked.Objects
{
    [HarmonyPatch]
    public class Footprint
    {
        public readonly PlayerControl Player;
        private GameObject GObject;
        private SpriteRenderer Sprite;
        private readonly float Time2;
        private readonly Vector2 Velocity;
        public Color Color;
        public Vector3 Position;
        public Role Role;
        public static float Duration => CustomGameOptions.FootprintDuration;
        public static bool Grey => CustomGameOptions.AnonymousFootPrint || DoUndo.IsCamoed;

        public Footprint(PlayerControl player, Role role)
        {
            Role = role;
            Position = player.transform.position;
            Velocity = player.gameObject.GetComponent<Rigidbody2D>().velocity;
            Player = player;
            Time2 = (int) Time.time;
            Color = Color.black;
            Start();
            role.AllPrints.Add(this);
        }

        public static void DestroyAll(Role role)
        {
            while (role.AllPrints.Count != 0)
                role.AllPrints[0].Destroy();
        }

        private void Start()
        {
            GObject = new GameObject("Footprint");
            GObject.AddSubmergedComponent(SubmergedCompatibility.ElevatorMover);
            GObject.transform.position = Position;
            GObject.transform.Rotate(Vector3.forward * Vector2.SignedAngle(Vector2.up, Velocity));
            GObject.transform.SetParent(Player.transform.parent);

            Sprite = GObject.AddComponent<SpriteRenderer>();
            Sprite.sprite = AssetManager.Footprint;
            Sprite.color = Color;
            var appearance = Player.GetAppearance();
            var size = appearance.SizeFactor;
            GObject.transform.localScale *= new Vector2(1.2f, 1f) * new Vector2(size.x, size.y);

            GObject.SetActive(true);
        }

        private void Destroy()
        {
            Object.Destroy(GObject);
            Role.AllPrints.Remove(this);
        }

        public bool Update()
        {
            var currentTime = Time.time;
            var alpha = Mathf.Max(1f - ((currentTime - Time2) / Duration), 0f);

            if (alpha is < 0 or > 1)
                alpha = 0;

            if (ColorUtils.IsRainbow(Player.GetDefaultOutfit().ColorId) && !Grey)
                Color = ColorUtils.Rainbow;
            else if (ColorUtils.IsChroma(Player.GetDefaultOutfit().ColorId) && !Grey)
                Color = ColorUtils.Chroma;
            else if (ColorUtils.IsMantle(Player.GetDefaultOutfit().ColorId) && !Grey)
                Color = ColorUtils.Mantle;
            else if (ColorUtils.IsMonochrome(Player.GetDefaultOutfit().ColorId) && !Grey)
                Color = ColorUtils.Monochrome;
            else if (ColorUtils.IsFire(Player.GetDefaultOutfit().ColorId) && !Grey)
                Color = ColorUtils.Fire;
            else if (ColorUtils.IsGalaxy(Player.GetDefaultOutfit().ColorId) && !Grey)
                Color = ColorUtils.Galaxy;
            else if (Grey)
                Color = Color.grey;
            else
                Color = Palette.PlayerColors[Player.GetDefaultOutfit().ColorId];

            Color = new Color(Color.r, Color.g, Color.b, alpha);
            Sprite.color = Color;

            if (Time2 + (int) Duration < currentTime)
            {
                Destroy();
                return true;
            }
            else
                return false;
        }
    }
}