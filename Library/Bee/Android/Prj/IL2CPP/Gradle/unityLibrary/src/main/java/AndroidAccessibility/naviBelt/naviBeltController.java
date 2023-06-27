package AndroidAccessibility.naviBelt;



import android.bluetooth.BluetoothDevice;
import android.content.Context;
import android.util.Log;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import java.util.ArrayList;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.ScheduledThreadPoolExecutor;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicBoolean;

import de.feelspace.fslib.BeltBatteryStatus;
import de.feelspace.fslib.BeltButtonPressEvent;
import de.feelspace.fslib.BeltCommandInterface;
import de.feelspace.fslib.BeltCommandListener;
import de.feelspace.fslib.BeltConnectionInterface;
import de.feelspace.fslib.BeltConnectionListener;
import de.feelspace.fslib.BeltConnectionState;
import de.feelspace.fslib.BeltMode;
import de.feelspace.fslib.BeltOrientation;
import de.feelspace.fslib.BeltVibrationSignal;
import de.feelspace.fslib.NavigationEventListener;
import de.feelspace.fslib.NavigationState;
import de.feelspace.fslib.PowerStatus;

public class naviBeltController {

    // Debug
    @SuppressWarnings("unused")
    private static final String DEBUG_TAG = "FeelSpace-Debug";
    @SuppressWarnings("unused")
    private static final boolean DEBUG = true;
    private @NonNull BeltConnectionInterface beltConnection;


    // Belt controller
    private @NonNull BeltCommandInterface beltController;
    // Navigation signal
    private @Nullable BeltVibrationSignal navigationSignal;

    // State of the navigation
    private @NonNull NavigationState navigationState;

    // Flag that indicate that the current pause mode has been set by the application
    private boolean isPauseModeForNavigation = false;

    // Listeners
    private @NonNull ArrayList<NavigationEventListener> listeners = new ArrayList<>();


    // Flag for scheduled vibration command
    private @NonNull AtomicBoolean isVibrationCommandScheduled = new AtomicBoolean(false);

    // Executor for delayed vibration command
    private @NonNull ScheduledThreadPoolExecutor executor;
    // Minimum period between two vibration command
    private static final long MINIMUM_VIBRATION_COMMAND_UPDATE_PERIOD_NANO = 0;// 50*1000000;

    // Scheduled vibration command
    private @Nullable ScheduledFuture vibrationCommandTask;

    // Last vibration command send time
    private long lastVibrationCommandNanoTime = 0;
    private Integer[][] channels;


    public naviBeltController(
            Context applicationContext) throws NullPointerException {

        beltConnection = BeltConnectionInterface.create(applicationContext);
        beltController = beltConnection.getCommandInterface();
        BeltListener beltListener = new BeltListener();
        beltConnection.addConnectionListener(beltListener);
        beltController.addCommandListener(beltListener);
        this.navigationState = NavigationState.STOPPED;
        this.navigationSignal = BeltVibrationSignal.NAVIGATION;
        executor = new ScheduledThreadPoolExecutor(1);
        channels = new Integer[][] {{null, null}, {null, null}, {null, null}, {null, null}, {null, null}, {null, null}};
    }

    /**
     * Searches and connects a belt.
     */
    public void searchAndConnectBelt() {
        if (beltConnection.getState() != BeltConnectionState.STATE_DISCONNECTED) {
            return;
        }
        try {
            beltConnection.scanAndConnect();
        } catch (Exception e) {
            Log.e(DEBUG_TAG, "Connection failed", e);
            notifyBeltConnectionFailed();
        }
    }

    /**
     * Disconnects the belt.
     */
    public void disconnectBelt() {
        if (navigationState == NavigationState.PAUSED && isPauseModeForNavigation &&
                beltConnection.getState() == BeltConnectionState.STATE_CONNECTED) {
            // TODO Check if mode change with timeout is necessary, or delayed disconnection
            beltController.changeMode(BeltMode.WAIT);
        }
        beltConnection.stopScan();
        beltConnection.disconnect();
    }

    /**
     * Returns the connection state.
     *
     * @return The connection state.
     */
    public BeltConnectionState getConnectionState() {
        return beltConnection.getState();
    }

    /**
     * Returns the navigation state.
     *
     * @return The navigation state.
     */
    public NavigationState getNavigationState() {
        return navigationState;
    }

    /**
     * Returns the battery level of the belt in percent.
     *
     * @return the battery level of the belt in percent, or <code>null</code> if the level is unknown
     * or no belt is connected.
     */
    public Integer getBeltBatteryLevel() {
        BeltBatteryStatus batteryStatus = beltController.getBatteryStatus();
        if (batteryStatus == null) {
            return null;
        } else {
            return (int) batteryStatus.getLevel();
        }
    }

