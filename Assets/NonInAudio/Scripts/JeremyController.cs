using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JeremyController : MonoBehaviour
{
    public static JeremyController instance;
    public Animator anim;
    public CharacterController controller;
    public KeyCode key_left, key_right, key_crouch;
    public float moveSpeed, walkspeed, crouchSpeed;
    public static bool isDead;
    public InAudioNode footstep;

    private float cameraDistance = 4, cameraHeight = 2, cameraLead = 0.2f;
    private float cameraDistanceTarget = 4, cameraHeightTarget = 2, cameraLeadTarget = 0.2f;
    private float cameraLerp = 0, camLerpMax = 1;
    public float CameraLerpTime
    {
        get { return cameraLerp; }
        set { cameraLerp = value; camLerpMax = value; }
    }
    public float CameraDistance
    {
        get { return cameraDistance; }
        set { cameraDistanceTarget = value; }
    }
    public float CameraHeight
    {
        get { return cameraHeight; }
        set { cameraHeightTarget = value; }
    }
    public float CameraLead
    {
        get { return cameraLead; }
        set { cameraLeadTarget = value; }
    }

    public Vector3 gravity;
    public float xVelocity = 0;
    Vector3 MoveVector = Vector3.zero;
    public int move = 0;
    Camera cam;
    float hangingDuration = 0;
    public LayerMask groundRaycast;
    float timeInSafety = 0;
    public bool canHeal = false;
    public bool inSafety = false;

    public GameObject head;

    void Awake()
    {
        instance = this;
        cam = Camera.main;
    }

    void OnEnable()
    {
        head = GameObject.Find("HeadJ");
    }

    public bool isGrounded;

    void Grounded()
    {
        Ray r = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        isGrounded = controller.isGrounded || Physics.SphereCast(r, 0.25f, 0.5f, groundRaycast);
    }

    void Update()
    {
        Vector3 corrected = transform.position;
        corrected.z = 0;
        transform.position = corrected;
        UpdateInput();
        if (!isDead)
        {
            UpdateMovement();
            UpdateGravity();
        }

        if (!isGrounded || jumping)
            hangingDuration += Time.deltaTime;
        else
            hangingDuration = 0;

        if (hangingDuration > 0.2f)
        {
            if (anim.GetBool("hanging"))
                anim.SetBool("jumping", true);
            anim.SetBool("hanging", true);

        }
        else
        {
            if (!jumping)
                anim.SetBool("jumping", false);

            anim.SetBool("hanging", false);
            if (isGrounded && Input.GetKeyDown(KeyCode.Space) && !jumping)
            {
                if (!waitingForJump)
                {
                    jumping = true;
                    anim.SetBool("jumping", true);
                    StartCoroutine(DelayedJump());
                }
            }
        }
        UpdateCamera();
    }

    [HideInInspector]
    public bool walking, crouching, running;

    void UpdateInput()
    {
        if (isDead)
            return;
        if (Input.GetKeyDown(KeyCode.K))
        {
            Ragdoll(!anim.enabled);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("crouching", true);
            controller.height = 11;
            controller.center = new Vector3(0, 5.5f, 0);
            crouching = true;
        }
        else
        {
            anim.SetBool("crouching", false);
            controller.height = 16;
            controller.center = new Vector3(0, 8.5f, 0);
            crouching = false;
        }

        MoveVector = Vector3.zero;
        move = 0;
        if (Input.GetKey(key_left))
        {
            move -= 1;
        }
        if (Input.GetKey(key_right))
        {
            move += 1;
        }

        canHeal = false;

        if (move > 0)
        {
            xVelocity = Mathf.Lerp(xVelocity, GetSpeed(), Time.deltaTime * 3);
            timeInSafety = 0;
            isInSafety();
        }
        else if (move < 0)
        {
            xVelocity = Mathf.Lerp(xVelocity, -GetSpeed(), Time.deltaTime * 3);
            timeInSafety = 0;
            isInSafety();
        }
        else
        {
            xVelocity = Mathf.Lerp(xVelocity, 0, Time.deltaTime * 3);
            if (isInSafety())
            {
                timeInSafety += Time.deltaTime;

                canHeal = true;
                if (healingParticle.isStopped)
                    healingParticle.Play();
                healingParticle.startColor = new Color(1, 1, 1, timeInSafety * 0.85f);
            }
            else
            {
                canHeal = false;
            }
        }

        if ((canHeal && timeInSafety > 1))
        {
            TimeControl.time += Time.deltaTime * (timeInSafety - 1) * 0.1f;
            TimeControl.time = Mathf.Clamp(TimeControl.time, 0, 1);
        }


        else if (!canHeal)
        {
            TimeControl.time -= Time.deltaTime * 0.1f * TimeControl.time + 0.01f * Time.deltaTime;
            TimeControl.time = Mathf.Clamp(TimeControl.time, 0, 1);

            healingParticle.Stop();
        }

        MoveVector = new Vector3(xVelocity, 0, 0);
        if (MoveVector.x > 0)
        {
            transform.localEulerAngles = new Vector3(0, Mathf.Lerp(transform.localEulerAngles.y, 90f, Time.deltaTime * 10), 0);
        }
        else
        if (MoveVector.x < 0)
        {
            transform.localEulerAngles = new Vector3(0, Mathf.Lerp(transform.localEulerAngles.y, 270f, Time.deltaTime * 10), 0);
        }
    }

    void LateUpdate()
    {
        if (Sight.SomeoneSeeing && TimeControl.time > 0 && !isDead)
        {
            TimeControl.time += Time.deltaTime * 0.1f;
            TimeControl.time = Mathf.Clamp(TimeControl.time, 0, 1);
        }
    }

    void Ragdoll(bool enable)
    {
        anim.enabled = enable;
        foreach (CharacterJoint j in GetComponentsInChildren<Joint>())
        {
            Rigidbody r = j.GetComponent<Rigidbody>();
            if (enable)
            {
                //j.projectionDistance = 01f;
                //j.projectionAngle *= 2;
                r.velocity = Vector3.zero;
            }
        }
        StartCoroutine(DampenRagdoll(enable));
    }

    IEnumerator DampenRagdoll(bool enable)
    {
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForFixedUpdate();



            foreach (CharacterJoint j in GetComponentsInChildren<Joint>())
            {
                Rigidbody r = j.GetComponent<Rigidbody>();
                r.velocity = new Vector3(xVelocity / 3 * r.transform.position.y, gravity.y / 3, 0);
                r.isKinematic = enable;
            }
        }
    }

    float GetSpeed()
    {
        if (crouching)
            return crouchSpeed;
        else if (walking)
            return walkspeed;
        else
            return moveSpeed;
    }

    bool waitingForJump = false;
    bool jumping = false;

    IEnumerator DelayedJump()
    {
        waitingForJump = true;
        yield return new WaitForSeconds(0.4f);
        jumping = true;
        anim.SetBool("hanging", true);
        gravity = Vector3.up * 5;
        waitingForJump = false;
        yield return new WaitForSeconds(0.5f);
        jumping = false;
    }

    void UpdateGravity()
    {
        Grounded();
        controller.Move(gravity * Time.deltaTime);
        if (!isGrounded)
        {
            gravity += Vector3.down * 10 * Time.deltaTime;
        }
        else if (!jumping)
        {
            if (gravity.y < -8)
            {
                Damage(Mathf.Abs((gravity.y + 8)) * 20);
            }
            gravity = Vector3.down * 0;
        }
    }

    void UpdateMovement()
    {
        controller.Move(MoveVector * Time.deltaTime);
        anim.SetFloat("MoveSpeed", Mathf.Abs(MoveVector.x));
    }

    void UpdateCamera()
    {
        cameraLerp -= Time.deltaTime;
        cameraDistance = Mathf.Lerp(cameraDistanceTarget, cameraDistance, (cameraLerp) / camLerpMax);
        cameraHeight = Mathf.Lerp(cameraHeightTarget, cameraHeight, (cameraLerp) / camLerpMax);
        cameraLead = Mathf.Lerp(cameraLeadTarget, cameraLead, (cameraLerp) / camLerpMax);
        cam.transform.position = transform.position + Vector3.back * cameraDistance + Vector3.up * cameraHeight + Vector3.right * xVelocity * cameraLead;
        cam.transform.LookAt(transform.position + Vector3.right * xVelocity * 0.25f + Vector3.up);
    }

    public void HitByBullet(Vector3 position)
    {
        //Kill
        Damage(101);
    }

    public void Damage(float damage)
    {
        TimeControl.time -= damage * 0.01f;
        TimeControl.BarIndication(1.5f, 0.4f, Color.red);
    }

    public void Kill()
    {
        if (!isDead)
        {
            Ragdoll(false);
            isDead = true;
            Debug.Log("Dead");
            MoveVector = Vector3.zero;
        }
    }

    public ParticleSystem rightFootParticle, leftFootParticle;

    public void Footstep(int foot)
    {
        foreach (Soldier s in Soldier.soldiers)
            s.MakeSound(transform.position);
        // 0 == right
        if (foot == 0)
        {
            rightFootParticle.Emit(3);
        }
        else if (foot == 1)
        {
            leftFootParticle.Emit(3);
        }
        else
        {
            leftFootParticle.Emit(3);
            rightFootParticle.Emit(3);
            InAudio.Play(gameObject, footstep);
        }
        InAudio.Play(gameObject, footstep);
    }

    public ParticleSystem healingParticle;

    bool isInSafety()
    {
        
        return false;
    }
}

internal class Sight
{
    public static bool SomeoneSeeing { get; set; }
}

public class TimeControl
{
    public static float time;

    public static void BarIndication(float f, float f1, Color red)
    {
        throw new System.NotImplementedException();
    }
}

public class Soldier
{
    public static IEnumerable<Soldier> soldiers;

    public void MakeSound(Vector3 position)
    {
        throw new System.NotImplementedException();
    }
}

internal class SafeArea
{
//    internal static IEnumerable<SafeArea> safeAreas;

    public bool Check()
    {
        throw new System.NotImplementedException();
    }
}