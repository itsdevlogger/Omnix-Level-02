using UnityEngine;
using UnityEngine.UI;



public class ObjectEvents : MonoBehaviour
{
    [SerializeField]  Button.ButtonClickedEvent onAwake;
    [SerializeField]  Button.ButtonClickedEvent onEnabled;
    [SerializeField]  Button.ButtonClickedEvent onStart;
    [SerializeField] Button.ButtonClickedEvent collisionEnter;
    [SerializeField] Button.ButtonClickedEvent collisionExit;
    [SerializeField] Button.ButtonClickedEvent triggerEnter;
    [SerializeField] Button.ButtonClickedEvent triggerExit;
    [SerializeField]  Button.ButtonClickedEvent onDestroy;

    private void Awake()
    {
        onAwake.Invoke();
    }

    private void OnEnable()
    {
        onEnabled.Invoke();
    }

    private void Start()
    {
        onStart.Invoke();
    }

    private void OnDestroy()
    {
        onDestroy.Invoke();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        collisionEnter.Invoke();
    }

    private void OnCollisionExit(Collision collision)
    {
        collisionExit.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerEnter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        triggerExit.Invoke();
    }

}