using MVsToolkit.Dev;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace GGJ2026
{
    [CreateAssetMenu(fileName = "New Mask DB", menuName = "GGJ2026/MaskDB")]
    public sealed class MaskDatabase : ScriptableObject, IEnumerable<Mask>
    {
        [Serializable]
        public struct MaskPool
        {
            public int gameLevel;
            public WeightedList<Mask> masks;
        }

        [SerializeField] private Mask[] m_Masks;
        [SerializeField] private MaskPool[] m_MaskPools;

        public int MaskCount => m_Masks.Length;
        public Mask this[int index] => m_Masks[index];

        public IEnumerator<Mask> GetEnumerator() => ((IEnumerable<Mask>) m_Masks).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_Masks.GetEnumerator();

        public MaskPool GetMaskPoolForLevel(int level)
        {
            var pool = m_MaskPools
                .Where(p => p.gameLevel <= level)
                .OrderByDescending((p) => p.gameLevel)
                .First();

            pool.masks.RecalculateWeights();
            return pool;
        }

#if UNITY_EDITOR
        private const string MasksFolderPath = "Assets/App/Datas/Masks";

        [ContextMenu("Sync All Masks")]
        private void SyncAllMasks()
        {
            if (string.IsNullOrEmpty(MasksFolderPath))
                return;

            // Trouve tous les assets de type Mask dans le dossier
            string[] guids = AssetDatabase.FindAssets("t:Mask", new[] { MasksFolderPath });

            m_Masks = guids.Select(guid =>
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                return AssetDatabase.LoadAssetAtPath<Mask>(assetPath);
            })
            .Where(mask => mask != null)
            .Distinct()
            .ToArray();

            // Marque l'asset comme modifié
            EditorUtility.SetDirty(this);
        }
#endif
    }
}