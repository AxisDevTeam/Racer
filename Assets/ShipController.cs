using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShipController : MonoBehaviour
{
    Rigidbody rigidbody;
    [Header("General Settings")]
    public GameObject carModel;
    public GameObject animParent;
    public ShipBoost boost;
    public TMP_Text speedText;
    public Vector3 debugRayStart;
    public Vector3 debugRayDir;
    public float debugRayLength = 1f;

    public float currentGravForce;
    public bool useGrav;

    [Header("Movement Settings")]
    public float moveSpeed = 0.5f;
    public float currentSpeed;
    public float accelerationSpeed;
    public float decelerationSpeed;
    public float turnDecelSpeed;
    public float brakeDecelSpeed;
    public float accelerationLerpSpeed;
    public Vector3 extraneousForce;
    public float extraneousForceDecel = 0.05f;

    [Header("Drift Settings")]
    public bool isDrifting;
    public float driftTurnSpeed;
    public float driftTurnAcc;
    public float driftLateralMoveAmt;
    public float driftBoostAdd;
    public float driftBoostAcc;

    [Header("Turn Settings")]
    public float turnSpeed = 1f;
    public float currentTurnSpeed = 0;
    public float turnSpeedAcc = 1f;

    [Header("Animation Settings")]
    public float rotateTiltFactor = 10f;
    public float rotateLerpSpeed = 0.1f;

    public float bobSpeed = 1f;
    public float bobAmp = 1f;

    [Header("Hover Settings")]
    public float rayOffset = 0.5f;
    public float angleLerpSpeed =0.05f;
    public float heightOffset = 1f;
    public float heightCorrection = 0.01f;

    [Header("Cinemachine Settings")]
    public CinemachineVirtualCamera vcam;
    public float speedCamShake = 0.15f;
    public float baseFOV;
    public float FOVBoostSpeedAdd;
    public float FOVBoostAcc;

    [Header("Collision Settings")]
    public float bounceFactor = 0.5f;
    public float bounceSpeedDecrease;
    public float bounceBoostDecrease;

    [Header("Boost Settings")]
    public bool isBoosting;
    public float boostSpeed;
    public float boostDecelerationSpeed;
    public string boostPadTag;

    [Header("Post Process Settings")]
    public VolumeProfile volume;
    ChromaticAberration chromaticAberration;
    public float baseCMAmount;
    public float boostCMAmount;

    
    



    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        volume.TryGet<ChromaticAberration>(out ChromaticAberration cm);
        chromaticAberration = cm;
        baseFOV = Camera.main.fieldOfView;
        boost = GetComponent<ShipBoost>();
        speedText = GameObject.Find("Speed Text").GetComponent<TMP_Text>();
        //vcam = GameObject.Find("CM Ship Cam").GetComponent<CinemachineVirtualCamera>();
        vcam = GameObject.FindGameObjectsWithTag("CMShipCam")[0].GetComponent<CinemachineVirtualCamera>();
        vcam = GameObject.FindGameObjectsWithTag("CMShipCam")[0].GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        extraneousForce = Vector3.Lerp(extraneousForce, Vector3.zero, extraneousForceDecel);


        RaycastHit hit;
        Ray gravityCheck = new Ray(transform.position-(transform.up* rayOffset), -transform.up);
        Debug.DrawRay(transform.position - (transform.up * rayOffset), -transform.up * 10, Color.red);


        if (Physics.Raycast(gravityCheck, out hit, heightOffset * 2))
        {
            rigidbody.useGravity = false;
            useGrav = false;
            currentGravForce = 0;
            Vector3 interpolatedNormal = BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit);

            rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, interpolatedNormal) * rigidbody.rotation, angleLerpSpeed));

            rigidbody.MovePosition(Vector3.Lerp(transform.position, hit.point + (interpolatedNormal * heightOffset), heightCorrection));
        }
        else
        {
            rigidbody.useGravity = true;
            useGrav = true;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }

        float targetTurnSpeed;
        float targetTurnAcc;

        if (isDrifting && useGrav==false)
        {
            targetTurnSpeed = driftTurnSpeed;
            targetTurnAcc = driftTurnAcc;
            extraneousForce += carModel.transform.right * driftLateralMoveAmt * Input.GetAxisRaw("Horizontal") * rigidbody.velocity.magnitude / moveSpeed;
            boost.addBoost((Mathf.Abs(currentTurnSpeed)/driftTurnSpeed)*driftBoostAdd);
        }
        else
        {
            targetTurnSpeed = turnSpeed;
            targetTurnAcc = turnSpeedAcc;
        }
        if (useGrav == false)
        {
            currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, Input.GetAxisRaw("Horizontal") * targetTurnSpeed, targetTurnAcc);
        }
        else
        {
            currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, Input.GetAxisRaw("Horizontal") * targetTurnSpeed / 4, targetTurnAcc);

        }

        carModel.transform.Rotate(0.0f, currentTurnSpeed, 0.0f);



        //if (rigidbody.useGravity == false) {
        if (true){
            //rigidbody.velocity = carModel.transform.forward * Input.GetAxis("Vertical") * moveSpeed;
            float targetFOV = baseFOV;
            if (currentSpeed > moveSpeed)
            {
                currentSpeed -= boostDecelerationSpeed;
                chromaticAberration.intensity.value = BarycentricCoordinateInterpolator.Remap(boostCMAmount*((currentSpeed-moveSpeed) / (boostSpeed-moveSpeed)), 0, boostCMAmount, baseCMAmount, boostCMAmount);
                
                targetFOV = baseFOV + (FOVBoostSpeedAdd*((currentSpeed - moveSpeed) / (boostSpeed - moveSpeed)));
            }
            else
            {
                if (Input.GetKey(KeyCode.W) || Input.GetButton("Fire1"))
                {
                    currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, accelerationLerpSpeed);
                }
                else
                {
                    if (currentSpeed > 0)
                    {
                        currentSpeed -= decelerationSpeed;
                    }
                }
                chromaticAberration.intensity.value = baseCMAmount;
                targetFOV = baseFOV;
            }


            vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, targetFOV, FOVBoostAcc);

            rigidbody.velocity = (carModel.transform.forward * currentSpeed) + extraneousForce - (Vector3.down*currentGravForce);
            if (useGrav)
            {
                currentGravForce -= 9.81f * Time.deltaTime;
            }
            currentSpeed -= turnDecelSpeed * Mathf.Abs(Input.GetAxisRaw("Horizontal"));
            currentSpeed = Mathf.Max(0, currentSpeed);

            if (Input.GetKey(KeyCode.S) || Input.GetButton("Cancel"))
            {
                if (currentSpeed > 0 && useGrav == false)
                {
                    currentSpeed -= brakeDecelSpeed;
                }
            }
        }
        
        

        animParent.transform.localRotation = Quaternion.Lerp(animParent.transform.localRotation, Quaternion.Euler(new Vector3(0,0, (currentTurnSpeed/ targetTurnSpeed) * rotateTiltFactor)), rotateLerpSpeed);
        vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = (rigidbody.velocity.magnitude/moveSpeed)* speedCamShake;

        //animParent.transform.localPosition = new Vector3(0,Mathf.Sin(Time.time*bobSpeed)*bobAmp,0);

        


        


        speedText.text = "Speed: " + Mathf.RoundToInt(rigidbody.velocity.magnitude*10) + "km/h";

        Debug.DrawRay(debugRayStart, debugRayDir * debugRayLength,Color.red);

        if (Input.GetKey(KeyCode.E))
        {
            boost.isBoosting = true;
            if(boost.canBoost == true)
            {
                //currentSpeed = boostSpeed;
                currentSpeed = Mathf.Lerp(currentSpeed, boostSpeed, driftBoostAcc);
            }
        }
        else
        {
            boost.isBoosting = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            transform.position = Vector3.zero;
            rigidbody.velocity = Vector3.zero;
            currentSpeed = 0;
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (useGrav == false)
        {
            currentSpeed *= bounceSpeedDecrease;
        }
        else
        {
            currentSpeed = 0;
        }
        var calc = (Mathf.Sqrt(collision.impulse.magnitude / moveSpeed)) * bounceFactor;
        //rigidbody.AddForce(calc * collision.contacts[0].normal);
        Vector3 pushForce = carModel.transform.InverseTransformVector(calc * collision.contacts[0].normal);
        pushForce = new Vector3(pushForce.x, 0, pushForce.z);
        pushForce = carModel.transform.TransformVector(pushForce);
        extraneousForce = pushForce;

            boost.subtractBoost(bounceBoostDecrease * (collision.impulse.magnitude / moveSpeed));

        debugRayDir = collision.contacts[0].normal;
        debugRayLength = calc;
        debugRayStart = transform.position;

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == boostPadTag)
        {
            currentSpeed = boostSpeed;
        }
    }

    public IEnumerator Boost(float seconds)
    {
        float timePassed = 0;
        while (timePassed < seconds)
        {
            // Code to go left here
            timePassed += Time.deltaTime;
            currentSpeed = boostSpeed;
            yield return null;
        }
    }
}
