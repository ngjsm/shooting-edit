using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneButton : MonoBehaviour
{
    public string sceneName;

    [Header("Visual Components")]
    public Renderer boxRenderer;       // �ڽ� ������Ʈ�� Renderer
    public Material normalMaterial;    // �⺻ ���� ���͸���
    public Material highlightMaterial; // ���õǾ��� �� ���͸���

    public TextMeshPro label;          // �ؽ�Ʈ (�ڽ� ������Ʈ�� �ٿ��� ��)

    public void Highlight(bool isSelected)
    {
        if (boxRenderer != null)
        {
            boxRenderer.material = isSelected ? highlightMaterial : normalMaterial;
        }

        if (label != null)
        {
            label.fontStyle = isSelected ? FontStyles.Bold : FontStyles.Normal;
        }
    }
}