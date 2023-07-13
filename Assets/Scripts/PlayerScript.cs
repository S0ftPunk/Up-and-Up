using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] GameObject particles;

    public float swapForce = 2;
    [SerializeField] bool isGrounded;
    [SerializeField] LayerMask wall;
    [SerializeField] Transform checkWall;

    private bool isEnbledToGround;
    private int countOfJumps = 0;

    public Image loseEffect;
    private bool isLosed = false;

    public Spawner spawner;
    public ManagerScene manager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void SideSwap()
    {
        if (countOfJumps <= 1)
        {
            swapForce *= -1;
            anim.SetTrigger("SideSwap");
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            particles.GetComponent<ParticleSystem>().startRotation *= -1;
            StartCoroutine(GroundDelay());
            countOfJumps += 1;
        }
    }
    public void Lose()
    {
        Debug.Log("Loooooooose");
        isLosed = true;
        loseEffect.GetComponent<Animator>().SetTrigger("Losed");
        rb.bodyType = RigidbodyType2D.Static;
        manager.StopGame();
        particles.SetActive(false);
    }
    public void Continue()
    {
        isLosed = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        particles.SetActive(true);
    }
    public void Boost()
    {
        spawner.isBoosted = true;
        spawner.CopyVars();
        StartCoroutine(BoostOff());
        particles.GetComponent<ParticleSystem>().playbackSpeed = 3;
    }

    private void Update()
    {
        if (!isLosed)
        {
            if (Input.anyKeyDown)
            {
                SideSwap();
            }
            isGrounded = Physics2D.OverlapCircle(checkWall.position, 0.05f, wall);

            if (isGrounded & isEnbledToGround)
            {
                particles.SetActive(true);
                isEnbledToGround = false;
                countOfJumps = 0;
            }

            if (!isGrounded)
            {
                particles.SetActive(false);
                isEnbledToGround = true;
            }
        }
    }
    void FixedUpdate()
    {
       if(!isLosed)
        rb.velocity = new Vector2(swapForce, rb.velocity.y);
    }
    IEnumerator GroundDelay()
    {
        particles.SetActive(false);
        yield return new WaitForSeconds(0.4f);
        isEnbledToGround = true;
    }
    IEnumerator BoostOff()
    {
        yield return new WaitForSeconds(4f);
        spawner.isBoosted = false;
        particles.GetComponent<ParticleSystem>().playbackSpeed = 1;
    }
}
