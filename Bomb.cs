// The script that deals with bomb interactions and raycasts, 
// it affects some UI stuff and deals with player respawns

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStar;
using UnityEngine.SceneManagement;

public class Bomb : MonoBehaviour
{
    private float timer;    
    public float bombWait;
    public Expl explPrefab;
    public LayerMask levelLayer;
    private MeshRenderer mr;
    

    public GameObject explosionNoise;

    public PlayerController player1;
    public PlayerController player2;
    public ResourceManager player;
    private bool respawn1;
    private bool respawn2;
    private int spawn1;
    private int spawn2;

    public float[] respawns;
    public Vector3[] spawns;

    private bool expl;
    private bool once;
    public bool debug1;
    public bool rocket;
    private PlayerController[] players1;
    private PlayerController[] players2;
    private int count1= 0;
    private int count2 = 0;


    // Start is called before the first frame update
    void Start()
    {
        respawn1 = true;
        respawn2 = true;
        expl = true;
        mr = GetComponent<MeshRenderer>();
        once = true;

        // rockets explode on impact so need a slightly different implementation
        if (rocket)
        {
            print("bomb");
            Explode(player);
        }

        else
        {
            rocket = false;

        }
   

        
    }

    // Update is called once per frame
    void Update()
    {
        if (!rocket)
        {

            // bomb count down
            timer += Time.deltaTime;
            bombWait -= Time.deltaTime;
            if (expl = true && bombWait <= 0)
            {
                Explode(player);

            }
        }
        

    }

    private void Explode(ResourceManager player)
    {
        
        expl = false;

        Vector3 bomb = transform.position;
        bomb.x -= .5f;
        bomb.z += .5f;

        // makes the bomb explode
        // plays a noise and instaniates the explosion prefab
        if (once)
        {
            Instantiate(explosionNoise);
            Instantiate(explPrefab, bomb, Quaternion.identity);
            once = false;
        }

        // Raycast for player and box detection
        RaycastHit hitFor, hitFor1, hitFor2, hitBack, hitBack1, hitBack2, hitLeft,
            hitLeft1, hitLeft2, hitRight, hitRight1, hitRight2, hitDown;

        float x = transform.position.x;
        float y = transform.position.y+.5f;
        float z = transform.position.z;

     
        Physics.Raycast(new Vector3(x,y,z), Vector3.forward, out hitFor, 2*player.explosionSize, levelLayer);
        Physics.Raycast(new Vector3(x - .25f, y, z), Vector3.forward, out hitFor1, 2 * player.explosionSize, levelLayer);
        Physics.Raycast(new Vector3(x + .25f, y, z), Vector3.forward, out hitFor2, 2 * player.explosionSize, levelLayer);
        Physics.Raycast(new Vector3(x, y, z), Vector3.back, out hitBack, 2 * player.explosionSize, levelLayer);
        Physics.Raycast(new Vector3(x-.25f,y,z), Vector3.back, out hitBack1, 2 * player.explosionSize, levelLayer);
        Physics.Raycast(new Vector3(x + .25f, y, z), Vector3.back, out hitBack2, 2 * player.explosionSize, levelLayer);
        Physics.Raycast(new Vector3(x, y, z), Vector3.left, out hitLeft, 2 * player.explosionSize, levelLayer);
        Physics.Raycast(new Vector3(x, y, z+.25f), Vector3.left, out hitLeft1, 2 * player.explosionSize, levelLayer);
        Physics.Raycast(new Vector3(x, y, z - .25f), Vector3.left, out hitLeft2, 2 * player.explosionSize, levelLayer);
        Physics.Raycast(new Vector3(x, y, z), Vector3.right, out hitRight, 2 * player.explosionSize, levelLayer);
        Physics.Raycast(new Vector3(x, y, z + .25f), Vector3.right, out hitRight1, 2 * player.explosionSize, levelLayer);
        Physics.Raycast(new Vector3(x, y, z - .25f), Vector3.right, out hitRight2, 2 * player.explosionSize, levelLayer);
        Physics.Raycast(new Vector3(x, y, z), Vector3.down, out hitDown, 2 * player.explosionSize, levelLayer);


        RaycastHit[] rays = {hitFor, hitFor1, hitFor2, hitBack, hitBack1, hitBack2, hitLeft,
            hitLeft1, hitLeft2, hitRight, hitRight1, hitRight2, hitDown};

        foreach (RaycastHit ray in rays)
        {
            RayChecks(ray);
        }

        respawn1 = true;
        respawn2 = true;
        count1 = 0;
        count2 = 0;

        // this makes the bomb invisable
        mr.enabled = false;

        // starts a corutine to destroy the game object after a little time in case the player enters the explosion
        // area
        if (!rocket)
        {
            // destroy after a couple seconds
            StartCoroutine(destroyBomb(bombWait));
        }
        else
        {
            StartCoroutine(destroyBomb(.1f));
            player.bombsPlaced += 1;
        }
    
      
    }

   
    private IEnumerator destroyBomb(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        player.bombsPlaced -= 1;
        Destroy(this.gameObject);
    }

    // Raychecks takes each ray and checks for if it hit a player or a box
    private void RayChecks(RaycastHit hit)
    {
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player1"))
            {
                if (count1 == 0)
                {
                    respawn1 = false;

                    spawn1 = Random.Range(0, 4);

                    // increases the score
                    GameManager.instance.p2Score +=1;
                    GameManager.instance.p2ScorePanel.SetScore(GameManager.instance.p2Score);

                    // respawns the player 
                    hit.collider.transform.position = spawns[spawn1];
                    hit.collider.gameObject.GetComponent<ResourceManager>().Reset();
                    count1++;
                }
            }
            // this is identical to player1 just different variables for player2
            else if (hit.collider.CompareTag("Player2"))
            {
                
                if (count2== 0)
                {
                    respawn2 = false;

                    print(hit.collider.tag);
               
                    spawn2 = Random.Range(0, 4);

                    GameManager.instance.p1Score += 1;
                    GameManager.instance.p1ScorePanel.SetScore(GameManager.instance.p1Score);


                    hit.collider.transform.position = spawns[spawn2];
                    hit.collider.gameObject.GetComponent<ResourceManager>().Reset();
                    count2++;

                }

            }
            // this is for the practice scene simply reloads the scene
            else if (hit.collider.CompareTag("AIPlayer"))
            {
                Destroy(hit.collider.gameObject);

                SceneManager.LoadScene("Practice");
            }
            // if it hits a box call BlownUp() which destroys the object and spawns a powerup
            else if (hit.collider.CompareTag("Box"))
            {
                hit.collider.GetComponent<Box>().BlownUp();

                if (SceneManager.GetActiveScene().Equals("Practice"))
                {
                    // to update the AI grid
                    Vector3 boxPos = hit.collider.GetComponent<Box>().transform.position;
                    Location boxLoc = Program.ConvertFromWorld(boxPos);

                    ConvertToWorld.grid[boxLoc.X + 1, boxLoc.Z + 1] = 0;
                }
                Destroy(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("Indestructable"))
            {
                // do nothing
            }
        }
    }
}
