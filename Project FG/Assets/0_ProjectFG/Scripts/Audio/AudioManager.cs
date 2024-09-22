using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Audio;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace JH
{
    public class AudioManager : MonoBehaviour
    {
        #region 싱글톤
        public static AudioManager Instance
        {
            get
            {
                // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
                if (m_Instance == null)
                {
                    // 씬에서 GameManager 오브젝트를 찾아 할당
                    //m_Instance = FindObjectOfType<AudioManager>();
                    GameObject audioManager = new GameObject("AudioManager");
                    m_Instance = audioManager.AddComponent<AudioManager>();
                    m_Instance.GetComponent<AudioManager>().Initialized();

                }

                // 싱글톤 오브젝트를 반환
                return m_Instance;
            }
        }
        private static AudioManager m_Instance; // 싱글톤이 할당될 static 변수    
        #endregion

        public Dictionary<string, Sound> musicSounds = new Dictionary<string, Sound>();
        public Dictionary<string, Sound> sfxSounds = new Dictionary<string, Sound>();

        public AudioMixer audioMixer;
        public AudioSource musicSource, sfxSource;
        public GameObject sourceParent;


        private string[] audioPath = new string[2];


        private void Awake()
        {
            // 싱글톤 인스턴스 초기화
            if (m_Instance == null)
            {
                m_Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }



        // 초기화
        private void Initialized()
        {
            audioMixer = Resources.Load<AudioMixer>("AudioManager");
            musicSource = this.gameObject.AddComponent<AudioSource>();
            sfxSource = this.gameObject.AddComponent<AudioSource>();

            musicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];
            sfxSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
            musicSource.loop = true;
            sfxSource.loop = true;
            AudioInit();
        }
        // 오디오 초기화
        public void AudioInit()
        {
            musicSounds.Clear();
            sfxSounds.Clear();
            musicSounds = new Dictionary<string, Sound>();
            sfxSounds = new Dictionary<string, Sound>();
            GameObject parent = new GameObject("Audio Sources");
            sourceParent = parent;
        }

        #region ######################_Play Audio_#####################
        /// <summary> BGM을 재생하는 메서드 </summary>
        public void PlayBGM(string name)
        {
            musicSource.Stop();
            try
            {
                Sound sound = musicSounds[name];

                musicSource.clip = sound.clip;
                musicSource.Play();
            }

            catch
            {
                AddBGM(name);

                if (musicSounds.ContainsKey(name) == false)
                    return;

                Sound sound = musicSounds[name];

                musicSource.clip = sound.clip;
                musicSource.Play();
            }
        }
        public void StopBGM()
        {
            musicSource.Stop();

        }


        /// <summary> 사운드 이펙트를 재생하는 메서드 </summary>
        public void PlaySFX(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return;
            }

            try
            {

                Sound sound = sfxSounds[name];
                if (sound.lastSoundTime + 0.075f < Time.time)
                {
                    sfxSource.PlayOneShot(sound.clip);
                    sound.lastSoundTime = Time.time;
                }
            }
            catch 
            {

                AddSFX(name);
                Sound sound = sfxSounds[name];
                if (sound.lastSoundTime + 0.075f < Time.time)
                {

                    sfxSource.PlayOneShot(sound.clip);
                    sound.lastSoundTime = Time.time;
                }
            }

        }
        /// <summary> 특정 위치에 사운드 이펙트를 재생하는 메서드 </summary>
        public void PlaySFXPoint(string name, Vector3 position = default, bool loop = false)
        {
            try
            {
                Sound sound = sfxSounds[name];
                PlayClipAtPoint(sound.clip, position, loop);
            }
            catch
            {
           
                //GFunc.Log($"{ex.Message}");
            }
        }

        // 특정 위치에 사운드를 플레이
        private void PlayClipAtPoint(AudioClip clip, Vector3 position, bool loopEnable = false)
        {
            // SFX가 재생될 게임오브젝트 생성
            GameObject gameObject = new GameObject(clip.name + " Audio Source");
            gameObject.transform.parent = sourceParent.transform;
            gameObject.transform.position = position;

            // 오디오 소스 지정
            AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
            audioSource.clip = clip;        // 재생할 오디오 클립
            audioSource.spatialBlend = 1f;  // 3D 공간에서 재생하기 위한 거리 설정
            audioSource.volume = 1f;
            audioSource.Play();
            audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];

            // 루프와 관련된 경우
            audioSource.loop = loopEnable;
            if (loopEnable == false)
            {
                UnityEngine.Object.Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
            }
        }
        #endregion


        #region ######################_Add Audio_#####################
        /// <summary>
        /// BGM을 추가하는 메서드
        /// </summary>
        /// <param name="name">"Audio/BGM/" 경로의 파일명을 입력</param>
        public void AddBGM(string name)
        {
            audioPath[0] = "Audio/BGM/";
            audioPath[1] = name;
            string path = GFunc.SumString(audioPath);
            AudioClip audio = Resources.Load<AudioClip>(path);

            if (audio == null)
            {
                return;
            }
            if (musicSounds.ContainsKey(name))
            {
                //GFunc.Log(name + "은 이미 등록된 BGM입니다.");
                return;
            }

            Sound newSound = new Sound();
            newSound.name = name;
            newSound.clip = audio;

            musicSounds.Add(name, newSound);

        }
        /// <summary>
        /// 사운드 이펙트를 추가하는 메서드
        /// </summary>
        /// <param name="name">"Audio/SFX/" 경로의 파일명을 입력</param>
        public void AddSFX(string name)
        {
            audioPath[0] = "Audio/SFX/";
            audioPath[1] = name;
            AudioClip audio = Resources.Load<AudioClip>(GFunc.SumString(audioPath));

            if (audio == null)
            {
                return;
            }
            if (sfxSounds.ContainsKey(name))
            {
                return;
            }

            Sound newSound = new Sound();
            newSound.name = name;
            newSound.clip = audio;
            newSound.lastSoundTime = Time.time - 0.1f;

            sfxSounds.Add(name, newSound);
        }

        #endregion


    }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public float lastSoundTime;
    }
}