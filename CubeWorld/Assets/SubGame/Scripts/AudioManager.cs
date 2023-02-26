using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sounds[] sounds;
    public static AudioManager instance;


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach (Sounds s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            AudioClip clip;
            if(s.clips.Length > 1)
            {
                clip = Combine(s.clips);
            }
            else
            {
                if(s.clips.Length == 0) { continue; }
                clip = s.clips[0];
            }
            s.source.clip = clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sounds s = Array.Find(sounds, sounds => sounds.name == name);
        if(s == null)
        {
            Debug.LogWarning("Sound: " + name + "not found!");
            return;
        }
        if (!s.source.isPlaying)
        {
            s.source.Play();
        }
    }

    public static AudioClip Combine(params AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return null;

        int length = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null)
                continue;

            length += clips[i].samples * clips[i].channels;
        }

        float[] data = new float[length];
        length = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null)
                continue;

            float[] buffer = new float[clips[i].samples * clips[i].channels];
            clips[i].GetData(buffer, 0);
            //System.Buffer.BlockCopy(buffer, 0, data, length, buffer.Length);
            buffer.CopyTo(data, length);
            length += buffer.Length;
        }

        if (length == 0)
            return null;

        AudioClip result = AudioClip.Create("Combine", length / 2, 2, 44100, false);
        result.SetData(data, 0);

        return result;
    }
}
