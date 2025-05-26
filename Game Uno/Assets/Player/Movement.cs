using UnityEngine;

public class Movement : MonoBehaviour
{
    ////////////////////////////// [Movement Attributes] //////////////////////////////
    [SerializeField] private float move_speed = 6.0f;
    [SerializeField] private float move_acceleration = 15.0f;
    [SerializeField] private float movement_resistance = 0.1f;
    [SerializeField] private float sprint_speed = 10.0f;
    [SerializeField] private float sprint_acceleration = 20.0f;
    [SerializeField] private float sprint_stamina_consumption = 5;
    [SerializeField] private float stamina_penalty = 2;
    [SerializeField] private float dash_power = 20.0f;
    [SerializeField] private float dash_cooldown = 5;
    [SerializeField] private float dash_stamina_consumption = 10;
    [SerializeField] private float camera_offset_speed = 5;

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

    Vector3 getCursorDirectionFromObject()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, this.transform.position);

        if (ground.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        Debug.LogError("Can't hit ground with ray");
        return this.transform.position;
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
        if (getVelocity().magnitude < Mathf.Sqrt(max_speed * max_speed)) rb.AddForce(getDirectionMove() * acceleration, ForceMode.Force);
        
        float resistance_cof = Mathf.Clamp01(getVelocity().magnitude / max_speed);
        if (max_speed != 0) rb.AddForce(-getVelocity() * resistance_cof, ForceMode.Force);
        if (getDirectionMove().magnitude == 0) rb.linearVelocity += -getVelocity() * movement_resistance;
    }

    // [NOTE] ADD MAX CAMERA DISTANCE
    void cameraOffset()
    {
        Vector3 camera_direction = this.transform.position - last_position;
        Vector3 camera_velocity = camera_direction * camera_offset_speed * Time.deltaTime;
        cam.transform.position = camera_offset + last_position;
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && pl.stamina >= dash_stamina_consumption + stamina_penalty && dash_cooldown_time <= 0)
        {
            pl.stamina -= dash_stamina_consumption;
            if (pl.stamina < sprint_stamina_consumption) pl.stamina -= stamina_penalty;
            Vector3 dash_direction = (getCursorDirectionFromObject() - this.transform.position).normalized;
            rb.AddForce(dash_direction * dash_power, ForceMode.Impulse);
            dash_cooldown_time = dash_cooldown;
        }
    }
}
