using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BubbleContentHandler : MonoBehaviour {

    public int maxSize = 300;

    ContentSizeFitter filter;
    RectTransform rect;
    private TextMeshProUGUI childText, childTitle, childTimestamp;
    private Image childImage;
    public Message relatedMessage;

    public void Init (int _membersCount) {
        filter = this.GetComponent<ContentSizeFitter>();
        rect = this.GetComponent<RectTransform>();
        foreach (Transform t in transform) {
            if (t.name.Equals("Title")) childTitle = t.gameObject.GetComponent<TextMeshProUGUI>();
            if (t.name.Equals("Text")) childText = t.GetComponent<TextMeshProUGUI>();
            if (t.name.Equals("Timestamp")) childTimestamp = t.GetComponent<TextMeshProUGUI>();
            if (t.name.Equals("Image")) childImage = t.GetComponent<Image>();
        }
        if (_membersCount <= 2) Destroy(childTitle);
        if (!relatedMessage.hasImage) Destroy(childImage);
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
        childText.SetText(_text);
    }

    public void SetTitle(string _title) {
        childTitle.text = _title;
    }

    public void SetTimestamp(string _time) {
        childTimestamp.text = _time;
    }
}
