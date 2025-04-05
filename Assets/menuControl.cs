using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class menuControl : MonoBehaviour
{
    public GameObject addPlayerButton, removePlayerButton;
    public GameObject startButton, quitButton, mainMenuButton, startGameButton;
    public RectTransform addRemoveParent, setupGameBanner, setupObject;
    public List<GameObject> playerSelectBanners = new List<GameObject>();

    int playerCount = 0;
    int yAnchor = 122;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button addBtn = addPlayerButton.GetComponent<Button>();
        Button remBtn = removePlayerButton.GetComponent<Button>();

        addBtn.onClick.AddListener(() => AddPlayer());
        remBtn.onClick.AddListener(() => RemovePlayer());
        startButton.GetComponent<Button>().onClick.AddListener(() => StartSetup());
        quitButton.GetComponent<Button>().onClick.AddListener(() => QuitGame());
        mainMenuButton.GetComponent<Button>().onClick.AddListener(() => MainMenu());
        startGameButton.GetComponent<Button>().onClick.AddListener(() => StartGame());
    }

    // Update is called once per frame
    void Update()
    {
    }

    void StartSetup(){
        setupGameBanner.DOAnchorPos(new Vector2(0, 188), 0.8f, false).SetEase(Ease.OutElastic);
        setupObject.DOAnchorPos(new Vector2(0, -15), 0.9f);
        startGameButton.GetComponent<RectTransform>().DOAnchorPos(new Vector2(312, -188), 0.85f);
        mainMenuButton.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-312, -188), 0.85f);
        startButton.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -500), 0.85f);
        quitButton.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -500), 0.85f);
    }

    void QuitGame(){

    }

    void MainMenu(){
        setupGameBanner.DOAnchorPos(new Vector2(0, 300), 0.8f);
        setupObject.DOAnchorPos(new Vector2(0, 600), 0.9f);
        startGameButton.GetComponent<RectTransform>().DOAnchorPos(new Vector2(312, -500), 0.85f);
        mainMenuButton.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-312, -500), 0.85f);
        startButton.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -125), 0.85f);
        quitButton.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -175), 0.85f);

        yAnchor = 122;
        playerCount = 0;
        addRemoveParent.DOAnchorPos(new Vector2(0, yAnchor), 0.2f);
        removePlayerButton.SetActive(false);
        playerSelectBanners[0].GetComponent<bannerControl>().ResetAllValues();
    }

    void StartGame(){
        playerSelectBanners[0].GetComponent<bannerControl>().SendData();
    }

    void AddPlayer(){
        yAnchor -= 61;
        CloseAllMenus(this.gameObject);
        addRemoveParent.DOAnchorPos(new Vector2(0, yAnchor), 0.2f);
        playerSelectBanners[playerCount].SetActive(true);
        playerSelectBanners[playerCount].transform.localScale = Vector3.zero;
        playerSelectBanners[playerCount].transform.DOScale(1f, 0.4f).SetEase(Ease.OutBounce);

        if(playerCount==0){
            removePlayerButton.SetActive(true);
            removePlayerButton.transform.localScale = Vector3.zero;
            removePlayerButton.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBounce);
        }

        if(playerCount<4){
            playerCount++;
        }else{
            addPlayerButton.SetActive(false);
        }
    }

    void RemovePlayer(){
        yAnchor += 61;
        CloseAllMenus(this.gameObject);                                                                 //??reckeck
        playerSelectBanners[0].GetComponent<bannerControl>().ResetBannerVal(playerCount-1);
        addRemoveParent.DOAnchorPos(new Vector2(0, yAnchor), 0.2f);
        playerSelectBanners[playerCount-1].SetActive(false);
        if(playerCount == 1){
            removePlayerButton.SetActive(false);
        }
        playerCount--;
    }

    public void CloseAllMenus(GameObject clickedBanner){
        for(int i=0; i<5; i++){
            if(playerSelectBanners[i] != clickedBanner){
                playerSelectBanners[i].GetComponent<bannerControl>().CloseMenus();
            }
        }
    }
}
