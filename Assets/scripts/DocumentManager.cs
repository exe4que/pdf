using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DocumentManager : MonoBehaviour {

    public GameObject greenBubble, greenContainer, greyBubble, greyContainer, orangeBubble, orangeContainer, page;
    public Sprite[] backgrounds;
    private int activePage, activeColumn;
    private Transform activeParent;
    private string mainMember;
    private Dropdown cmbMembers;
    private FileLoader fileLoader;
    private DocumentRenderer documentRenderer;
    private BubbleContentHandler lastBubble;
    private Transform lastContainer;
    private float accumulatedHeight;
    private int lastBackground = 0;

    private void Awake() {
        activeParent = GameObject.Find("Column1").transform;
        cmbMembers = GameObject.FindGameObjectWithTag("Members").GetComponent<Dropdown>();
        fileLoader = GameObject.FindGameObjectWithTag("Engine").GetComponent<FileLoader>();
        documentRenderer = GameObject.FindGameObjectWithTag("Engine").GetComponent<DocumentRenderer>();
        activePage = 1;
        activeColumn = 1;
        documentRenderer.SetPageCount(1);
    }

    public void AddMessage(Message _message) {
        
        ManageDocumentFlow();

        bool isMainMember = _message.emitter == mainMember;
        GameObject bubble = _message.emitter == null ? greyBubble : isMainMember ? greenBubble : orangeBubble;
        GameObject container = _message.emitter == null ? greyContainer : isMainMember ? greenContainer : orangeContainer;

        GameObject newContainer = Instantiate(container, activeParent);
        GameObject newBubble = Instantiate(bubble, newContainer.transform);
        
        BubbleContentHandler bubbleHandler = newBubble.GetComponent<BubbleContentHandler>();
        bubbleHandler.relatedMessage = _message;
        bubbleHandler.Init(fileLoader.members.Count);
        bubbleHandler.SetText(_message.message);
        if (fileLoader.members.Count > 2 && _message.emitter != null) bubbleHandler.SetTitle(_message.emitter);
        if (_message.emitter != null) bubbleHandler.SetTimestamp(_message.dateTime.ToShortTimeString());
        
        lastBubble = bubbleHandler;
        lastContainer = newContainer.transform;
    }

    private void ManageDocumentFlow() {
        if (lastContainer != null) {
            RectTransform lastBubbleTransform = lastBubble.GetComponent<RectTransform>();
            RectTransform lastContainerTransform = lastContainer.GetComponent<RectTransform>();
            if (lastBubbleTransform.sizeDelta.y <= 1080) {
                lastContainerTransform.sizeDelta = new Vector2(384, lastBubbleTransform.sizeDelta.y + 20);
            } else {
                lastContainerTransform.sizeDelta = new Vector2(384, 1080);
                lastContainer.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset(20, 0, 0, 0);
                lastBubble.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(18, 5, 0, 0);
                TextMeshProUGUI txt;
                foreach (Transform t in lastBubble.transform) {
                    if (t.gameObject.name == "Text") {
                        txt = t.GetComponent<TextMeshProUGUI>();
                        txt.fontSizeMin = 5;
                        txt.enableAutoSizing = true;
                        txt.overflowMode = TextOverflowModes.Truncate;
                        //txt.text = lastBubble.GetComponent<BubbleContentHandler>().relatedMessage.message;
                    }
                }
                lastBubble.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
                lastBubbleTransform.sizeDelta = new Vector2(340, 1080);
            }
            
            accumulatedHeight += lastContainerTransform.sizeDelta.y;
        }
        
        if (accumulatedHeight > 1082f) {
            if (activeColumn == 1) {
                foreach (Transform tra in activeParent.parent) {
                    if (tra.gameObject.name == "Column2") {
                        activeParent = tra;
                    }
                }
                activeColumn = 2;
            } else {
                GameObject newPage = Instantiate(page, this.transform);
                activePage++;
                newPage.name = "Page" + activePage;
                documentRenderer.SetPageCount(activePage);
                int backIndex;
                do {
                    backIndex = (int) (UnityEngine.Random.Range(0, backgrounds.Length - 1));
                } while (backIndex == lastBackground);
                lastBackground = backIndex;
                newPage.GetComponent<Image>().sprite = backgrounds[backIndex]; 
                foreach (Transform tra in newPage.transform) {
                    if (tra.gameObject.name == "Column1") {
                        activeParent = tra;
                    }
                }
                activeColumn = 1;
                
            }
            lastContainer.transform.SetParent(activeParent);
            accumulatedHeight = lastContainer.GetComponent<RectTransform>().sizeDelta.y;
        }
        string name = lastContainer == null ? "null" : lastContainer.name;
        Rect r = lastContainer == null ? new Rect() : lastContainer.GetComponent<RectTransform>().rect;
        //Debug.Log("Page: " + activePage + ", Column: " + activeColumn + ", Parent: " + activeParent.gameObject.name + ", Container: " + name + ", Height: " + r);
    }

    public void SetMainMember() {
        mainMember = fileLoader.members[cmbMembers.value];
    }

    public void AdjustLastBubbleSize() {
        if (lastBubble == null) {
            return;
        }
        lastBubble.ApplyMaxSize();
    }
}
