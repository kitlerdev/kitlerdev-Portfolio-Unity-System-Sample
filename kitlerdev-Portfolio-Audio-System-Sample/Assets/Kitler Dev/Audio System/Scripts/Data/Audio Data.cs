using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[System.Serializable]
public class AudioData
{
    public string Name;
    public AudioClip Clip;
    [Range(0f, 4f)]
    public float Volume;
    [Range(0f, 4f)]
    public float Pitch;
    public bool Loop;
    [HideInInspector]
    public  AudioSource source; 


}
