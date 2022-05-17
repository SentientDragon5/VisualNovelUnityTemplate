using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DR : MonoBehaviour
{
    public static DR i;
    private void OnValidate()
    {
        i = this;
    }
    public CharacterSO so;
    [ContextMenu("Load")]
    public void Load()
    {
        so.Create();
        events = so.events;
    }

    public List<DialougeEvent> events;
    public int index;

    [SerializeField] private TextMeshProUGUI speakerLableUII;
    [SerializeField] private TextMeshProUGUI textUI;


    private void Start()
    {
        index = 0;
        events[index].Invoke();
    }

    string display = "howdy";
    public string Display
    {
        get
        {
            return display;
        }
        set
        {
            textUI.text = value;
            display = value;
            if (display == fullText)
            {
                Done = true;
            }
        }
    }

    bool done;
    public bool Done
    {
        get
        {
            return done;
        }
        set
        {
            if (value && !done)
            {
                StopAllCoroutines();
                Display = fullText;
            }
            done = value;
        }
    }
    public List<Speaker> speakers;

    public float speed = 0.1f;

    public IEnumerator Read(string text)
    {
        display = "";
        Done = false;
        for (int c = 0; c < text.Length -1; c++)
        {
            string add = text.ToCharArray()[c].ToString();
            if (add == " ")//if it is a space then do the next letter too.
            {
                c++;
                add += text.ToCharArray()[c].ToString();
            }

            Display += add;
            yield return new WaitForSecondsRealtime(speed);
        }
        Done = true;
    }
    [ContextMenu("Next")]
    public void Next()
    {
        if (Done)
        {
            // start next chunk
            index++;

            events[index].Invoke();
        }
        else
        {
            Done = true;
        }
    }

    public string fullText;
    public void SetSpeaker(Speaker speaker)
    {
        //events[index]
        speakerLableUII.color = speaker.color;
        speakerLableUII.text = speaker.name;
    }

}