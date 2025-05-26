using UnityEngine;

public class Player : MonoBehaviour
{
    ////////////////////////////// [Player Stats] //////////////////////////////
    [SerializeField] public float max_stamina = 30;
    [SerializeField] public float stamina_recovery = 1;

    // Current
    public float stamina = 0;

    ////////////////////////////// [Main] //////////////////////////////
    void Start()
    {
        stamina = max_stamina;
    }
}
