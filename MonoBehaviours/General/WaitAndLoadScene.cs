using System.Collections;
using UnityEngine;

namespace Omnix.Monos
{
    public class WaitAndLoadScene : MonoBehaviour
    {
        [SerializeField] private float waitForSec;
        [SerializeField] private SceneProperties sceneProperties;

        private void OnEnable()
        {
            StartCoroutine(WaitAndLoad());
        }

        private IEnumerator WaitAndLoad()
        {
            yield return new WaitForSeconds(waitForSec);
            SceneController.LoadScene(sceneProperties);
        }
    }
}