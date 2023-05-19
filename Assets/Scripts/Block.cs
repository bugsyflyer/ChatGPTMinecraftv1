using UnityEngine;

public class Block : MonoBehaviour
{
    private bool isVisible = false;

    public void SetVisible(bool visible)
    {
        isVisible = visible;
        if (isVisible)
        {
            GetComponent<MeshRenderer>().enabled = true;
            GetComponent<Collider>().enabled = true;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
        }
    }

    public void Break()
    {
        // Play particle effects, remove the block, etc.
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Handle collision with the player, such as taking fall damage
            // and breaking blocks with a pickaxe or sword
        }
    }
}