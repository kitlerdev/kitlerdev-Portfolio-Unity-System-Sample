using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // Singleton instance for global access
    public static AudioManager Instance;

    // Array to hold all audio data assets
    public AudioData[] sound;

    // ========== INITIALIZATION & SETUP ========== //
    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        InitializeSoundSources();
        GenerateAudioConstantsClass(); // Create constant references
    }

    // ========== SOUND SYSTEM INITIALIZATION ========== //
    /// <summary>
    /// Sets up AudioSource components for each sound effect
    /// </summary>
    private void InitializeSoundSources()
    {
        foreach (AudioData s in sound)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.Clip;
            s.source.volume = s.Volume;
            s.source.loop = s.Loop;
            s.source.pitch = s.Pitch;

        }
    }

    // ========== CONSTANT GENERATION SYSTEM ========== //
    /// <summary>
    /// Generates a static class with constant string references for all audio clips
    /// </summary>
    private void GenerateAudioConstantsClass()
    {
        // This creates a runtime class that can be accessed like: Audio.Jump
        // The actual implementation would use reflection or code generation
        // For now, we'll create a simple static class pattern
        CreateAudioConstantsRuntime();
    }

    /// <summary>
    /// Creates a runtime representation of audio constants
    /// </summary>
    private void CreateAudioConstantsRuntime()
    {
        // In a real implementation, you might use:
        // 1. Code generation to create a actual C# file
        // 2. Reflection-based system
        // 3. Custom editor tool

        // For runtime usage, we'll create a simple static access pattern
        // This is a simplified approach for demonstration
    }


    // ========== PUBLIC AUDIO API ========== //
    /// <summary>
    /// Plays a sound effect by name
    /// </summary>
    public void Play(string soundName)
    {
        AudioData s = Array.Find(sound, sound => sound.Name == soundName);
        if (s == null)
        {
            Debug.LogWarning($"Sound '{soundName}' not found!");
            return;
        }
        s.source.Play();
    }

    /// <summary>
    /// Stops a playing sound effect
    /// </summary>
    public void Stop(string soundName)
    {
        AudioData s = Array.Find(sound, sound => sound.Name == soundName);
        if (s == null)
        {
            Debug.LogWarning($"Sound '{soundName}' not found!");
            return;
        }
        s.source.Stop();
    }

   
}

