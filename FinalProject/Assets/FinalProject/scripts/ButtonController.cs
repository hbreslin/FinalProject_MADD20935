using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private List<GameObject> Buttons = new List<GameObject>();
    [SerializeField] private List<GameObject> Prefabs = new List<GameObject>();
    

    private PlaceObject placeObject;

    void Start()
    {
        GameObject xrOrigin = GameObject.FindWithTag("XROrigin");

        if (xrOrigin != null)
        {
            placeObject = xrOrigin.GetComponent<PlaceObject>();

            if (placeObject == null)
            {
                Debug.LogError("PlaceObject script not found on XR Origin.");
                return;
            }

            // Hook each button to the corresponding prefab
            for (int i = 0; i < Buttons.Count && i < Prefabs.Count; i++)
            {
                int index = i; // capture for closure
                Button btn = Buttons[index].GetComponent<Button>();

                if (btn != null)
                {
                    btn.onClick.AddListener(() => placeObject.setPrefab(Prefabs[index]));
                }
                else
                {
                    Debug.LogWarning($"No Button component found on {Buttons[index].name}");
                }
            }
        }
        else
        {
            Debug.LogError("XR Origin with tag 'XROrigin' not found.");
        }
    }
}
