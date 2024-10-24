using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoScrollTemplateBehaviour : MonoBehaviour
{
    // ref) https://noracle.jp/unity-dropdown/

    private ScrollRect scrollRect;

    void Awake()
    {
        scrollRect = this.gameObject.GetComponent<ScrollRect>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Dropdown dropdown = GetComponentInParent<Dropdown>();

        if (dropdown != null)
        {
            RectTransform viewport = this.transform.Find("Viewport").GetComponent<RectTransform>();
            RectTransform contentArea = this.transform.Find("Viewport/Content").GetComponent<RectTransform>();
            RectTransform contentItem = this.transform.Find("Viewport/Content/Item").GetComponent<RectTransform>();

            float areaHeight = contentArea.rect.height - viewport.rect.height;
            float itemHeight = contentItem.rect.height;

            float ratio = (itemHeight * dropdown.value) / areaHeight;
            scrollRect.verticalNormalizedPosition = 1f - Mathf.Clamp(ratio, 0f, 1f);
        }
    }

    /*
    // Update is called once per frame
    void Update()
    {
    }
    */
}
