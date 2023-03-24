using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Cosmetics.CustomColors;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Objects
{
    public class Footprint
    {
        public readonly PlayerControl Player;
        private GameObject _gameObject;
        private SpriteRenderer _spriteRenderer;
        private readonly float _time;
        private readonly Vector2 _velocity;
        public Color Color;
        public Vector3 Position;
        public Role Role;
        public static float Duration => CustomGameOptions.FootprintDuration;
        public static bool Grey => CustomGameOptions.AnonymousFootPrint || CamouflageUnCamouflage.IsCamoed;

        public Footprint(PlayerControl player, Role role)
        {
            Role = role;
            Position = player.transform.position;
            _velocity = player.gameObject.GetComponent<Rigidbody2D>().velocity;

            Player = player;
            _time = (int) Time.time;
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
            _gameObject = new GameObject("Footprint");
            _gameObject.AddSubmergedComponent(SubmergedCompatibility.ElevatorMover);
            _gameObject.transform.position = Position;
            _gameObject.transform.Rotate(Vector3.forward * Vector2.SignedAngle(Vector2.up, _velocity));
            _gameObject.transform.SetParent(Player.transform.parent);

            _spriteRenderer = _gameObject.AddComponent<SpriteRenderer>();
            _spriteRenderer.sprite = AssetManager.Footprint;
            _spriteRenderer.color = Color;
            var appearance = Player.GetAppearance();
            var size = appearance.SizeFactor;
            _gameObject.transform.localScale *= new Vector2(1.2f, 1f) * new Vector2(size.x, size.y);

            _gameObject.SetActive(true);
        }

        private void Destroy()
        {
            Object.Destroy(_gameObject);
            Role.AllPrints.Remove(this);
        }

        public bool Update()
        {
            var currentTime = Time.time;
            var alpha = Mathf.Max(1f - ((currentTime - _time) / Duration), 0f);

            if (alpha < 0 || alpha > 1)
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
            _spriteRenderer.color = Color;

            if (_time + (int) Duration < currentTime)
            {
                Destroy();
                return true;
            }

            return false;
        }
    }
}