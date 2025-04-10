using UnityEngine;

[ExecuteInEditMode]
public class PathSystem : MonoBehaviour
{
    [Header("Path Settings")]
    public float waypointRadius = 0.5f;
    public bool loopPath = true;
    public Color pathColor = Color.green;
    public Color waypointColor = Color.yellow;
    public bool alwaysDrawGizmos = true;

    public Vector2[] PathPoints { get; private set; }
    public Transform[] Waypoints { get; private set; }

    private void OnValidate()
    {
        UpdatePathPoints();
    }

    private void Awake()
    {
        UpdatePathPoints();
    }

    private void Update()
    {
        // Solo en el editor, actualiza constantemente los puntos
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UpdatePathPoints();
        }
#endif
    }

    public void UpdatePathPoints()
    {
        // Verificar si hay cambios en la jerarquía
        if (Waypoints == null || transform.childCount != Waypoints.Length)
        {
            InitializeWaypoints();
            return;
        }

        // Actualizar posiciones de los waypoints existentes
        for (int i = 0; i < Waypoints.Length; i++)
        {
            if (Waypoints[i] != null)
            {
                PathPoints[i] = Waypoints[i].position;
            }
        }
    }

    private void InitializeWaypoints()
    {
        Waypoints = new Transform[transform.childCount];
        PathPoints = new Vector2[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            Waypoints[i] = transform.GetChild(i);
            PathPoints[i] = Waypoints[i].position;
        }
    }

    private void OnDrawGizmos()
    {
        if (!alwaysDrawGizmos) return;

        // Forzar actualización en el editor
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UpdatePathPoints();
        }
#endif

        if (Waypoints == null || Waypoints.Length == 0) return;

        // Dibujar waypoints
        Gizmos.color = waypointColor;
        for (int i = 0; i < Waypoints.Length; i++)
        {
            if (Waypoints[i] != null)
            {
                Gizmos.DrawWireSphere(Waypoints[i].position, waypointRadius);

                // Etiqueta del waypoint
#if UNITY_EDITOR
                UnityEditor.Handles.Label(Waypoints[i].position + Vector3.up * 0.2f, i.ToString());
#endif
            }
        }

        // Dibujar líneas del path
        Gizmos.color = pathColor;
        for (int i = 0; i < Waypoints.Length; i++)
        {
            if (Waypoints[i] == null) continue;

            int nextIndex = (i + 1) % Waypoints.Length;
            if (!loopPath && nextIndex <= i) break;

            if (nextIndex < Waypoints.Length && Waypoints[nextIndex] != null)
            {
                Gizmos.DrawLine(Waypoints[i].position, Waypoints[nextIndex].position);
            }
        }
    }

#if UNITY_EDITOR
    // Menú contextual para añadir waypoints fácilmente
    [UnityEditor.MenuItem("GameObject/Path System/Add Waypoint", false, 10)]
    static void AddWaypoint()
    {
        GameObject selected = UnityEditor.Selection.activeGameObject;
        if (selected != null && selected.GetComponent<PathSystem>())
        {
            GameObject waypoint = new GameObject("Waypoint_" + selected.transform.childCount);
            waypoint.transform.SetParent(selected.transform);
            waypoint.transform.position = selected.transform.position;
            UnityEditor.Selection.activeGameObject = waypoint;
        }
    }
#endif
}