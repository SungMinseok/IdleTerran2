using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum BotState{
    Stop,
    Mine,
}
public class BotScript : MonoBehaviour
{
    public BotState botState;
    public int botType;
    public float efficiency;
    [HideInInspector]public Rigidbody2D rb;
    [HideInInspector]public SpriteRenderer sr;
    private Vector2 defaultSide;
    [HideInInspector]public Vector2 movement;
    [HideInInspector]public Vector2 movementDirection;
    [HideInInspector]public bool canMove;
    [HideInInspector]public bool isAlive;
    [HideInInspector]public bool isHolding;//뭔가 들고 있을 때
    [HideInInspector]public bool isMining;//채취 중일 때
    [SerializeField] public UnitType type;
    [SerializeField] public ShadowType shadowType;
    [SerializeField] public PackageType packageType = PackageType.none;
    private Animator animator;
    public bool gotMine;//미네랄 발견
    public bool gotDestination;//센터 발견
    public Transform destination;

    [Header("능력치")]
    public float speed;
    public float weldingSec;
    public float fuelUsagePerWalk;
    public long capacity;
    ////////////////////
    public GameObject workLight;
    public GameObject mineral;
    public GameObject booster;
    public SpriteRenderer[] boosters;
    bool miningFlag;
    //public GameObject floatingText;
    //public GameObject floatingCanvas;
    public GameObject miningMineral;
    public Transform selectedMineral;
    void Awake(){
        if(shadowType==ShadowType.booster){
            
            booster = transform.GetChild(0).gameObject;
            booster.SetActive(false);
            boosters = new SpriteRenderer[3];
            for(int i=0;i<3;i++){
                //Debug.Log(booster.transform.childCount);
                //Debug.Log(booster.transform.GetChild(i).name);
                boosters[i] = booster.transform.GetChild(i).GetComponent<SpriteRenderer>();
            }
            defaultSide = boosters[1].GetComponent<RectTransform>().localPosition;
        }
    }
    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();   
        //floatingCanvas = PlayerManager.instance.floatingCanvas;     
        
