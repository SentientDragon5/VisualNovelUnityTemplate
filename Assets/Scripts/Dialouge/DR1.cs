using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

namespace VisualNovel
{
    public class DR1 : MonoBehaviour
    {

        public DialougeSO d;
        public CharacterSO castSo;
        public List<DEvent> events;
        public List<Speaker> cast;
        public List<Speaker> speakers;
        public int speakerIndex;
        public List<int> speakerIndies;

        public TextAsset textAsset;
        [SerializeField, TextArea(5, 100)] private string big;

        [ContextMenu("Make from Big")]
        public void Set()
        {
            List<Speaker> cast = castSo.speakers;
            events = DEvent.Create(cast, big);
        }
        [ContextMenu("Make from Asset")]
        public void SetAsset()
        {
            cast = castSo.speakers;
            events = DEvent.Create(cast, textAsset.text);
        }

        [SerializeField] private TextMeshProUGUI textUI;
        [SerializeField] private TextMeshProUGUI nameUI;
        public Color colorSV = Color.grey;

        public int i = 0;

        public Speaker speaker = Speaker.None;
        public string text = "";

        public List<Image> currentImages;
        public RectTransform characterParent;
        public GameObject imagePrefab;

        public Vector2 leftAnchor;
        public Vector2 rightAnchor;
        public Transform leftAnchorT;
        public Transform rightAnchorT;

        public void Start()
        {
            i = -1;
            text = "";
            Next();
            DisplaySprites();
            Display();
        }
        [ContextMenu("Next")]
        public void Next()
        {
            i++;
            if (i >= events.Count)
            {
                // End
            }
            else
            {
                if(events[i].action == ActionType.Text)
                {
                    speakerIndex = events[i].speakerIndex;
                    text = events[i].text;
                    Display();
                }
                else if(events[i].action == ActionType.Enter)
                {
                    //speakers.AddRange(events[i].speakers);
                    speakerIndies.AddRange(events[i].speakerIndies);
                    DisplaySprites();
                }
                else if(events[i].action == ActionType.Exit)
                {
                    /*
                    for (int i = 0; i < speakers.Count; i++)
                    {
                        for (int j = 0; j < events[i].speakers.Count; j++)
                        {
                            if (speakers[i].name == events[i].speakers[j].name)
                            {
                                speakers.RemoveAt(i);
                            }
                        }
                    }
                    for (int i = 0; i < speakerIndies.Count; i++)
                    {
                        for (int j = 0; j < events[i].speakerIndies.Count; j++)
                        {
                            if (speakerIndies[i] == events[i].speakerIndies[j])
                            {
                                speakerIndies.RemoveAt(i);
                            }
                        }
                    }*/
                    foreach(int i in events[i].speakerIndies)
                    {
                        speakerIndies.RemoveAll(u => u == i);
                    }
                    DisplaySprites();
                }
            }
        }

        [ContextMenu("Disp")]
        public void Display()
        {
            float H, S, V;
            float Hb, Sb, Vb;
            Color.RGBToHSV(cast[speakerIndex].color, out H, out S, out V);
            Color.RGBToHSV(colorSV, out Hb, out Sb, out Vb);
            Color nameCol = cast[speakerIndex].color;//Color.HSVToRGB(H, Sb, Vb);
            nameCol.a = 1;
            nameUI.color = nameCol;
            nameUI.text = cast[speakerIndex].name + ":";
            textUI.text = text;
        }

