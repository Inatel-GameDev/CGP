using UnityEngine;

public class Movable : MonoBehaviour
{
    private Rigidbody2D rb2d;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();   
        rb2d.constraints = RigidbodyConstraints2D.FreezePositionX;  
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation; 
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player")){
            var player = collision.gameObject.GetComponent<Movement>();
            if(player.wallGrab){
                rb2d.constraints = RigidbodyConstraints2D.FreezeRotation; 
                rb2d.constraints = RigidbodyConstraints2D.None;
            }else{
                rb2d.constraints = RigidbodyConstraints2D.FreezePositionX;   
            }   
        }
    }
}
