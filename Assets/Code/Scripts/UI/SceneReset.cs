using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class SceneReset : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(ResetScene);
        }

        public static void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
