using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    private static readonly int MainColourID = Shader.PropertyToID("_Main_Colour");
    private static readonly int ComplementaryColourID = Shader.PropertyToID("_Complementary_Colour");
    private static readonly int FresnelColourID = Shader.PropertyToID("_Fresnel_Colour");

    public delegate void MenuChange(TitleScreenState state);
    public static MenuChange OnMenuChange;

    public static TitleScreen Instance;
    
    public enum TitleScreenState
    {
        MainMenu,
        SettingsMenu,
        InfoMenu,
        TutorialMenu
    }

    [System.Serializable]
    private struct MenuColours
    {
        public TitleScreenState state;
        public Color mainColour;
        public Color complementaryColour;
        public Color fresnelColour;
    }
    
    [SerializeField] private MenuColours[] menuColours;
    [SerializeField] private Image titleScreenImage;
    
    [Space, SerializeField] private float lerpSpeed = 1f;

    [Header("Settings Menu")] [SerializeField]
    private Image playersHolder;
    
    public TitleScreenState CurrentState;
    private Color _currentMainColour;
    private Color _currentComplementaryColour;
    private Color _currentFresnelColour;
    
    private MenuColours _currentMenuColour;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentState = TitleScreenState.MainMenu;
        _currentMenuColour = GetCurrentMenuColour();
        OnMenuChange?.Invoke(CurrentState);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateColours();
    }

    private MenuColours GetCurrentMenuColour()
    {
        foreach (MenuColours menuColour in menuColours)
        {
            if (menuColour.state == CurrentState)
            {
                return menuColour;
            }
        }
        return menuColours[0];
    }

    private void UpdateColours()
    {
        _currentMainColour = Color.Lerp(_currentMainColour, _currentMenuColour.mainColour, Time.deltaTime * lerpSpeed);
        _currentComplementaryColour = Color.Lerp(_currentComplementaryColour, _currentMenuColour.complementaryColour, Time.deltaTime * lerpSpeed);
        _currentFresnelColour = Color.Lerp(_currentFresnelColour, _currentMenuColour.fresnelColour, Time.deltaTime * lerpSpeed);
        
        titleScreenImage.material.SetColor(MainColourID, _currentMainColour);
        titleScreenImage.material.SetColor(ComplementaryColourID, _currentComplementaryColour);
        titleScreenImage.material.SetColor(FresnelColourID, _currentFresnelColour);
    }
    
    public void PlayGame()
    {
        switch (CurrentState)
        {
            case TitleScreenState.MainMenu:
                CurrentState = TitleScreenState.SettingsMenu;
                break;
            case TitleScreenState.SettingsMenu:
                CurrentState = TitleScreenState.TutorialMenu;
                break;
            case TitleScreenState.TutorialMenu:
                CurrentState = TitleScreenState.MainMenu;
                break;
        }
        
        _currentMenuColour = GetCurrentMenuColour();
        OnMenuChange?.Invoke(CurrentState);
    }

    public void Info()
    {
        if (CurrentState == TitleScreenState.InfoMenu)
        {
            CurrentState = TitleScreenState.MainMenu;
            _currentMenuColour = GetCurrentMenuColour();
            OnMenuChange?.Invoke(CurrentState);
            return;
        }
        
        CurrentState = TitleScreenState.InfoMenu;
        _currentMenuColour = GetCurrentMenuColour();
        OnMenuChange?.Invoke(CurrentState);
    }
    
    public void MenuBack()
    {
        switch (CurrentState)
        {
            case TitleScreenState.SettingsMenu:
            case TitleScreenState.InfoMenu:
                CurrentState = TitleScreenState.MainMenu;
                break;
            case TitleScreenState.TutorialMenu:
                CurrentState = TitleScreenState.SettingsMenu;
                break;

        }
        
        _currentMenuColour = GetCurrentMenuColour();
        OnMenuChange?.Invoke(CurrentState);
    }
    
    public void ExitGame()
    {
        Application.Quit();
        print("Quit Game");
    }
}
