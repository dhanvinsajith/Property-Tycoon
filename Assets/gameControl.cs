using UnityEngine;
using UnityEngine.UI;
using Random=UnityEngine.Random;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using DG.Tweening;
using Newtonsoft.Json;

public class gameControl : MonoBehaviour
{
    public Transform[] waypoints;
    private Sprite[] diceSides, pfpPics;
    public GameObject playerObject, dieOne, dieTwo, rollButton, TurnHighlight, scoreboardPics, popups;
    private GameObject skipObj, bailObj, pickObj, fineObj;
    private SpriteRenderer dieOneRenderer, dieTwoRenderer;
    private Collider2D rollCollider;
    private Button skipButton, bailButton, pickButton, fineButton;
    private Sprite placeholderSprite;

    [SerializeField]
    private float moveSpeed = 1f;

    [HideInInspector]
    public List<int> waypointIndex = new List<int>();
    private List<int> turnHighlightPos = new List<int>();
    private List<int> moneyData = new List<int>();

    private List<GameObject> players = new List<GameObject>();

    int playerCount, turn, die1, die2, parkingFines, rollStartPos;
    int turnCount = 1;
    bool moveAllowed;
    bool noSkipEvent = false;

    Dictionary<GameObject, int> jailTime = new Dictionary<GameObject, int>();

    public TextMeshProUGUI turnText;
    public TextMeshProUGUI[] scoreboardTextArr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skipObj = popups.transform.Find("JailMsg").gameObject.transform.Find("SkipTurns").gameObject;
        bailObj = popups.transform.Find("JailMsg").gameObject.transform.Find("Pay50").gameObject;
        pickObj = popups.transform.Find("OppChoice").gameObject.transform.Find("PickCard").gameObject;
        fineObj = popups.transform.Find("OppChoice").gameObject.transform.Find("Pay10").gameObject;
        dieOneRenderer = dieOne.GetComponent<SpriteRenderer>();
        dieTwoRenderer = dieTwo.GetComponent<SpriteRenderer>();
        rollCollider = rollButton.GetComponent<Collider2D>();
        // skipCollider = skipObj.GetComponent<Collider2D>();
        // payCollider = bailObj.GetComponent<Collider2D>();
        skipButton = skipObj.GetComponent<Button>();
        bailButton = bailObj.GetComponent<Button>();
        pickButton = pickObj.GetComponent<Button>();
        fineButton = fineObj.GetComponent<Button>();
        diceSides = Resources.LoadAll<Sprite>("Sprites/dice/");
        pfpPics = Resources.LoadAll<Sprite>("Sprites/pfps/");
        
        moveAllowed = false;
        rollStartPos = 0;
        parkingFines = 0;
        turn = 1;

        string playerDataJson = File.ReadAllText("Assets/playerData.json");
        dynamic playerData = Newtonsoft.Json.JsonConvert.DeserializeObject(playerDataJson);
        turnHighlightPos.Add(100);
        for(int i=0; i<5; i++){
            if(playerData["data"][i]["token"].ToString() != "-1"){
                playerCount++;
                GameObject player = playerObject.transform.Find($"Player{i+1}").gameObject;
                player.SetActive(true);
                players.Add(player);
                waypointIndex.Add(0);
                moneyData.Add(1500);
                players[i].transform.position = waypoints[waypointIndex[0]].transform.position;
                if(i != 0){
                    turnHighlightPos.Add(turnHighlightPos[^1]-50);
                }
            }
        }

