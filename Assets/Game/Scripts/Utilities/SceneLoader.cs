#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Singleton class for loading scenes
/// </summary>
public class SceneLoader : Singleton<SceneLoader>
{
    [Header("Loading Screen References")]
    [SerializeField, Tooltip("Root GameObject of the entire loading screen (usually the Canvas)")]
    private GameObject loadingScreenUI = null!;

    [SerializeField, Tooltip("Progress bar (0–1 range)")]
    private Slider progressBar = null!;

    [SerializeField, Tooltip("Optional: text showing scene name or tip")]
    private TextMeshProUGUI progressText = null!;

    [SerializeField, Range(0.1f, 2f), Tooltip("Minimum time the loading screen is shown")]
    private float minimumDisplayTime = 0.6f;
    [SerializeField]
    private List<string> managerScenesToLoad = new();

    [SerializeField, Tooltip("The scene to load after all managers are initialized")]
    private string firstGameplayScene = null!;

    // Events (useful for UI animations, sound, etc.)
    public event Action? OnLoadingStarted;
    public event Action? OnLoadingCompleted;

    private bool isLoading;

    public override void Awake()
    {
        base.Awake();
        if (loadingScreenUI != null)
        {
            loadingScreenUI.SetActive(false);
        }
    }

    private void Start()
    {
        // If we are in the bootstrap scene (index 0), start the sequence.
        // This allows us to use the SceneLoader in other scenes for manual testing if needed.
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartCoroutine(BootstrapRoutine());
        }
    }

    private IEnumerator BootstrapRoutine()
    {
        // 1. Show the loading screen
        ShowLoadingScreenImmediately();

        // 2. Load all manager scenes additively
        foreach (var sceneObject in managerScenesToLoad)
        {
            if (SceneIsAlreadyLoaded(sceneObject)) continue;

            Debug.Log($"[Bootstrap] Loading manager scene: {sceneObject}");
            // We use a internal load method that doesn't mess with the 'isLoading' flag
            // to allow multiple additive loads during bootstrap.
            yield return StartCoroutine(LoadSceneInternal(sceneObject, LoadSceneMode.Additive));
        }

        // 3. Wait a frame to ensure all Awake/OnEnable in managers have run
        yield return null;

        // 4. Load the first gameplay scene (Main Menu)
        Debug.Log($"[Bootstrap] Loading first gameplay scene: {firstGameplayScene}");
        if (firstGameplayScene != null)
        {
            yield return StartCoroutine(LoadSceneInternal(firstGameplayScene, LoadSceneMode.Single));
            // 5. Initialize Game State
            if (GameManager.Instance != null)
            {
                //if (firstGameplayScene.name == "MainMenu")
                //{
                //    GameManager.Instance.SwitchState(new MainMenuState());
                //}
                //else
                //{
                //    GameManager.Instance.SwitchState(new NewGameState());
                //}
            }
            else
            {
                Debug.LogError("[Bootstrap] GameManager instance not found after loading manager scenes!");
            }
        }

        // 6. Finalize
        HideLoadingScreenImmediately();
        OnLoadingCompleted?.Invoke();
    }

    /// <summary>
    /// Internal loading logic used by Bootstrap and Public methods
    /// </summary>
    private IEnumerator LoadSceneInternal(string sceneName, LoadSceneMode mode)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);
        if (op == null) yield break;
        progressText.text = "Loading " + sceneName;
        while (!op.isDone)
        {
            if (progressBar != null)
            {
                progressBar.value = op.progress;
            }
            yield return null;
        }
    }

    private void OnValidate()
    {
        // Small quality-of-life in editor
        if (loadingScreenUI != null && loadingScreenUI.GetComponent<Canvas>() == null)
        {
            Debug.LogWarning("Loading screen root should have a Canvas component", this);
        }
    }

    /// <summary>
    /// Load scene by name
    /// </summary>
    public void LoadScene(string sceneName, LoadSceneMode mode, Action? onComplete = null)
    {
        if (isLoading) return;
        if (SceneIsAlreadyLoaded(sceneName)) return;
        StartCoroutine(LoadSceneRoutine(SceneLoadParams.ByName(sceneName, mode), onComplete));
    }

    /// <summary>
    /// Load scene by build index
    /// </summary>
    public void LoadScene(int sceneBuildIndex, LoadSceneMode mode, Action? onComplete = null)
    {
        if (isLoading) return;
        StartCoroutine(LoadSceneRoutine(SceneLoadParams.ByIndex(sceneBuildIndex, mode), onComplete));
    }

    /// <summary>
    /// Main loading routine
    /// </summary>
    private IEnumerator LoadSceneRoutine(SceneLoadParams parameters, Action? onComplete)
    {
        if (isLoading)
        {
            yield break;
        }

        isLoading = true;
        OnLoadingStarted?.Invoke();

        // Safety guard
        if (loadingScreenUI == null || progressBar == null)
        {
            Debug.LogError("SceneLoader: Loading screen or progress bar is not assigned!", this);
            isLoading = false;
            yield break;
        }

        loadingScreenUI.SetActive(true);

        // Optional: give one frame for UI to appear / fade in
        yield return null;

        AsyncOperation operation = parameters.Load();

        // Protect against invalid scene requests
        if (operation == null)
        {
            Debug.LogError($"SceneLoader: Cannot load scene {parameters}", this);
            loadingScreenUI.SetActive(false);
            isLoading = false;
            yield break;
        }

        operation.allowSceneActivation = true; // usually true, but can be controlled

        float startTime = Time.unscaledTime;

        while (!operation.isDone)
        {
            // Unity's AsyncOperation.progress goes 0 → 0.9 (scene loading)
            // then jumps to 1 when allowSceneActivation = true and activation finishes
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // Optional: smooth fake progress after 90% (common UX trick)
            // float displayedProgress = Mathf.MoveTowards(progressBar.value, progress, Time.unscaledDeltaTime * 1.5f);

            progressBar.value = progress;

            // Optional text update
            if (progressText != null)
            {
                progressText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";
            }

            yield return null;
        }

        // Enforce minimum display time (better UX)
        float elapsed = Time.unscaledTime - startTime;
        if (elapsed < minimumDisplayTime)
        {
            yield return new WaitForSecondsRealtime(minimumDisplayTime - elapsed);
        }

        loadingScreenUI.SetActive(false);

        isLoading = false;
        OnLoadingCompleted?.Invoke();

        onComplete?.Invoke();
    }

    /// <summary>
    /// Unloads a scene by its build index.
    /// </summary>
    public void UnloadScene(int sceneBuildIndex)
    {
        SceneManager.UnloadSceneAsync(sceneBuildIndex);
    }

    /// <summary>
    /// Helper struct to unify name/index loading
    /// </summary>
    private readonly struct SceneLoadParams
    {
        private readonly string? name;
        private readonly int? index;
        private readonly LoadSceneMode mode;

        private SceneLoadParams(string? name, int? index, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            this.name = name;
            this.index = index;
            this.mode = mode;
        }

        public static SceneLoadParams ByName(string sceneName, LoadSceneMode mode) => new(sceneName, null, mode);
        public static SceneLoadParams ByIndex(int buildIndex, LoadSceneMode mode) => new(null, buildIndex, mode);

        public AsyncOperation Load()
        {
            if (name != null) return SceneManager.LoadSceneAsync(name, mode);
            if (index.HasValue) return SceneManager.LoadSceneAsync(index.Value, mode);
            return null!;
        }

        public override string ToString() => name ?? $"build index {index}";
    }

    /// <summary>
    /// Quick helper methods
    /// </summary>
    public bool IsLoading => isLoading;

    /// <summary>
    /// Show loading screen immediately
    /// </summary>
    public void ShowLoadingScreenImmediately() => loadingScreenUI?.SetActive(true);

    /// <summary>
    /// Hide loading screen immediately
    /// </summary>
    public void HideLoadingScreenImmediately() => loadingScreenUI?.SetActive(false);

    /// <summary>
    /// Helper method to check if a scene is already loaded
    /// </summary>
    private static bool SceneIsAlreadyLoaded(string name)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (s.name == name && s.isLoaded) return true;
        }
        return false;
    }

}