using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UI_DissolveController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField][Range(0, 1)] private float m_DissolveAmount;
    [SerializeField] private Image m_DissolveImage;

    private void Update()
    {
        m_DissolveImage.materialForRendering.SetFloat("_Dissolve", m_DissolveAmount);
    }
}