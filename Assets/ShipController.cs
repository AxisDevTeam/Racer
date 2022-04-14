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
    public TMP_Text speedText;
    public Vector3 debugRayStart;
    public Vector3 debugRayDir;
    public float debugRayLength = 1f;

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
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        extraneousForce = Vector3.Lerp(extraneousForce, Vector3.zero, extraneousForceDecel);


        RaycastHit hit;
        Ray gravityCheck = new Ray(transform.position-(transform.up* rayOffset), -transform.up);
        Debug.DrawRay(transform.position - (transform.up * rayOffset), -transform.up * 10, Color.red);


        if(Physics.Raycast(gravityCheck, out hit, heightOffset*2))
        {
            rigidbody.useGravity = false;
            Vector3 interpolatedNormal = BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit);

            rigidbody.MoveRotation(Quaternion.Lerp(transform.rotation,Quaternion.FromToRotation(transform.up, interpolatedNormal) * rigidbody.rotation, angleLerpSpeed));

            rigidbody.MovePosition(Vector3.Lerp(transform.position, hit.point + (interpolatedNormal * heightOffset),heightCorrection));
        }
        else
        {
            rigidbody.useGravity = true;
        }

        currentTurnSpeed = Mathf.Lerp(currentTurnSpeed, Input.GetAxisRaw("Horizontal") * turnSpeed, turnSpeedAcc);

        carModel.transform.Rotate(0.0f, currentTurnSpeed, 0.0f);

        

        if (rigidbody.useGravity == false) {
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

            rigidbody.velocity = (carModel.transform.forward * currentSpeed) + extraneousForce;
            currentSpeed -= turnDecelSpeed * Mathf.Abs(Input.GetAxisRaw("Horizontal"));
            currentSpeed = Mathf.Max(0, currentSpeed);

            if (Input.GetKey(KeyCode.S) || Input.GetButton("Cancel"))
            {
                if (currentSpeed > 0)
                {
                    currentSpeed -= brakeDecelSpeed;
                }
            }
        }




        animParent.transform.localRotation = Quaternion.Lerp(animParent.transform.localRotation, Quaternion.Euler(new Vector3(0,0, (currentTurnSpeed/turnSpeed) * rotateTiltFactor)), rotateLerpSpeed);
        vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = (rigidbody.velocity.magnitude/moveSpeed)* speedCamShake;

        animParent.transform.localPosition = new Vector3(0,Mathf.Sin(Time.time*bobSpeed)*bobAmp,0);

        


        


        speedText.text = "Speed: " + Mathf.RoundToInt(rigidbody.velocity.magnitude*10) + "km/h";

        Debug.DrawRay(debugRayStart, debugRayDir * debugRayLength,Color.red);
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
        currentSpeed *= bounceSpeedDecrease;
        var calc = (Mathf.Sqrt(collision.impulse.magnitude / moveSpeed)) * bounceFactor;
        //rigidbody.AddForce(calc * collision.contacts[0].normal);
        extraneousForce = calc * collision.contacts[0].normal;

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
}