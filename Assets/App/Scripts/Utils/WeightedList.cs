
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2026
{
    [Serializable]
    public class WeightedList<T>
    {
        [Serializable]
        public class Entry
        {
            public T Value;
            public float Weight;
        }

        [SerializeField]
        private List<Entry> entries = new List<Entry>();

        private float totalWeight;

        public void RecalculateWeights()
        {
            totalWeight = 0f;

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Weight > 0f)
                    totalWeight += entries[i].Weight;
            }
        }

        public void Add(T value, float weight)
        {
            entries.Add(new Entry
            {
                Value = value,
                Weight = Mathf.Max(0f, weight)
            });

            totalWeight += Mathf.Max(0f, weight);
        }

        public T GetRandom()
        {
            if (entries.Count == 0 || totalWeight <= 0f)
            {
                Debug.LogWarning("WeightedList is empty or total weight is zero.");
                return default;
            }

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float current = 0f;

            for (int i = 0; i < entries.Count; i++)
            {
                float w = entries[i].Weight;
                if (w <= 0f)
                    continue;

                current += w;

                if (roll <= current)
                    return entries[i].Value;
            }

            return entries[entries.Count - 1].Value;
        }

        public T GetRandomAndRemove()
        {
            if (entries.Count == 0 || totalWeight <= 0f)
                return default;

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float current = 0f;

            for (int i = 0; i < entries.Count; i++)
            {
                current += entries[i].Weight;

                if (roll <= current)
                {
                    T value = entries[i].Value;

                    totalWeight -= entries[i].Weight;
                    entries.RemoveAt(i);

                    return value;
                }
            }

            return default;
        }

        public WeightedList<T> Clone()
        {
            var clone = new WeightedList<T>();

            for (int i = 0; i < entries.Count; i++)
            {
                clone.entries.Add(new Entry
                {
                    Value = entries[i].Value,
                    Weight = entries[i].Weight
                });
            }

            clone.totalWeight = totalWeight;
            return clone;
        }
    }
}
