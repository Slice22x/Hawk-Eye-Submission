using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    public delegate void Transitioning();
    public static Transitioning OnTransitioning;
    
    public static GameSettings Instance;
    [SerializeField] private int minRemainingCards;
    [SerializeField] private Fade transitionScreen;

    public delegate void GameStart();
    public static GameStart OnGameStart;
    
    public List<string> PlayerNames;
    public int StartingCards;
    public int GainCardsAfterPlacing;
    
    private string _nextSceneName;
    private bool _transitioning;
    private bool _startTransition;
    
    private const int PLAYABLE_CARDS = 56;
    private const string GAME_SCENE = "MainGame";
    private const string TITLE_SCENE = "TitleScreen";
    
    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        OnGameStart += () =>
        {
            if(WarnStartingCards() || PlayerNames.Count < 2 || StartingCards == 0) return;

            StartTransitionGame();
        };
    }

    private void StartTransitionGame()
    {
        transitionScreen.Active = false;
        _transitioning = false;
        _startTransition = true;
        _nextSceneName = GAME_SCENE;
    }

    public void StartTransitionTitle()
    {
        transitionScreen.Active = false;
        _transitioning = false;
        _startTransition = true;
        _nextSceneName = TITLE_SCENE;
    }
    
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == _nextSceneName)
        {
            transitionScreen.Active = true;
            _startTransition = false;
        }

        if (transitionScreen.Done && !_transitioning && _startTransition)
        {
            _transitioning = true;
            SceneManager.LoadScene(_nextSceneName);
            OnTransitioning?.Invoke();
        }
    }

    public void UpdateStartingCards(int amount)
    {
        StartingCards = amount;
    }

    public bool WarnStartingCards()
    {
        return PLAYABLE_CARDS - (StartingCards * PlayerNames.Count) < minRemainingCards;
    }
    
    public int AddPlayer(string playerName)
    {
        PlayerNames.Add(playerName);
        
        for(int i = 0; i < PlayerNames.Count; i++)
        {
            if(PlayerNames[i] == playerName)
            {
                return i;
            }

            if (string.IsNullOrEmpty(PlayerNames[i]))
            {
                PlayerNames[i] = playerName;
                return i;
            }
        }
        return PlayerNames.Count;
    }
}
