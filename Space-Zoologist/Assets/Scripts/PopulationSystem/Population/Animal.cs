using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    [SerializeField] public SpriteRenderer Overlay = default;
    public MovementData MovementData { get; private set; }
    public Population PopulationInfo { get; private set; }
    private Animator Animator = null;
    public MovementController MovementController {get; set; }
    private AnimalBehaviorManager AnimalBehaviorManager = default;
    private Vector3 lastPos;

    [SerializeField][EditorReadOnly] private string foodTargetSpeciesName;
    public string FoodTargetSpeciesName => foodTargetSpeciesName;

    public void Start()
    {
        lastPos = this.gameObject.transform.position;
        this.AnimalBehaviorManager = this.GetComponent<AnimalBehaviorManager>();
        if (!this.gameObject.TryGetComponent(out this.Animator))
        {
            this.Animator = null;
            Debug.Log("Animator component not attached");
        }
    }

    public void Initialize(Population population, MovementData data)
    {
        this.MovementController = this.gameObject.GetComponent<MovementController>();
        this.MovementData = data;
        this.PopulationInfo = population;
        this.gameObject.GetComponent<Animator>().runtimeAnimatorController = this.PopulationInfo.Species.AnimatorController;
    }

    void LateUpdate()
    {
        // set the shader data
        SpriteRenderer AnimalSpriteRenderer = GetComponent<SpriteRenderer>();
        Vector2[] uvs = AnimalSpriteRenderer.sprite.uv;
        Vector2 maxUV = new Vector2(0, 0);
        Vector2 minUV = new Vector2(1, 1);

        // since the sprites have a weird number of uvs (probably generated during runtime)
        // find min and max values to use for center uv calculation
        foreach (Vector2 uvCoordinate in uvs)
        {
            if (uvCoordinate.x > maxUV.x)
                maxUV.x = uvCoordinate.x;
            if (uvCoordinate.x < minUV.x)
                minUV.x = uvCoordinate.x;
            if (uvCoordinate.y > maxUV.y)
                maxUV.y = uvCoordinate.y;
            if (uvCoordinate.y < minUV.y)
                minUV.y = uvCoordinate.y;
        }

        AnimalSpriteRenderer.material.SetVector("_Max_UV", maxUV);
        AnimalSpriteRenderer.material.SetVector("_Min_UV", minUV);


        if (this.MovementData == null)
        {
            return;
        }
        float velocity = this.PythagoreanTheorem(lastPos, this.gameObject.transform.position) / Time.deltaTime;
        lastPos = this.gameObject.transform.position;
        if (velocity < 0.01f)
        {
            this.MovementData.MovementStatus = Movement.idle;
            this.UpdateAnimations();
            return;
        }
        if (velocity > MovementData.RunThreshold)
        {
            this.MovementData.MovementStatus = Movement.running;
            this.UpdateAnimations();
            return;
        }
        this.MovementData.MovementStatus = Movement.walking;
        this.UpdateAnimations();
        if (this.AnimalBehaviorManager.activeBehaviorPattern == null)
        {
            this.MovementData.MovementStatus = Movement.idle;
        }
    }

    public void UpdateAnimations()
    {
        if (this.Animator != null && this.MovementData != null)
        {
            this.Animator.SetInteger("Movement", (int)this.MovementData.MovementStatus);
            this.Animator.SetInteger("Direction", (int)this.MovementData.CurrentDirection);
        }
    }

    public void SetAnimatorTrigger(string triggerName)
    {
        Animator.SetTrigger(triggerName);
    }

    public void SetAnimatorBool(string boolName, bool value)
    {
        Animator.SetBool(boolName, value);
    }

    public void SetAnimatorInt(string intName, int value)
    {
        Animator.SetInteger(intName, value);
    }

    public void SetAnimatorFloat(string floatName, float value)
    {
        Animator.SetFloat(floatName, value);
    }

    private float PythagoreanTheorem(Vector3 v1, Vector3 v2)
    {
        float distx = Mathf.Abs(v1.x - v2.x);
        float disty = Mathf.Abs(v1.y - v2.y);
        return Mathf.Sqrt(distx * distx + disty * disty);
    }

    /// <summary>
    /// Updates the current foodTargetSpeciesName for lookup by EatingPattern
    /// </summary>
    /// <returns></returns>
    public void SetFoodTargetSpeciesName(string newFoodTargetSpeciesName)
    {
        foodTargetSpeciesName = newFoodTargetSpeciesName;
    }
}