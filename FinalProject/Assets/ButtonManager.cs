using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private List<GameObject> buttons = new List<GameObject>(); // buttons in top-down order
    [SerializeField] private GameObject sliderPanelPrefab; // assign dropdown prefab here
    private List<int> buttonStatus = new List<int>(); //0 = closed, 1 = open

    [Header("Settings")]
    [SerializeField] private float dropdownHeight = 300f;
    [SerializeField] private float animationRate = 0.3f;


    public void Awake()
    {
        // Initialize buttonStatus with default values (0 for closed)
        for (int i = 0; i < buttons.Count; i++)
        {
            buttonStatus.Add(0); // default status: all buttons are closed
        }
    }


    public void ToggleMenu(GameObject clickedButton)
    {
        int clickedIndex = buttons.IndexOf(clickedButton);

        if (buttonStatus[clickedIndex] == 1) // If button is open, close it
        {
            CloseMenu(clickedIndex);
        }
        else // If button is closed, open it
        {
            OpenMenu(clickedIndex);
        }
    }


    public void CloseMenu(int clickedIndex){

    }
    public void OpenMenu(int clickedIndex){
        buttonStatus[clickedIndex] = 1;

        GameObject currentPanelInstance = Instantiate(sliderPanelPrefab, buttons[clickedIndex].transform.parent);
        RectTransform panelRect = currentPanelInstance.GetComponent<RectTransform>();
    }
    // private GameObject currentPanelInstance;
    // private RectTransform panelRect;
    // private bool isMenuOpen = false;
    // private int activeButtonIndex = -1;

    // public void ToggleMenu(GameObject clickedButton)
    // {
    //     int clickedIndex = buttons.IndexOf(clickedButton);

    //     if (isMenuOpen && clickedIndex == activeButtonIndex)
    //     {
    //         CloseMenu(clickedIndex);
    //         return;
    //     }

    //     if (isMenuOpen)
    //     {
    //         CloseMenu(activeButtonIndex);
    //     }

    //     OpenMenu(clickedIndex, clickedButton);
    // }

    // private void OpenMenu(int index, GameObject clickedButton)
    // {
    //     isMenuOpen = true;
    //     activeButtonIndex = index;

    //     // Instantiate dropdown panel
    //     currentPanelInstance = Instantiate(sliderPanelPrefab, clickedButton.transform.parent); // same parent as buttons
    //     panelRect = currentPanelInstance.GetComponent<RectTransform>();

    //     RectTransform buttonRect = clickedButton.GetComponent<RectTransform>();

    //     // Position panel directly under the button
    //     Vector2 buttonPos = buttonRect.anchoredPosition;
    //     float buttonHeight = buttonRect.sizeDelta.y;

    //     panelRect.anchoredPosition = new Vector2(buttonPos.x, buttonPos.y - buttonHeight);
    //     panelRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, 0); // start at height 0

    //     StartCoroutine(ExpandPanel(panelRect, dropdownHeight));

    //     // Shift buttons below
    //     for (int i = index + 1; i < buttons.Count; i++)
    //     {
    //         RectTransform rect = buttons[i].GetComponent<RectTransform>();
    //         Vector2 newPos = rect.anchoredPosition + new Vector2(0, -dropdownHeight);
    //         StartCoroutine(ShiftButton(rect, newPos));
    //     }
    // }

    // private void CloseMenu(int index)
    // {
    //     isMenuOpen = false;

    //     // Shift buttons back up
    //     for (int i = index + 1; i < buttons.Count; i++)
    //     {
    //         RectTransform rect = buttons[i].GetComponent<RectTransform>();
    //         Vector2 newPos = rect.anchoredPosition + new Vector2(0, dropdownHeight);
    //         StartCoroutine(ShiftButton(rect, newPos));
    //     }

    //     if (currentPanelInstance != null)
    //     {
    //         Destroy(currentPanelInstance);
    //     }

    //     activeButtonIndex = -1;
    // }

    // private IEnumerator ExpandPanel(RectTransform panel, float targetHeight)
    // {
    //     float elapsed = 0f;
    //     float startHeight = 0f;

    //     while (elapsed < animationDuration)
    //     {
    //         float newHeight = Mathf.Lerp(startHeight, targetHeight, elapsed / animationDuration);
    //         panel.sizeDelta = new Vector2(panel.sizeDelta.x, newHeight);
    //         elapsed += Time.deltaTime;
    //         yield return null;
    //     }

    //     panel.sizeDelta = new Vector2(panel.sizeDelta.x, targetHeight);
    // }

    // private IEnumerator ShiftButton(RectTransform rect, Vector2 targetPos)
    // {
    //     Vector2 startPos = rect.anchoredPosition;
    //     float elapsed = 0;

    //     while (elapsed < animationDuration)
    //     {
    //         rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / animationDuration);
    //         elapsed += Time.deltaTime;
    //         yield return null;
    //     }

    //     rect.anchoredPosition = targetPos;
    // }
}
