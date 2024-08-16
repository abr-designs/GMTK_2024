using UnityEngine;

public class MoveController : MonoBehaviour
{
    [SerializeField, Min(0f)]
    private float thrustSpeed;
    [SerializeField, Min(0f)]
    private float rotationSpeed;

    [SerializeField]
    private Transform[] thrusters;

    private Vector3 _velocity;
    private float _rotVelocity;
    
    //============================================================================================================//
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            ApplyForce(thrustSpeed, 1f);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            ApplyForce(thrustSpeed, -1f);
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            ApplyRotation(rotationSpeed, 1);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            ApplyRotation(rotationSpeed, -1);
        }


        transform.position += _velocity * (Time.deltaTime * Time.deltaTime);
        transform.eulerAngles += new Vector3(0f, 0f, _rotVelocity * Time.deltaTime* Time.deltaTime);
    }

    private void ApplyForce(float force, float mult)
    {
        for (int i = 0; i < thrusters.Length; i++)
        {
            _velocity -= thrusters[i].up * (mult * force);
        }
    }

    private void ApplyRotation(float force, float mult)
    {
        _rotVelocity += force * mult;
    }
    
    //============================================================================================================//
}
