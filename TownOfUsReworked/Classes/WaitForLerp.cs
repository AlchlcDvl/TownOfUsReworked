using System;
using System.Collections;
using UnityEngine;

namespace TownOfUsReworked.Classes
{
    public class WaitForLerp : IEnumerator
    {
        private readonly Action<float> act;
        private readonly float duration;
        private float timer;
        public object Current => null;

        public WaitForLerp(float seconds, Action<float> act)
        {
            duration = seconds;
            this.act = act;
        }

        public bool MoveNext()
        {
            timer = Mathf.Min(timer + Time.deltaTime, duration);
            act(timer / duration);
            return timer < duration;
        }

        public void Reset() => timer = 0f;
    }
}