    /**
     * Returns the power status of the belt.
     *
     * @return the power status of the belt, or <code>null</code> if the power status is unknown
     * or no belt is connected.
     */
    public PowerStatus getBeltPowerStatus() {
        BeltBatteryStatus batteryStatus = beltController.getBatteryStatus();
        if (batteryStatus == null) {
            return null;
        } else {
            return batteryStatus.getPowerStatus();
        }
    }

    /**
     * Adds a listener for navigation events.
     *
     * @param listener The listener to add.
     */
    public void addNavigationEventListener(NavigationEventListener listener) {
        synchronized (this) {
            if (!listeners.contains(listener)) {
                listeners.add(listener);
            }
        }
    }

    public void startVibration (Integer intensity, Integer channelIndex) throws IllegalArgumentException {
        int position = 0;
        switch (channelIndex){
            case 0: case 3:
                position = 12;
                break;
            case 2: case 5:
                position = 4;
        }
        channels[channelIndex] [0] = position;
        channels[channelIndex] [1] = intensity;
        if (navigationState == NavigationState.NAVIGATING) {
            scheduleOrSendVibrationCommand(channelIndex);
            return;
        }
        navigationState = NavigationState.NAVIGATING;
        if (beltConnection.getState() == BeltConnectionState.STATE_CONNECTED) {
            if (beltController.getMode() == BeltMode.APP) {
                scheduleOrSendVibrationCommand(channelIndex);
            } else {
                beltController.changeMode(BeltMode.APP);
            }
        }
        notifyNavigationStateChanged();
    }

    public void startPulse(Integer intensity, int duration, int period, int continuous, Integer channelIndex ){
        int position = 0;
        switch (channelIndex){
            case 0: case 3:
                position = 12;
                break;
            case 2: case 5:
                position = 4;
        }
        channels[channelIndex] [0] = position;
        channels[channelIndex] [1] = intensity;
        if (navigationState == NavigationState.NAVIGATING) {
            scheduleOrSendVibrationCommand(duration, period, continuous, channelIndex);
            return;
        }
        navigationState = NavigationState.NAVIGATING;
        if (beltConnection.getState() == BeltConnectionState.STATE_CONNECTED) {
            if (beltController.getMode() == BeltMode.APP) {
                scheduleOrSendVibrationCommand(duration, period, continuous, channelIndex);
            } else {
                beltController.changeMode(BeltMode.APP);
            }
        }
        notifyNavigationStateChanged();
    }


