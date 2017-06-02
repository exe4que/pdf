using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocumentRenderer : MonoBehaviour, PageRenderer {

    FileLoader fileLoader;
    DocumentManager documentManager;

    private void Init() {
        fileLoader = GetComponent<FileLoader>();
        documentManager = GameObject.FindGameObjectWithTag("DocumentManager").GetComponent<DocumentManager>();
    }
    public int GetCurrentPage() {
        throw new NotImplementedException();
    }

    public int GetPageCount() {
        throw new NotImplementedException();
    }

    public void LoadDocument() {
        Init();
        for (int i = 0; i < fileLoader.messages.Length; i++) {
            documentManager.AddMessage(fileLoader.messages[i]);
        }
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
