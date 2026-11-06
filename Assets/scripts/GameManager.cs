using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    public void startGame()
    {
        
        SceneManager.LoadScene("GameScene");

    }
}
