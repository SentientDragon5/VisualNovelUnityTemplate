using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VisualNovel
{
    public class DialougeReader1 : MonoBehaviour
    {
        #region Singleton
        public static DialougeReader1 instance;
        void Awake()
        {
            if (instance == null) instance = this;
            if (instance != this) Destroy(this);
        }
        #endregion


        public DialougeSO dialougeObject;

        public int currentIndex;    //The index on the SO's List
        public string outputText;   //The text being outputted
        public int choiceID;        //The choice number for which choice of the List of SO[index].Choices[choiceID]
        public float speed = 40f;   //The reading speed factor, 1 is unbearable, 40 is snappy
        public bool doneWithLine = true;   //is the reader done with the current line?

        [Header("References to UI")]
        [SerializeField] private TextMeshProUGUI textUI;
        [SerializeField] private TextMeshProUGUI nameUI;

        public void Begin()
        {
            doneWithLine = true;//Allow the reader to continue
            currentIndex = 0;
            DisplayText();
        }
        public void Next()
        {
            string text = dialougeObject.events[currentIndex].text;

            if (doneWithLine)
            {
                //If you are done reading:
                //change the UI to the next screen
                if (currentIndex + 1 >= dialougeObject.events.Count)
                {
                    End();
                }
                else
                {
                    currentIndex++;
                    DisplayText();
                }
            }
            else
            {
                //If you are not done:
                //Skip the reading and complete the text but don't move on.
                StopCoroutine(Read(text, speed));
                DoneReading(text);
            }


        }
        void DoneReading(string text)
        {
            StopAllCoroutines();
            outputText = text;
            textUI.text = outputText;
            doneWithLine = true;
        }

        private void Update()
        {
            /*
            if(inputYes.triggered)
            {
                SetChoice(0);
                Next();
            }
            if (inputNo.triggered)
            {
                SetChoice(1);
                Next();
            }
            */
        }

        void Display()
        {
            string text = dialougeObject.events[currentIndex].text;
            //Next

            //Stop any coroutines
            StopAllCoroutines();
            //StartReading the next dialouge
            StartCoroutine(Read(text, speed));
        }
        void DisplayText()
        {
            string text = dialougeObject.dialouge[currentIndex].text;

            //Stop any coroutines
            StopAllCoroutines();
            //StartReading the next dialouge
            StartCoroutine(Read(text, speed));
        }

        public void End()
        {
            Debug.Log("DONE");
        }

        public IEnumerator Read(string text, float speed)
        {
            doneWithLine = false;
            outputText = "";
            for (int c = 0; c < text.Length; c++)
            {
                string add = text.ToCharArray()[c].ToString();
                if (add == " ")//if it is a space then do the next letter too.
                {
                    c++;
                    add += text.ToCharArray()[c].ToString();
                }

                outputText += add;
                textUI.text = outputText;
                yield return new WaitForSecondsRealtime(speed);
            }
            DoneReading(text);
        }
    }
}