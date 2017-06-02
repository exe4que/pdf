using System.IO;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Linq;
#if UNITY_EDITOR || UNITY_STANDALONE
using System.Windows.Forms;
#endif

public class FileLoader : MonoBehaviour {

    public string[] sourceText;
    public Message[] messages;
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
            CleanSourceText();
            messages = new Message[sourceText.Length];
            for (int i = 0; i < sourceText.Length; i++) {
                messages[i] = LineToMessage(sourceText[i]);
            }

            btnGenerate.interactable = true;
        } else{
            btnGenerate.interactable = false;
        }
    }

    private void CleanSourceText() {
        for (int i = sourceText.Length-1; i > -1; i--) {

            Regex rgx = new Regex(@"\d{1,2}/\d{1,2}/\d{2}");
            Match mat = sourceText[i].Length < 8 ? rgx.Match(sourceText[i]) : rgx.Match(sourceText[i], 0, 8);
            //Debug.Log(mat);
            if (mat.ToString().Equals(String.Empty)) {
                if ((i - 1) >= 0) {
                    sourceText[i - 1] = sourceText[i - 1] +"\n"+ sourceText[i];
                    //Debug.Log(sourceText[i - 1]);
                }
                sourceText[i] = null;
                continue;
            }

            if (sourceText[i].Split(new string[] { ": " }, StringSplitOptions.None).Length == 1) {
                sourceText[i] = null;
                continue;
            }

        }
        sourceText = sourceText.Where(c => c != null).ToArray();
    }

    private Message LineToMessage(string _line) {
        string[] split = _line.Split(new string[] { ": " }, StringSplitOptions.None);

        string message = split[1].TrimStart();
        split = split[0].Split('-');
        string emitter = split[1].TrimStart(); 
        string dateTimeString = split[0].TrimEnd();
        dateTimeString = dateTimeString.Replace(",", string.Empty);
        DateTime dateTime = DateTime.Parse(dateTimeString);
        return new Message(dateTime, emitter, message);
    }

}
[Serializable]
public struct Message {
    public DateTime dateTime;
    public string emitter;
    public string message;

    public Message(DateTime _dateTime, string _emitter, string _message) {
        dateTime = _dateTime;
        emitter = _emitter;
        message = _message;
    }


}


