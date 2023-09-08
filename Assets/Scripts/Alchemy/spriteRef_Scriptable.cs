using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SpriteRef : ScriptableObject
{
    public static Sprite copium;
    public static Sprite himikals;
    public static Sprite ayaminium;
    public static Sprite primabait;
    public static Sprite folly;
    public static Sprite horny;
    public static Sprite heinz;
    public static Sprite egg;

    public Dictionary<string, Sprite> dictionary = new()
    {
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