    /**
     * Schedules or sends the vibration command for the navigation command according to the last
     * update time.
     *
     * This is used to avoid flooding the Bluetooth interface.
     */
    private void scheduleOrSendVibrationCommand(Integer channelIndex) {
        int[] position = null;
        Integer intensity = channels[channelIndex] [1];
        if (channels[channelIndex] [0] != null){
            position = new int[] { channels[channelIndex] [0]};
        }

        if (isVibrationCommandScheduled.compareAndSet(false, true)) {
            long currentTimeNano = System.nanoTime();


            if (lastVibrationCommandNanoTime+MINIMUM_VIBRATION_COMMAND_UPDATE_PERIOD_NANO <
                    currentTimeNano) {
                // Send command
                lastVibrationCommandNanoTime = System.nanoTime();
                sendVibrationCommand(beltConnection, position, intensity, navigationSignal, 0 , 0 ,0, channelIndex);
                isVibrationCommandScheduled.set(false);
            } else {
                // Schedule command
                try {
                    int[] finalPosition = position;
                    vibrationCommandTask = executor.schedule(new Runnable(){
                        @Override
                        public void run() {
                            sendVibrationCommand(beltConnection, finalPosition, intensity, navigationSignal, 0 , 0 ,0, channelIndex);
                            vibrationCommandTask = null;
                            isVibrationCommandScheduled.set(false);
                        }
                    }, MINIMUM_VIBRATION_COMMAND_UPDATE_PERIOD_NANO, TimeUnit.NANOSECONDS);
                } catch (Exception e) {
                    Log.e(DEBUG_TAG, "NavigationController: Unable to delay the " +
                            "vibration command.");
                    // Send command
                    lastVibrationCommandNanoTime = System.nanoTime();
                    sendVibrationCommand(beltConnection, position, intensity, navigationSignal, 0 , 0 ,0, channelIndex);
                    isVibrationCommandScheduled.set(false);
                }
            }
        } // Else: a vibration command is already scheduled
    }/**
     * Schedules or sends the vibration command for the navigation command according to the last
     * update time.
     *
     * This is used to avoid flooding the Bluetooth interface.
     */
    private void scheduleOrSendVibrationCommand(int duration, int period, int continuous, Integer channelIndex) {
        int[] position = null;
        Integer intensity = channels[channelIndex] [1];
        if (channels[channelIndex] [0] != null){
            position = new int[] { channels[channelIndex] [0]};
        }

        if (isVibrationCommandScheduled.compareAndSet(false, true)) {
            long currentTimeNano = System.nanoTime();
            if (lastVibrationCommandNanoTime+MINIMUM_VIBRATION_COMMAND_UPDATE_PERIOD_NANO <
                    currentTimeNano) {
                // Send command
                lastVibrationCommandNanoTime = System.nanoTime();
                sendVibrationCommand(beltConnection, position, intensity, navigationSignal, duration, period,continuous, channelIndex);
                isVibrationCommandScheduled.set(false);
            } else {
                // Schedule command
                try {
                    int[] finalPosition = position;
                    vibrationCommandTask = executor.schedule(new Runnable() {
                        @Override
                        public void run() {
                            sendVibrationCommand(beltConnection, finalPosition, intensity, navigationSignal, duration, period,continuous, channelIndex);
                            vibrationCommandTask = null;
                            isVibrationCommandScheduled.set(false);
                        }
                    }, MINIMUM_VIBRATION_COMMAND_UPDATE_PERIOD_NANO, TimeUnit.NANOSECONDS);
                } catch (Exception e) {
                    Log.e(DEBUG_TAG, "NavigationController: Unable to delay the " +
                            "vibration command.");
                    // Send command
                    lastVibrationCommandNanoTime = System.nanoTime();
                    sendVibrationCommand(beltConnection, position, intensity, navigationSignal, duration, period,continuous, channelIndex);
                    isVibrationCommandScheduled.set(false);
                }
            }
        } // Else: a vibration command is already scheduled*/
    }

    /**
     * Schedules or sends the vibration command for the navigation command according to the last
     * update time.
     *
     * This is used to avoid flooding the Bluetooth interface.
     */
    private void scheduleOrSendVibrationCommand(int[] position, Integer intensity, Integer channelIndex) {

        if (isVibrationCommandScheduled.compareAndSet(false, true)) {
            long currentTimeNano = System.nanoTime();
            if (lastVibrationCommandNanoTime+MINIMUM_VIBRATION_COMMAND_UPDATE_PERIOD_NANO <
                    currentTimeNano) {
                // Send command
                lastVibrationCommandNanoTime = System.nanoTime();
                sendVibrationCommand(beltConnection, position, intensity, navigationSignal, 0, 0, 0, channelIndex);
                isVibrationCommandScheduled.set(false);
            } else {
                // Schedule command
                try {
                    vibrationCommandTask = executor.schedule(new Runnable() {
                        @Override
                        public void run() {
                            sendVibrationCommand(beltConnection, position, intensity, navigationSignal, 0, 0, 0, channelIndex);
                            vibrationCommandTask = null;
                            isVibrationCommandScheduled.set(false);
                        }
                    }, MINIMUM_VIBRATION_COMMAND_UPDATE_PERIOD_NANO, TimeUnit.NANOSECONDS);
                } catch (Exception e) {
                    Log.e(DEBUG_TAG, "NavigationController: Unable to delay the " +
                            "vibration command.");
                    // Send command
                    lastVibrationCommandNanoTime = System.nanoTime();
                    sendVibrationCommand(beltConnection, position, intensity, navigationSignal, 0, 0, 0, channelIndex);
                    isVibrationCommandScheduled.set(false);
                }
            }
        } // Else: a vibration command is already scheduled*/
    }



    protected void sendVibrationCommand(BeltConnectionInterface connection, int[] position, Integer intensity, BeltVibrationSignal signal, int duration, int period, int continuous,  Integer channelIndex) {
        BeltCommandInterface controller = connection.getCommandInterface();
        if (connection.getState() != BeltConnectionState.STATE_CONNECTED ||
                controller.getMode() != BeltMode.APP) {
            return;
        }
        if (signal == null || !signal.isRepeated()) {
            // Stop the vibration
            controller.stopVibration(channelIndex);
        } else if( duration != 0){
            controller.pulseAtPositions(
                    position,
                    duration,
                    period,
                    continuous,
                    intensity,
                    channelIndex,
                    null
            );
        }
        else {
            controller.vibrateAtPositions(
                    position,
                    intensity,
                    signal,
                    channelIndex,
                    null);
        }
    }

