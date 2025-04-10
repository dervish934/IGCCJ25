using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AgentExample : Agent
{
    public enum AgentState { Idle, Pursuit, PathFollow, Cover, Evading}//Ejemplo de los diferentes estados que puede tener el agente

    [Header("Sensor Settings")]
    public float meleeSensorRadius = 0.5f;//Ejemplo de cuando alguien esté a mele para que lo acuchiche 
    public LayerMask enemyDetectionMask;//Usar layer mask o tags para filtrar entre las percepciones()
    //public Vector2[] pathFollowPoints;
    [Header("Path Settings")]
    public PathSystem currentPath;

    [Header("Debug")]
    [SerializeField] private AgentState currentState;

    private Animator animator;
    private float normalSpeed;
    private int currentPathFollowPoint;
    

    protected override void Start()
    {
        base.Start();
        currentPathFollowPoint = 0;
        normalSpeed = 5;
        animator = GetComponent<Animator>();
        normalSpeed = speed;
        SetState(AgentState.Pursuit);
    }

    private void FixedUpdate()
    {
        StateManager();
    }

    protected override void InitAgent()
    {
        
    }

    //Revisar sus alrededores constantemente para tomar decisiones
    protected override void PerceptionCheck()
    {
        //LookForTarget(enemyDetectionMask, out currentTarget);
    }

    //Aqui defines el comportamiento del agente para cada posible estado que tenga
    protected override void StateManager()
    {
        switch (currentState)
        {
            case AgentState.Pursuit:
                if (currentPath.PathPoints.Length == 0) return;
                rb.linearVelocity = steering.PathFollowing(currentPath.PathPoints, this.transform, this.rb, normalSpeed, 0.5f, 1);//llamas al steering correspondiente
                //Esto solo es para dibujar la linea de direccion
                GameObject temp = new GameObject("TempTarget");
                temp.transform.position = currentPath.PathPoints[steering.currentWaypoint];
                temp.transform.rotation = Quaternion.identity;
                temp.transform.localScale = Vector3.one;

                currentTarget = temp.transform;
                break;
        }
    }

    protected override void MovementManager()
    {
    }

    private void SetState(AgentState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case AgentState.Pursuit:
                //animator.Play("Charger_Idle");
                break;
        }
    }


    public override void Die(float timeToDie = 1f)
    {
        //animator.Play("Me_Muero");Animación de muerte del agente y luego muere
        base.Die(timeToDie);
    }

    //Gizmos para debug
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeSensorRadius);

        if(currentTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(currentPath.PathPoints[steering.currentWaypoint], 0.2f);
        }

        if (currentTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }
}
