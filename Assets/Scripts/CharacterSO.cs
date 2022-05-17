using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
[CreateAssetMenu(fileName = "D", menuName = "EDitor")]
public class CharacterSO : ScriptableObject
{
    //https://answers.unity.com/questions/12598/help-me-about-reading-txt-file.html
    public TextAsset dictionaryTextFile;
    private string theWholeFileAsOneLongString;
    private List<string> eachLine;
    public List<Speaker> speakers;

    [ContextMenu("Create")]
    public void Create()
    {
        theWholeFileAsOneLongString = dictionaryTextFile.text;

        eachLine = new List<string>();
        eachLine.AddRange(theWholeFileAsOneLongString.Split("\n"[0]));

        events = CreateDialoge2(eachLine);
        
    }
    public List<DialougeEvent> events = new List<DialougeEvent>();
    /*
    public List<DialougeEvent> CreateDialoge(List<string> lines)
    {
        List<DialougeEvent> e = new List<DialougeEvent>();
        int i = 0;
        Speaker last = speakers[0];//??
        Chunk current = new Chunk(last, "");
        while(i < lines.Count -1 )
        {
            string l = lines[i];
            if (l.IndexOf("Scene") > -1)
            {
                e.Add(current);
                current = new Chunk(last, "");
                break;
            }
            else if (l.IndexOf("Enter") > -1)
            {
                e.Add(current);
                current = new Chunk(last, "");
                e.Add(new Enter(ParseStringForSpeakers(l)));
            }
            else if (l.IndexOf("Exit") > -1)
            {
                e.Add(current);
                current = new Chunk(last, "");
                List<Speaker> found = ParseStringForSpeakers(l);
                if (found.Count > 0)
                {
                    e.Add(new Exit(found));
                }
                else
                {
                    found.Add(last);
                    e.Add(new Exit(found));
                }
            }
            else if(ParseStringForSpeakers(l).Count > -1)
            {
                current.speaker = ParseStringForSpeakers(l)[0];
            }
            else
            {
                current.text += l;
            }
            i++;
        }
        return e;
    }*/
    public List<DialougeEvent> CreateDialoge2(List<string> lines)
    {
        List<DialougeEvent> e = new List<DialougeEvent>();
        int i = 0;
        while (i < lines.Count - 1)
        {
            string l = lines[i];

            Speaker s;
            if (ParseStringForSpeakers(l).Count > -1)
            {
                s = ParseStringForSpeakers(l)[0];
                while(ParseStringForSpeakers(lines[i]).Count > -1 && i < lines.Count - 1)
                {
                    Debug.Log(ParseStringForSpeakers(l)[0].name);
                    e.Add(new DialougeEvent(ParseStringForSpeakers(l)[0], lines[i]));
                    i++;
                }
            }
            
            i++;
        }
        return e;
    }




    public List<Speaker> ParseStringForSpeakers(string line)
    {
        List<Speaker> found = new List<Speaker>();
        List<string> sp = Split(line, " ").ToList();
        for (int i = 0; i < sp.Count; i++)
        {
            for (int j = 0; j < speakers.Count; j++)
            {
                if (sp[i].IndexOf(speakers[j].name) > -1)
                {
                    found.Add(speakers[j]);
                }
            }
        }
        return found;
    }

    string[] Split(string phrase, string split)
    {
        List<string> o = new List<string>();
        string curr = "";
        for (int i = 0; i < phrase.Length; i++)
        {
            if (phrase.Substring(i, 1) == split)
            {
                o.Add(curr);
                curr = "";
            }
            else
            {
                curr += phrase.Substring(i, 1);
            }
        }
        return o.ToArray();
    }
}
[System.Serializable]
public class Speaker
{
    public string name = "Jill";
    public Color color;
    public Sprite sprite;

    public static Speaker None
    {
        get
        {
            return new Speaker()
            {
                name = "???",
                color = Color.grey,
            };
        }
    }
    //public override bool Equals(object obj) { return ((Speaker)obj).name == this.name; }
}
public enum ActionType { Text, Enter, Exit }
[System.Serializable]
public class DialougeEvent
{
    public ActionType action = ActionType.Text;

    public Speaker speaker;
    public string text;


    public DialougeEvent(Speaker speaker, string text)
    {
        action = ActionType.Text;
        this.speaker = speaker;
        this.text = text;
    }


    public List<Speaker> speakers;
    public DialougeEvent(ActionType actionType, List<Speaker> speakers)
    {
        action = actionType;
        this.speakers = speakers;
    }

    public virtual void Invoke()
    {
        if(action == ActionType.Text)
        {
            DR.i.SetSpeaker(speaker);
            DR.i.fullText = text;
            DR.i.StartCoroutine(DR.i.Read(text));
        }
        else if( action == ActionType.Enter)
        {
            foreach (Speaker s in speakers)
                DR.i.speakers.Add(s);
        }
        else if(action == ActionType.Exit)
        {
            for (int i = 0; i < DR.i.speakers.Count; i++)
            {
                for (int j = 0; j < speakers.Count; j++)
                {
                    if (DR.i.speakers[i].name == speakers[j].name)
                    {
                        DR.i.speakers.RemoveAt(i);
                    }
                }
            }
        }
    }

    public virtual bool HasText(out string str)
    {
        if(action == ActionType.Text)
        {
            str = text;
            return true;
        }
        str = "";
        return false;
    }
}

/*

Sample Dialouge:

Name1:
hi.



[System.Serializable]
public class Chunk : DialougeEvent
{

    public Speaker speaker;
    public string text;

    public Chunk(Speaker speaker, string text)
    {
        this.speaker = speaker;
        this.text = text;
    }

    public override void Invoke()
    {
        DR.i.SetSpeaker(speaker);
        DR.i.fullText = text;
        DR.i.StartCoroutine(DR.i.Read(text));
    }
    public override bool HasText(out string str)
    {
        str = text;
        return true;
    }
}
[System.Serializable]
public class Enter : DialougeEvent
{
    public Enter(List<Speaker> speakers)
    {
        this.speakers = speakers;
    }
    public Enter(Speaker speaker)
    {
        speakers = new List<Speaker>();
        speakers.Add(speaker);
    }

    public override void Invoke()
    {
        foreach (Speaker s in speakers)
            DR.i.speakers.Add(s);
    }
}
[System.Serializable]
public class Exit : DialougeEvent
{
    public Exit(List<Speaker> speakers)
    {
        this.speakers = speakers;
    }
    public Exit(Speaker speaker)
    {
        speakers = new List<Speaker>();
        speakers.Add(speaker);
    }

    public override void Invoke()
    {
        //foreach (Speaker s in speakers) { DR.i.speakers.Remove(s); }

        for (int i = 0; i < DR.i.speakers.Count; i++)
        {
            for (int j = 0; j < speakers.Count; j++)
            {
                if (DR.i.speakers[i].name == speakers[j].name)
                {
                    DR.i.speakers.RemoveAt(i);
                }
            }
        }
    }
}

*/