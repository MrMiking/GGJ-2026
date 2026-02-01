using GGJ2026;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private TextMeshProUGUI m_GoldText;

    

    private void OnEnable()
    {
        GameManager.Instance.OnGoldChange += SetGoldText;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGoldChange -= SetGoldText;
    }

    private void SetGoldText(int amount)
    {
        m_GoldText.text = amount.ToString("F0");
    }
}
