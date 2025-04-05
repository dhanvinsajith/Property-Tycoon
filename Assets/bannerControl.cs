using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using Newtonsoft.Json;

public class bannerControl : MonoBehaviour
{
    public GameObject captionImage, playerType, playerTypeDropdown, playerTypeCaption, colorImage, menuController;

    public List<GameObject> tokenSelectImages = new List<GameObject>();
    public List<GameObject> colSelectImages = new List<GameObject>();
    public List<GameObject> playerTypeOptions = new List<GameObject>();

    public List<GameObject> tokenPicks = new List<GameObject>();
    public List<GameObject> typePicks = new List<GameObject>();
    public List<GameObject> nameObjects = new List<GameObject>();
    public List<GameObject> colorPicks = new List<GameObject>();
    public List<GameObject> playerBanners = new List<GameObject>();

    private Sprite placeholderSprite;
    
    bool tokenClicked, typeClicked, colorClicked = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button captionBtn = captionImage.GetComponent<Button>();
        Button typeBtn = playerType.GetComponent<Button>();
        Button colBtn = colorImage.GetComponent<Button>();

        captionBtn.onClick.AddListener(() => TokenSelect());
        typeBtn.onClick.AddListener(() => TypeSelect());
        colBtn.onClick.AddListener(() => ColorSelect());

        placeholderSprite = tokenPicks[0].GetComponent<Image>().sprite;

