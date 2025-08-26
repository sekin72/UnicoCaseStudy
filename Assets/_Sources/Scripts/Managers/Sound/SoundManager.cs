using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnicoCaseStudy.Configs;
using UnicoCaseStudy.Managers.Pool;
using UnicoCaseStudy.Managers.Vibration;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace UnicoCaseStudy.Managers.Sound
{
    public sealed class SoundManager : Manager
    {
        [SerializeField] private SoundsList SoundsList;

        private const int MaxSoundPlaySimultaneously = 3;

        private readonly Dictionary<SoundKeys, AudioClip> _audioClips = new();
        private readonly List<AudioSource> _sourceList = new();
        private GameObject _loadedAsset;

        private SettingsManager _settingsManager;
        private PoolManager _poolManager;

        private bool _sequenceStarts;

        private AudioSource _currentSource;
        private UnityEngine.AudioSource[] _sources;

        protected override async UniTask WaitDependencies(CancellationToken disposeToken)
        {
            _settingsManager = AppManager.GetManager<SettingsManager>();
            _poolManager = AppManager.GetManager<PoolManager>();

            await UniTask.WaitUntil(() => _settingsManager.IsInitialized && _poolManager.IsInitialized,
                cancellationToken: disposeToken);
        }

        protected override async UniTask Initialize(CancellationToken disposeToken)
        {
            LoadData();

            var audioClips = new List<AudioClip>();
            foreach (var clip in SoundsList.AudioClips)
            {
                audioClips.Add(clip);
            }

            foreach (var audioClip in audioClips)
            {
                if (!Enum.TryParse(audioClip.name, out SoundKeys soundKey))
                {
                    throw new Exception($"SoundKeys enum does not contain {audioClip.name} value");
                }

                _audioClips.Add(soundKey, audioClip);
            }

            _loadedAsset = _poolManager.GetGameObject(PoolKeys.SoundController);

            Object.DontDestroyOnLoad(_loadedAsset);

            var toBeDeleted = _loadedAsset.GetComponents<UnityEngine.AudioSource>();
            foreach (var component in toBeDeleted)
            {
                Object.DestroyImmediate(component, true);
            }

            for (var i = 0; i < MaxSoundPlaySimultaneously; i++)
            {
                _loadedAsset.AddComponent<UnityEngine.AudioSource>().playOnAwake = true;
            }

            _sources = _loadedAsset.GetComponents<UnityEngine.AudioSource>();

            for (var i = 0; i < MaxSoundPlaySimultaneously; i++)
            {
                _sourceList.Add(new AudioSource(_sources[i], false));
                InitAudioSource(_sources[i], i);
            }

            _loadedAsset.SetActive(false);
            _loadedAsset.SetActive(true);

            _currentSource = _sourceList[0];

            SetSoundVolume(_settingsManager.GetVolume());
            SetSoundActive(_settingsManager.IsSoundActive());
        }

        public override void Dispose()
        {
            if (_loadedAsset == null)
            {
                return;
            }

            _poolManager.SafeReleaseObject(PoolKeys.SoundController, _loadedAsset);
            _loadedAsset = null;
        }

        public void SetSoundActive(bool isActive)
        {
            VibrationManager.VibrateStatic(VibrationType.Selection);

            foreach (var source in _sources)
            {
                source.volume = isActive ? _settingsManager.GetVolume() : 0f;
            }
        }

        public void SetSoundVolume(float value)
        {
            for (var i = 0; _settingsManager.IsSoundActive() && i < _sources.Length; i++)
            {
                _sources[i].volume = _settingsManager.GetVolume();
            }
        }

        public async UniTask PlayForAWhile(SoundKeys soundType, float duration, float frequency,
            bool playSimultaneously = true, bool randomPitch = false, Func<bool> interruptCondition = null)
        {
            await PlayForAWhileRoutine(soundType, duration, frequency, playSimultaneously, randomPitch,
                interruptCondition);
        }

        public void ActivateSound(bool value)
        {
            if (!value)
            {
                foreach (var source in _sourceList)
                {
                    source.Source.Stop();
                    source.IsPlayingSequence = false;
                }
            }
        }

        private static void InitAudioSource(UnityEngine.AudioSource source, int priority)
        {
            source.playOnAwake = false;
            source.priority = priority;
        }

        private AudioSource PickMostAvailableSource(SoundKeys type, float pitch = 1f, bool stopAll = false,
            bool playInLoop = false, float volumeMultiplier = 1f)
        {
            AudioSource tempSource = null;

            foreach (var source in _sourceList)
            {
                if (stopAll)
                {
                    source.Source.Stop();
                    source.IsPlayingSequence = false;
                }

                if (tempSource == null && !source.IsPlayingSequence && !source.Source.isPlaying)
                {
                    tempSource = source;
                }
            }

            if (tempSource == null)
            {
                foreach (var source in _sourceList)
                {
                    if (source.CurrentSound == type)
                    {
                        tempSource = source;
                        break;
                    }
                }
            }

            if (tempSource == null)
            {
                foreach (var source in _sourceList)
                {
                    if (!source.IsPlayingSequence)
                    {
                        tempSource = source;
                        break;
                    }
                }
            }

            tempSource ??= _sourceList[0];

            tempSource.Reset();
            tempSource.Source.volume = _settingsManager.GetVolume() * volumeMultiplier;
            tempSource.Source.clip = _audioClips[type];
            tempSource.Source.pitch = pitch;
            tempSource.Source.loop = playInLoop;
            tempSource.CurrentSound = type;

            return tempSource;
        }

        public void PlayOneShot(SoundKeys soundType, float pitchValue = 1f, bool playSimultaneously = true,
            bool playInLoop = false, float volumeMultiplier = 1f)
        {
            PickMostAvailableSource(soundType, pitchValue, !playSimultaneously, playInLoop, volumeMultiplier)
                .Play();
        }

        public void StopAll(bool fadeOut = false, float fadeDuration = 0.25f)
        {
            foreach (var source in _sourceList)
            {
                source.Stop(fadeOut, fadeDuration);
            }
        }

        public void UpdateContinuousSoundPitchAndVolume(SoundKeys soundTypes, float pitch,
            float volumeMultiplier = 1f)
        {
            foreach (var source in _sourceList)
            {
                if (source.CurrentSound == soundTypes)
                {
                    source.Source.pitch = pitch;
                    source.Source.volume *= volumeMultiplier;
                }
            }
        }

        public void PlayWithFadeIn(SoundKeys soundType, float pitchValue, bool playSimultaneously = true,
            bool playInLoop = false, float fadeInDuration = 0.25f, float fadeToValue = 1f)
        {
            PickMostAvailableSource(soundType, pitchValue, !playSimultaneously, playInLoop, 0f)
                .Play(true, fadeInDuration, fadeToValue);
        }

        public void PlayOneShotWithRandomPitch(SoundKeys soundType, float minPitch, float maxPitch,
            bool playSimultaneously = true, bool playInLoop = false, float volumeMultiplier = 1f)
        {
            PickMostAvailableSource(soundType, Random.Range(minPitch, maxPitch), !playSimultaneously, playInLoop,
                    volumeMultiplier)
                .Play();
        }

        public void StopSound(SoundKeys soundType, bool fadeOut, float fadeDuration)
        {
            foreach (var source in _sourceList)
            {
                if (source.CurrentSound == soundType)
                {
                    source.Stop(fadeOut, fadeDuration);
                }
            }
        }

        public async UniTask PlayForAWhileRoutine(SoundKeys soundType, float duration, float frequency,
            bool playSimultaneously, bool randomPitch = false, Func<bool> interruptCondition = null)
        {
            if (_sequenceStarts)
            {
                return;
            }

            var source = PickMostAvailableSource(soundType, 1f, !playSimultaneously);

            source.IsPlayingSequence = true;
            _sequenceStarts = true;
            var time = 0f;

            do
            {
                if (!source.IsPlayingSequence || (interruptCondition != null && interruptCondition()))
                {
                    source.Stop();
                    break;
                }

                source.Play(randomPitch: randomPitch);
                time += frequency;
                await UniTask.Delay(TimeSpan.FromSeconds(frequency));
            } while (time <= duration);

            _currentSource.IsPlayingSequence = false;
            _sequenceStarts = false;
        }
    }

    public class AudioSource
    {
        private object _tweenId;
        public SoundKeys CurrentSound;
        public bool IsPlayingSequence;

        public UnityEngine.AudioSource Source;

        public AudioSource(UnityEngine.AudioSource source, bool isPlayingSequence)
        {
            Source = source;
            IsPlayingSequence = isPlayingSequence;
        }

        public void Play(bool fadeIn = false, float fadeDuration = 0.25f, float toFadeValue = 0.75f,
            bool randomPitch = false)
        {
            DOTween.Kill(_tweenId);

            if (fadeIn)
            {
                Source.volume = 0f;

                if (randomPitch)
                {
                    Source.pitch = Random.Range(0.8f, 1.2f);
                }

                Source.Play();
                _tweenId = DOVirtual.Float(0, toFadeValue, fadeDuration, value => Source.volume = value).id;
            }
            else
            {
                if (randomPitch)
                {
                    Source.pitch = Random.Range(0.8f, 1.2f);
                }

                Source.Play();
            }
        }

        public void PlayOneShot(AudioClip clip, bool fadeIn = false, float fadeDuration = 0.25f,
            float toFadeValue = 0.75f)
        {
            DOTween.Kill(_tweenId);

            if (fadeIn)
            {
                Source.volume = 0f;
                Source.PlayOneShot(clip);
                _tweenId = DOVirtual.Float(0, toFadeValue, fadeDuration, value => Source.volume = value).id;
            }
            else
            {
                Source.PlayOneShot(clip);
            }
        }

        public void Reset()
        {
            Source.loop = false;
            Source.clip = null;
            Source.pitch = 1f;
            Source.Stop();
        }

        public void Stop(bool fadeout = false, float fadeDuration = 0.25f)
        {
            if (fadeout)
            {
                _tweenId = DOVirtual.Float(0.6f, 0f, fadeDuration, value => Source.volume = value)
                    .OnComplete(Reset).id;
            }
            else
            {
                Reset();
            }
        }
    }
}