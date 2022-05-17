using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VisualNovel
{
    [CreateAssetMenu(fileName = "Dialouge", menuName = "Dialouge")]
    public class DialougeSO : ScriptableObject
    {
        public string characterName = "Name";
        public List<DialougeChunk> dialouge = new List<DialougeChunk>();
        public List<DialougeEvent> events = new List<DialougeEvent>();

        public DialougeSO ()
        {
            characterName = "Name";
            dialouge = new List<DialougeChunk>();
            editText = "";
        }
        public DialougeSO (string str)
        {
            editText = str;
            StringToDialouge();
        }
        public void StringToDialouge(string str)
        {
            editText = str;
            StringToDialouge();
        }
        //bool editing = false;
        [SerializeField, TextArea(5, 100)] private string editText = "";
        [ContextMenu("Add Chunck")]
        public void AddChunk()
        {
            List<Choice> c = new List<Choice>();
            c.Add(new Choice("", dialouge.Count + 1));
            dialouge.Add(new DialougeChunk(dialouge.Count, editText, c));
        }
        [ContextMenu("String To Dialouge")]
        public void StringToDialouge()
        {
            dialouge = new List<DialougeChunk>();
            string[] chunks = Split(editText, ";");//editText.Split(';');
            characterName = chunks[0];
            //Debug.Log(string.Join(",", chunks));
            for (int c=1; c<chunks.Length; c++)
            {
                string[] ra = chunks[c].Split('/');
                List<Choice> choices = new List<Choice>();
                for(int i=1; i+1<ra.Length;i+=2)
                {
                    if (Int32.TryParse(ra[i+1], out int next))
                        choices.Add(new Choice(ra[i], next));
                }
                dialouge.Add(new DialougeChunk(c, ra[0].Trim(), choices));
            }
        }
        [ContextMenu("String To Dialouge 2")]
        public void StringToDialouge2()
        {
            events = new List<DialougeEvent>();
            string[] chunks = editText.Split("\n"[0]);
            for (int c = 0; c < chunks.Length; c++)
            {
                events.Add(new DialougeEvent(Speaker.None, chunks[c]));
            }
        }
        /*
        name1;
        hello/
        howdy/
        how are you?/
        name2;
        I am good/

         */
        string[] Split(string phrase, string split)
        {
            List<string> o = new List<string>();
            string curr = "";
            for(int i=0; i<phrase.Length; i++)
            {
                if(phrase.Substring(i, 1) == split)
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
    public class DialougeChunk
    {
        string n = "";
        public string name
        {
            get
            {
                return "(" + index + ") " + text + " " + n;
            }
            set
            {
                n = value;
            }
        }

        public int index = 0;
        public string text = "Speaking now!";
        public List<Choice> choices;

        public DialougeChunk(int i, string text, List<Choice> choices)
        {
            index = i;
            this.text = text;
            this.choices = choices;
        }
    }

    [System.Serializable]
    public class Choice
    {
        public string text = "Choice 0";
        public int to = 0;
        public Choice(string text, int to)
        {
            this.text = text;
            this.to = to;
        }
    }
}