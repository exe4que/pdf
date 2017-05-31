public interface PageRenderer {
    void Page(int _page);
    void NextPage();
    void PreviousPage();
    void Render();
    int GetPageCount();
    int GetCurrentPage();
}
