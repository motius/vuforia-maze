using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public static GameController Instance;
    public Transform StartPosition;
    public Transform Player;
    public Text LevelTime, EndTime;
    public Canvas InGameCanvas, FinishCanvas;

    public EnumGameState GameState { get; private set; }
    public bool RedCollected { get; private set; }
    public bool GreenCollected { get; private set; }
    public bool YellowCollected { get; private set; }

    public Camera ARCamera;

    private DateTime _levelStartTime, _levelFinishTime;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(gameObject);
        ARCamera.depthTextureMode = DepthTextureMode.Depth;
    }

    void Start()
    {
        RestartLevel();
    }
    
    void Update()
    {
        switch (GameState)
        {
            case EnumGameState.Playing:
                var currentLevelTime = DateTime.Now - _levelStartTime;
                LevelTime.text = currentLevelTime.Minutes.ToString("D2") + ":" + currentLevelTime.Seconds.ToString("D2");
                break;
            case EnumGameState.Paused:
                _levelStartTime = _levelStartTime.AddSeconds(Time.deltaTime);
                break;
            case EnumGameState.Finished:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void RestartLevel()
    {
        RedCollected = false;
        GreenCollected = false;
        YellowCollected = false;

        _levelStartTime = DateTime.Now;
        Player.GetComponent<NavMeshAgent>().Warp(StartPosition.position);
        FinishCanvas.enabled = false;
        InGameCanvas.enabled = true;

        GameState = EnumGameState.Playing;
    }

    public void FinishLevel()
    {
        _levelFinishTime = DateTime.Now;
        var totalTime = (_levelFinishTime - _levelStartTime);
        LevelTime.text = totalTime.Minutes.ToString("D2") + ":" + totalTime.Seconds.ToString("D2");
        EndTime.text = EndTime.text.Remove(EndTime.text.Length - 5) + LevelTime.text;


        FinishCanvas.enabled = true;
        InGameCanvas.enabled = false;

        GameState = EnumGameState.Paused;
    }

    public void CollectOrb(EnumOrbType orb)
    {
        Debug.Log("Collected orb: " + Enum.GetName(typeof(EnumOrbType), orb));
        switch (orb)
        {
            case EnumOrbType.Green:
                GreenCollected = true;
                break;
            case EnumOrbType.Red:
                RedCollected = true;
                break;
            case EnumOrbType.Yellow:
                YellowCollected = true;
                break;
            default:
                throw new ArgumentOutOfRangeException("orb", orb, null);
        }
    }
}

public enum EnumOrbType
{
    Green, Red, Yellow
}

public enum EnumGameState
{
    Playing, Paused, Finished
}
