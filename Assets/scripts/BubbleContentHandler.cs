using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleContentHandler : MonoBehaviour {

    public int maxSize = 300;

    ContentSizeFitter filter;
    RectTransform rect;
    Text childText;

    void Awake () {
        filter = GetComponent<ContentSizeFitter>();
        rect = GetComponent<RectTransform>();
        childText = GetComponentInChildren<Text>();
	}

    public void ApplyMaxSize() {
        //filter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        if (rect.rect.width > maxSize) {
            filter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            rect.sizeDelta = new Vector2(maxSize, rect.sizeDelta.y);
        }
    }

    public void SetText(string _text) {
        childText.text = _text;
        ApplyMaxSize();
    }
}
