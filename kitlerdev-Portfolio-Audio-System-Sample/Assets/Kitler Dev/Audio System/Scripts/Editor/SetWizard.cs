using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public static class SetWizard
{
    [MenuItem("Tools/Kitler Dev/Setup Audio Manager", false, 1)]
    public static void SetupAudioManager()
    {
        // Check if AudioManager already exists
        AudioManager existingManager = Object.FindAnyObjectByType<AudioManager>();
        if (existingManager != null)
        {
            Debug.Log("ℹ️ AudioManager already exists in scene.");
            Selection.activeObject = existingManager.gameObject;
            return;
        }

        // Create new AudioManager GameObject
        GameObject audioManagerGO = new GameObject("AudioManager");
        AudioManager audioManager = audioManagerGO.AddComponent<AudioManager>();

        // Add audio source for music
        AudioSource musicSource = audioManagerGO.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;

        // Create EventSystem if it doesn't exist
        SetupEventSystem();

        // Select the newly created AudioManager in the hierarchy
        Selection.activeGameObject = audioManagerGO;

        // Generate Audio Constants
        GenerateAudioConstants(audioManager);

        Debug.Log("✅ Audio Manager created and configured. Please assign your Audio Mixer Groups in the Inspector.");
    }

    [MenuItem("Tools/Kitler Dev/Cleanup Audio Manager", false, 2)]
    public static void CleanupAudioManager()
    {
        AudioManager audioManager = Object.FindAnyObjectByType<AudioManager>();
        if (audioManager != null && audioManager.gameObject.name == "AudioManager")
        {
            Object.DestroyImmediate(audioManager.gameObject);
            Debug.Log("🗑️ AudioManager removed.");
        }

        EventSystem eventSystem = Object.FindAnyObjectByType<EventSystem>();
        if (eventSystem != null && eventSystem.gameObject.name == "EventSystem")
        {
            Object.DestroyImmediate(eventSystem.gameObject);
            Debug.Log("🗑️ EventSystem removed.");
        }
    }

    private static void SetupEventSystem()
    {
        // Check if EventSystem already exists
        EventSystem existingEventSystem = Object.FindAnyObjectByType<EventSystem>();
        if (existingEventSystem != null)
        {
            return;
        }

        // Create EventSystem if it doesn't exist
        GameObject eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.AddComponent<EventSystem>();
        eventSystemGO.AddComponent<StandaloneInputModule>();

        Debug.Log("✅ EventSystem created.");
    }

    private static void GenerateAudioConstants(AudioManager targetManager)
    {
        // Check if the generator script exists and call it
        System.Type generatorType = System.Type.GetType("AudioConstantsGenerator, Assembly-CSharp-Editor");
        if (generatorType != null)
        {
            // Use Reflection to call the generation method
            System.Reflection.MethodInfo method = generatorType.GetMethod("GenerateAudioConstants", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            if (method != null)
            {
                method.Invoke(null, new object[] { targetManager });
                Debug.Log("🔨 Triggered Audio Constant Generation.");
            }
            else
            {
                Debug.LogWarning("Could not find the GenerateAudioConstants method.");
            }
        }
        else
        {
            Debug.LogWarning("AudioConstantsGenerator Editor script not found. Please ensure it's in an Editor folder.");
        }
    }
}