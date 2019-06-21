package com.unity.mynativeapp;

import android.content.Intent;
import android.graphics.Point;
import android.os.Bundle;
import android.view.Display;
import android.view.View;
import android.widget.Button;

import com.company.product.OverrideUnityActivity;

public class MainUnityActivity extends OverrideUnityActivity {

    public static int CurrentSelectedItem;
    public static int CurrentSelectedItemConfig;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        addControlsToUnityFrame();
    }

    @Override
    protected void showMainActivity(String string) {
        Intent intent = new Intent(this, MainActivity.class);
        startActivity(intent);
    }

    public void addControlsToUnityFrame() {
            Button backButton = new Button(this);
            backButton.setText("Back");
            backButton.setX(10);
            backButton.setY(10);

            backButton.setOnClickListener(new View.OnClickListener() {
                public void onClick(View v) {
                    showMainActivity("");
                }
            });
            getUnityFrameLayout().addView(backButton, 300, 200);

            Display display = getWindowManager(). getDefaultDisplay();
            Point point = new Point();
            display.getSize(point);

            Button colorButton = new Button(this);
            colorButton.setText("Color");
            colorButton.setX(point.x - 310);
            colorButton.setY(10);
            colorButton.setOnClickListener(new View.OnClickListener() {
                public void onClick(View v) {
                    CurrentSelectedItemConfig = (CurrentSelectedItemConfig + 1) % MainActivity.NumberOfColors;
                    UpdateUnityShopItem();
                }
            });
            getUnityFrameLayout().addView(colorButton, 300, 200);
    }

    private void UpdateUnityShopItem() {
        UnitySendMessage("AR Session Origin", "SetProduct", Integer.toString(CurrentSelectedItem ));
        UnitySendMessage("AR Session Origin", "SetColor", MainActivity.ColorsStrings[CurrentSelectedItemConfig]);
    }
}
