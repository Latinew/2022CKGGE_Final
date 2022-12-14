using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;
    public static SceneLoader Instance { get { return instance; } }

    [SerializeField] private GameObject LoadingTextObj;
    [SerializeField] private DOTweenAnimation CoverAnim;

    bool loadingInProgress = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        if (loadingInProgress) return;

        StartCoroutine(Cor_LoadingSequence(sceneName));
    }

    public void QuitApplication() { Application.Quit(); }

    private IEnumerator Cor_LoadingSequence(string sceneName)
    {
        loadingInProgress = true;
        Tween tw = CoverAnim.tween;
        CoverAnim.DORestartById("Loading_Start");
        yield return tw.WaitForCompletion();
        yield return new WaitForEndOfFrame();

        LoadingTextObj.SetActive(true);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        yield return new WaitUntil(() => async.isDone);
        yield return new WaitForEndOfFrame();

        LoadingTextObj.SetActive(false);
        CoverAnim.DORestartById("Loading_End");
        yield return tw.WaitForCompletion();
        loadingInProgress = false;
    }

}
