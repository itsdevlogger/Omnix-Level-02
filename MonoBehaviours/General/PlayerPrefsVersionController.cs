using UnityEngine;

namespace Omnix.Monos
{
    public class PlayerPrefsVersionController : MonoBehaviour
    {
        private static readonly string VERSION = "2.0";

        // Start is called before the first frame update
        void Awake()
        {
            if (PlayerPrefs.GetString("version", "") != VERSION)
            {
                PlayerPrefs.DeleteAll();
                Caching.ClearCache();
            }

            PlayerPrefs.SetString("version", VERSION);
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}