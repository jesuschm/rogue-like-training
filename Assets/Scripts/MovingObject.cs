using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit){
        bool res = false;

        Vector2 start = transform.position; // Automatically discard Z coord
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false; // To be sure the following linecast doesn't hit against ourself
        hit = Physics2D.Linecast(start, end, blockingLayer);
        if (hit.transform == null){
            // We can move to
            StartCoroutine(SmoothMovement(end));
            res = true;
        }
        boxCollider.enabled = true;

        return res;
    }
    protected IEnumerator SmoothMovement(Vector3 end){
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon){
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);

            // Reevaluate while condition
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null; // Wait until the next frame to run the next loop
        }
    }
    
    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T: Component {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null){
            return;
        }

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
    }
    protected  abstract void OnCantMove <T> (T component) 
        where T : Component;
}
