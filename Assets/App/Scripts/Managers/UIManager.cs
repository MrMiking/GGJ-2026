using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private TextMeshProUGUI m_GoldText;

    private void OnEnable()
    {
        GameManager.OnGoldChange += SetGoldText;
    }

    private void OnDisable()
    {
        GameManager.OnGoldChange -= SetGoldText;
    }

    private void SetGoldText(int amount)
    {
        m_GoldText.text = amount.ToString("F0");
    }
}
