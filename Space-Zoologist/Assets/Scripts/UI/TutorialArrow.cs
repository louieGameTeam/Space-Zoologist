using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArrow : MonoBehaviour
{
    [SerializeField] Vector3 origPos;
    [SerializeField] float timePassed;
    [SerializeField] float period = 1f;
    // Start is called before the first frame update
    void OnEnable()
    {
        timePassed = 0;
        origPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float displacement = 10 * Mathf.Sin(timePassed*2*Mathf.PI/period); // sin function for animation
        Vector3 displacementVector = new Vector3(displacement, displacement);
        transform.localPosition = origPos + displacementVector;
        timePassed += Time.deltaTime;
    }
}
