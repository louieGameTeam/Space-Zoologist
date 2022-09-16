using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoadEffectsHandler : MonoBehaviour {
    public static LevelLoadEffectsHandler Instance {
        get {
            if (instance == null) {
                instance = new GameObject ().AddComponent<LevelLoadEffectsHandler>();
                DontDestroyOnLoad (instance.gameObject);
            }
            return instance;
        }
        private set {
            instance = value;
        }
    }

    static LevelLoadEffectsHandler instance;

    static bool useTransitionEffects = true;
    static bool isTransitioning = false;
    static float sceneTransitionFadeSpeed = 1.5f;
    static float sceneTransitionWaitTime = 0.5f;

    public static IEnumerator SceneTransition (string sceneName) {
        if (isTransitioning) {
            Debug.LogWarning ("Tried to load new scene while scene loading!");
            yield break;
        }

        if (!useTransitionEffects) {
            SceneManager.LoadScene (sceneName);
            yield break;
        }

        // Setup
        isTransitioning = true;
        GameObject sceneTransitionPrefab = Resources.Load ("UI/Effects/SceneTransition") as GameObject;
        GameObject sceneTransition = Instantiate (sceneTransitionPrefab);
        DontDestroyOnLoad (sceneTransition);
        Material mat = sceneTransition.transform.GetChild (0).GetChild (0).GetComponent<Image> ().material;
        Transform zeig = sceneTransition.transform.GetChild (0).GetChild (1);
        zeig.gameObject.SetActive (false);

        // Fade in
        mat.SetFloat ("_Flip", 0);
        for (float i = 0; i < 1.0f; i += Time.deltaTime * sceneTransitionFadeSpeed) {
            mat.SetFloat ("_Fade", i);
            if (i > 0.2f) {
                zeig.gameObject.SetActive (true);
            }
            yield return null;
        }
        mat.SetFloat ("_Fade", 1);

        sceneTransition.transform.GetChild (0).GetChild (1).gameObject.SetActive (true);

        yield return new WaitForSeconds (sceneTransitionWaitTime);

        // Load scene
        SceneManager.LoadScene (sceneName);

        yield return new WaitForSeconds (sceneTransitionWaitTime);

        sceneTransition.transform.GetChild (0).GetChild (1).gameObject.SetActive (false);

        // Fade out
        mat.SetFloat ("_Flip", 1);
        zeig.gameObject.SetActive (false);
        for (float i = 1; i > 0.0f; i -= Time.deltaTime * sceneTransitionFadeSpeed) {
            mat.SetFloat ("_Fade", i);
            yield return null;
        }
        mat.SetFloat ("_Fade", 0);

        // Cleanup
        Destroy (sceneTransition);
        isTransitioning = false;
    }
}
