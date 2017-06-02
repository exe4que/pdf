using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleContentHandler : MonoBehaviour {

    public int maxSize = 300;

    ContentSizeFitter filter;
    RectTransform rect;
    Text childText;

    void Init () {
        filter = this.GetComponent<ContentSizeFitter>();
        rect = this.GetComponent<RectTransform>();
        childText = this.GetComponentInChildren<Text>();
	}

    public void ApplyMaxSize() {
        //filter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        //Debug.Log(filter.horizontalFit + " - " + rect.rect);
        if (rect.rect.width > maxSize) {
            filter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            rect.sizeDelta = new Vector2(maxSize, rect.sizeDelta.y);
            
        }
    }

    public void SetText(string _text) {
        Init();
        childText.text = _text;
    }
}
