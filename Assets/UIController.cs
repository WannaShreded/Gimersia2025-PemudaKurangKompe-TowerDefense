using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text wavesText;
    [SerializeField] private GameObject pausePanel;
    private bool _isGamePaused = false;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject missionCompletePanel;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Subscribe to wave change events if EnemySpawner exposes them
        try
        {
            EnemySpawner.onWaveChanged.AddListener(UpdateWavesText);
        }
        catch {
            // If EnemySpawner or the event isn't present yet, we'll handle in Start/OnSceneLoaded
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        try
        {
            EnemySpawner.onWaveChanged.RemoveListener(UpdateWavesText);
        }
        catch { }
    }

    private void Start()
    {
        if (livesText == null)
        {
            Debug.LogError("Assign a TMP_Text to UIController.livesText in the inspector.");
            return;
        }

        // Try to find pause panel if not assigned
        if (pausePanel == null)
        {
            pausePanel = GameObject.Find("PausePanel");
            if (pausePanel == null)
            {
                Debug.LogError("Could not find PausePanel in the scene. Make sure it's named 'PausePanel'");
            }
        }

        // Try to find game over panel if not assigned
        if (gameOverPanel == null)
        {
            gameOverPanel = GameObject.Find("GameOverPanel");
            if (gameOverPanel == null)
            {
                Debug.LogError("Could not find GameOverPanel in the scene. Make sure it's named 'GameOverPanel'");
            }
        }

        if (LifeSystem.main != null)
        {
            // initialize display
            UpdateLivesText(LifeSystem.main.GetLives());
            // subscribe to changes
            LifeSystem.main.onLivesChanged += UpdateLivesText;
        }
        else
        {
            Debug.LogWarning("LifeSystem not found in scene. Add LifeSystem to a GameObject.");
        }

        // Ensure panels are hidden at start
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Try to find waves text if it's not assigned and initialize it from EnemySpawner
        if (wavesText == null)
        {
            GameObject wt = GameObject.Find("WavesText");
            if (wt != null)
                wavesText = wt.GetComponent<TMP_Text>();
        }

        if (wavesText != null)
        {
            // Try to find an EnemySpawner instance to read the current wave
            EnemySpawner sp = FindObjectOfType<EnemySpawner>();
            if (sp != null)
            {
                UpdateWavesText(sp.CurrentWave);
            }
            else
            {
                wavesText.text = "Wave: 0";
            }
        }
    }

    private void Update() {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            TogglePause();
        }
    }

    private void OnDestroy()
    {
        if (LifeSystem.main != null)
            LifeSystem.main.onLivesChanged -= UpdateLivesText;

        // Ensure we don't leave the game paused if this object is destroyed
        if (_isGamePaused)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            AudioListener.pause = false;
        }
    }

    private void UpdateLivesText(int currentLives)
    {
        if (livesText != null)
            livesText.text = $"Lives: {currentLives}";

        if (currentLives <= 0)
        {
            ShowGameOver();
        }
    }

    private void UpdateWavesText(int currentWave)
    {
        if (wavesText != null)
        {
            wavesText.text = $"Wave: {currentWave}";
        }
    }

    private void ShowPausePanel()
    {
        // Try one last time to find the panel if it's null
        if (pausePanel == null)
        {
            pausePanel = GameObject.Find("PausePanel");
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        else
        {
            Debug.LogError("PausePanel not found in scene. Create a UI Panel named 'PausePanel'");
        }
    }

    public void HidePausePanel()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void TogglePause() {
        if (_isGamePaused)
        {
            // Resume
            HidePausePanel();
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f; // default fixed delta
            AudioListener.pause = false;
            _isGamePaused = false;
        }
        else
        {
            // Pause
            ShowPausePanel();
            Time.timeScale = 0f;
            // When timeScale is 0, physics won't advance; keep fixedDeltaTime safe
            Time.fixedDeltaTime = 0f;
            AudioListener.pause = true;
            _isGamePaused = true;
        }
    }
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowGameOver()
    {
        // Try one last time to find the panel if it's null
        if (gameOverPanel == null)
        {
            gameOverPanel = GameObject.Find("GameOverPanel");
        }

        if (gameOverPanel != null)
        {
            Time.timeScale = 0f;
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("GameOverPanel not found in scene. Create a UI Panel named 'GameOverPanel'");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Try to find objective text if it's not assigned
        if (objectiveText == null)
        {
            GameObject obj = GameObject.Find("ObjectiveText");
            if (obj != null)
            {
                objectiveText = obj.GetComponent<TMP_Text>();
            }
        }

        if (objectiveText != null)
        {
            StartCoroutine(ShowObjective());
        }
        else
        {
            Debug.LogWarning("ObjectiveText not found. Make sure you have a TMP_Text named 'ObjectiveText' in the scene.");
        }
    }

    private IEnumerator ShowObjective()
    {
        if (objectiveText != null)
        {
            objectiveText.text = "Objective: Defend your base! Survive 10 waves.";
            objectiveText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            objectiveText.gameObject.SetActive(false);
        }
    }
    
    // private void ShowMissionCompelete() {}
} 
