using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScript : MonoBehaviour
{
    
    public void NextLevel()
    {
        SceneManager.LoadScene("Level1");
    }
}