    /**
     * Pauses the navigation and changes the mode of the belt to Pause if connected and in App mode.
     */
    public void pauseNavigation() {
        if (navigationState != NavigationState.NAVIGATING) {
            return;
        }
        navigationState = NavigationState.PAUSED;
        if (beltConnection.getState() == BeltConnectionState.STATE_CONNECTED &&
                beltController.getMode() == BeltMode.APP) {
            beltController.changeMode(BeltMode.PAUSE);
        }
        notifyNavigationStateChanged();
    }

    public void stopPulsation(Integer channelIndex){
        if (navigationState == NavigationState.NAVIGATING) {
            scheduleOrSendVibrationCommand(null, null, channelIndex);
            return;
        }
        navigationState = NavigationState.NAVIGATING;
        if (beltConnection.getState() == BeltConnectionState.STATE_CONNECTED) {
            if (beltController.getMode() == BeltMode.APP) {
                scheduleOrSendVibrationCommand(null, null, channelIndex);
            } else {
                beltController.changeMode(BeltMode.APP);
            }
        }
        notifyNavigationStateChanged();
    }
    public void continueSignal(){
        if (navigationState == NavigationState.NAVIGATING) {
            scheduleOrSendVibrationCommand(0);
            scheduleOrSendVibrationCommand(1);
            scheduleOrSendVibrationCommand(2);
            return;
        }
        navigationState = NavigationState.NAVIGATING;
        if (beltConnection.getState() == BeltConnectionState.STATE_CONNECTED) {
            if (beltController.getMode() == BeltMode.APP) {
                scheduleOrSendVibrationCommand(0);
                scheduleOrSendVibrationCommand(1);
                scheduleOrSendVibrationCommand(2);
            } else {
                beltController.changeMode(BeltMode.APP);
            }
        }
        notifyNavigationStateChanged();
    }

    /**
     * Stops the navigation and changes the mode of the belt to Wait if connected and in App mode.
     */
    public void stopNavigation() {
        if (navigationState == NavigationState.STOPPED) {
            return;
        }
        navigationState = NavigationState.STOPPED;
        if (beltConnection.getState() == BeltConnectionState.STATE_CONNECTED &&
                (beltController.getMode() == BeltMode.APP ||
                        (beltController.getMode() == BeltMode.PAUSE && isPauseModeForNavigation))) {
            beltController.changeMode(BeltMode.WAIT);
        }
        notifyNavigationStateChanged();
    }


    /**
     * Notifies listeners of a navigation state change.
     */
    private void notifyNavigationStateChanged() {
        ArrayList<NavigationEventListener> targets;
        NavigationState state = navigationState;
        synchronized (this) {
            if (listeners.isEmpty()) {
                return;
            }
            targets = new ArrayList<>(listeners);
        }
        for (NavigationEventListener l: targets) {
            l.onNavigationStateChanged(state);
        }
    }

    /**
     * Notifies listeners that the default vibration intensity has changed.
     *
     * @param intensity The default vibration intensity.
     */
    private void notifyBeltDefaultVibrationIntensityChanged(int intensity) {
        ArrayList<NavigationEventListener> targets;
        synchronized (this) {
            if (listeners.isEmpty()) {
                return;
            }
            targets = new ArrayList<>(listeners);
        }
        for (NavigationEventListener l: targets) {
            l.onBeltDefaultVibrationIntensityChanged(intensity);
        }
    }

    /**
     * Notifies listeners that the belt battery level has been updated.
     * @param batteryLevel The battery level.
     * @param status The power status.
     */
    private void notifyBeltBatteryLevelUpdated(int batteryLevel, PowerStatus status) {
        ArrayList<NavigationEventListener> targets;
        synchronized (this) {
            if (listeners.isEmpty()) {
                return;
            }
            targets = new ArrayList<>(listeners);
        }
        for (NavigationEventListener l: targets) {
            l.onBeltBatteryLevelUpdated(batteryLevel, status);
        }
    }


    /**
     * Notifies listeners that the connection failed.
     */
    private void notifyBeltConnectionFailed() {
        ArrayList<NavigationEventListener> targets;
        synchronized (this) {
            if (listeners.isEmpty()) {
                return;
            }
            targets = new ArrayList<>(listeners);
        }
        for (NavigationEventListener l: targets) {
            l.onBeltConnectionFailed();
        }
    }

