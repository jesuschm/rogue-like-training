using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public float restartLevelDelay = 1f;
    public int wallDamage = 2; // Damage to the walls
    public int pointsPerFood = 10; // Get 10 points when the player get food
    public int pointsPerSoda = 20; // Same with soda items

    private Animator animator;
    private int food; // The food the player has in the current level

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.instance.playersTurn) return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int) Input.GetAxisRaw("Horizontal");
        vertical = (int) Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if (horizontal != 0 || vertical != 0){
            AttemptMove<Wall>(horizontal, vertical); // The player MAY hit a wall
        }
    }

    protected override void OnCantMove <T> (T component){
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);

        animator.SetTrigger("playerChop");
    }

    private void Restart(){
        // Function runs when the player reachs Exit and the game is reloaded
        Application.LoadLevel(Application.loadedLevel); // We are not loading a new scene, but if we want to, this is the place
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void OnDisable(){
        // When the player reachs exit, set the current food points to the Game manager instance food points, in order to make it available when the new level loads.
        GameManager.instance.playerFoodPoints = food; 
    }

    public void LoseFood(int loss){
        animator.SetTrigger("playerHit");
        food -= loss;

        CheckIfGameOver();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        switch (other.tag){
            case "Exit":
                Invoke("Restart", restartLevelDelay);
                enabled = false;
                break;
            case "Food":
                food += pointsPerFood;
                other.gameObject.SetActive(false);
                break;
            case "Soda":
                food += pointsPerSoda;
                other.gameObject.SetActive(false);
                break;
        }
    }
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--; // Every player move, one food lost

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        CheckIfGameOver();

        GameManager.instance.playersTurn = false; 
    }
    private void CheckIfGameOver(){
        if (food <= 0){
            GameManager.instance.GameOver();
        }
    }
}
