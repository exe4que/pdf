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
            lastContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(384, lastBubble.GetComponent<RectTransform>().sizeDelta.y + 20);
            accumulatedHeight += lastContainer.GetComponent<RectTransform>().rect.height;
        }
        
        if (accumulatedHeight > 1102f) {
            if (activeColumn == 1) {
                activeParent = activeParent.parent.FindChild("Column2");
                lastContainer.transform.SetParent(activeParent);
                activeColumn = 2;
                accumulatedHeight = lastContainer.GetComponent<RectTransform>().sizeDelta.y;
            } else {
                GameObject newPage = Instantiate(page, this.transform);
                activePage++;
                newPage.name = "Page" + activePage;
                activeParent = newPage.transform.FindChild("Column1");
                activeColumn = 1;
                accumulatedHeight = 0;
                lastContainer.transform.SetParent(activeParent);
                accumulatedHeight = lastContainer.GetComponent<RectTransform>().sizeDelta.y;
            }
        }
        string name = lastContainer == null ? "null" : lastContainer.name;
        Rect r = lastContainer == null ? new Rect() : lastContainer.GetComponent<RectTransform>().rect;
        Debug.Log("Page: " + activePage + ", Column: " + activeColumn + ", Parent: " + activeParent.gameObject.name + ", Container: " + name + ", Height: " + r);
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
