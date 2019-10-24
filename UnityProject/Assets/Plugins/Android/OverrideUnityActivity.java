package com.company.product;
import android.os.Bundle;
import android.widget.FrameLayout;

import com.unity3d.player.UnityPlayerActivity;

public abstract class OverrideUnityActivity extends UnityPlayerActivity
{ 
  public static OverrideUnityActivity instance = null;

  abstract protected void updateUnityShopItem();
  abstract protected void itemPlacedInAR();
  
  @Override
  protected void onCreate(Bundle savedInstanceState)
  {
      super.onCreate(savedInstanceState);
      instance = this;
  }

  @Override
  protected void onDestroy() {
      super.onDestroy();
      instance = null;
  }
}
