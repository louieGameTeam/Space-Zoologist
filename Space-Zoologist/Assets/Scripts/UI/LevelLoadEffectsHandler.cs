using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public static IEnumerator SceneTransition (string sceneName) {
        if (isTransitioning) {
            Debug.LogWarning ("Tried to load new scene while scene loading!");
            yield break;
        }
        isTransitioning = true;
        GameObject sceneTransitionPrefab = Resources.Load ("UI/Effects/SceneTransition") as GameObject;
        GameObject sceneTransition = Instantiate (sceneTransitionPrefab);
        DontDestroyOnLoad (sceneTransition);
        sceneTransition.GetComponentInChildren<Animation> ().Play ();
        yield return new WaitForSeconds (1f);
        SceneManager.LoadScene (sceneName);
        yield return new WaitForSeconds (1f);
        Destroy (sceneTransition);
        isTransitioning = false;
    }
}
