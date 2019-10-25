package com.unity.uaal;

import android.content.Intent;
import android.graphics.Color;
import android.graphics.PorterDuff;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.View;
import android.widget.ImageView;
import java.util.Arrays;

public class MainActivity extends AppCompatActivity {
    static int CurrentSelectedItem; // 0 = mug, 1 = shirt.
    boolean isUnityLoaded = false;
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

    public static int getNextColorForCurrentItem() {
        return (Colors[(ItemConfigs[CurrentSelectedItem] + 1) % NumberOfColors]);
    }

    public static String getColorStringForCurrentItem() {
        return ColorsStrings[ItemConfigs[CurrentSelectedItem]];
    }

    public static void incCurrentItemConfig() {
        ItemConfigs[CurrentSelectedItem] = (ItemConfigs[CurrentSelectedItem] + 1) % NumberOfColors;
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
                incConfig(0);
            }
        });
        ImageView mugDisplay = findViewById(R.id.mugDisplay);
        mugDisplay.setImageResource(MugImages[ItemConfigs[0]]);
        updateColorButton(mugColorButton, ItemConfigs[0]);

        ImageView shirtColorButton = findViewById(R.id.shirtColorChanger);
        shirtColorButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                incConfig(1);
            }
        });
        ImageView shirtDisplay = findViewById(R.id.shirtDisplay);
        shirtDisplay.setImageResource(ShirtImages[ItemConfigs[1]]);
        updateColorButton(shirtColorButton, ItemConfigs[1]);

        View unloadButton = findViewById(R.id.unloadButton);
        if (MainUnityActivity.instance == null) {
            unloadButton.getBackground().setColorFilter(Color.GRAY, PorterDuff.Mode.MULTIPLY);
            unloadButton.setEnabled(false);
        }
        else {
            unloadButton.getBackground().setColorFilter(null);
            unloadButton.setEnabled(true);
        }

        Intent intent = getIntent();
        handleIntent(intent);
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        handleIntent(intent);
        setIntent(intent);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if(requestCode == 1) {
            isUnityLoaded = false;
            updateUnloadButton();
        }
    }

    void handleIntent(Intent intent) {

        if(intent == null || intent.getExtras() == null) return;

        if(intent.getExtras().containsKey("showMain")) {
            updateUnloadButton();
        }
        if(intent.getExtras().containsKey("productColor")) {
            String currentColorString = intent.getStringExtra("productColor");
            int colorIndex = Arrays.asList(ColorsStrings).indexOf(currentColorString);
            ItemConfigs[CurrentSelectedItem] = colorIndex;
            updateColors();
        }
    }

    // Start the Unity Activity.
    private void selectShopItem(int i) {
        CurrentSelectedItem = i;
        isUnityLoaded = true;
        Intent intent = new Intent(this, MainUnityActivity.class);
        intent.putExtra("product",CurrentSelectedItem+"");
        intent.putExtra("productColor", getColorStringForCurrentItem());
        intent.putExtra("nextProductColor",getNextColorForCurrentItem()+"");
        intent.setFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
        startActivityForResult(intent,1);
    }

    public void unloadUnity(View view) {
        if(isUnityLoaded) {
            Intent intent = new Intent(this, MainUnityActivity.class);
            intent.setFlags(Intent.FLAG_ACTIVITY_REORDER_TO_FRONT);
            intent.putExtra("unload", true);
            startActivity(intent);
            isUnityLoaded = false;
        }
    }

    // Increments the configuration (color setting) for a given item. 0 = mug, 1 = shirt.
    public void incConfig(int itemIndex) {
        int config = (ItemConfigs[itemIndex] + 1) % NumberOfColors;
        ItemConfigs[itemIndex] = config;

        ImageView v = findViewById(itemIndex == 0 ? R.id.mugDisplay : R.id.shirtDisplay);
        v.setImageResource(itemIndex == 0 ? MugImages[config] : ShirtImages[config]);

        ImageView colorChangeButton = findViewById(itemIndex == 0 ? R.id.mugColorChanger : R.id.shirtColorChanger);
        updateColorButton(colorChangeButton, config);
    }

    // Update the product image and color change button, called when returning from the UnityActivity
    public void updateColors() {
        ImageView v = findViewById(CurrentSelectedItem == 0 ? R.id.mugDisplay : R.id.shirtDisplay);
        v.setImageResource(CurrentSelectedItem == 0 ? MugImages[ItemConfigs[CurrentSelectedItem]] : ShirtImages[ItemConfigs[CurrentSelectedItem]]);

        ImageView colorChangeButton = findViewById(CurrentSelectedItem == 0 ? R.id.mugColorChanger : R.id.shirtColorChanger);
        updateColorButton(colorChangeButton, ItemConfigs[CurrentSelectedItem]);
    }

    // Color change buttons onClick
    private void updateColorButton(ImageView colorButton, int colorIndex) {
        int colorChangeButtonColorIndex = (colorIndex + 1) % NumberOfColors;
        colorButton.setImageResource(colorChangeButtonColorIndex > 0 ? R.drawable.colour_button : R.drawable.colour_button_white);
        colorButton.setColorFilter(colorChangeButtonColorIndex > 0 ? Colors[colorChangeButtonColorIndex] : Color.TRANSPARENT);
    }

    private void updateUnloadButton() {
        View unloadButton = findViewById(R.id.unloadButton);
        if (isUnityLoaded) {
            unloadButton.getBackground().setColorFilter(null);
            unloadButton.setEnabled(true);
        }
        else {
            unloadButton.getBackground().setColorFilter(Color.GRAY, PorterDuff.Mode.MULTIPLY);
            unloadButton.setEnabled(false);
        }
    }

}
