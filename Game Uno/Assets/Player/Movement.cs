using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody player;
    [SerializeField] private float move_speed = 6.0f;
    [SerializeField] private float move_acceleration = 15.0f;
    [SerializeField] private float sprint_speed = 10.0f;
    [SerializeField] private float sprint_acceleration = 20.0f;
    [SerializeField] private float dash = 20.0f;
    [SerializeField] private float dash_cooldown = 5;
    private Vector3 direction_move = new Vector3(0, 0, 0);
    private float dash_cooldown_time = 0;
    private Vector3 current_velocity = new Vector3(0, 0, 0);
    private float current_speed = 0.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        direction_move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (direction_move.magnitude != 0) direction_move /= direction_move.magnitude;
        current_velocity = new Vector3(player.linearVelocity.x, 0, player.linearVelocity.z);
        current_speed = current_velocity.magnitude;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            ApplyForce(sprint_acceleration, sprint_speed);
            ApplyResistance(sprint_speed);
        }
        else
        {
            ApplyForce(move_acceleration, move_speed);
            ApplyResistance(move_speed);
        }

        MakeDash();
    }

    void ApplyForce(float acceleration, float max_speed)
    {
        if (current_speed < Mathf.Sqrt(max_speed * max_speed))
            player.AddForce(direction_move * acceleration, ForceMode.Force);
    }

    void ApplyResistance(float max_speed)
    {
        float resistance_cof = Mathf.Clamp01(current_speed / max_speed);
        if (max_speed != 0) player.AddForce(-current_velocity * resistance_cof, ForceMode.Acceleration);
    }

    void MakeDash()
    {
       if(dash_cooldown_time > 0) dash_cooldown_time -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dash_cooldown_time <= 0)
        {
            player.AddForce(direction_move * dash, ForceMode.Impulse);
            dash_cooldown_time = dash_cooldown;
        }
    }
}
