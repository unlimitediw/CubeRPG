using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public event Action BeforeSceneUnload;
    public event Action AfterSceneLoad;


    public CanvasGroup faderCanvasGroup;
    public float fadeDuration = 1f;
    public string startingSceneName = "Base";
    public string initialStartingPositionName = "Turns";
    public SaveData playerSaveData;

    public GameObject camera1;
    
    
    private bool isFading;


    private IEnumerator Start ()
    {
        faderCanvasGroup.alpha = 1f;

        playerSaveData.Save (MyRpgController.startingPositionKey, initialStartingPositionName);

        yield return StartCoroutine (LoadSceneAndSetActive (startingSceneName));

        StartCoroutine (Fade (0f));
    }

    private void Update()
    {
        if (faderCanvasGroup.alpha == 1)
        {
            camera1.SetActive(true);
        }
        else
        {
            camera1.SetActive(false);
        }
    }

    public void FadeAndLoadScene (SceneReaction sceneReaction)
    {
        if (!isFading)
        {
            StartCoroutine (FadeAndSwitchScenes (sceneReaction.sceneName));
        }
    }

    public void BaseFadeAndLoadScene(string sceneName)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName));
        }
    }


    private IEnumerator FadeAndSwitchScenes (string sceneName)
    {
        yield return StartCoroutine (Fade (1f));

        if (BeforeSceneUnload != null)
            BeforeSceneUnload ();

        yield return SceneManager.UnloadSceneAsync (SceneManager.GetActiveScene ().buildIndex);

        yield return StartCoroutine (LoadSceneAndSetActive (sceneName));

        if (AfterSceneLoad != null)
            AfterSceneLoad ();
        
        yield return StartCoroutine (Fade (0f));
    }


    private IEnumerator LoadSceneAndSetActive (string sceneName)
    {
        yield return SceneManager.LoadSceneAsync (sceneName, LoadSceneMode.Additive);

        Scene newlyLoadedScene = SceneManager.GetSceneAt (SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene (newlyLoadedScene);
    }


    private IEnumerator Fade (float finalAlpha)
    {
        isFading = true;
        faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs (faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        while (!Mathf.Approximately (faderCanvasGroup.alpha, finalAlpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards (faderCanvasGroup.alpha, finalAlpha,
                fadeSpeed * Time.deltaTime);

            yield return null;
        }

        isFading = false;
        faderCanvasGroup.blocksRaycasts = false;
    }
}
