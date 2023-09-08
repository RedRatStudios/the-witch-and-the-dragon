using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SpriteRef : ScriptableObject
{
    public Sprite copium;
    public Sprite himikals;
    public Sprite ayaminium;
    public Sprite primabait;
    public Sprite folly;
    public Sprite horny;
    public Sprite heinz;
    public Sprite egg;

    public Dictionary<string, Sprite> dictionary = new();
    private void OnEnable()
    {
        dictionary = new() {
           { "Distilled Copium", copium },
           { "Unstable Himikals", himikals },
           { "Ayaminium", ayaminium },
           { "Primabait", primabait },
           { "Witch's Folly", folly },
           { "Horn of the Dog", horny },
           { "Heinz", heinz },
           { "Polished Egg", egg },
        };
    }
}
