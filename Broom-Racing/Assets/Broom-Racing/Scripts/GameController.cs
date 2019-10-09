using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BroomRacing
{
    public class GameControl : MonoBehaviour
    {
        public static GameControl instance;            //A reference to our game control script so we can access it statically.
        public Text scoreText;                        //A reference to the UI text component that displays the player's score.
        public GameObject gameOvertext;                //A reference to the object that displays the text which appears when the player dies.

        private float time = 0.0f;                        //The player's time in race
        public bool gameOver = false;                //Is the game over?
        public float scrollSpeed = -1.5f;

        private int playerPlace = 2;


        void Awake()
        {
            //If we don't currently have a game control...
            if (instance == null)
                //...set this one to be it...
                instance = this;
            //...otherwise...
            else if (instance != this)
                //...destroy this one because it is a duplicate.
                Destroy(gameObject);
        }

        void Update()
        {
            //If the game is over and the player has pressed some input...
            if (gameOver && Input.GetMouseButtonDown(0))
            {
                //...reload the current scene.
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        public void UpdateTime()
        {
            //The bird can't score if the game is over.
            if (gameOver)
                return;
            //If the game is not over, increase the score...
            time++;
            //...and adjust the score text.
            scoreText.text = "Score: " + time.ToString();
        }

        public void RaceEnded()
        {
            //Activate the game over text.
            gameOvertext.SetActive(true);
            //Set the game to be over.
            gameOver = true;
        }
    }
}