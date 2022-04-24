using AI.Agents;
using UnityEngine;

public class ExclamationMark : MonoBehaviour
{
    private Teacher _teacher;

    private void Update()
    {
        if (!_teacher)
        {
            _teacher = FindObjectOfType<Teacher>();

            if (!_teacher) return;
        }

        transform.position = _teacher.ExclamationMarkPosition.position;
    }
}