        selectedMineral = UIManager.instance.mineralsInMap[Random.Range(0,UIManager.instance.mineralsInMap.Length)] ;
    }

    // Update is called once per frame
    void Update()
    {
        if(botState == BotState.Mine){
            if(!isHolding&&!isMining){
                //destination = GameObject.FindWithTag("Mineral Field").transform;
                
                //destination = PlayerManager.instance.selectedMineral;
                destination = selectedMineral;
                if(gotMine){
                    gotMine = false;
                    isMining = true;
                    miningMineral = destination.gameObject;
                    destination = null;

                }
                else if(Mathf.Abs(transform.position.y - destination.position.y)>=0.1f){
                    if(transform.position.y > destination.position.y){
                        movement = new Vector2(0,-1);
                    }
                    else{
                        movement = new Vector2(0,1);

                    }
                    sr.flipX = false;
                        SetBooster("UPDOWN");
                    //rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

                    
                }
                else if(Mathf.Abs(transform.position.x - destination.position.x)>=0.1f){

                    if(transform.position.x > destination.position.x){
                        movement = new Vector2(-1,0);
                    }
                    else{
                        movement = new Vector2(1,0);

                    }
                    sr.flipX = animator.GetFloat("Horizontal") < 0 ? true : false;

                        SetBooster("LEFTRIGHT"); 
                    //rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

                }
            }
            else if(isMining){
                animator.SetBool("Stop", true);
                workLight.SetActive(true);                    
                if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Att")){
                        
                        animator.SetTrigger("Att");
                    }
                if(!miningFlag){
                    miningFlag = true;
                    StartCoroutine(MiningCoroutine());
                } 
            }
            else if(isHolding&&!isMining){
                destination = PlayerManager.instance.centerPos;
                if(gotDestination){
                        HandleMineral();
                    gotDestination = false;
                    isHolding = false;
                    destination = null;
                    mineral.gameObject.SetActive(false);
                    //packageType = PackageType.none;
                }
                else if(Mathf.Abs(transform.position.y - destination.position.y)>=
                destination.GetComponent<BoxCollider2D>().size.y * destination.localScale.y /2){                    
                    if(transform.position.y > destination.position.y){
                        movement = new Vector2(0,-1);
                    }
                    else{
                        movement = new Vector2(0,1);

                    }
                    sr.flipX = false;
                        SetBooster("UPDOWN");
                    //rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

                    
                }
                else if(Mathf.Abs(transform.position.x - destination.position.x)>=0.1f){
                    if(transform.position.x > destination.position.x){
                        movement = new Vector2(-1,0);
                    }
                    else{
                        movement = new Vector2(1,0);

                    }
                    sr.flipX = animator.GetFloat("Horizontal") < 0 ? true : false;

                        SetBooster("LEFTRIGHT"); 
                    //rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

                }
            }
        }

    }

    void FixedUpdate(){
        //rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        //movement = new Vector2(animator.GetFloat("Horizontal"),animator.GetFloat("Vertical"));
        if(movement != Vector2.zero && !isMining){
            if(PlayerManager.instance.curFuel>0){
                rb.MovePosition(rb.position + movement * (speed * PlayerManager.instance.bonusSpeed) * Time.deltaTime);
//                Debug.Log("봇 속도 : "+(speed * PlayerManager.instance.bonusSpeed) * Time.deltaTime);

                PlayerManager.instance.HandleFuel(-fuelUsagePerWalk);

            }
        }
        
        movementDirection = new Vector2(movement.x, movement.y);
        if (movementDirection != Vector2.zero){
            animator.SetFloat("Horizontal", movementDirection.x);
            animator.SetFloat("Vertical", movementDirection.y);
        }
        // if (shadowType == ShadowType.booster){
        //     if(animator.GetFloat("Speed")!=0 && !animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Att")){
        //         booster.SetActive(true);
        //     }
        //     else{
        //         booster.SetActive(false);
        //     }
        // }
    }


    IEnumerator MiningCoroutine(){
                    //Debug.Log("미네랄 채취 시작");
        yield return new WaitForSeconds(weldingSec);
                    //Debug.Log("미네랄 채취 완료");
                    workLight.SetActive(false);
        animator.SetBool("Stop", false);
        isHolding = true;
        isMining = false;                        
        miningFlag = false;
        mineral.gameObject.SetActive(true);

        miningMineral.GetComponent<MineralScript>().GotMined(capacity);
        miningMineral = null;
        //미네랄 종류 선택()
        //packageType = PackageType.normal;
    }

    private void OnTriggerStay2D(Collider2D collision){
        // if(!placeFlag){
        //     placeFlag = true;
            //if(collision.tag == "Mineral Field"){
            if(collision.transform == selectedMineral.transform){
                if(!isHolding){
                    gotMine = true;
                    //miningMineral = collision.gameObject.GetComponent<MineralScript>();
                }
                //Debug.Log("미네랄 발견");
            }        
            
            else if(destination != null){
                    
                if(collision.name == destination.name){
                    //if(isHolding){
                        gotDestination = true;
                    //}
                    //Debug.Log("센터 발견");
                }
            }
        //}
    
    }
        private void OnTriggerExit2D(Collider2D collision){
        // if(!placeFlag){
        //     placeFlag = true;
            if(collision.CompareTag("Mineral Field")){

                    gotMine = false;
        isMining = false;
        
        // if(isHolding) miningMineral.GotMined(capacity);
        // miningMineral = null;
            }        
            
            else if(collision.CompareTag("Center")){

                    gotDestination = false;
            }
        //}
    
    }
    public void SetBooster(string dir){
        if(dir=="UPDOWN"){
            if (shadowType == ShadowType.booster){
                if(animator.GetFloat("Vertical") < 0){//하
                    boosters[0].gameObject.SetActive(false);
                    boosters[1].gameObject.SetActive(false);
                    boosters[2].gameObject.SetActive(true);
                    mineral.transform.localPosition = new Vector2(0,-0.05f);
                    workLight.transform.localPosition = new Vector2(0,-0.2f);
                }
                else{//상
                    boosters[0].gameObject.SetActive(true);
                    boosters[1].gameObject.SetActive(false);
                    boosters[2].gameObject.SetActive(false);
                    mineral.transform.localPosition = new Vector2(0,0.11f);
                    workLight.transform.localPosition = new Vector2(0.1f,0.2f);
                }
            }
        }
        else{
            if (shadowType == ShadowType.booster){
                if(animator.GetFloat("Horizontal") > 0){//우
                    boosters[1].flipX = false;
                    boosters[1].transform.localPosition = new Vector2(defaultSide.x,defaultSide.y);
                    boosters[0].gameObject.SetActive(false);
                    boosters[1].gameObject.SetActive(true);
                    boosters[2].gameObject.SetActive(false);
                    mineral.transform.localPosition = new Vector2(0.084f,0.034f);
                    workLight.transform.localPosition = new Vector2(0.25f,0);
                }
                else{//좌
                    boosters[1].flipX = true;
                    boosters[1].transform.localPosition = new Vector2(-defaultSide.x,defaultSide.y);
                    boosters[0].gameObject.SetActive(false);
                    boosters[1].gameObject.SetActive(true);
                    boosters[2].gameObject.SetActive(false);
                    mineral.transform.localPosition = new Vector2(-0.084f,0.034f);
                    workLight.transform.localPosition = new Vector2(-0.25f,0);
                }
            }
        }
    }

    public void HandleMineral(long amount = 0,bool floating = true){
        //int temp0 = curMineral;
        //int preMineral = curMineral;
        if(amount==0){
            float temp = Mathf.Ceil(capacity * PlayerManager.instance.bonusCapacity);
//long temp = Mathf.Ceil((float)capacity * PlayerManager.instance.bonusCapacity);
            switch(packageType){
                case PackageType.normal :
                    PlayerManager.instance.curMineral += (long)temp;
                    AchvManager.instance.totalMineral +=(long)temp;
                    break;
                default :
                    break;
            }
            
            if(UIManager.instance.set_floating) {
                if(floating) UIManager.instance.PrintFloating("+ "+temp.ToString(), transform);
                //PrintFloating("+ "+temp.ToString());
            }
        }
        else{
            PlayerManager.instance.curMineral += amount;
        }


    }    
    // public void PrintFloating(string text, Sprite sprite = null)
    // {

    //     if (text != "")
    //     {
    //         //var clone = Instantiate(floatingText, floatingCanvas.transform.position, Quaternion.identity);
    //         var clone = Instantiate(floatingText, new Vector2(transform.position.x,transform.position.y+0.3f), Quaternion.identity);
    //         clone.transform.GetChild(0).GetComponent<Text>().text = text;
    //         if(floatingCanvas.transform.childCount>=1){
    //             var temp = floatingCanvas.transform.GetChild(floatingCanvas.transform.childCount-1).transform;
    //             //Vector2 tempVect = new Vector2(temp.localScale.x * 1.05f,temp.localScale.y *1.05f);
    //             //clone.transform.localScale = new Vector2(temp.localScale.x+ 0.05f,temp.localScale.y +0.05f);;
    //             clone.GetComponent<Canvas>().sortingOrder = temp.GetComponent<Canvas>().sortingOrder+1;
            
            
    //             // Debug.Log(clone.transform.localScale);
    //             // Debug.Log(floatingCanvas.transform.GetChild(floatingCanvas.transform.childCount-1).transform.localScale);
            
    //         }
    //         clone.transform.SetParent(floatingCanvas.transform);
    //     }
    // }
    public void RepSound(){
        SoundManager.instance.Play("rep"+Random.Range(0,5));
    }    
    public void DestroyBot(){
        Debug.Log("봇 파괴");
        Destroy(this.gameObject);
    }
    // public void UnsetBot(int typeNum){
    //     for(int i=0;i<FactoryManager.instance.botManager.childCount;i++){
    //         if(typeNum == FactoryManager.instance.botManager)
    //     }
    // }
}
