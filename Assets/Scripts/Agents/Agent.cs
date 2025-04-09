using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Agent : MonoBehaviour
{
    [Header("Agent Settings")]
    public float speed = 3f;
    public float perceptionRadius = 5f;
    public float maxForce = 10f;
    public float proximityRadius = 1.5f;

    [Header("Debug")]
    [SerializeField] protected Transform currentTarget;

    protected Rigidbody2D rb;
    protected SteeringBehaviour steering;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        steering = new SteeringBehaviour();
    }

    protected virtual void Start()
    {
        InitAgent();
    }

    protected virtual void Update()
    {
        PerceptionCheck();
        StateManager();
        MovementManager();
    }

    protected abstract void InitAgent();
    protected abstract void PerceptionCheck();
    protected abstract void StateManager();

    protected virtual void MovementManager() { }

    protected void LookForTarget(LayerMask targetLayer, out Transform target)
    {
        target = null;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, perceptionRadius, targetLayer);
        float closest = float.MaxValue;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < closest)
            {
                closest = dist;
                target = hit.transform;
            }
        }

        currentTarget = target;
    }

    public virtual void Die(float timeToDie = 5f)
    {
        TimerManager.Instance.StartTimer(timeToDie, () =>
        {
            Destroy(gameObject);
        });
    }

    protected virtual void OnDrawGizmosSelected()
    {
        DrawGizmos();
    }

    protected virtual void DrawGizmos() { }

}
