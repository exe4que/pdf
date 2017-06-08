using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;

public class DocumentRenderer : MonoBehaviour, PageRenderer {

    public Camera attachedCamera;
    private FileLoader fileLoader;
    private DocumentManager documentManager;
    private bool start = false;
    private int index=0, tick=1, pageCount, activePage = 1;
    private DateTime lastDate;

    private void Update() {
        if (start && index < fileLoader.messages.Count) {
            if (tick==1) {
                documentManager.AdjustLastBubbleSize();
            }
            if(tick==2) {
                Message mes = fileLoader.messages[index];
                if (lastDate == null || lastDate.ToShortDateString() != mes.dateTime.ToShortDateString()) {
                    lastDate = mes.dateTime;
                    Message dateTag = new Message(mes.dateTime, null, mes.dateTime.ToLongDateString(), false);
                    documentManager.AddMessage(dateTag);
                } else {
                    documentManager.AddMessage(fileLoader.messages[index]);
                    index++;

                    float perc = (index * 100) / fileLoader.messages.Count;
                    Debug.Log(index + "/" + fileLoader.messages.Count + " (" + perc + "%)");
                }
                tick = 0;
            }
            tick++;
        }
    }

    private void Init() {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
        fileLoader = GetComponent<FileLoader>();
        documentManager = GameObject.FindGameObjectWithTag("DocumentManager").GetComponent<DocumentManager>();
        start = true;
    }
    public int GetCurrentPage() {
        return activePage;
    }

    public int GetPageCount() {
        return pageCount;
    }

    public void LoadDocument() {
        Init();
    }

    public void NextPage() {
        activePage++;
        attachedCamera.transform.position += Vector3.up * -10;
    }

    public void Page(int _page) {
        activePage = _page;
        attachedCamera.transform.position = Vector3.up * (_page-1) * -10;
    }

    public void PreviousPage() {
        activePage--;
        attachedCamera.transform.position += Vector3.up * 10;
    }

    public void Render() {
    }

    public void SetPageCount(int _count) {
        pageCount = _count;
    }
}
