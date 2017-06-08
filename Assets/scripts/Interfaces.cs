public interface PageRenderer {
    void Page(int _page);
    void NextPage();
    void PreviousPage();
    void Render();
    int GetPageCount();
    void SetPageCount(int _count);
    int GetCurrentPage();
    void LoadDocument();
}
