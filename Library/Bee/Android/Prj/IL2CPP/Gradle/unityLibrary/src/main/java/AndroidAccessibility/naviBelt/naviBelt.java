package AndroidAccessibility.naviBelt;

import android.Manifest;
import android.app.Activity;
import android.app.Application;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.content.Context;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Handler;

import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import androidx.appcompat.app.AppCompatActivity;
import androidx.fragment.app.FragmentActivity;
import androidx.fragment.app.FragmentManager;

import com.unity3d.player.UnityPlayer;

import de.feelspace.fslib.BeltConnectionState;
import de.feelspace.fslib.NavigationController;
import de.feelspace.fslib.NavigationEventListener;
import de.feelspace.fslib.NavigationState;
import de.feelspace.fslib.PowerStatus;

public class naviBelt implements OnBluetoothActivationCallback, NavigationEventListener {

    private Context context;
    private naviBeltController navigationController;

    private BluetoothAdapter bluetoothAdapter;
    private String deviceName;

    private String connectionStatus;
    private String beltStatus;
    public naviBelt(Context context) {
        this.context = context;
        this.deviceName = "naviGuertel";
        bluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
        navigationController = new naviBeltController(context);
        navigationController.addNavigationEventListener(this);
    }

    public void connectBelt() {
        if(checkBluetoothPermission()){
            navigationController.searchAndConnectBelt();
        }
        else{
            requestBluetoothPermission();
            return;
        }

    }

    public void disconnectBelt() {
        navigationController.disconnectBelt();
    }
    private boolean checkBluetoothPermission() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            int check = ContextCompat.checkSelfPermission(this.context, Manifest.permission.BLUETOOTH);
            int checkConnect = ContextCompat.checkSelfPermission(this.context, Manifest.permission.BLUETOOTH_CONNECT);
            int checkScan = ContextCompat.checkSelfPermission(this.context, Manifest.permission.BLUETOOTH_SCAN);
            int checkLocation = ContextCompat.checkSelfPermission(this.context, Manifest.permission.ACCESS_FINE_LOCATION);
            return (check == PackageManager.PERMISSION_GRANTED && checkConnect == PackageManager.PERMISSION_GRANTED && checkScan == PackageManager.PERMISSION_GRANTED  && checkLocation == PackageManager.PERMISSION_GRANTED);
        }
        else {
            return true;
        }
    }
    public void startVibration(int intensity, int channelIndex){
        navigationController.startVibration(intensity, channelIndex);
    }

    public void stopVibration(int channelIndex){
        navigationController.stopPulsation(channelIndex);
    }
    public void startPulse(int intensity, int duration, int period, int continuous, int channelIndex){
        navigationController.startPulse(intensity, duration, period, continuous, channelIndex);
    }

    public void stopAll(){
        navigationController.stopPulsation(0);
        navigationController.stopPulsation(1);
        navigationController.stopPulsation(2);
        navigationController.stopPulsation(3);
        navigationController.stopPulsation(4);
        navigationController.stopPulsation(5);
    }

    public void unpause(){
        navigationController.continueSignal();
    }

    private static final int REQUEST_BLUETOOTH_PERMISSION = 1;

    private void requestBluetoothPermission() {
        ActivityCompat.requestPermissions(UnityPlayer.currentActivity, new String[]{Manifest.permission.BLUETOOTH, Manifest.permission.BLUETOOTH_CONNECT, Manifest.permission.BLUETOOTH_SCAN, Manifest.permission.ACCESS_FINE_LOCATION}, REQUEST_BLUETOOTH_PERMISSION);
    }



    public String connectionStatus(){
        return navigationController.getConnectionState().toString();
    }


    @Override
    public void onBluetoothActivated() {

    }

    @Override
    public void onBluetoothActivationRejected() {

    }

    @Override
    public void onBluetoothActivationFailed() {

    }

    @Override
    public void onNavigationStateChanged(NavigationState state) {

    }

    @Override
    public void onBeltHomeButtonPressed(boolean navigating) {

    }

    @Override
    public void onBeltDefaultVibrationIntensityChanged(int intensity) {

    }

    @Override
    public void onBeltOrientationUpdated(int beltHeading, boolean accurate) {

    }

    @Override
    public void onBeltBatteryLevelUpdated(int batteryLevel, PowerStatus status) {

    }

    @Override
    public void onCompassAccuracySignalStateUpdated(boolean enabled) {

    }

    @Override
    public void onBeltConnectionStateChanged(BeltConnectionState state) {

    }

    @Override
    public void onBeltConnectionLost() {

    }

    @Override
    public void onBeltConnectionFailed() {

    }

    @Override
    public void onNoBeltFound() {

    }
}
