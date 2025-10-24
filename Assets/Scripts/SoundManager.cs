using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private AudioSource audioSource;
    public static SoundManager Instance { get; private set; }

    private List<AudioClip> clipList = new List<AudioClip>();
    public float volume = 0.7f; // Default volume, can be adjusted later, serialize??

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

        LoadSounds();

        audioSource = GetComponent<AudioSource>();

    }

    private void LoadSounds()
    {
        print("Loading sounds...");
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds");

        foreach (AudioClip clip in clips)
        {

            print("Loaded sound: " + clip.name);
            clipList.Add(clip);
        }
        print("Sounds loaded successfully.");
    }

    public void PlaySFX(string name)
    {
        AudioClip clip = clipList.Find(c => c.name == name);
        if (clip == null) return;
        audioSource.PlayOneShot(clip, volume);
    }

    public void PlaySFXRandomPitch(string name)
    {
        AudioClip clip = clipList.Find(c => c.name == name);
        if (clip == null) return;

        AudioSource tempSource = gameObject.AddComponent<AudioSource>();
        tempSource.playOnAwake = false;
        tempSource.spatialBlend = 0f;
        tempSource.volume = volume;
        tempSource.pitch = Random.Range(1f - 0.5f, 1f + 0.5f);

        tempSource.PlayOneShot(clip);

        // Automatically destroy component when finished
        Destroy(tempSource, clip.length / tempSource.pitch);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
