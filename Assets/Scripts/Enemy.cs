using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage; // The damage against the player

    private Animator animator;
    private Transform target; // The player positon
    private bool skipMove;

    // Start is called before the first frame update
    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove){
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);
        skipMove = true;
    }

    public void MoveEnemy(){
        // Called by the Game Manager
        int xDir = 0;
        int yDir = 0;

        float yDistance = Mathf.Abs(target.position.y - transform.position.y),
                xDistance = Mathf.Abs(target.position.x - transform.position.x);
        if (yDistance > xDistance && yDistance > float.Epsilon){ // If we are closer in Y than in X, we move in Y direction
            yDir = target.position.y > transform.position.y ? 1 : -1; // Should the enemy move up (1)? or down (-1)?
        }
        else{
            xDir = target.position.x > transform.position.x ? 1 : -1; // Should right (1)? or left (-1)?
        }

        AttemptMove<Player> (xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        animator.SetTrigger("enemyAttack");
        hitPlayer.LoseFood(playerDamage);
    }
}
