// A script that converts the path found in AStar to the worldview so that 
// the AI can move and interact and repeatedly find the player


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStar;
using System.Linq;
using UnityEngine.SceneManagement;

public class ConvertToWorld : MonoBehaviour
{

    private Location PlayerLocation;
    private Location runningLocation;


    private float updateWait = .75f;
    public static Vector3 AIposition;
    private Rigidbody rgb;
    public static int speed = 5;
    public static int[,] grid = new int[19, 19];
    // 0 = empty space
    // 1 = indestructable box
    // 2 = destructable box
    // 3 = AI
    // 4 = actual player



    // Start is called before the first frame update
    void Start()
    {

        rgb = GetComponent<Rigidbody>();


    }

    public static int[,] makeMap()
    {
        int counterI = 0;
        int counterJ = 0;

        for (float i = 0; i <= 36; i += 2)
        {
            for (float j = 0; j <= 36; j += 2)
            {
                // the plus one is to get to the center of the boxes since their
                // size is two, and the 3 is to get the top to avoid other collisions
                Vector3 position = new Vector3(i + 1 , 3, -(j +1));
                Collider[] objects = Physics.OverlapSphere(position, .5f);


                if (objects.FirstOrDefault(obj => obj.CompareTag("Indestructable")))
                {
                    grid[counterI, counterJ] = 1;

                }
                else if (objects.FirstOrDefault(obj => obj.CompareTag("Box"))) 
                {
                    grid[counterI, counterJ] = 2;

                }
                else
                {
                    grid[counterI, counterJ] = 0;

                }
                counterJ++;

            }
            counterJ = 0;
            counterI++;
        }

        return grid;
    }
    // reload the scene if touched
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("singlePlayer1"))
        {

            SceneManager.LoadScene("Practice");
        }
    }


    // Update is called once per frame
    void Update()
    {
        

        Vector3 playerLoc = GameObject.FindGameObjectWithTag("singlePlayer1").transform.position;
        float playerLocX = Mathf.Round(playerLoc.x);
        float playerLocY = Mathf.Round(playerLoc.y);
        float playerLocZ = Mathf.Round(playerLoc.z);

        Vector3 newLoc = new Vector3(playerLocX, playerLocY, playerLocZ);

        //RunAway(newLoc);

        PlayerLocation = Program.ConvertFromWorld(newLoc);

        grid[PlayerLocation.X, PlayerLocation.Z] = 4;


        updateWait -= Time.deltaTime;

        AIposition = transform.position;
        


        // to have more realistic updating
        if (updateWait <= 0)
        {
            Vector3 vel = WhereToGo();

            // This is where we would have called the MoveAI method in order to 
            // move him more fluidly
            //MoveAI(vel);

            updateWait = .5f;
        }

        
    }
    /*
    // method that updates a location far away from the player to simulate 
    // a "Tag" like game for practice
    //  didn't end up getting to this
    public void RunAway(Vector3 playerLoc)
    {
        Location oppositeLoc = Program.ConvertFromWorld(playerLoc);

        runningLocation = oppositeLoc; 
    }
    */
    public Vector3 WhereToGo()
    {
        // if the AI should run from the player
        //List<Location> path = Program.Search(runningLocation);

        // if the AI should chase towards the player
        List<Location> path = Program.Search(PlayerLocation);


        Program.grid[path.First().X, path.First().Z] = 1; // resetting the old position to open

        int oldPosX = path.First().X;
        int oldPosZ = path.First().Z;

        path.Remove(path.First());

        int newPosX = path.First().X;
        int newPosZ = path.First().Z;
        
               
        Vector3 nextCoords = new Vector3(path.First().X*2 + 1, 3f, - (path.First().Z*2 + 1));


        Program.grid[path.First().X, path.First().Z] = 3; // setting the new position of the AI

        transform.position = nextCoords; // looks worse but couldn't get movement to work fluidly so using this

        // this is to have the AI face towards where it is moving
        Quaternion currentRotation = transform.rotation;

        if (newPosZ > oldPosZ)
        {
            // 100 is just a constant to have immedient rotation
            Quaternion target = Quaternion.Euler(0, 180, 0);
            transform.rotation = Quaternion.Slerp(currentRotation, target, 100);

        }
        if (newPosX > oldPosX)
        {
            Quaternion target = Quaternion.Euler(0, 90, 0);
            transform.rotation = Quaternion.Slerp(currentRotation, target, 100);
        }
        if (oldPosZ > newPosZ)
        {
            Quaternion target = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Slerp(currentRotation, target, 100);
        }
        if (oldPosX > newPosX)
        {
            Quaternion target = Quaternion.Euler(0, 270, 0);
            transform.rotation = Quaternion.Slerp(currentRotation, target, 100);
        }



        return nextCoords;

    }

    // Can't get movement to work fluidly so am just using transform.position
    // this contains aspects of various things I tried
    public void MoveAI(Vector3 vec)
    {
        //transform.Rotate(vec);
        //Vector3 targetDir = vec - transform.position;

        //float step = speed * Time.deltaTime;
        //Vector3 dir = rgb.velocity; // maybe vec?
        //dir.y = 0;

        //if(dir != Vector3.zero)
        //{
        //    dir.Normalize();
        //    Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step);
        //}
        //print(Program.grid[(int)vec.x, (int)vec.z]);
       

        //transform.position += (vec - transform.position).normalized * 20 * Time.deltaTime;

        Vector3 pos = transform.position;
        //Quaternion direction = transform.rotation;
        //print("direction is " + direction);

        float deltaX = vec.x - pos.x;
        float deltaZ = vec.z - pos.z;

        float tempX = transform.position.x;
        float tempZ = transform.position.z;
        Vector3 veloc;
        // something with first rotation here



        if (deltaX >= 0)// if moving in x direction
        {
            tempX += deltaX;

        }
        else
        {
            tempZ += deltaZ;
           //tempZ -= deltaZ;

        }



        veloc = new Vector3(Mathf.Floor(tempX), Mathf.Floor(3), Mathf.Ceil(tempZ));

        veloc = veloc.normalized;
        //rgb.velocity = veloc * speed;

        rgb.AddForce(veloc * 1000);
        //transform.position.Set(tempX, 3, tempZ); 

    
        // another thing that I should have tried and wish I did is actually
        // using the AI grid and creating vectors based on it and convertToWorld
        // instead of doing what it currently does
    }
}
