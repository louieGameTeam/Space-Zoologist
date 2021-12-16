using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class CameraPositionLock
{
    #region Public Properties
    public Vector3 Position => position;
    public float OrthographicSize => orthographicSize;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Position to lock the camera to")]
    private Vector3 position;
    [SerializeField]
    [Tooltip("Orthographic size to lock the camera to")]
    private float orthographicSize;
    [SerializeField]
    [Tooltip("Smooth move time for locking and unlocking the camera")]
    private float smoothingTime = 1f;
    #endregion

    #region Private Fields
    // position/size when the camera was locked, moves back to this when unlocked
    private Vector3 unlockPosition;
    private float unlockOrthographicSize;
    #endregion

    #region Constructors
    public CameraPositionLock(Vector3 position, float orthographicSize, float smoothingTime)
    {
        this.position = position;
        this.orthographicSize = orthographicSize;
        this.smoothingTime = smoothingTime;
    }
    #endregion

    #region Public Methods
    public void Lock(Camera cam)
    {
        // Kill any active tweens
        cam.transform.DOKill();
        cam.DOKill();

        // Set the unlock position and size before moving them
        unlockPosition = cam.transform.position;
        unlockOrthographicSize = cam.orthographicSize;

        // Start new tweens to target sizes
        cam.transform.DOMove(position, smoothingTime).SetEase(Ease.OutQuint);
        cam.DOOrthoSize(orthographicSize, smoothingTime).SetEase(Ease.OutQuint);
    }
    public void Unlock(Camera cam)
    {
        // Kill any active tweens
        cam.transform.DOKill();
        cam.DOKill();

        // Start new tweens to target sizes
        cam.transform.DOMove(unlockPosition, smoothingTime).SetEase(Ease.OutQuint);
        cam.DOOrthoSize(unlockOrthographicSize, smoothingTime).SetEase(Ease.OutQuint);
    }
    #endregion
}
