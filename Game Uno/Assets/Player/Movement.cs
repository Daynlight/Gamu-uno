using UnityEngine;

public class Movement : MonoBehaviour
{
    ////////////////////////////// [Movement Attributes] //////////////////////////////
    [SerializeField] private float move_speed = 6.0f;
    [SerializeField] private float move_acceleration = 50.0f;
    [SerializeField] private float movement_resistance = 5;
    [SerializeField] private float sprint_speed = 15.0f;
    [SerializeField] private float sprint_acceleration = 60.0f;
    [SerializeField] private float sprint_stamina_consumption = 4;
    [SerializeField] private float stamina_penalty = 3;
    [SerializeField] private float dash_power = 25.0f;
    [SerializeField] private float dash_cooldown = 5;
    [SerializeField] private float dash_stamina_consumption = 6;
    [SerializeField] private float camera_offset_speed = 5;
    [SerializeField] private float camera_max_distance = 10;


    // [References]
    private Player pl;
    private Rigidbody rb;
    private Camera cam;

    //  [Local]
    private float dash_cooldown_time = 0;
    private Vector3 camera_offset = new Vector3(0, 0, 0);
    private Vector3 last_position = new Vector3(0, 0, 0);

    ////////////////////////////// [Main] //////////////////////////////
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        pl = GetComponent<Player>();

        camera_offset = cam.transform.position - this.transform.position;
        last_position = this.transform.position;
    }

    void FixedUpdate()
    {
        staminaControl();
        movement();
    }

    ////////////////////////////// [Base Functions] //////////////////////////////
    Vector3 getDirectionMove()
    {
        Vector3 direction_move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        return direction_move.normalized;
    }

    Vector3 getVelocity()
    {
        return new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
    }

    ////////////////////////////// [Movement] //////////////////////////////
    void movement()
    {
        if (Input.GetKey(KeyCode.LeftShift) && pl.stamina >= sprint_stamina_consumption)
        {
            moveForce(sprint_acceleration, sprint_speed);

            pl.stamina -= sprint_stamina_consumption * Time.deltaTime;
            if (pl.stamina < sprint_stamina_consumption) pl.stamina -= stamina_penalty;
        }
        else
            moveForce(move_acceleration, move_speed);

        dash();
        cameraOffset();
    }

    void moveForce(float acceleration, float max_speed)
    {
        if (getVelocity().magnitude < Mathf.Sqrt(max_speed * max_speed))
            rb.AddForce(getDirectionMove() * (acceleration - movement_resistance), ForceMode.Acceleration);
        rb.AddForce(-getVelocity() * movement_resistance, ForceMode.Acceleration);
    }

    void cameraOffset()
    {
        Vector3 camera_direction = this.transform.position - last_position;
        Vector3 camera_velocity = camera_direction * camera_offset_speed * Time.deltaTime;
        cam.transform.position = camera_offset + last_position;

        if (camera_max_distance < camera_direction.magnitude)
            last_position = this.transform.position - camera_direction.normalized * camera_max_distance;
        else
            last_position = last_position + camera_velocity;
    }

    public void staminaControl()
    {
        if (pl.stamina < pl.max_stamina) pl.stamina += pl.stamina_recovery * Time.deltaTime;
        if (pl.stamina > pl.max_stamina) pl.stamina = pl.max_stamina;
        if (pl.stamina < 0) pl.stamina = 0;
    }

    void dash()
    {
        if (dash_cooldown_time > 0) dash_cooldown_time -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && pl.stamina >= dash_stamina_consumption + stamina_penalty && dash_cooldown_time <= 0 && getDirectionMove().magnitude != 0)
        {
            pl.stamina -= dash_stamina_consumption;
            if (pl.stamina < sprint_stamina_consumption) pl.stamina -= stamina_penalty;
            rb.AddForce(getDirectionMove() * dash_power, ForceMode.VelocityChange);
            dash_cooldown_time = dash_cooldown;
        }
    }
}
