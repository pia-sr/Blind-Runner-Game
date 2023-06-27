package AndroidAccessibility;


/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

import android.content.Context;
import android.os.VibrationEffect;
import android.os.Vibrator;

public class Haptics {

    private static Context context;
    private static Vibrator vibrator;

    //Function to set the context so it can connect to the Unity script
    public static void setContext(Context context) {
        Haptics.context = context;
    }

    //Function to start a phone vibration with a given amplitude for a given amount of time
    public static void vibrate(Context context, int milliseconds, int amplitude) {
        if(Haptics.context == null)
        {
            setContext(context);
        }
        vibrator = (Vibrator) context.getSystemService(Context.VIBRATOR_SERVICE);
        VibrationEffect effect = null;
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O) {
            effect = VibrationEffect.createWaveform(new long[]{0,milliseconds}, new int[]{0, amplitude}, -1);
            vibrator.vibrate(effect);
        }
    }


    //Function to start a phone vibration at the highest amplitude for a given amount of time
    public static void vibrateIntense(Context context, int milliseconds) {
        if(Haptics.context == null)
        {
            setContext(context);
        }
        vibrator = (Vibrator) context.getSystemService(Context.VIBRATOR_SERVICE);
        vibrator.vibrate(milliseconds);
    }

    //Function to stop the phone vibration
    public static void stopPulsing() {
        vibrator.cancel();
    }
}

