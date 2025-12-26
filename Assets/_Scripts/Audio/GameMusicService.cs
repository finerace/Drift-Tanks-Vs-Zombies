using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameMusicService : MonoBehaviour
{

    [SerializeField] private AudioClip[] roadMusic;
    [SerializeField] private AudioClip idleMusic;

    [Space] 
    
    [SerializeField] private AudioSource mainMusicSource;

    private Coroutine roadMusicCoroutine;

    private void Awake()
    {
        mainMusicSource.clip = idleMusic;
        mainMusicSource.Play();
        
        GlobalGameEvents.instance.levelStartChangeState += MainMusicAlgorithm;
        GlobalGameEvents.instance.trainingStartChangeState += MainMusicAlgorithm;
    }

    private void MainMusicAlgorithm(bool isLevelStarted)
    {
        void DeleteCoroutine()
        {
            if (roadMusicCoroutine != null)
            {
                StopCoroutine(roadMusicCoroutine);
                roadMusicCoroutine = null;
            }
        }

        if (isLevelStarted)
        {
            DeleteCoroutine();

            roadMusicCoroutine = StartCoroutine(RoadMusicLoop());

            return;
        }

        DeleteCoroutine();
        
        mainMusicSource.clip = idleMusic;
        mainMusicSource.Play();
    }

    private IEnumerator RoadMusicLoop()
    {
        while (true)
        {
            var targetMusic = roadMusic[Random.Range(0, roadMusic.Length)];
            
            mainMusicSource.clip = targetMusic;
            mainMusicSource.Play();
            
            yield return new WaitForSeconds(targetMusic.length);
        }
        
    }

}
