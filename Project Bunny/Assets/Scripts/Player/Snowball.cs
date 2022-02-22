using Player;
using UnityEngine;

public class Snowball : MonoBehaviour
{
    [SerializeField] private Rigidbody _snowballRigidbody;
    [SerializeField] private Transform _snowballTransform;

    // ReSharper disable once NotAccessedField.Local
    private StudentController _studentThrower;

    private void Awake()
    {
        if (_snowballRigidbody == null)
        {
            _snowballRigidbody = gameObject.GetComponent<Rigidbody>();
        }
    }

    /// <summary>
    /// Throws Snowball by the Student
    /// </summary>
    /// <param name="force"></param>
    public void ThrowSnowball(float force)
    {
        transform.parent = null;
        _snowballRigidbody.isKinematic = false;
        // TODO: Direction will be handled via hand release on Animation
        var direction = new Vector3(0f, 0.2f, 0.0f);
        direction += _snowballTransform.forward;
        _snowballRigidbody.AddForce(direction.normalized * force * 1000f);
    }

    /// <summary>
    /// Assigns student who threw the snowball
    /// Will be handy later on when calculating scores, etc
    /// </summary>
    /// <param name="student"></param>
    public void SetSnowballThrower(StudentController student)
    {
        _studentThrower = student;
    }
}
