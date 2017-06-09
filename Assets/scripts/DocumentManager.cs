using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DocumentManager : MonoBehaviour {

    public GameObject greenBubble, greenContainer, greyBubble, greyContainer, orangeBubble, orangeContainer, page;
    public Sprite[] backgrounds;
    public GameObject[][] bubbles;
    public GameObject[][] containers;
    private int activePage, activeColumn, lastGreenPulled, lastOrangePulled, lastGreyPulled;
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
        lastGreenPulled = -1;
        lastOrangePulled = -1;
        lastGreyPulled = -1;
        documentRenderer.SetPageCount(1);
    }

    public void AddMessage(Message _message) {

        ManageDocumentFlow();

        bool isMainMember = _message.emitter == mainMember;
        int bubbleType = _message.emitter == null ? 2 : isMainMember ? 0 : 1;
        //Debug.Log("bubbleType= " + bubbleType);
        GameObject newContainer;
        GameObject newBubble;
        if (bubbleType == 2) {
            lastGreyPulled++;
            newContainer = containers[2][lastGreyPulled];
            newBubble = bubbles[2][lastGreyPulled];
        } else {
            if (bubbleType == 0) {
                lastGreenPulled++;
                newContainer = containers[0][lastGreenPulled];
                newBubble = bubbles[0][lastGreenPulled];
            } else {
                lastOrangePulled++;
                newContainer = containers[1][lastOrangePulled];
                newBubble = bubbles[1][lastOrangePulled];
            }

        }

        newContainer.transform.SetParent(activeParent);
        newContainer.SetActive(true);

        newBubble.transform.SetParent(newContainer.transform);
        newBubble.SetActive(true);

        BubbleContentHandler bubbleHandler = newBubble.GetComponent<BubbleContentHandler>();
        bubbleHandler.relatedMessage = _message;
        bubbleHandler.Init(fileLoader.members.Length);
        bubbleHandler.SetText(_message.message);
        if (fileLoader.members.Length > 2 && _message.emitter != null) bubbleHandler.SetTitle(_message.emitter);
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

        if (accumulatedHeight > 1086f) {
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
        //string name = lastContainer == null ? "null" : lastContainer.name;
        //Rect r = lastContainer == null ? new Rect() : lastContainer.GetComponent<RectTransform>().rect;
        //Debug.Log("Page: " + activePage + ", Column: " + activeColumn + ", Parent: " + activeParent.gameObject.name + ", Container: " + name + ", Height: " + r);
    }

    public void SetMainMember() {
        mainMember = fileLoader.members[cmbMembers.value].name;
        GeneratePool();
        documentRenderer.LoadDocument();
    }

    public void AdjustLastBubbleSize() {
        if (lastBubble == null) {
            return;
        }
        lastBubble.ApplyMaxSize();
    }

    private void GeneratePool() {
        bubbles = new GameObject[3][];
        containers = new GameObject[3][];
        int mainCont = 0;
        GameObject pool = GameObject.Find("Pool");
        for (int i = 0; i < fileLoader.members.Length; i++) {
            if (fileLoader.members[i].name.Equals(mainMember)) {
                mainCont = fileLoader.members[i].messageCount;
            }
        }
        for (int i = 0; i < bubbles.Length; i++) {
            if (i == 0) {
                bubbles[i] = new GameObject[mainCont];
                containers[i] = new GameObject[mainCont];
            } else
            if (i == 1) {
                bubbles[i] = new GameObject[fileLoader.messages.Count - mainCont];
                containers[i] = new GameObject[fileLoader.messages.Count - mainCont];
            }
            if (i == 2) {
                bubbles[i] = new GameObject[fileLoader.GREYBUBBLECONT];
                containers[i] = new GameObject[fileLoader.GREYBUBBLECONT];
            }
            for (int j = 0; j < bubbles[i].Length; j++) {
                if (i == 0) {
                    bubbles[i][j] = Instantiate(greenBubble, pool.transform);
                    containers[i][j] = Instantiate(greenContainer, pool.transform);
                }
                if (i == 1) {
                    bubbles[i][j] = Instantiate(orangeBubble, pool.transform);
                    containers[i][j] = Instantiate(orangeContainer, pool.transform);
                }
                if (i == 2) {
                    bubbles[i][j] = Instantiate(greyBubble, pool.transform);
                    containers[i][j] = Instantiate(greyContainer, pool.transform);
                }
                bubbles[i][j].SetActive(false);
                containers[i][j].SetActive(false);
            }
        }
    }
}
