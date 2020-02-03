package com.bose.DataCrea;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.appwidget.AppWidgetManager;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Build;
import android.os.IBinder;
import android.widget.RemoteViews;

import androidx.core.app.NotificationCompat;
import androidx.core.app.NotificationManagerCompat;

import java.text.DateFormat;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;

//foreground service that always runs, and has a notification, and counts steps
public class ForegroundStepService extends Service implements SensorEventListener, StepListener {
    private StepDetector simpleStepDetector;
    private SensorManager sensorManager;
    private Sensor accel;
    private static final String TEXT_NUM_STEPS = "Number of Steps: ";
    private static int numSteps=0;
    public static boolean started=false;
    public static final String CHANNEL_ID = "ForegroundServiceChannel";
    public static final String CHANNEL_ID2 = "DataCreaVictoryPoints";
    @Override
    public void onCreate() {
        super.onCreate();
    }
    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        String input = intent.getStringExtra("inputExtra");
        createNotificationChannel();
        Intent notificationIntent = new Intent(this, UnityPlayerActivity.class);
        PendingIntent pendingIntent = PendingIntent.getActivity(this,
                0, notificationIntent, 0);
        Notification notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                .setContentTitle("DataCrea Service")
                .setContentText(input)
                .setSmallIcon(R.drawable.logo1)
                .setContentIntent(pendingIntent)
                .build();
        startForeground(1, notification);
        //do heavy work on a background thread
        SharedPreferences prefs = this.getApplicationContext().getSharedPreferences(this.getApplicationContext().getPackageName()+ ".v2.playerprefs", Context.MODE_PRIVATE);
        numSteps=prefs.getInt("Steps", 0);
        String date=prefs.getString("Date", "none");
        assert date != null;
        Date now= Calendar.getInstance().getTime();
        try {
            Date date1=new SimpleDateFormat("dd/MM/yyyy").parse(date);

            if(date.equalsIgnoreCase("none")||!isSameDay(now,date1)){

                DateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy");
                String strDate = dateFormat.format(now);
                prefs.edit().putString("Date",strDate).commit();
                numSteps=0;
                prefs.edit().putInt("Steps",0).commit();
            }
        } catch (ParseException e) {
            e.printStackTrace();
        }




        if(!started){
            started=true;
            // Get an instance of the SensorManager
            sensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
            accel = sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);
            simpleStepDetector = new StepDetector();
            simpleStepDetector.registerListener(this);
            sensorManager.registerListener(ForegroundStepService.this, accel, SensorManager.SENSOR_DELAY_FASTEST);

        }
        //stopSelf();
        return START_NOT_STICKY;
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        try{
            sensorManager.unregisterListener(ForegroundStepService.this);
        }catch(Exception e){}
    }

    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }
    private void createNotificationChannel() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            NotificationChannel serviceChannel = new NotificationChannel(
                    CHANNEL_ID,
                    "DataCrea" +"Service Channel1",
                    NotificationManager.IMPORTANCE_DEFAULT
            );

            NotificationChannel serviceChannel2 = new NotificationChannel(
                    CHANNEL_ID2,
                    "DataCrea" +"Service Channel2",
                    NotificationManager.IMPORTANCE_DEFAULT
            );
            NotificationManager manager = getSystemService(NotificationManager.class);
            manager.createNotificationChannel(serviceChannel);
            manager.createNotificationChannel(serviceChannel2);
        }
    }

    @Override
    public void step(long timeNs) {
        numSteps++;
        SharedPreferences prefs = this.getApplicationContext().getSharedPreferences(this.getApplicationContext().getPackageName()+ ".v2.playerprefs", Context.MODE_PRIVATE);
        prefs.edit().putInt("Steps",numSteps).commit();
        //unity uses package name for SharedPreferences file ("com.example.app")
        if(numSteps>=10&&numSteps%10==0){
            prefs.edit().putInt("VictoryPoints",prefs.getInt("VictoryPoints",0)+1).commit();
            NotificationManagerCompat manager=NotificationManagerCompat.from(this);

            Notification notification = new NotificationCompat.Builder(this, CHANNEL_ID2)
                    .setContentTitle("DataCrea Step Counter")
                    .setContentText("Gained 1 Victory Point for 10 steps!")
                    .setSmallIcon(R.drawable.logo1)
                    .build();
            manager.notify(2, notification);
        }
        String date=prefs.getString("Date", "none");
        assert date != null;
        Date now= Calendar.getInstance().getTime();
        try {

            if(date.equalsIgnoreCase("none")){
                DateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy");
                String strDate = dateFormat.format(now);
                prefs.edit().putString("Date",strDate).commit();
            }
            else{
                Date date1=new SimpleDateFormat("dd/MM/yyyy").parse(date);
                if(!isSameDay(now,date1)) {

                    DateFormat dateFormat = new SimpleDateFormat("dd/MM/yyyy");
                    String strDate = dateFormat.format(now);
                    prefs.edit().putString("Date", strDate).commit();
                    numSteps = 0;
                    prefs.edit().putInt("Steps", 0).commit();
                }
            }

        } catch (ParseException e) {
            e.printStackTrace();
        }
        Context context = this;
        AppWidgetManager appWidgetManager = AppWidgetManager.getInstance(context);
        RemoteViews remoteViews = new RemoteViews(context.getPackageName(), R.layout.data_crea_widget);
        ComponentName thisWidget = new ComponentName(context, DataCreaWidget.class);
        remoteViews.setTextViewText(R.id.appwidget_text, "Steps taken: "+String.valueOf(numSteps));
        appWidgetManager.updateAppWidget(thisWidget, remoteViews);

        //TvSteps.setText(TEXT_NUM_STEPS + numSteps);
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        if (event.sensor.getType() == Sensor.TYPE_ACCELEROMETER) {
            simpleStepDetector.updateAccel(
                    event.timestamp, event.values[0], event.values[1], event.values[2]);
        }
    }
    //check if we are on the same day
    public static boolean isSameDay(Date date1, Date date2) {
        Calendar calendar1 = Calendar.getInstance();
        calendar1.setTime(date1);
        Calendar calendar2 = Calendar.getInstance();
        calendar2.setTime(date2);
        return calendar1.get(Calendar.YEAR) == calendar2.get(Calendar.YEAR)
                && calendar1.get(Calendar.MONTH) == calendar2.get(Calendar.MONTH)
                && calendar1.get(Calendar.DAY_OF_MONTH) == calendar2.get(Calendar.DAY_OF_MONTH);
    }

}
