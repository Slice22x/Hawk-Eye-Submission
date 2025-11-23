using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] pages;
    [SerializeField] private float responsiveness;

    [SerializeField] private TMP_Text header;
    [SerializeField] private Image leftButton, rightButton, playButton;
    [SerializeField] private Grow leftGrow, rightGrow;
    
    private int _currentPage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _currentPage = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(TitleScreen.Instance.CurrentState != TitleScreen.TitleScreenState.TutorialMenu)
        {
            rightButton.gameObject.SetActive(false);
            leftButton.gameObject.SetActive(false);
            playButton.gameObject.SetActive(true);
            
            foreach (CanvasGroup page in pages)
            {
                page.alpha = Mathf.Lerp(page.alpha, 0f, Time.deltaTime * responsiveness);
            }
            
            return;
        }
        
        foreach (CanvasGroup page in pages)
        {
            page.alpha = Mathf.Lerp(page.alpha, page == pages[_currentPage] ? 1f : 0f, Time.deltaTime * responsiveness);
        }

        header.enabled = _currentPage == 0;
        
        leftButton.gameObject.SetActive(_currentPage > 0);
        rightButton.gameObject.SetActive(_currentPage < pages.Length - 1);
        
        leftGrow.enabled = _currentPage > 0;
        rightGrow.enabled = _currentPage < pages.Length - 1;
        
        playButton.gameObject.SetActive(_currentPage >= pages.Length - 1);
    }

    public void IncrementPage()
    {
        if(_currentPage >= pages.Length - 1) return;
        
        _currentPage++;
    }
    
    public void DecrementPage()
    {
        if(_currentPage <= 0) return;
        
        _currentPage--;
    }
}
