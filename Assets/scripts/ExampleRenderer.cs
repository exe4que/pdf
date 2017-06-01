using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleRenderer : MonoBehaviour, PageRenderer {
    public Transform logo;
    public Vector2[] positions;
    private int pos = 0;

    public int GetCurrentPage() {
        return pos;
    }

    public int GetPageCount() {
        return positions.Length;
    }

    public void LoadDocument() {
        throw new NotImplementedException();
    }

    public void NextPage() {
        pos = pos < positions.Length - 1 ? pos + 1 : pos;
        Render();
    }

    public void Page(int _page) {
        pos = _page;
        Render();
    }

    public void PreviousPage() {
        pos = pos > 0? pos - 1 : pos;
        Render();
    }

    public void Render() {
        logo.position = positions[pos];
    }
}