        public void DisplaySprites()
        {
            leftAnchor = leftAnchorT.position;
            rightAnchor = rightAnchorT.position;
            DestroyAllChildren(characterParent);
            int numSpeakers = speakerIndies.Count;
            Vector3 left = new Vector3(leftAnchor.x, leftAnchor.y, 0);
            Vector3 right = new Vector3(rightAnchor.x, rightAnchor.y, 0);
            if (numSpeakers < 1) return;
            for (int i = 0; i < numSpeakers; i++)
            {
                //Vector3 position = Vector3.Lerp(leftAnchorT.position, rightAnchorT.position, (float)(i + 1) / (numSpeakers + 1));
                GameObject g = Instantiate(imagePrefab, characterParent);
                //g.GetComponent<SpriteRenderer>().sprite = cast[speakerIndies[i]].sprite;
                g.GetComponent<Image>().sprite = cast[speakerIndies[i]].sprite;
                g.GetComponent<RectTransform>().anchorMin = new Vector2((float)(i + 1) / (numSpeakers + 1), 0.5f);
                g.GetComponent<RectTransform>().anchorMax = new Vector2((float)(i + 1) / (numSpeakers + 1), 0.5f);
            }
        }
        public void DestroyAllChildren(Transform t)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                Destroy(t.GetChild(i).gameObject);
            }
            /*
            int i = 0;
            while (t.childCount > 0 && i<20)
            {
                Debug.Log(i + t.GetChild(0).name);
                #if UNITY_EDITOR
                                DestroyImmediate(t.GetChild(0).gameObject);
                #else
                                Destroy(t.GetChild(0).gameObject);
                #endif
                i++;
            }*/
            
        }
        private void OnDrawGizmosSelected()
        {
            leftAnchor = leftAnchorT.position;
            rightAnchor = rightAnchorT.position;

            Vector3 left = new Vector3(leftAnchor.x, leftAnchor.y, 0);
            Vector3 right = new Vector3(rightAnchor.x, rightAnchor.y, 0);
            Vector3 center = Vector3.Lerp(left, right, 0.5f) + characterParent.position;
            Vector3 size = new Vector3(left.x + right.x, 1, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }
    [System.Serializable]
    public class DEvent
    {
        public string text;
        public int speakerIndex;
        public Speaker speaker;

        public DEvent(Speaker speaker, string text)
        {
            this.text = text;
            this.speaker = speaker;
            action = ActionType.Text;
        }
        public DEvent(int speaker, string text)
        {
            this.text = text;
            this.speakerIndex = speaker;
            action = ActionType.Text;
        }

        public ActionType action = ActionType.Text;
        public List<Speaker> speakers;
        public List<int> speakerIndies;
        public DEvent(List<Speaker> speakers, bool enter)
        {
            this.text = enter ? "Enter" : "Exit";
            this.speakers = speakers;
            if (enter) action = ActionType.Enter;
            else action = ActionType.Exit;
        }
        public DEvent(List<int> speakers, bool enter)
        {
            this.text = enter ? "Enter" : "Exit";
            this.speakerIndies = speakers;
            if (enter) action = ActionType.Enter;
            else action = ActionType.Exit;
        }
        public static List<DEvent> Create(List<Speaker> cast, string big)
        {
            List<DEvent> events = new List<DEvent>();
            string[] chunks = big.Split("\n"[0]);

            Speaker last = Speaker.None;
            int lastI = -1;
            for (int c = 0; c < chunks.Length; c++)
            {
                
                List<Speaker> s = ParseStringForSpeakers(cast, chunks[c]);
                List<int> si = ParseStringForSpeakersInt(cast, chunks[c]);
                if (chunks[c].Trim() == "")
                {

                }
                else if (chunks[c].Contains("Enter"))
                {
                    events.Add(new DEvent(si, true));
                }
                else if (chunks[c].Contains("Exit"))
                {
                    events.Add(new DEvent(si, false));
                }
                else if (s.Count == 1)
                {
                    last = s[0];
                    lastI = si[0];
                }
                else
                {
                    events.Add(new DEvent(lastI, chunks[c]));
                }
            }
            return events;
        }


        public static List<Speaker> ParseStringForSpeakers(List<Speaker> cast, string line)
        {
            List<Speaker> found = new List<Speaker>();
            /*
            List<string> sp = line.Split(" "[0]).ToList();
            for (int i = 0; i < sp.Count; i++)
            {
                for (int j = 0; j < cast.Count; j++)
                {
                    if (sp[i].IndexOf(cast[j].name) > -1)
                    {
                        found.Add(cast[j]);
                    }
                }
            }*/
            string str = line;

            while (str.Length > 1)
            {
                bool shortened = false;
                for (int j = 0; j < cast.Count; j++)
                {
                    if (str.IndexOf(cast[j].name) > -1)
                    {
                        found.Add(cast[j]);
                        str = str.Substring(str.IndexOf(cast[j].name) + cast[j].name.Length);
                        shortened = true;
                    }
                }
                if(!shortened)
                    str = str.Substring(1);
            }

            return found;
        }

        public static List<int> ParseStringForSpeakersInt(List<Speaker> cast, string line)
        {
            List<int> found = new List<int>();
            string str = line;
            while (str.Length > 1)
            {
                bool shortened = false;
                for (int j = 0; j < cast.Count; j++)
                {
                    if (str.IndexOf(cast[j].name) > -1)
                    {
                        found.Add(j);
                        str = str.Substring(str.IndexOf(cast[j].name) + cast[j].name.Length);
                        shortened = true;
                    }
                }
                if (!shortened)
                    str = str.Substring(1);
            }

            return found;
        }
    }
}