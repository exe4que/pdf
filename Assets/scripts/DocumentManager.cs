using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DocumentManager : MonoBehaviour {

    public GameObject greenBubble, greenContainer, orangeBubble, orangeContainer, page;
    private int activePage, activeColumn;
    private Transform activeParent;
    private string mainMember;
    private Dropdown cmbMembers;
    private FileLoader fileLoader;
    private BubbleContentHandler lastBubble;
    private Transform lastContainer;
    private float accumulatedHeight;

    private void Awake() {
        activeParent = GameObject.Find("Column1").transform;
        cmbMembers = GameObject.FindGameObjectWithTag("Members").GetComponent<Dropdown>();
        fileLoader = GameObject.FindGameObjectWithTag("Engine").GetComponent<FileLoader>();
        activePage = 1;
        activeColumn = 1;
    }

    public void AddMessage(Message _message) {
        
        ManageDocumentFlow();

        bool isMainMember = _message.emitter == mainMember;
        GameObject bubble = isMainMember ? greenBubble : orangeBubble;
        GameObject container = isMainMember ? greenContainer : orangeContainer;

        GameObject newContainer = Instantiate(container, activeParent);
        GameObject newBubble = Instantiate(bubble, newContainer.transform);

        BubbleContentHandler bubbleHandler = newBubble.GetComponent<BubbleContentHandler>();
        bubbleHandler.SetText(_message.message);
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
                lastBubble.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset(18, 5, 0, 0);
                Text txt = lastBubble.GetComponentInChildren<Text>();
                txt.resizeTextMinSize = 0;
                txt.resizeTextForBestFit = true;
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
