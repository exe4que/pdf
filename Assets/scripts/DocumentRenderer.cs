using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocumentRenderer : MonoBehaviour, PageRenderer {

    private FileLoader fileLoader;
    private DocumentManager documentManager;
    private bool start = false, tick = true;
    private int index;

    private void Update() {
        if (start && index < fileLoader.messages.Length) {
            if (tick) {
                documentManager.AdjustLastBubbleSize();
            } else {
                documentManager.AddMessage(fileLoader.messages[index]);
                index++;
            }
            tick = !tick;
        }
    }

    private void Init() {
        fileLoader = GetComponent<FileLoader>();
        documentManager = GameObject.FindGameObjectWithTag("DocumentManager").GetComponent<DocumentManager>();
        start = true;
    }
    public int GetCurrentPage() {
        throw new NotImplementedException();
    }

    public int GetPageCount() {
        return 1;
    }

    public void LoadDocument() {
        Init();
    }

    public void NextPage() {
        throw new NotImplementedException();
    }

    public void Page(int _page) {
        throw new NotImplementedException();
    }

    public void PreviousPage() {
        throw new NotImplementedException();
    }

    public void Render() {
        throw new NotImplementedException();
    }
}