        skipButton.onClick.AddListener(() => DisplayPopup("close", turn-1));
        bailButton.onClick.AddListener(() => DisplayPopup("bail", turn-1));
        pickButton.onClick.AddListener(() => DisplayPopup("pick", turn-1));
        fineButton.onClick.AddListener(() => DisplayPopup("fine", turn-1));

    }

    // Update is called once per frame
    void Update()
    {
        DisplayData();

        Vector3 screenPosDepth = Input.mousePosition;
        screenPosDepth.z = 9.1f;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(screenPosDepth);

        if(!noSkipEvent){
            if(moveAllowed){
                if(players[turn-1].transform.position != waypoints[waypointIndex[turn-1]].transform.position){
                    Move();
                }else{
                    moveAllowed = false;
                    if(turnCount == 1 && moveAllowed == false){
                        if(turn==playerCount){
                            turn=1;
                            TurnHighlight.transform.localPosition = new Vector3(0,100,0);
                        }else{
                            turn++;
                            TurnHighlight.transform.localPosition = new Vector3(0, turnHighlightPos[turn-1], 0);
                        }
                    }
                }
            }else{
                if(rollCollider.OverlapPoint(mousePosition)){
                rollButton.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                }else{rollButton.transform.localScale = new Vector3(1.1f, 1.1f, 1f);}

                //BOARDSPECIFIC
                if(jailTime.ContainsKey(players[turn-1])){
                    Roll();
                }

                if(Input.GetMouseButtonDown(0)){
                    if(rollCollider.OverlapPoint(mousePosition)){
                        StartCoroutine("RollButtonAnim");
                        Roll();
                    }
                }
            }
        }
    }

    private void Roll(){
        //BOARDSPECIFIC
        if(jailTime.ContainsKey(players[turn-1])){
            if(jailTime[players[turn-1]] < 2){
                if(jailTime[players[turn-1]] == 0){
                    Debug.Log("IN JAIL, SKIP 2 TURNS OR PAY $50");
                    DisplayPopup("jail", turn-1);
                }
                jailTime[players[turn-1]] += 1;
            }else if(jailTime[players[turn-1]] == 2){
                jailTime.Remove(players[turn-1]);
                waypointIndex[turn-1] = 10;
                rollStartPos = waypointIndex[turn-1];
            }
            moveAllowed = true;
        }else{
            StartCoroutine("RollAnim");
            die1 = Random.Range(1,7);
            die2 = Random.Range(1,7);
            int totalRoll = die1+die2;
            Debug.Log($"{die1.ToString()}+{die2.ToString()}={totalRoll.ToString()}/n{waypointIndex[turn-1]}+{totalRoll.ToString()}");
            
            rollStartPos = waypointIndex[turn-1];
            waypointIndex[turn-1] = (waypointIndex[turn-1]+totalRoll)%40;

            //BOARDSPECIFIC
            if(die1 == die2 && turnCount<3){
                turnCount += 1;
            }else if(die1 == die2 && turnCount == 3){
                jailTime.Add(players[turn-1], 0);
                turnCount = 1;
                Debug.Log("GO TO JAIL");
                waypointIndex[turn-1] = 40;
                rollStartPos = waypointIndex[turn-1];
            }else{
                turnCount = 1;
            }

            // Debug.Log($"{players[turn-1].ToString()}");
            moveAllowed = true;
        }
    }

    private IEnumerator RollAnim()
    {
        noSkipEvent = true;

        for(int i=0; i<=15; i++){
            dieOne.transform.DOScale(0.85f, 0.15f);
            dieTwo.transform.DOScale(0.85f, 0.15f);
            dieOneRenderer.sprite = diceSides[Random.Range(0,6)];
            dieTwoRenderer.sprite = diceSides[Random.Range(0,6)];
            yield return new WaitForSeconds(0.07f);
        }

        dieOne.transform.DOScale(1f, 0.15f);
        dieTwo.transform.DOScale(1f, 0.15f);
        dieOneRenderer.sprite = diceSides[die1-1];
        dieTwoRenderer.sprite = diceSides[die2-1];
        yield return new WaitForSeconds(0.2f);
        noSkipEvent = false;
    }

    private void Move()
    {
        //  int prevStartPos = 0;

        if(players[turn-1].transform.position == waypoints[rollStartPos].transform.position){
            int prevStartPos = rollStartPos;
            rollStartPos = (rollStartPos+1)%40;
            
            //BOARDSPECIFIC
            if(rollStartPos == 0 && prevStartPos == 39){
                UpdateMoney(200, turn-1);
            }

            if(prevStartPos == 3 && waypointIndex[turn-1] == 4){
                UpdateMoney(-200, turn-1);
                parkingFines += 200;
            }else if(prevStartPos == 37 && waypointIndex[turn-1] == 38){
                UpdateMoney(-100, turn-1);
                parkingFines += 100;
            }else if(prevStartPos == 19 && waypointIndex[turn-1] == 20){
                UpdateMoney(parkingFines, turn-1);
                parkingFines = 0;
            }else if((prevStartPos == 6 && waypointIndex[turn-1] == 7) || (prevStartPos == 21 &&  waypointIndex[turn-1] == 22) || (prevStartPos == 35 && waypointIndex[turn-1] == 36)){
                PickCard("oppknocks", turn-1);
            }else if((prevStartPos == 1 && waypointIndex[turn-1] == 2) || (prevStartPos == 16 && waypointIndex[turn-1] == 17) || (prevStartPos == 32 && waypointIndex[turn-1] == 33)){
                PickCard("potluck", turn-1);
            }

            Debug.Log($"{string.Join( ",", moneyData.ToArray())}");
        }

        players[turn-1].transform.position = Vector2.MoveTowards(players[turn-1].transform.position, waypoints[rollStartPos].transform.position, moveSpeed*Time.deltaTime);
    }

    private IEnumerator RollButtonAnim()
    {
        Quaternion startPos = rollButton.transform.rotation;
        
        rollButton.transform.rotation = Quaternion.Euler(0, 180, 5);
        rollButton.transform.DOScale(1.2f, 0.15f);
        yield return new WaitForSeconds(0.15f);
        rollButton.transform.DOScale(1.1f, 0.15f);
        rollButton.transform.rotation = startPos;
    }

    private void DisplayData()
    {
        turnText.text = $"PLAYER {turn}";
        Color[] colors = {new Color(0.39f,1f,0.39f), new Color(0.39f,0.39f,1f), new Color(1f,1f,0.39f), new Color(1f,0.39f,0.39f,1f), new Color(1f,0.39f,1f)};
        string playerDataJson = File.ReadAllText("Assets/playerData.json");
        dynamic playerData = Newtonsoft.Json.JsonConvert.DeserializeObject(playerDataJson);
        for(int i=0; i<5; i++){
            string scoreText;
            if(playerData["data"][i]["token"].ToString() == "-1"){
                scoreText = ".....";
                scoreboardPics.transform.GetChild(i+1).gameObject.GetComponent<Image>().sprite = pfpPics[6];
            }else{
                scoreText = $"{playerData["data"][i]["name"]}<br>£{moneyData[i]}";
                // int index = int.Parse(playerData["data"][i]["token"].ToString());
                scoreboardPics.transform.GetChild(i+1).gameObject.GetComponent<Image>().sprite = pfpPics[int.Parse(playerData["data"][i]["token"].ToString())];
                scoreboardPics.transform.GetChild(i+1).gameObject.GetComponent<Image>().color = colors[int.Parse(playerData["data"][i]["color"].ToString())];
            }
            scoreboardTextArr[i].text = scoreText;
        }
    }

    private void UpdateMoney(int offset, int index)
    {
        moneyData[index] += offset;
    }

    private void DisplayPopup(string reason, int index)
    {
        if(reason == "jail"){
            noSkipEvent = true;
            popups.SetActive(true);
            popups.transform.Find("JailMsg").gameObject.SetActive(true);
            popups.transform.localScale = Vector3.zero;
            popups.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBounce);
        }else if(reason == "oppchoice"){
            noSkipEvent = true;
            popups.SetActive(true);
            popups.transform.Find("OppChoice").gameObject.SetActive(true);
            popups.transform.localScale = Vector3.zero;
            popups.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBounce);
        }else if(reason == "bail"){
            jailTime.Remove(players[index]);
            waypointIndex[index] = 10;
            rollStartPos = waypointIndex[index];
            UpdateMoney(-50, index);
            popups.transform.Find("JailMsg").gameObject.SetActive(false);
            popups.SetActive(false);
            noSkipEvent = false;
        }else if(reason == "close"){
            popups.transform.Find("JailMsg").gameObject.SetActive(false);
            popups.SetActive(false);
            noSkipEvent = false;
        }else if(reason == "pick"){
            popups.transform.Find("OppChoice").gameObject.SetActive(false);
            popups.SetActive(false);
            noSkipEvent = false;
            PickCard("oppknocks")
        }else if(reason == "fine") {
            UpdateMoney(-10, index);
            popups.transform.Find("OppChoice").gameObject.SetActive(false);
            popups.SetActive(false);
            noSkipEvent = false;
        }
    }

    private void PickCard(string pile, int index){
        if(pile == "potluck"){
            string[] action = {"You inherit £200", "You have won 2nd prize in a beauty contest, collect £50", "You are up the creek with no paddle - go back to the Old Creek", "Student loan refund. Collect £20", "Bank error in your favour. Collect £200", "Pay bill for text books of £100", "Mega late night taxi bill pay £50", "Advance to go", "From sale of Bitcoin you get £50", "Bitcoin assets fall - pay off Bitcoin short fall of £50", "Pay a £10 fine or take opportunity knocks", "Pay insurance fee of £50", "Savings bond matures, collect £100", "Go to jail. Do not pass GO, do not collect £200", "Received interest on shares of £25", "It's your birthday. Collect £10 from each player", "Get out of jail free"};
            // int actionIndex = 0;
            int actionIndex = Random.Range(0, action.Length);
            Debug.Log(action[actionIndex]);
            // if(new[6] {0,1,3,4,8,12,14}.Contains(actionIndex)){
            if(actionIndex == 0 || actionIndex == 1 || actionIndex == 3 || actionIndex == 4 || actionIndex == 8 || actionIndex == 12 || actionIndex == 14){
                int amount = int.Parse(action[actionIndex].Split('£')[1]);
                UpdateMoney(amount, index);
            }else if(actionIndex == 5 || actionIndex == 6 || actionIndex == 9){
                int amount = int.Parse(action[actionIndex].Split('£')[1]);
                UpdateMoney(-amount, index);
            }else if(actionIndex == 2){
                waypointIndex[index] =  1;
                rollStartPos = waypointIndex[index];
                moveAllowed = true;
            }else if(actionIndex == 7){
                rollStartPos = waypointIndex[index]+1;
                waypointIndex[index] = 0;
                moveAllowed = true;
            }else if(actionIndex == 10){DisplayPopup("oppchoice", index);
            }else if(actionIndex == 11){
                UpdateMoney(-50, index);
                parkingFines += 50;

            }
        }if(pile == "oppknocks"){
            string[] action = {"Bank pays you divided of £50", "You have won a lip sync battle. Collect £100", "Advance to Turing Heights", "Advance to Han Xin Gardens. If you pass GO, collect £200", "Fined £15 for speeding", "Pay university fees of £150", "Take a trip to Hove station. If you pass GO collect £200", "Loan matures, collect £150", "You are assessed for repairs, £40/house, £115/hotel", "Advance to GO", "You are assessed for repairs, £25/house, £100/hotel", "Go back 3 spaces", "Advance to Skywalker Drive. If you pass GO collect £200", "Go to jail. Do not pass GO, do not collect £200", "Drunk in charge of a hoverboard. Fine £30", "Get out of jail free"};
            int actionIndex = Random.Range(0, action.Length);
            Debug.Log(action[actionIndex]);
        }
    }
}
