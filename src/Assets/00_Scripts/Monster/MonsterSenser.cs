using UnityEngine;

public class MonsterSenser : MonoBehaviour
{
    GameObject target;
    public GameObject Target { get { return target; } }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.gameObject;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = null;
        }
    }

    private void OnEnable()
    {
        target = null;
    }

    public void ResetSenser()
    {
        target = null;
    }
}
