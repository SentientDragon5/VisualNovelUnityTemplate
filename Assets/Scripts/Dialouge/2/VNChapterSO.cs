using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD5VisualNovel
{
    [CreateAssetMenu(fileName = "Chapter", menuName = "VN Chapter Asset")]
    public class VNChapterSO : ScriptableObject
    {
        public VNCastSO castSO;
        public List<VNEvent> events;

        public TextAsset textAsset;

        [ContextMenu("Make from Asset")]
        public void SetAsset()
        {
            events = VNEvent.Create(castSO.cast, textAsset.text);
        }
        //[SerializeField, TextArea(5, 100)] private string big;
    }
}