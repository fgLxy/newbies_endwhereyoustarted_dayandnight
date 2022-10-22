using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace SunAndMoon
{
    public class AudioResourceManager : MonoBehaviour
    {
        private static AudioResourceManager _instance;
        public static AudioResourceManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject();
                    go.name = "AudioResourceManager";
                    _instance = go.AddComponent<AudioResourceManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        

        private AudioSource _source;
        private AudioSource _effect;

        private float _bgmVolume = 1f;
        private float _effectVolume = 1f;
        public async UniTask PlayBGM(string path)
        {
            if (_source == null)
            {
                _source = _instance.gameObject.AddComponent<AudioSource>();
                var mixer = await Resources.LoadAsync<AudioMixerGroup>("music/BGM") as AudioMixerGroup;
                _source.outputAudioMixerGroup = mixer;
            }
            if (_source.clip != null)
            {
                await _source.DOFade(0, 1f);
                _source.Stop();
            }
            var audio = await Resources.LoadAsync<AudioClip>(path) as AudioClip;
            _source.playOnAwake = true;
            _source.clip = audio;
            _source.volume = _bgmVolume * 0.2f;
            _source.Play();
            var startTime = UnityEngine.Time.realtimeSinceStartup;
            await UniTask.WaitUntil(() => UnityEngine.Time.realtimeSinceStartup - startTime >= audio.length || _source.clip != audio);
        }
    
        public async UniTask PlayAudioEffect(string path, float volume = 1f)
        {
            var audio = await Resources.LoadAsync<AudioClip>(path) as AudioClip;
            if (_effect == null)
            {
                _effect = _instance.gameObject.AddComponent<AudioSource>();
                var mixer = await Resources.LoadAsync<AudioMixerGroup>("music/Effect") as AudioMixerGroup;
                _effect.outputAudioMixerGroup = mixer;
            }
            var source = _effect;
            source.volume = 1f;
            source.PlayOneShot(audio, _effectVolume * volume);
            var startTime = UnityEngine.Time.realtimeSinceStartup;
            await UniTask.WaitUntil(() => startTime + audio.length >= UnityEngine.Time.realtimeSinceStartup);
        }

        public async UniTask PlayAudio3DEffect(string path, Vector3 position)
        {
            var listenerPosition = Camera.main.transform.position;
            var direct = position - listenerPosition;
            var effectPosition = listenerPosition + direct.normalized * 1f;
            var audio = await Resources.LoadAsync<AudioClip>(path) as AudioClip;
            AudioSource.PlayClipAtPoint(audio, effectPosition, _effectVolume);
            var startTime = UnityEngine.Time.realtimeSinceStartup;
            await UniTask.WaitUntil(() => startTime + audio.length >= UnityEngine.Time.realtimeSinceStartup);
        }
    }
}