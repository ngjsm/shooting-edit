using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    public SceneButton[] sceneButtons;
    public int columnCount = 3; // ���� �� �� (��: 3�̸� 3�� 2��)

    private int currentIndex = 0;

    void Start()
    {
        UpdateSelection();
    }

    void Update()
    {
        int prevIndex = currentIndex;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex = (currentIndex + 1) % sceneButtons.Length;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex = (currentIndex - 1 + sceneButtons.Length) % sceneButtons.Length;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentIndex -= columnCount;
            if (currentIndex < 0)
                currentIndex += sceneButtons.Length; // ���� ������ �� �� �Ʒ��� ��
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentIndex += columnCount;
            if (currentIndex >= sceneButtons.Length)
                currentIndex %= sceneButtons.Length; // �Ʒ��� ������ �� �� ���� ��
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(sceneButtons[currentIndex].sceneName);
        }

        if (prevIndex != currentIndex)
        {
            UpdateSelection();
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < sceneButtons.Length; i++)
        {
            sceneButtons[i].Highlight(i == currentIndex);
        }
    }
}