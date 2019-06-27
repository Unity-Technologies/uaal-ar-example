package com.unity.mynativeapp;

import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.ImageView;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity
{
    static int CurrentSelectedItem;
    static int[] ItemConfigs = new int[2];

    static int[] Colors = {Color.parseColor("#FFFFFF"),
                           Color.parseColor("#EC1746"),
                           Color.parseColor("#04BAD2"),
                           Color.parseColor("#CADA29")};

    static int[] MugImages = {R.drawable.mugwhite,
                              R.drawable.mugmagenta,
                              R.drawable.mugcyan,
                              R.drawable.muglime};

    static int[] ShirtImages = {R.drawable.shirtwhite,
                                R.drawable.shirtmagenta,
                                R.drawable.shirtcyan,
                                R.drawable.shirtlime};

    static String[] ColorsStrings = {"White", "Magenta", "Cyan", "Lime"};

    static int NumberOfColors = 4;

    public static int GetNextColorForCurrent()
    {
        return Colors[(ItemConfigs[CurrentSelectedItem] + 1) % MainActivity.NumberOfColors];
    }

    public static String GetColorStringForCurrent()
    {
        return ColorsStrings[ItemConfigs[CurrentSelectedItem]];
    }

    public static void IncCurrentConfig()
    {
        ItemConfigs[CurrentSelectedItem] = (ItemConfigs[CurrentSelectedItem] + 1) % MainActivity.NumberOfColors;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_main);
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        View v0 = findViewById(R.id.previewbutton0);
        v0.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                selectShopItem(0);
            }
        });

        View v1 = findViewById(R.id.previewbutton1);
        v1.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                selectShopItem(1);
            }
        });

        View imageView0 = findViewById(R.id.mugColorChanger);
        imageView0.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                IncConfig(0);
            }
        });
        ImageView mugDisplay = findViewById(R.id.mugDisplay);
        mugDisplay.setImageResource(MugImages[ItemConfigs[0]]);
        ImageView mugColorChangeButton = findViewById(R.id.mugColorChanger);
        UpdateColorButton(mugColorChangeButton, ItemConfigs[0] );

        View imageView1 = findViewById(R.id.shirtColorChanger);
        imageView1.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                IncConfig(1);
            }
        });
        ImageView shirtDisplay = findViewById(R.id.shirtDisplay);
        shirtDisplay.setImageResource(ShirtImages[ItemConfigs[1]]);
        ImageView shirtColorChangeButton = findViewById(R.id.shirtColorChanger);
        UpdateColorButton(shirtColorChangeButton, ItemConfigs[1]);
    }

    private void selectShopItem(int i)
    {
        CurrentSelectedItem = i;
        Intent intent = new Intent(this, MainUnityActivity.class);
        startActivity(intent);
    }

    private void IncConfig(int i)
    {
        int config = (ItemConfigs[i] + 1) % NumberOfColors;
        ItemConfigs[i] = config;

        ImageView v = findViewById(i == 0 ? R.id.mugDisplay : R.id.shirtDisplay);
        v.setImageResource(i == 0 ? MugImages[config] : ShirtImages[config]);

        // show next colour
        ImageView colorChangeButton = findViewById(i == 0 ? R.id.mugColorChanger : R.id.shirtColorChanger);
        UpdateColorButton(colorChangeButton, config);
    }

    private void UpdateColorButton(ImageView colorButton, int colorIndex)
    {
        int next = (colorIndex + 1) % 4;
        colorButton.setImageResource(next > 0 ? R.drawable.colour_button : R.drawable.colour_button_white);
        colorButton.setColorFilter(next > 0 ? Colors[next] : Color.TRANSPARENT);
        MainUnityActivity.UpdateColorButton();
    }

    public void onUnityUnload(View v) {
        if(MainUnityActivity.instance != null)
            MainUnityActivity.instance.finish();
        else showToast("Show Unity First");
    }

    public void showToast(String message) {
        CharSequence text = message;
        int duration = Toast.LENGTH_SHORT;
        Toast toast = Toast.makeText(getApplicationContext(), text, duration);
        toast.show();
    }
}
