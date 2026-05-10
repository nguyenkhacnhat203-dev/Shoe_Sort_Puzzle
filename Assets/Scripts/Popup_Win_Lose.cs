using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Popup_Win_Lose : PopupBase
{
   public Button btnPlay;
   public Button btnHome;

   protected override void OnEnable()
   {
      btnPlay.onClick.AddListener(this.OnPlay);
      btnHome.onClick.AddListener(this.ReturnHome);
   }


   protected override void OnDisable()
   {
      btnPlay.onClick.RemoveAllListeners();
      btnHome.onClick.RemoveAllListeners();
   }

   protected override void Start()
   {
      
   }
   

}