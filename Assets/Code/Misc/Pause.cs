using UnityEngine;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    public bool paused;

    [SerializeField] private CanvasGroup pauseCanvas;
    
    void Update()
    {
        if(Keyboard.current.escapeKey.wasPressedThisFrame)
            paused = !paused;
        
        pauseCanvas.alpha = Mathf.Lerp(pauseCanvas.alpha, paused ? 1 : 0, Time.deltaTime * 10f);
        pauseCanvas.interactable = paused;
        pauseCanvas.blocksRaycasts = paused;
    }
    
    public void Resume()
    {
        paused = false;
    }
}
