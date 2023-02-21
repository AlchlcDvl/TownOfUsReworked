using System;
using UnityEngine;

namespace TownOfUsReworked.Cosmetics
{
    public class RainbowBehaviour : MonoBehaviour
    {
        public Renderer Renderer;
        public int Id;

        public RainbowBehaviour(IntPtr ptr) : base(ptr) {}

        public void AddRend(Renderer rend, int id)
        {
            Renderer = rend;
            Id = id;
        }

        public void Update()
        {
            if (Renderer == null)
                return;

            if (RainbowUtils.IsRainbow(Id))
                RainbowUtils.SetRainbow(Renderer);
        }
    }
}
