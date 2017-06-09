using System.IO;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Globalization;
using System.Text;
#if UNITY_EDITOR || UNITY_STANDALONE
using System.Windows.Forms;
#endif

public class FileLoader : MonoBehaviour {

    public List<string> sourceText;
    public List<Message> messages;
    public Member[] members;
    public int GREYBUBBLECONT = 0;

    private UnityEngine.UI.Button btnGenerate;
    private Dropdown cmbMembers;
    private int lastMessageDay = -1;

    private void Awake() {
        members = new Member[30];
        btnGenerate = GameObject.Find("btnGeneratePdf").GetComponent<UnityEngine.UI.Button>();
        cmbMembers = GameObject.FindGameObjectWithTag("Members").GetComponent<Dropdown>();
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

            sourceText = content.Split('\n').ToList<string>();
            CleanSourceText();
            //messages = new Message[sourceText.Length];
            //for (int i = 0; i < sourceText.Length; i++) {
            //    messages[i] = LineToMessage(sourceText[i]);
            //}

            foreach (string line in sourceText) {
                messages.Add(LineToMessage(line));
            }
            sourceText.Clear();
            RemoveNullMembers();
            cmbMembers.options.Clear();
            foreach (Member member in members) {
                cmbMembers.options.Add(new Dropdown.OptionData(member.name));
            }

            cmbMembers.interactable = true;
            btnGenerate.interactable = true;
        } else {
            cmbMembers.interactable = false;
            btnGenerate.interactable = false;
        }
    }

    private void CleanSourceText() {
        for (int i = sourceText.Count - 1; i > -1; i--) {

            Regex rgx = new Regex(@"\d{1,2}/\d{1,2}/\d{2}");
            Match mat = sourceText[i].Length < 8 ? rgx.Match(sourceText[i]) : rgx.Match(sourceText[i], 0, 8);
            //Debug.Log(mat);
            if (mat.ToString().Equals(String.Empty)) {
                if ((i - 1) >= 0) {
                    sourceText[i - 1] = sourceText[i - 1] + "\n" + sourceText[i];
                    //Debug.Log(sourceText[i - 1]);
                }
                sourceText[i] = null;
                continue;
            }

            //if (sourceText[i].Split(new string[] { ": " }, StringSplitOptions.None).Length == 1) {
            //    sourceText[i] = null;
            //    continue;
            //}

        }
        sourceText = sourceText.Where(c => c != null).ToList<string>();
    }

    private Message LineToMessage(string _line) {
        string[] split;
        string message;
        string emitter;
        bool hasImage = false;
        if (_line.Contains(": ")) {
            split = _line.Split(new string[] { ": " }, StringSplitOptions.None);
            message = split[1].TrimStart();
            split = split[0].Split('-');
            emitter = split[1].TrimStart();

            bool found = false;
            for (int i = 0; i < members.Length && !found; i++) {
                if (members[i].name == null) {
                    members[i] = new Member(emitter);
                    found = true;
                } else {
                    if (members[i].name.Equals(emitter)) {
                        members[i].messageCount++;
                        found = true;
                    }
                }
            }
        } else {
            split = _line.Split('-');
            message = split[1].TrimStart();
            emitter = null;
        }
        if (message.Contains("<Archivo omitido>")) {
            hasImage = true;
            message = message.Replace("<Archivo omitido>", String.Empty);
        }

        
        string dateTimeString = split[0].TrimEnd();
        dateTimeString = dateTimeString.Replace(",", string.Empty);
        DateTime dateTime = Convert.ToDateTime(dateTimeString, new System.Globalization.CultureInfo("es-ES", true));

        if (emitter == null || dateTime.Day != lastMessageDay) {
            GREYBUBBLECONT++;
        }
        if (dateTime.Day != lastMessageDay) {
            lastMessageDay = dateTime.Day;
        }
        return new Message(dateTime, emitter, message, hasImage);
    }

    string DecodeUTF16(string text) {
        return Regex.Replace(
            text,
            @"\\U(?<Value>[a-zA-Z0-9]{8})",
            m => char.ConvertFromUtf32(int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)));
    }

    private void RemoveNullMembers() {
        List<Member> list = new List<Member>();
        for (int i = 0; i < members.Length; i++) {
            if (members[i].name!=null) {
                list.Add(members[i]);
            }
        }
        members = list.ToArray();
    }

}
[Serializable]
public struct Message {
    public DateTime dateTime;
    public string emitter;
    public string message;
    public bool hasImage;

    public Message(DateTime _dateTime, string _emitter, string _message, bool _hasImage) {
        dateTime = _dateTime;
        emitter = _emitter;
        message = _message;
        hasImage = _hasImage;
    }
}
[Serializable]
public struct Member {
    public string name;
    public int messageCount;

    public Member(string _name) {
        name = _name;
        messageCount = 1;
    }

    public void IncreaseCounter() {
        messageCount++;
    }
}




