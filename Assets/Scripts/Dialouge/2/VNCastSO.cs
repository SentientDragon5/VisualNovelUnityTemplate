using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SD5VisualNovel
{
    [CreateAssetMenu(fileName = "Cast", menuName = "VN Cast Asset")]
    public class VNCastSO : ScriptableObject
    {
        public List<Speaker> cast;
        //H: var, S: 33, V: 100

    }
}