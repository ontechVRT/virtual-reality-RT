using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class AmbientSoundPlayer : MonoBehaviour
{
    AudioSource currentlyPlaying;

    void Start()
    {
        InitButton("Canvas/ForestAmbience", "Sounds/ForestAmbience");
        InitButton("Canvas/RuralWindyAmbience", "Sounds/RuralWindyAmbience");
        InitButton("Canvas/SuburbanSummerDayAmbience", "Sounds/SuburbanSummerDayAmbience");
    }

    void InitButton(string buttonPath, string clipPath)
    {
        Debug.Log("Create button for " + buttonPath + " with audio clip " + clipPath);
        GameObject buttonGameObject = GameObject.Find(buttonPath);
        
        AudioSource audioSource = buttonGameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        AudioClip clip = Resources.Load(clipPath) as AudioClip;
        audioSource.clip = clip;

        Button button = buttonGameObject.GetComponent<Button>();
        button.onClick.AddListener(delegate { PlayAudioSource(audioSource); });
    }

    void PlayAudioSource(AudioSource audioSource)
    {
        Debug.Log(audioSource);
        if (currentlyPlaying != null)
        {
            currentlyPlaying.Stop();
        }

        currentlyPlaying = audioSource;
        currentlyPlaying.Play();
    }
}
