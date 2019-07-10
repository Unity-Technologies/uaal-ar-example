package com.unity.mynativeapp;

import android.content.Intent;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.ImageView;

public class MainActivity extends AppCompatActivity {
    static int CurrentSelectedItem; // 0 = mug, 1 = shirt.

    // Shop item color configurations. 0 = mug, 1 = shirt.
    static int[] ItemConfigs = new int[2];

    static int[] Colors = {Color.parseColor("#FFFFFF"),
            Color.parseColor("#EC1746"),
            Color.parseColor("#04BAD2"),
            Color.parseColor("#CADA29")};

    static String[] ColorsStrings = {"White", "Magenta", "Cyan", "Lime"};

    static int[] MugImages = {R.drawable.mugwhite,
            R.drawable.mugmagenta,
            R.drawable.mugcyan,
            R.drawable.muglime};

    static int[] ShirtImages = {R.drawable.shirtwhite,
            R.drawable.shirtmagenta,
            R.drawable.shirtcyan,
            R.drawable.shirtlime};

    static int NumberOfColors = 4;

    public static int GetNextColorForCurrentItem() {
        return Colors[(ItemConfigs[CurrentSelectedItem] + 1) % MainActivity.NumberOfColors];
    }

    public static String GetColorStringForCurrentItem() {
        return ColorsStrings[ItemConfigs[CurrentSelectedItem]];
    }

    public static void IncCurrentItemConfig() {
        ItemConfigs[CurrentSelectedItem] = (ItemConfigs[CurrentSelectedItem] + 1) % MainActivity.NumberOfColors;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_main);
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        // assign on click listeners to the AR Preview buttons
        View mugPreviewButton = findViewById(R.id.previewbutton0);
        mugPreviewButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                selectShopItem(0);
            }
        });

        View shirtPreviewButton = findViewById(R.id.previewbutton1);
        shirtPreviewButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                selectShopItem(1);
            }
        });

        // assign the correct display image for the shop items and assign on click listeners.
        ImageView mugColorButton = findViewById(R.id.mugColorChanger);
        mugColorButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                IncConfig(0);
            }
        });
        ImageView mugDisplay = findViewById(R.id.mugDisplay);
        mugDisplay.setImageResource(MugImages[ItemConfigs[0]]);
        UpdateColorButton(mugColorButton, ItemConfigs[0]);

        ImageView shirtColorButton = findViewById(R.id.shirtColorChanger);
        shirtColorButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                IncConfig(1);
            }
        });
        ImageView shirtDisplay = findViewById(R.id.shirtDisplay);
        shirtDisplay.setImageResource(ShirtImages[ItemConfigs[1]]);
        UpdateColorButton(shirtColorButton, ItemConfigs[1]);

        View unloadButton = findViewById(R.id.unloadButton);
        if (MainUnityActivity.instance == null) {
            unloadButton.getBackground().setColorFilter(Color.GRAY, PorterDuff.Mode.MULTIPLY);
            unloadButton.setEnabled(false);
        }
        else {
            unloadButton.getBackground().setColorFilter(null);
            unloadButton.setEnabled(true);
        }
    }

    public void unloadUnity(View view) {
        if(MainUnityActivity.instance != null)
        {
            MainUnityActivity.instance.finish();
        }
    }

    // Start the Unity Activity.
    private void selectShopItem(int i) {
        CurrentSelectedItem = i;
        Intent intent = new Intent(this, MainUnityActivity.class);
        startActivity(intent);
    }

    // Increments the configuration (color setting) for a given item. 0 = mug, 1 = shirt.
    private void IncConfig(int itemIndex) {
        int config = (ItemConfigs[itemIndex] + 1) % NumberOfColors;
        ItemConfigs[itemIndex] = config;

        ImageView v = findViewById(itemIndex == 0 ? R.id.mugDisplay : R.id.shirtDisplay);
        v.setImageResource(itemIndex == 0 ? MugImages[config] : ShirtImages[config]);

        // show next colour
        ImageView colorChangeButton = findViewById(itemIndex == 0 ? R.id.mugColorChanger : R.id.shirtColorChanger);
        UpdateColorButton(colorChangeButton, config);
    }

    private void UpdateColorButton(ImageView colorButton, int colorIndex) {
        int next = (colorIndex + 1) % 4;
        colorButton.setImageResource(next > 0 ? R.drawable.colour_button : R.drawable.colour_button_white);
        colorButton.setColorFilter(next > 0 ? Colors[next] : Color.TRANSPARENT);
    }
}
