using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MonitorManager : MonoBehaviour
{
    private VideoPlayer vidplayer;
    private VideoClip empty;
    public VideoClip sans;
    public VideoClip turutuututututututurutuuututututututurutuuutuuutu_tu_tuuuu;
    // Start is called before the first frame update
    void Start()
    {
        vidplayer = gameObject.GetComponent<VideoPlayer>();
        SceneMoodManager.Instance.OnMoodUpdate += mood => UpdateVideo(mood);
    }

    private void UpdateVideo(SceneMoods mood){    
        // fuck you im stealing your entire function
        // how's that for a "quick and dirty"
        if (mood == SceneMoods.Default)
            vidplayer.clip = empty;
        else if (mood == SceneMoods.Persona4)
            vidplayer.clip = turutuututututututurutuuututututututurutuuutuuutu_tu_tuuuu;
        else if (mood == SceneMoods.Undertale)
            vidplayer.clip = sans;
    }

}
