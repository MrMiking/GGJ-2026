using System;
using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace MVsToolkit.Utilities
{
    public static class MVsCoroutine
    {
        public static void Delay(this MonoBehaviour hook, Action ev, YieldInstruction yieldInstruction)
        {
            IEnumerator DelayCoroutine()
            {
                yield return yieldInstruction;
                ev?.Invoke();
            }

            hook.StartCoroutine(DelayCoroutine());
        }

        public static void Delay(this MonoBehaviour hook, Action ev, float time, bool affectTimeScale = true)
        {
            IEnumerator DelayCoroutine()
            {
                if (affectTimeScale)
                    yield return new WaitForSeconds(time);
                else
                    yield return new WaitForSecondsRealtime(time);
                ev?.Invoke();
            }

            hook.StartCoroutine(DelayCoroutine());
        }

        public static void Delay(this MonoBehaviour hook, Action ev, IEnumerator coroutine)
        {
            IEnumerator DelayCoroutine()
            {
                yield return coroutine;
                ev?.Invoke();
            }

            hook.StartCoroutine(DelayCoroutine());
        }
    }
}