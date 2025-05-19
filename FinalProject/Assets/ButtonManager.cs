using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private List<GameObject> buttons = new(); // Top-down ordered buttons
    [SerializeField] private GameObject sliderPanelPrefab;
    [SerializeField] private List<GameObject> Prefabs = new List<GameObject>();

    [Header("Settings")]
    [SerializeField] private float dropdownHeight = 300f;
    [SerializeField] private float animationDuration = 0.3f;

    // [SerializeField] private List<GameObject> Buttons = new List<GameObject>();
    // [SerializeField] private List<GameObject> Prefabs = new List<GameObject>();

    public List<int> buttonStates = new(); // 0 = closed, 1 = open
    private GameObject currentPanelInstance;
    private bool isAnimating = false;

    private void Awake()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttonStates.Add(0);
        }
    }

    public void ToggleMenu(GameObject clickedButton)
    {
        if (!isAnimating)
        {
            StartCoroutine(HandleToggle(clickedButton));
        }
    }

    private IEnumerator HandleToggle(GameObject clickedButton)
    {
        int index = buttons.IndexOf(clickedButton);
        if (index == -1) yield break;

        if (buttonStates[index] == 1)
        {
            CloseMenu(index);
            yield break;
        }

        isAnimating = true;

        // Close other open menus
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i != index && buttonStates[i] == 1)
            {
                CloseMenu(i);
            }
        }

        // Wait for animations to finish
        yield return new WaitForSeconds(animationDuration);

        OpenMenu(index);
        isAnimating = false;
    }

    private void CloseMenu(int index)
    {
        buttonStates[index] = 0;

        // Shift buttons back up
        for (int i = index + 1; i < buttons.Count; i++)
        {
            RectTransform rect = buttons[i].GetComponent<RectTransform>();
            Vector2 targetPos = rect.anchoredPosition + new Vector2(0, dropdownHeight);
            StartCoroutine(ShiftButton(rect, targetPos));
        }

        if (currentPanelInstance != null)
        {
            Destroy(currentPanelInstance);
            currentPanelInstance = null;
        }
    }

    private void OpenMenu(int index)
    {
        buttonStates[index] = 1;

        GameObject panel = Instantiate(sliderPanelPrefab, buttons[index].transform.parent);
        currentPanelInstance = panel;

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        RectTransform buttonRect = buttons[index].GetComponent<RectTransform>();

        Vector3 buttonWorldPos = buttons[index].transform.position;
        float buttonHeight = buttonRect.sizeDelta.y * buttons[index].transform.lossyScale.y;
        Vector3 panelWorldPos = new(buttonWorldPos.x, buttonWorldPos.y - buttonHeight, buttonWorldPos.z);

        panel.transform.position = panelWorldPos;
        panel.transform.SetSiblingIndex(buttons[index].transform.GetSiblingIndex());
        panelRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, 0);

        StartCoroutine(ExpandPanel(panelRect, dropdownHeight));

        for (int i = index + 1; i < buttons.Count; i++)
        {
            RectTransform rect = buttons[i].GetComponent<RectTransform>();
            Vector2 targetPos = rect.anchoredPosition - new Vector2(0, dropdownHeight);
            StartCoroutine(ShiftButton(rect, targetPos));
        }
    }

    private IEnumerator ExpandPanel(RectTransform panel, float targetHeight)
    {
        float elapsed = 0f;
        float startHeight = 0f;

        while (elapsed < animationDuration)
        {
            float newHeight = Mathf.Lerp(startHeight, targetHeight, elapsed / animationDuration);
            panel.sizeDelta = new Vector2(panel.sizeDelta.x, newHeight);
            elapsed += Time.deltaTime;
            yield return null;
        }

        panel.sizeDelta = new Vector2(panel.sizeDelta.x, targetHeight);
    }

    private IEnumerator ShiftButton(RectTransform rect, Vector2 targetPos)
    {
        Vector2 startPos = rect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / animationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rect.anchoredPosition = targetPos;
    }
}
