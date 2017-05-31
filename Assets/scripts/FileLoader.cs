using System.IO;
using UnityEngine;
#if UNITY_EDITOR || UNITY_STANDALONE
using System.Windows.Forms;
#endif

public class FileLoader : MonoBehaviour {

    public string[] sourceText;
    UnityEngine.UI.Button btnGenerate;

    private void Awake() {
        btnGenerate = GameObject.Find("btnGeneratePdf").GetComponent<UnityEngine.UI.Button>();
    }

    public void LoadFile() {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.DefaultExt = ".txt";
        dialog.Filter = "Text documents (.txt)|*.txt";
        DialogResult result = dialog.ShowDialog();
        if (result == DialogResult.OK) {
            string path = dialog.FileName;
            FileStream fs = new FileStream(path, FileMode.Open);
            string content = "";
            using (StreamReader read = new StreamReader(fs, true)) {
                content = read.ReadToEnd();
            }

            sourceText = content.Split('\n');

            btnGenerate.interactable = true;
        } else{
            btnGenerate.interactable = false;
        }
    }
}
