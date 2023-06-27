package AndroidAccessibility;

import android.content.Context;
import android.os.VibrationEffect;
import android.os.Vibrator;

public class Haptics {

    private static Context context;
    private static int duration = 500;
    private static int amplitude = 0;
    private static Vibrator vibrator;

    public static void setContext(Context context) {
        Haptics.context = context;
    }

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


    public static void vibrateIntense(Context context, int milliseconds) {
        if(Haptics.context == null)
        {
            setContext(context);
        }
        vibrator = (Vibrator) context.getSystemService(Context.VIBRATOR_SERVICE);
        vibrator.vibrate(milliseconds);
    }

    public static void stopPulsing() {
        // Stop the pulsing
        vibrator.cancel();
    }
}

