using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public static GameController Instance;
    public Transform StartPosition;
    public Transform Player;
    public Text LevelTime;

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

    private void RestartLevel()
    {
        RedCollected = false;
        GreenCollected = false;
        YellowCollected = false;

        _levelStartTime = DateTime.Now;
        Player.SetPositionAndRotation(StartPosition.position, StartPosition.rotation);

        GameState = EnumGameState.Playing;
    }

    private void FinishLevel()
    {
        _levelFinishTime = DateTime.Now;
        var totalTime = (_levelFinishTime - _levelStartTime);
        LevelTime.text = totalTime.Minutes.ToString("D2") + ":" + totalTime.Seconds.ToString("D2");

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
