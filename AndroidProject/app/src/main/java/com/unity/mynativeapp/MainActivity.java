package com.unity.mynativeapp;

import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity
{
    private int[] itemConfigs  = new int[2];

    public static int[] Colors = {Color.parseColor("#FFFFFF"),
                                Color.parseColor("#EC1746"),
                                Color.parseColor("#04BAD2"),
                                Color.parseColor("#CADA29")};

    public static String[] ColorsStrings = {"White", "Magenta", "Cyan", "Lime"};

    public static int NumberOfColors = 4;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        setContentView(R.layout.activity_main);
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        View v = findViewById(R.id.previewbutton0);
        v.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                selectShopItem(0);
            }
        });

        View v2 = findViewById(R.id.previewbutton1);
        v2.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                selectShopItem(1);
            }
        });

        View imageView0 = findViewById(R.id.imageView0);
        imageView0.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                IncConfig(0);
            }
        });

        View imageView1 = findViewById(R.id.imageView1);
        imageView1.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                IncConfig(1);
            }
        });
    }

    private void selectShopItem(int i)
    {
        MainUnityActivity.CurrentSelectedItem = i;
        MainUnityActivity.CurrentSelectedItemConfig = itemConfigs[i];
        Intent intent = new Intent(this, MainUnityActivity.class);
        startActivity(intent);
    }

    private void IncConfig(int i)
    {
        itemConfigs[i] = (itemConfigs[i] + 1) % NumberOfColors;
        View v = findViewById(i == 0 ? R.id.imageView0 : R.id.imageView1);
        v.setBackgroundColor(Colors[itemConfigs[i]]);
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
