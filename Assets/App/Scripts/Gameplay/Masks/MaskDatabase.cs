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
        [SerializeField] private Mask[] m_Masks;

        public int MaskCount => m_Masks.Length;
        public Mask this[int index] => m_Masks[index];

        public IEnumerator<Mask> GetEnumerator() => ((IEnumerable<Mask>) m_Masks).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_Masks.GetEnumerator();


#if UNITY_EDITOR
        private const string MasksFolderPath = "Assets/App/Datas/Masks";

        private void OnValidate()
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