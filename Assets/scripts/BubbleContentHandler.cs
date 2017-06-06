using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleContentHandler : MonoBehaviour {

    public int maxSize = 300;

    ContentSizeFitter filter;
    RectTransform rect;
    public Text childText, childTitle, childTimestamp;
    public Message relatedMessage;

    public void Init (int _membersCount) {
        if (_membersCount <= 2) Destroy(childTitle);
        filter = this.GetComponent<ContentSizeFitter>();
        rect = this.GetComponent<RectTransform>();
        //childText = this.GetComponentInChildren<Text>();
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
        childText.text = _text;
    }

    public void SetTitle(string _title) {
        childTitle.text = _title;
    }

    public void SetTimestamp(string _time) {
        childTimestamp.text = _time;
    }
}