    /**
     * Notifies listeners that no belt has been found.
     */
    private void notifyNoBeltFound() {
        ArrayList<NavigationEventListener> targets;
        synchronized (this) {
            if (listeners.isEmpty()) {
                return;
            }
            targets = new ArrayList<>(listeners);
        }
        for (NavigationEventListener l: targets) {
            l.onNoBeltFound();
        }
    }

    /**
     * Notifies listeners that the connection has been lost.
     */
    private void notifyBeltConnectionLost() {
        ArrayList<NavigationEventListener> targets;
        synchronized (this) {
            if (listeners.isEmpty()) {
                return;
            }
            targets = new ArrayList<>(listeners);
        }
        for (NavigationEventListener l: targets) {
            l.onBeltConnectionLost();
        }
    }

    /**
     * Notifies listeners that the connection state has changed,
     * @param state The connection state.
     */
    private void notifyBeltConnectionStateChanged(BeltConnectionState state) {
        ArrayList<NavigationEventListener> targets;
        synchronized (this) {
            if (listeners.isEmpty()) {
                return;
            }
            targets = new ArrayList<>(listeners);
        }
        for (NavigationEventListener l: targets) {
            l.onBeltConnectionStateChanged(state);
        }
    }

    /**
     * Connection listener and command listener for the navigation controller.
     */
    class BeltListener implements BeltConnectionListener, BeltCommandListener {

        @Override
        public void onBeltModeChanged(BeltMode mode) {
            isPauseModeForNavigation = false;
            switch (mode) {
                case STANDBY:
                    // Nothing to do, the belt has been switched-off
                    break;
                case WAIT:
                    // The navigation should be in stop state
                    if (navigationState != NavigationState.STOPPED) {
                        Log.w(DEBUG_TAG, "NavigationController: Navigation state and " +
                                "belt mode out of sync.");
                    }
                    break;
                case APP:
                    // The navigation have been started
                    if (navigationState != NavigationState.NAVIGATING) {
                        Log.w(DEBUG_TAG, "NavigationController: Navigation state and " +
                                "belt mode out of sync.");
                        if (navigationState == NavigationState.STOPPED) {
                            beltController.changeMode(BeltMode.WAIT);
                        } else if (navigationState == NavigationState.PAUSED) {
                            beltController.changeMode(BeltMode.PAUSE);
                        }
                    } else {
                    }
                    break;
                case PAUSE:
                    // The navigation has been paused
                    if (navigationState != NavigationState.PAUSED) {
                        pauseNavigation();
                    } else {
                        isPauseModeForNavigation = true;
                    }
                    break;
                case COMPASS:
                case CALIBRATION:
                case CROSSING:
                    // The navigation should be in pause or stop state
                    if (navigationState == NavigationState.NAVIGATING) {
                        Log.w(DEBUG_TAG, "NavigationController: Navigation state and " +
                                "belt mode out of sync.");
                    }
                    break;
                case UNKNOWN:
                    // Nothing to do
                    break;
            }
        }


        @Override
        public void onBeltButtonPressed(BeltButtonPressEvent beltButtonPressEvent) {

        }

        @Override
        public void onBeltDefaultVibrationIntensityChanged(int intensity) {
            notifyBeltDefaultVibrationIntensityChanged(intensity);
        }


        @Override
        public void onBeltBatteryStatusUpdated(BeltBatteryStatus status) {
            notifyBeltBatteryLevelUpdated((int) status.getLevel(), status.getPowerStatus());
        }

        @Override
        public void onBeltOrientationUpdated(BeltOrientation orientation) {
        }

        @Override
        public void onBeltCompassAccuracySignalStateNotified(boolean signalEnabled) {
        }

        @Override
        public void onScanFailed() {
            notifyBeltConnectionFailed();
        }

        @Override
        public void onNoBeltFound() {
            notifyNoBeltFound();
        }

        @Override
        public void onBeltFound(BluetoothDevice belt) {

        }


        @Override
        public void onConnectionStateChange(BeltConnectionState state) {
            isPauseModeForNavigation = false;
            if (state == BeltConnectionState.STATE_CONNECTED) {
                beltController.setOrientationNotificationsActive(true);
                beltController.requestCompassAccuracySignalState();
                if (navigationState == NavigationState.NAVIGATING) {
                    if (beltController.getMode() == BeltMode.APP) {
                        continueSignal();
                    } else {
                        beltController.changeMode(BeltMode.APP);
                    }
                }
            }
            notifyBeltConnectionStateChanged(state);
        }

        @Override
        public void onConnectionLost() {
            notifyBeltConnectionLost();
        }

        @Override
        public void onConnectionFailed() {
            notifyBeltConnectionFailed();
        }
    }
}
