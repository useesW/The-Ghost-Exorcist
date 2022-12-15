
using UnityEngine;

public class gun : MonoBehaviour
{
    public GameObject photonBeam;
    public LayerMask npcMask;
    public float damage = 10f;
    public float range = 100f;

    public Transform FpsCam;

    private void Start() {
        photonBeam.transform.position = new Vector3(photonBeam.transform.position.x, photonBeam.transform.position.y, range);
        photonBeam.transform.localScale = new Vector3(photonBeam.transform.localScale.x, range, photonBeam.transform.localScale.z);
    }
    // Update is called once per frame
    void Update() {

        if (Input.GetButton("Fire1")) {   
            photonBeam.SetActive(true);
            Shoot();
        }else{
            photonBeam.SetActive(false);
        }
    }
    void Shoot()
    {
        RaycastHit hit;
        if(Physics.SphereCast(FpsCam.transform.position, 1.0f, FpsCam.transform.forward, out hit, range, npcMask))
        {
            Debug.Log(hit.transform.name);

            Target target;
            target = hit.transform.GetComponent<Target>();
            if(!target){ // target is a child, sometimes you may hit the parent
                target = hit.transform.GetComponentInChildren<Target>();
            } if(target != null) {
                target.TakeDamage(damage);
            }
        } 
    }
}
