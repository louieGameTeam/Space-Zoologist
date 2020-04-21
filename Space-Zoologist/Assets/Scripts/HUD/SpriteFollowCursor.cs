using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SetupItemPreviewEvent : UnityEvent<Sprite> { }
public class SpriteFollowCursor : MonoBehaviour
{
    [SerializeField] float InterpolationValue = 0.5f;
    private bool Following;

    // Update is called once per frame
    void Update()
    {
        if (this.Following)
        {
            this.gameObject.transform.position = Vector2.Lerp(this.gameObject.transform.position, Input.mousePosition, this.InterpolationValue);
        }
    }

    /// <summary>
    /// Takes the sprite off of the GameObject and attaches it to presized GameObject
    /// </summary>
    /// <param name="GameObjectSprite"></param>
    public void SetupSpriteToFollow(Sprite sprite)
    {
        this.gameObject.GetComponent<Image>().sprite = sprite;
        this.gameObject.transform.position = Input.mousePosition;
        this.gameObject.SetActive(true);
        this.Following = true;
    }

    public void StopFollowing()
    {
        this.gameObject.SetActive(false);
        this.Following = false;
    }
}
