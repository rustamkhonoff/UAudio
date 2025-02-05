# Unity Audio Service

## Overview

The **Unity Audio Service** (`UAudio`) is a flexible and optimized system for managing audio in Unity. It provides:

- **Audio Source Pooling** to efficiently manage sound sources.
- **Cancellation Token Support** for stopping sounds dynamically.
- **Background and Sound Mixer Groups** for better audio control.
- **Global Static Access** to interact with the service without dependency injection.

---

## Installation & Setup

### 1. Install via Git Package

To install the latest version of **UAudio**, use the following Git URL in Unity's **Package Manager**:
`https://github.com/rustamkhonoff/UAudio.git`

### 2. Install a Specific Version

UAudio also supports version tags. To install a specific version, append the tag to the URL:
`https://github.com/rustamkhonoff/UAudio.git#1.0.0`
Replace `1.0.0` with the desired version number.

### 3. Adding the Package to Unity

1. Open **Unity** and go to **Window > Package Manager**.
2. Click the **+** button and select **"Add package from git URL..."**.
3. Enter the Git URL and click **Add**.
4. (Optional) If installing a specific version, include the version tag as shown above.
5. Create an AudioConfiguration Asset
   In Unity, create a ScriptableObject asset for audio settings:

   _**Path: Assets > Create > Services > UAudio > Create UAudioConfiguration**_

6. Assign AudioMixer Groups (Master, Sound, Background) and Audio Data.

> [!NOTE] Ensure `UAudioConfiguration` asset is created if there is no other `IAudioConfiguration`

## AudioData

`AudioData` is a **serializable class** that stores key-value pairs for audio clips. It allows retrieving a specific audio clip by key and supports **
randomized clip selection** when multiple clips are available.

### Properties

| Property      | Type          | Description                                                              |
|---------------|---------------|--------------------------------------------------------------------------|
| `Key`         | `string`      | The unique identifier for the audio clip.                                |
| `RandomClip`  | `bool`        | Determines if a random clip should be selected from `RandomClips`.       |
| `Clip`        | `AudioClip`   | A single audio clip assigned to this entry.                              |
| `RandomClips` | `AudioClip[]` | An array of clips used for random selection when `RandomClip` is `true`. |

## API Reference

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `SoundGroup` | `AudioMixerGroup` | The audio mixer group used for sound effects. |
| `BackgroundGroup` | `AudioMixerGroup` | The audio mixer group used for background music. |

### Methods

| Method | Description |
|--------|-------------|
| `void Play` | Plays an audio clip based on the provided `AudioRequest`. Supports cancellation. |
| `void PlayAt` | Plays a sound at a specific **world position**. Supports cancellation. |
| `void ChangeBackground` | Changes the currently playing **background music**. Can restart the same track if needed. Supports cancellation. |
| `void StopBackground` | Stops the currently playing background music. |
| `void StopSounds` | Stops all currently playing **sound effects** (does not affect background music). |
| `void SetSoundPauseState` | Pauses or resumes all **sound effects**. (`true` = pause, `false` = resume) |
| `void SetBackgroundPauseState` | Pauses or resumes **background music**. (`true` = pause, `false` = resume) |
| `AudioSource PlayInBackground` | Plays a sound as **background audio**. Returns the `AudioSource` instance. |
| `AudioData AudioDataFor` | Retrieves **audio data** by key. |
| `void SetSoundVolume` | Sets the volume for **sound effects** (`0.0 - 1.0`). |
| `void SetBackgroundVolume` | Sets the volume for **background music** (`0.0 - 1.0`). |
| `void SetMasterVolume` | Sets the **global/master volume** (`0.0 - 1.0`). |

---

## Usage

### Default

```csharp
    //Load or create implementation for IAudioConfiguration
    UAudioConfiguration configuration = Resources.Load<UAudioConfiguration>("StaticData/Audio/AudioServiceConfiguration");
        
    //Create IAudioState
    DefaultAudioState defaultAudioState = new();
        
    //Create IAudioService
    DefaultAudioService defaultAudioService = new(configuration, defaultAudioState);
```

### Zenject

```csharp
    //Bind DefaultAudioService implements IAudioService
    Container.BindInterfacesTo<DefaultAudioService>().AsSingle();

    //Bind DefaultAudioState implements IAudioState
    Container.BindInterfacesTo<DefaultAudioState>().AsSingle();
        
    //Bind UAudioConfiguration from resources, asset need to be located anywhere in Resources folder, implements IAudioConfiguration
    Container.BindInterfacesTo<UAudioConfiguration>().FromResource("*PATH*").AsSingle();
```

> [!NOTE] After DefaultAudioService instantiated, UAudioGlobal also will be initialized
> Can access to `IAudioService` from `UAudioGlobal` static property `UAudioGlobal.Instance`

###     