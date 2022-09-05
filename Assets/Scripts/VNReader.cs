using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using static Saving.SaveUtil;

namespace SD5VisualNovel
{
    [AddComponentMenu("Visual Novel Reader")]
    public class VNReader : MonoBehaviour
    {
        public VNChapterSO chapterSO;
        public VNCastSO castSO;

        private List<VNEvent> events;
        private List<Speaker> cast;
        private int speakerIndex = 0;
        private List<int> speakerIndies = new List<int>();

        [ContextMenu("Make from Asset")]
        public void SetFromAsset()
        {
            cast = castSO.cast;
            events = chapterSO.events;
        }

        [SerializeField] private TextMeshProUGUI textUI;
        [SerializeField] private TextMeshProUGUI nameUI;
        private Color colorSV = Color.grey;

        private int i = 0;

        private string text = "";

        private List<Image> currentImages;
        public RectTransform characterParent;
        public GameObject imagePrefab;

        public void Start()
        {
            cast = castSO.cast;
            events = chapterSO.events;
            

            SetFromAsset();
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
                if (events[i].action == ActionType.Text)
                {
                    speakerIndex = events[i].speakerIndex;
                    text = events[i].text;
                    Display();
                }
                else if (events[i].action == ActionType.Enter)
                {
                    speakerIndies.AddRange(events[i].speakerIndies);
                    DisplaySprites();
                }
                else if (events[i].action == ActionType.Exit)
                {
                    foreach (int i in events[i].speakerIndies)
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
            /* I tried to do a fancy thing with HSV and making the colors simple with the same S and V but idk maybe ill try again
            float H, S, V;
            float Hb, Sb, Vb;
            Color.RGBToHSV(cast[speakerIndex].color, out H, out S, out V);
            Color.RGBToHSV(colorSV, out Hb, out Sb, out Vb);
            */

            Color nameCol = cast[speakerIndex].color;//Color.HSVToRGB(H, Sb, Vb);
            nameCol.a = 1;
            nameUI.color = nameCol;
            nameUI.text = cast[speakerIndex].name + ":";
            textUI.text = text;
        }

        public void DisplaySprites()
        {
            DestroyAllChildren(characterParent);
            int numSpeakers = speakerIndies.Count;
            if (numSpeakers < 1) return;
            for (int i = 0; i < numSpeakers; i++)
            {
                GameObject g = Instantiate(imagePrefab, characterParent);
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
        }


        public string path = "/save0.vns";
        public void Save()
        {
            VNSave toSave = new VNSave(0, i, speakerIndies.ToArray());
            Save<VNSave>(toSave, path);
        }
        public void Load()
        {
            VNSave loaded = Load<VNSave>(path);
            i = loaded.index -1;
            speakerIndies = loaded.speakers.ToList();

            Next();
            DisplaySprites();
            Display();
        }
        public GameObject quitPopup;
        public void OpenQuitPopup()
        {
            quitPopup.SetActive(true);
        }
        public void CloseQuitPopup()
        {
            quitPopup.SetActive(false);
        }
        public void Quit()
        {
            Application.Quit();
        }
        public GameObject settingsPopup;
        public void OpenSettingsPopup()
        {
            settingsPopup.SetActive(true);
        }
        public void CloseSettingsPopup()
        {
            settingsPopup.SetActive(false);
        }
    }

    /// <summary>
    /// This is the Event class that each event is stored. I tried using inheritence but in the end decided it was easier to just use an enum to toggle the action being done.
    /// </summary>
    [System.Serializable]
    public class VNEvent
    {
        public string text;
        public int speakerIndex;

        /// <summary>
        /// Constructor for standard text.
        /// </summary>
        public VNEvent(int speaker, string text)
        {
            this.text = text;
            this.speakerIndex = speaker;
            action = ActionType.Text;
        }

        public ActionType action = ActionType.Text;
        public List<int> speakerIndies;

        /// <summary>
        /// Constructor for an enter or exit event
        /// </summary>
        public VNEvent(List<int> speakers, bool enter)
        {
            this.text = enter ? "Enter" : "Exit";
            this.speakerIndies = speakers;
            if (enter) action = ActionType.Enter;
            else action = ActionType.Exit;
        }


        public static List<VNEvent> Create(List<Speaker> cast, string big)
        {
            List<VNEvent> events = new List<VNEvent>();
            string[] chunks = big.Split("\n"[0]);

            int lastI = -1;
            for (int c = 0; c < chunks.Length; c++)
            {
                List<int> si = ParseStringForSpeakersInt(cast, chunks[c]);
                if (chunks[c].Trim() == "")
                {

                }
                else if (chunks[c].Contains("Enter"))
                {
                    events.Add(new VNEvent(si, true));
                }
                else if (chunks[c].Contains("Exit"))
                {
                    events.Add(new VNEvent(si, false));
                }
                else if (si.Count == 1)
                {
                    lastI = si[0];
                }
                else
                {
                    events.Add(new VNEvent(lastI, chunks[c]));
                }
            }
            return events;
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

    [System.Serializable]
    public class VNSave
    {
        public int level;
        public int index;
        public int[] speakers;

        public VNSave(int l, int i, int[] s)
        {
            level = l;
            index = i;
            speakers = s;
        }
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