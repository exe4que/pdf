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
    FileLoader fileLoader;

    private void Awake() {
        activeParent = GameObject.Find("Column1").transform;
        cmbMembers = GameObject.FindGameObjectWithTag("Members").GetComponent<Dropdown>();
        fileLoader = GameObject.FindGameObjectWithTag("Engine").GetComponent<FileLoader>();
        activePage = 1;
        activeColumn = 1;
    }

    public void AddMessage(Message _message) {
        bool isMainMember = _message.emitter == mainMember;
        GameObject bubble = isMainMember ? greenBubble : orangeBubble;
        GameObject container = isMainMember ? greenContainer : orangeContainer;

        GameObject newContainer = Instantiate(container, activeParent);
        GameObject newBubble = Instantiate(bubble, newContainer.transform);

        BubbleContentHandler bubbleHandler = newBubble.GetComponent<BubbleContentHandler>();
        bubbleHandler.SetText(_message.message);
    }

    public void SetMainMember() {
        mainMember = fileLoader.members[cmbMembers.value];
    }
}
