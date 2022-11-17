using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct BoidCalculation : IJob{

#region Calculation Range
    public int startingIndex;
    public int endIndex;
#endregion

#region Boid Perameters
    public float aRange;
    public float cRange;
    public float sRange;
    public float aFactor;
    public float cFactor;
    public float sFactor;
#endregion
    
#region Pass Back
    public NativeArray<Vector3> positions;
    public NativeArray<Vector3> steers;
#endregion

// Boids Algorithm -> [https://www.youtube.com/watch?v=mhjuuHl6qHM&t=2211s]
    public void Execute(){
        for(int i = startingIndex; i < endIndex; i++){ // loop through all designated NPCs
            Vector3 desiredVelocity = steers[i];
            Vector3 desiredPosition = Vector3.zero;
            Vector3 desiredOffset = Vector3.zero;
            
            for(int j = 0; j < positions.Length; j++){ // Compare to all NPCs
                if(i == j) {continue;} // Don't compare to self

                if((positions[j] - positions[i]).magnitude <= aRange){
                    desiredVelocity += steers[j];
                }
                if((positions[j] - positions[i]).magnitude <= cRange){
                    desiredPosition += (positions[j] - positions[i]);
                }
                if((positions[j] - positions[i]).magnitude <= sRange){
                    Vector3 displacement = positions[j] - positions[i]; 
                    float distance = displacement.magnitude; 
                    // Seperation Vector is inversely proportional to distance from other: the closer the other is, the greater the seperation "force"
                    desiredOffset -= displacement * (sRange - distance); 
                }
            }

            desiredVelocity.Normalize();
            desiredPosition.Normalize();
            desiredOffset.Normalize();

            steers[i] = (steers[i] + (desiredVelocity * aFactor)).normalized;
            steers[i] = (steers[i] + (desiredPosition * cFactor)).normalized;
            steers[i] = (steers[i] + (desiredOffset * sFactor)).normalized;
        }
    }
}

public class NPCManager : MonoBehaviour {

#region Initialization
    [Header("NPC Initialization")]
    [SerializeField] Vector2 spawnBounds = new Vector2(10.0f,10.0f);
    [SerializeField] int totalNPCCoount = 100;
    [SerializeField] GameObject NPC;

    [Header("NPC Values")]
    [SerializeField] float NPCSpeed = 0.1f;
    [SerializeField] float obstacleCheckRange = 5.0f;
    [SerializeField] LayerMask obstacleMask;


    GameObject[] NPCs;
#endregion

#region Boid Perameters
    [Header("Boid Perameters")]
    [SerializeField] float alignmentCheckRange = 10.0f;
    [SerializeField] float cohesionCheckRange = 20.0f;
    [SerializeField] float seperationCheckRange = 5.0f;
    [SerializeField] float alignmentFactor = 1.0f;
    [SerializeField] float cohesionFactor = 1.0f;
    [SerializeField] float seperationFactor = 1.0f;
#endregion

#region Ghost
    [Header("Ghost Initialization")]
    [SerializeField] NavMesh navMesh;
    [SerializeField] int numGhosts;
    [SerializeField] bool assignRandomOnPathReached;
    [SerializeField] List<GameObject> totems;
    List<int> ghostIndecies;
    List<int> totemIndecies;
#endregion

    void Awake() {
        GenerateNPCInstancesInLevel();

        ghostIndecies = new List<int>();
        AssignGhosts(0,true); // Start by assigning all ghosts
    }

    void Update() {
        CreateBoidsJob();
    }


