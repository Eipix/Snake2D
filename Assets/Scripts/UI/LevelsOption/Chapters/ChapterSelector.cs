using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class ChapterSelector : SerializedMonoBehaviour
{
    [ChildGameObjectsOnly, OdinSerialize]
    private Chapter[] _chapters;

    private void Awake()
    {
        _chapters[0].Unlock();
    }
}
