using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TargetPoint : MonoBehaviour
{
    public Enemy Enemy { get; private set; }
    public Vector3 Position => transform.position;
    public float ColliderSize { get; private set; }

    private void Awake()
    {
        Enemy = transform.root.GetComponent<Enemy>();
        ColliderSize = GetComponent<SphereCollider>().radius * transform.localPosition.x;
    }
        
}