using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System;
#if UNITY_EDITOR || UNITY_STANDALONE
using System.Windows.Forms;
#endif

//code by Sammy Haddou - 03/08/2017 - AsteroGames

public class PdfGenerator : MonoBehaviour {

    private string pdfName = "My Generated PDF (" + System.DateTime.Now.ToString("yyy-MM-dd_HH-mm-ss") + ")";
    private string path = "";
    public Camera mainCam;
    byte[] imageBytes;
    RenderTexture rt;

    PageRenderer pageRenderer;
    bool takeScreenshoots = false;
    Texture2D[] docTextures;

    private void Init() {
        pageRenderer = GetComponent<PageRenderer>();
        docTextures = new Texture2D[pageRenderer.GetPageCount()];
        pageRenderer.Render();
        docTextures[pageRenderer.GetCurrentPage()] = TakeScreenshot(mainCam, 768, 1102);
    }
    private void Update() {
        if (takeScreenshoots) {
            if (pageRenderer.GetCurrentPage() < pageRenderer.GetPageCount()-1) {
                pageRenderer.NextPage();
                docTextures[pageRenderer.GetCurrentPage()] = TakeScreenshot(mainCam, 768, 1102);
            } else {
                takeScreenshoots = false;
                StartCoroutine("GeneratePDF2");
            }
        }
    }

    public void GeneratePDF() {
        Init();
        takeScreenshoots = true;
    }

        public void GeneratePDF2() {
        

#if UNITY_EDITOR || UNITY_STANDALONE

        SaveFileDialog dlg = new SaveFileDialog();
        dlg.DefaultExt = ".pdf";
        dlg.InitialDirectory = UnityEngine.Application.dataPath;
        dlg.Filter = "Pdf documents (.pdf)|*.pdf";
        dlg.FileName = pdfName;

        if (dlg.ShowDialog() == DialogResult.OK) {
            path = dlg.FileName;
            print("user have selected a path");
        } else if (dlg.ShowDialog() == DialogResult.Cancel || dlg.ShowDialog() == DialogResult.Abort) {
            path = "";
            print("user have closed the windows");
        }

        if (path != "") {
            createPDF(pdfName);
            print("pdf is saved !");
        }
#endif


    }

    public void createPDF(string fileName) {

        MemoryStream stream = new MemoryStream();

        Document doc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
        PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);

        PdfWriter.GetInstance(doc, new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None));


        BaseFont bfHelv = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);

        iTextSharp.text.Font fontNormal = new iTextSharp.text.Font(bfHelv, 10, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
        iTextSharp.text.Font fontBold = new iTextSharp.text.Font(bfHelv, 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);

        doc.Open();


        PdfPTable mainTable = new PdfPTable(1); //the table of the document
        mainTable.HorizontalAlignment = Element.ALIGN_CENTER;


        //PdfPCell tmpCell = new PdfPCell(); // a cell for the title
        //tmpCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
        //tmpCell.BorderWidth = 0;
        //tmpCell.AddElement(new Phrase("This is a title text", fontBold));
        //mainTable.AddCell(tmpCell);

        for (int i = 0; i < pageRenderer.GetPageCount(); i++) {


            doc.NewPage();

            PdfPCell tmpCell2 = new PdfPCell(); // a cell for the picture
            tmpCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
            tmpCell2.BorderWidth = 0;
            tmpCell2.Colspan = 2;

            AddImageToPDFCell(tmpCell2, docTextures[i], 570, 818);

            mainTable.AddCell(tmpCell2);
        }
        //PdfPCell tmpCell3 = new PdfPCell(); // a cell for the normal text
        //tmpCell3.Border = iTextSharp.text.Rectangle.NO_BORDER;
        //tmpCell3.BorderWidth = 0;
        //tmpCell3.AddElement(new Phrase("This is a normal text", fontNormal));
        //mainTable.AddCell(tmpCell3);

        doc.Add(mainTable);
        doc.Close();

        pdfWriter.Close();
        stream.Close();

        //Done editing the document, we close it and save it.
    }

    //used to take a raw screenshot from the app camera view.

    public static Texture2D TakeScreenshot(Camera cam, int width, int height) {

        RenderTexture rt = new RenderTexture(UnityEngine.Screen.width, UnityEngine.Screen.height, 24, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 8; //more quality
        cam.targetTexture = rt;
        cam.Render();

        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect((int)(UnityEngine.Screen.width * cam.rect.x), 0, width, height), 0, 0);
        tex.Apply();

        cam.targetTexture = null; //cleaning

        return tex;
    }

    //will draw the generated screenshot directly inside the PDF.

    public void AddImageToPDFCell(PdfPCell cell, Texture2D img, float scaleX, float scaleY) {

        imageBytes = img.EncodeToPNG();
        iTextSharp.text.Image finalImage = iTextSharp.text.Image.GetInstance(imageBytes);

        finalImage.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
        finalImage.Border = iTextSharp.text.Rectangle.NO_BORDER;
        finalImage.BorderColor = iTextSharp.text.BaseColor.WHITE;

        finalImage.ScaleToFit(scaleX, scaleY);

        cell.AddElement(finalImage);
    }
}