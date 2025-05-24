using UnityEngine;

public class Movement : MonoBehaviour
{
    ////////////////////////////// [Player] //////////////////////////////
    [SerializeField] private float move_speed = 6.0f;
    [SerializeField] private float move_acceleration = 15.0f;
    [SerializeField] private float sprint_speed = 10.0f;
    [SerializeField] private float sprint_acceleration = 20.0f;
    [SerializeField] private float dash_power = 20.0f;
    [SerializeField] private float dash_cooldown = 5;
    [SerializeField] private float camera_offset_cof = 5;

    // [References]
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private Camera camera;

    //  [Local]
    private float dash_cooldown_time = 0;
    private Vector3 camera_offset = new Vector3(0, 0, 0);

    ////////////////////////////// [Main] //////////////////////////////
    void Start()
    {
        camera_offset = camera.transform.position - this.transform.position;
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            applyForce(sprint_acceleration, sprint_speed);
            applyResistance(sprint_speed);
        }
        else
        {
            applyForce(move_acceleration, move_speed);
            applyResistance(move_speed);
        }

        dash();
        cameraOffset();
    }

    ////////////////////////////// [Base Functions] //////////////////////////////
    Vector3 getDirectionMove()
    {
        Vector3 direction_move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (direction_move.magnitude != 0) direction_move /= direction_move.magnitude;
        return direction_move;
    }

    Vector3 getVelocity() {
        return new Vector3(rigidbody.linearVelocity.x, 0, rigidbody.linearVelocity.z);   
    }

    void cameraOffset()
    {
        camera.transform.position = this.transform.position + camera_offset - getVelocity()/camera_offset_cof;
    }

    ////////////////////////////// [Movement] //////////////////////////////
    void applyForce(float acceleration, float max_speed)
    {
        if (getVelocity().magnitude < Mathf.Sqrt(max_speed * max_speed))
            rigidbody.AddForce(getDirectionMove() * acceleration, ForceMode.Force);
    }

    void applyResistance(float max_speed)
    {
        float resistance_cof = Mathf.Clamp01(getVelocity().magnitude / max_speed);
        if (max_speed != 0) rigidbody.AddForce(-getVelocity() * resistance_cof, ForceMode.Force);
    }

    void dash()
    {
       if(dash_cooldown_time > 0) dash_cooldown_time -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dash_cooldown_time <= 0)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            Plane ground = new Plane(Vector3.up, transform.position);
            if (ground.Raycast(ray, out float distance))
            {
                Vector3 dash_direction = ray.GetPoint(distance) - transform.position;
                dash_direction /= dash_direction.magnitude;
                rigidbody.AddForce(dash_direction * dash_power, ForceMode.Impulse);
                dash_cooldown_time = dash_cooldown;
            }
        }
    }
}
