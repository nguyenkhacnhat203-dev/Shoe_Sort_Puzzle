using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Popup_Fail : PopupBase
{
   public Button btnReplay;
   public Button btnHome;
   public TextMeshProUGUI txtCoin, txtHeart;

   protected override void OnEnable()
   {
      btnReplay.onClick.AddListener(this.OnPlay);
      btnHome.onClick.AddListener(this.ReturnHome);
      txtCoin.text = ResourceManager.Instance.GetCoin().ToString();
      txtHeart.text = ResourceManager.Instance.GetHeart().ToString();
   }


   protected override void OnDisable()
   {
      btnReplay.onClick.RemoveAllListeners();
      btnHome.onClick.RemoveAllListeners();
   }

   public override void ReturnHome()
   {
      AudioManager.Instance.BtnClick();
      UiManager.Instance.Return_Home();
      GameManager.Instance.ChangeState(GameState.OnMenu);
      Destroy(this.gameObject);
   }

   protected override void Start()
   {

   }


}