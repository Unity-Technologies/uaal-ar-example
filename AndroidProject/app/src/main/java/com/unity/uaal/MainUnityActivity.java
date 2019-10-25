package com.unity.uaal;

import android.content.Intent;
import android.graphics.Color;
import android.graphics.Point;
import android.os.Bundle;
import android.view.Display;
import android.view.View;
import android.widget.ImageButton;
import android.widget.ImageView;
import com.company.product.OverrideUnityActivity;

public class MainUnityActivity extends OverrideUnityActivity {

    ImageButton colorButton;
    String currentProduct;
    String currentProductColor;
    int nextProductColor;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        addControlsToUnityFrame();
        Intent intent = getIntent();
        handleIntent(intent);
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        handleIntent(intent);
        setIntent(intent);
    }

    void handleIntent(Intent intent) {
        if(intent == null || intent.getExtras() == null) return;

        if(intent.getExtras().containsKey("product") && intent.getExtras().containsKey("productColor")
                && intent.getExtras().containsKey("nextProductColor")) {

            currentProduct = intent.getStringExtra("product");
            currentProductColor = intent.getStringExtra("productColor");
            nextProductColor = Integer.parseInt(intent.getStringExtra("nextProductColor"));
        }

        if(intent.getExtras().containsKey("unload")) {
            if(mUnityPlayer != null) {
                mUnityPlayer.unload();
                finish();
            }
        }
    }

    // Send messages to Unity to update the AR item.
    @Override
    protected void updateUnityShopItem() {
        mUnityPlayer.UnitySendMessage("AR Session Origin", "SetProduct", currentProduct);
        mUnityPlayer.UnitySendMessage("AR Session Origin", "SetColor",currentProductColor);
    }

    // Called from the Unity Activity. Makes the color change button visible.
    @Override
    protected void itemPlacedInAR() {
        MainUnityActivity.this.runOnUiThread(new Runnable() {
            public void run() {
                colorButton.setColorFilter((nextProductColor));
                colorButton.setVisibility(View.VISIBLE);
                updateUnityShopItem();
            }
        });
    }

    public void addControlsToUnityFrame() {
        Display display = getWindowManager().getDefaultDisplay();
        Point point = new Point();
        display.getSize(point);
        int size = (point.x / 2);

        ImageButton backButton = new ImageButton(this);
        backButton.setImageResource(R.drawable.button_back);
        backButton.setBackgroundColor(Color.TRANSPARENT);
        backButton.setScaleType(ImageView.ScaleType.FIT_START);
        backButton.setX(10);

        backButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                showMainActivity();

                colorButton.setVisibility(View.GONE);
            }
        });
        mUnityPlayer.addView(backButton, size, size / 2);

        colorButton = new ImageButton(this);
        colorButton.setImageResource(R.drawable.button_colour);
        colorButton.setBackgroundColor(Color.TRANSPARENT);
        colorButton.setScaleType(ImageView.ScaleType.FIT_START);
        colorButton.setX(point.x - size * 0.8f);
        colorButton.setColorFilter(MainActivity.getNextColorForCurrentItem());
        colorButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                onColorButtonPressed();
            }
        });
        colorButton.setVisibility(View.GONE);

        mUnityPlayer.addView(colorButton, size, size / 2);
    }

    @Override public void onUnityPlayerUnloaded() {
        showMainActivity();
    }

    // Return to the MainActivity. Sends a message to the Unity Activity to clear the placed AR item.
    private void showMainActivity() {
        mUnityPlayer.UnitySendMessage("AR Session Origin", "ClearPlacedItem", "");
        Intent intent = new Intent(this, MainActivity.class);
        intent.putExtra("showMain",true);
        intent.putExtra("productColor",currentProductColor);
        intent.setFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT | Intent.FLAG_ACTIVITY_SINGLE_TOP);
        startActivity(intent);
    }

    private void onColorButtonPressed() {
        MainActivity.incCurrentItemConfig();
        currentProductColor = MainActivity.getColorStringForCurrentItem();
        colorButton.setColorFilter(MainActivity.getNextColorForCurrentItem());
        updateUnityShopItem();
    }
}