        foreach(GameObject tokenimage in tokenSelectImages){
            Button tokenOptBtn = tokenimage.GetComponent<Button>();
            tokenOptBtn.onClick.AddListener(() => TokenOptionSelect(tokenOptBtn));
        }
        foreach(GameObject typeOption in playerTypeOptions){
            Button typeOptBtn = typeOption.GetComponent<Button>();
            typeOptBtn.onClick.AddListener(() => TypeOptionSelect(typeOptBtn));
        }
        foreach(GameObject colImage in colSelectImages){
            Button colOptBtn = colImage.GetComponent<Button>();
            colOptBtn.onClick.AddListener(() => ColorOptionSelect(colOptBtn));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TokenSelect(){
        if(!typeClicked && !colorClicked){
            if(tokenClicked){
                foreach(GameObject tokenimage in tokenSelectImages){
                    tokenimage.transform.DOScale(0f, 0.3f).SetEase(Ease.OutBounce);
                    tokenimage.transform.GetChild(0).gameObject.SetActive(false);
                }
                tokenClicked = false;
            }else{
                tokenClicked = true;
                menuController.GetComponent<menuControl>().CloseAllMenus(this.gameObject);
                foreach(GameObject tokenimage in tokenSelectImages){
                    tokenimage.SetActive(true);
                    tokenimage.transform.localScale = Vector3.zero;
                    tokenimage.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce);
                    foreach(GameObject tokenPick in tokenPicks){
                        if(tokenimage.GetComponent<Image>().sprite == tokenPick.GetComponent<Image>().sprite){
                            tokenimage.transform.GetChild(0).gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    void TokenOptionSelect(Button btn){
        int tokenCount = 0;
        foreach(GameObject tokenPick in tokenPicks){
            if(btn.gameObject.GetComponent<Image>().sprite == tokenPick.GetComponent<Image>().sprite){
                tokenCount++;
            }
        }
        Debug.Log(tokenCount);
        if(tokenCount==0){
            captionImage.GetComponent<Image>().sprite = btn.gameObject.GetComponent<Image>().sprite;
            foreach(GameObject tokenimage in tokenSelectImages){
                tokenimage.transform.DOScale(0f, 0.3f).SetEase(Ease.OutBounce);
                tokenimage.transform.GetChild(0).gameObject.SetActive(false);
            }
            tokenClicked = false;
        }
    }

    void TypeSelect(){
        if(!tokenClicked && !colorClicked){
            if(typeClicked){
                playerTypeDropdown.transform.DOScale(0f, 0.3f).SetEase(Ease.OutBounce);
                typeClicked = false;
            }else{
                typeClicked = true;
                menuController.GetComponent<menuControl>().CloseAllMenus(this.gameObject);
                playerTypeDropdown.SetActive(true);
                playerTypeDropdown.transform.localScale = Vector3.zero;
                playerTypeDropdown.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce);
            }
        }
    }

    void TypeOptionSelect(Button btn){
        Debug.Log("clicked");
        playerTypeCaption.GetComponent<TMP_Text>().text = btn.gameObject.GetComponent<TMP_Text>().text;
        foreach(GameObject tokenimage in tokenSelectImages){
            playerTypeDropdown.transform.DOScale(0f, 0.3f).SetEase(Ease.OutBounce);
        }
        typeClicked = false;
    }

    void ColorSelect(){
        if(!tokenClicked && !typeClicked){
            if(colorClicked){
                foreach(GameObject colImage in colSelectImages){
                    colImage.transform.DOScale(0f, 0.3f).SetEase(Ease.OutBounce);
                    colImage.transform.GetChild(0).gameObject.SetActive(false);
                }
                colorClicked = false;
            }else{
                colorClicked = true;
                menuController.GetComponent<menuControl>().CloseAllMenus(this.gameObject);
                foreach(GameObject colImage in colSelectImages){
                    colImage.SetActive(true);
                    colImage.transform.localScale = Vector3.zero;
                    colImage.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce);
                    foreach(GameObject colPick in colorPicks){
                        if(colImage.GetComponent<Image>().sprite == colPick.GetComponent<Image>().sprite){
                            colImage.transform.GetChild(0).gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    void ColorOptionSelect(Button btn){
        int colCount = 0;
        foreach(GameObject colPick in colorPicks){
            if(btn.gameObject.GetComponent<Image>().sprite == colPick.GetComponent<Image>().sprite){
                colCount++;
            }
        }
        if(colCount == 0){
            colorImage.GetComponent<Image>().sprite = btn.gameObject.GetComponent<Image>().sprite;
            foreach(GameObject colorImage in colSelectImages){
                colorImage.transform.DOScale(0f, 0.3f).SetEase(Ease.OutBounce);
                colorImage.transform.GetChild(0).gameObject.SetActive(false);
            }
            colorClicked = false;
        }
    }

    public void CloseMenus(){
        foreach(GameObject colImage in colSelectImages){
            colImage.transform.DOScale(0f, 0.3f).SetEase(Ease.OutBounce);
            colImage.transform.GetChild(0).gameObject.SetActive(false);
        }
        playerTypeDropdown.transform.DOScale(0f, 0.3f).SetEase(Ease.OutBounce);
        foreach(GameObject tokenimage in tokenSelectImages){
            tokenimage.transform.DOScale(0f, 0.3f).SetEase(Ease.OutBounce);
            tokenimage.transform.GetChild(0).gameObject.SetActive(false);
        }
        typeClicked = false;
        colorClicked = false;
        tokenClicked = false;
    }

    public void ResetBannerVal(int index){
        tokenPicks[index].GetComponent<Image>().sprite = placeholderSprite;
        typePicks[index].GetComponent<TMP_Text>().text = "Human Player";
        nameObjects[index].GetComponentInParent<TMP_InputField>().text = "";
        colorPicks[index].GetComponent<Image>().sprite = placeholderSprite;
    }

    public void ResetAllValues(){
        for(int i=0; i<5; i++){
            tokenPicks[i].GetComponent<Image>().sprite = placeholderSprite;
            typePicks[i].GetComponent<TMP_Text>().text = "Human Player";
            nameObjects[i].GetComponentInParent<TMP_InputField>().text = "";
            colorPicks[i].GetComponent<Image>().sprite = placeholderSprite;
            playerBanners[i].SetActive(false);
        }
    }

    public void SendData(){
        int playerCount = 0;
        foreach(GameObject playerBanner in playerBanners){
            if(playerBanner.activeSelf){playerCount++;}
        }

        int emptyVals = 0;
        for(int i=0; i<playerCount; i++){
            if(tokenPicks[i].GetComponent<Image>().sprite == placeholderSprite){emptyVals++;}
            if(colorPicks[i].GetComponent<Image>().sprite == placeholderSprite){emptyVals++;}
            if(nameObjects[i].GetComponent<TMP_Text>().text.Length <= 1){emptyVals++;}
        }

        Debug.Log(emptyVals.ToString());
        if(emptyVals == 0){
            string playerDataJson = File.ReadAllText("Assets/playerData.json");
            dynamic playerData = Newtonsoft.Json.JsonConvert.DeserializeObject(playerDataJson);
            for(int i=0; i<5; i++){
                if(i>=playerCount){
                    playerData["data"][i]["token"] = -1;
                    playerData["data"][i]["type"] = "";
                    playerData["data"][i]["name"] = "";
                    playerData["data"][i]["color"] = -1;
                }else{
                    playerData["data"][i]["token"] = tokenPicks[i].GetComponent<Image>().sprite.name[^1].ToString();
                    playerData["data"][i]["type"] = typePicks[i].GetComponent<TMP_Text>().text;
                    playerData["data"][i]["name"] = nameObjects[i].GetComponent<TMP_Text>().text;
                    playerData["data"][i]["color"] = colorPicks[i].GetComponent<Image>().sprite.name[^1].ToString();
                }
            }
            string outputJson = Newtonsoft.Json.JsonConvert.SerializeObject(playerData, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("Assets/playerData.json", outputJson);
        }else{
            Debug.Log("Empty Values. Fill Out All Details.");
        }
    }
}