    void GenerateNPCInstancesInLevel(){
        NPCs = new GameObject[totalNPCCoount]; 
        for(int i = 0; i < totalNPCCoount; i++){
            NPCs[i] = (Instantiate( NPC, 
                        new Vector3(
                            Random.Range(-spawnBounds.x,spawnBounds.x),
                             0.0f,
                            Random.Range(-spawnBounds.y,spawnBounds.y)
                            ), 
                        Quaternion.identity, 
                        transform));
            NPCs[i].GetComponent<NPC>().Initialize(i, this, NPCSpeed, obstacleCheckRange, obstacleMask);
        }
    }
    void AssignGhosts(int ghostIndex, bool overrideAll){
        int index = -1;
        if(overrideAll){ // Generate All Ghosts Indecies
            for(int i = 0; i < numGhosts; i++){
                do{ // Select a ghost from within NPC list (which is not already contained in Ghost list)
                    index = Random.Range(0,totalNPCCoount);
                } while (ghostIndecies.Contains(index));
                ghostIndecies.Add(index);
            }
            foreach(int i in ghostIndecies){
                NPCs[i].GetComponent<NPC>().AddignIsGhost(true);
            }
        } else{
            do{ // Select a ghost from within NPC list (which is not already contained in Ghost list)
                    index = Random.Range(0,totalNPCCoount - 1);
            } while (ghostIndecies.Contains(index));
            for(int i = 0; i < numGhosts; i++){ // Iterate to ghostIndex
                if(ghostIndecies[i] == ghostIndex){
                    // Swap to new Ghost Index
                    NPCs[ghostIndecies[i]].GetComponent<NPC>().AddignIsGhost(false);
                    NPCs[index].GetComponent<NPC>().AddignIsGhost(true);
                    ghostIndecies.RemoveAt(i);
                    ghostIndecies.Add(index);
                }
            }
        }
    }

    public List<Vector3> GetPath(int ID_, bool reachedEnd){
        if(reachedEnd && assignRandomOnPathReached){ AssignGhosts(ID_, false); }
        int totemIndex = Random.Range(0,totems.Count - 1);
        return navMesh.FindPath(NPCs[ID_].transform.position, totems[totemIndex].transform.position);
    }
    
    void CreateBoidsJob(){
        NativeArray<Vector3> positions_ = new NativeArray<Vector3>(totalNPCCoount, Allocator.TempJob);
        NativeArray<Vector3> steers_ = new NativeArray<Vector3>(totalNPCCoount, Allocator.TempJob);
        for(int i = 0; i < totalNPCCoount; i++){ // Get current NPC positions and steer directions
            positions_[i] = NPCs[i].transform.position;
            steers_[i] = NPCs[i].GetComponent<NPC>().GetSteer();
        }
        
        /*
        int numJobs = totalNPCCoount / 50;
        BoidCalculation[] jobs = new BoidCalculation[numJobs];
        JobHandle[] jHandles = new JobHandle[numJobs];

        for(int i = 0; i < numJobs; i++){
            jobs[i] = new BoidCalculation{
                startingIndex = i * 50, // change for each job
                endIndex = (i * 50) + 50, // change for each job, but cannot exceed this

                aRange = alignmentCheckRange,
                cRange = cohesionCheckRange,
                sRange = seperationCheckRange,
                aFactor = alignmentFactor,
                cFactor = cohesionFactor,
                sFactor = seperationFactor,

                positions = positions_,
                steers = steers_
            };

            jHandles[i] = jobs[i].Schedule();
            jHandles[i].Complete();

            for(int j = jobs[i].startingIndex; j < jobs[i].endIndex; j++){
                NPCs[j].GetComponent<NPC>().SetSteer(jobs[i].steers[j]);
            }
        }
        */

        BoidCalculation job = new BoidCalculation{
            startingIndex = 0, // change for each job
            endIndex = totalNPCCoount, // change for each job, but cannot exceed this

            aRange = alignmentCheckRange,
            cRange = cohesionCheckRange,
            sRange = seperationCheckRange,
            aFactor = alignmentFactor,
            cFactor = cohesionFactor,
            sFactor = seperationFactor,

            positions = positions_,
            steers = steers_
        };


        JobHandle jHandle = job.Schedule();
        jHandle.Complete(); // may have to wait for complete

        for(int i = job.startingIndex; i < job.endIndex; i++){
            if(!ghostIndecies.Contains(i)){
                //NPCs[i].GetComponent<NPC>().SetSteer(job.steers[i]);
            } 
            NPCs[i].GetComponent<NPC>().SetSteer(job.steers[i]);
        }

        positions_.Dispose();
        steers_.Dispose();
    }

}
