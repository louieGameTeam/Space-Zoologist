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

    static bool isTransitioning = false;
    static float sceneTransitionFadeSpeed = 2;

    public static IEnumerator SceneTransition (string sceneName) {
        if (isTransitioning) {
            Debug.LogWarning ("Tried to load new scene while scene loading!");
            yield break;
        }

        // Setup
        isTransitioning = true;
        GameObject sceneTransitionPrefab = Resources.Load ("UI/Effects/SceneTransition") as GameObject;
        GameObject sceneTransition = Instantiate (sceneTransitionPrefab);
        DontDestroyOnLoad (sceneTransition);
        //sceneTransition.GetComponentInChildren<Animation> ().Play ();
        Material mat = sceneTransition.transform.GetChild (0).GetChild (0).GetComponent<Image> ().material;

        // Fade in
        for (float i = 0; i < 1.0f; i += Time.deltaTime * sceneTransitionFadeSpeed) {
            mat.SetFloat ("_Fade", i);
            yield return null;
        }
        mat.SetFloat ("_Fade", 1);

        // Load scene
        SceneManager.LoadScene (sceneName);

        // Fade out
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
