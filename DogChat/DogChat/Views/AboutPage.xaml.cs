using MediaManager;
using System;
using System.ComponentModel;
using System.IO;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DogChat.Views
{
    public partial class AboutPage : ContentPage
    {
        //string url = "https://ia800605.us.archive.org/32/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3";
        string url = @"C:\Users\tache\Documents\Sound recordings\Recording/m4a";
        private static System.Timers.Timer aTimer;
        private int LoopCount = 0;
        private bool Running = true;
        private string DurationType= "Minutes";

        public AboutPage()
        {
            InitializeComponent();

            var player = CrossMediaManager.Current;
            player.MediaItemFinished += Current_MediaItemFinished;

            lblDelayValue.Text = stepper.Value.ToString();

            test.Text = "0";

            //public player = CrossMediaManager.Current.MediaPlayer;
            ////media.MediaItemFinished -= Current_MediaItemFinished;

        }


        private void StartTimer()
        {
            int delayMiliSeconds = Convert.ToInt32(lblDelayValue.Text) * 60 * 1000;
            if(DurationType=="Seconds")
            {
                delayMiliSeconds = delayMiliSeconds / 60;
                if(delayMiliSeconds<15000) // 15 seconds
                {
                    delayMiliSeconds = 15000;

                    Device.BeginInvokeOnMainThread(() => {
                        stepper.Value = 15;
                    });
                }

            }
                

            aTimer = new System.Timers.Timer(delayMiliSeconds);

            if (!Running)
            {
                aTimer.Elapsed -= OnTimedEvent;
                aTimer.Enabled = false;
                return;
            }

            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void StopTimer()
        {

            //aTimer = new System.Timers.Timer(10000);
            //aTimer.Elapsed -= OnTimedEvent;
            aTimer.Enabled = false;

        }


        private  void OnTimedEvent(Object source, ElapsedEventArgs e)
        {

            
            PlayAudio();
        }

        void OnValueChanged(object sender, ValueChangedEventArgs e)
        {
            lblDelayValue.Text = e.NewValue.ToString();
        }


        private void Current_MediaItemFinished(object sender, MediaManager.Media.MediaItemEventArgs e)
        {

            Device.BeginInvokeOnMainThread(() => {
                LoopCount++;
                test.Text = LoopCount.ToString();
            });

            //SetTimer();
        }

        private  void PlayAudio()
        {
             CrossMediaManager.Current.Play("file:///android_asset/Recording.m4a");
           

    }

    private  void PlayButtonClicked(object sender, EventArgs e)
        {

             CrossMediaManager.Current.Play("file:///android_asset/Recording.m4a");
            //await CrossMediaManager.Current.Play("android.resource://[PackageName]/raw/myfile", MediaFileType.Audio, ResourceAvailability.Local);

            //if (File.Exists("Test.mp3"))
            //    //await CrossMediaManager.Current.Play(url);
            //    await CrossMediaManager.Current.PlayFromResource("Test.mp3");


        }
        private  void PauseButtonClicked(object sender, EventArgs e)
        {

             CrossMediaManager.Current.Play("file:///android_asset/Recording.m4a");
            //await CrossMediaManager.Current.Play("android.resource://[PackageName]/raw/myfile", MediaFileType.Audio, ResourceAvailability.Local);

            //if (File.Exists("Test.mp3"))
            //    //await CrossMediaManager.Current.Pause();
            //    await CrossMediaManager.Current.PlayFromResource("Test.mp3");
        }

        private  void Button_Clicked(object sender, EventArgs e)
        {
            stepper.IsEnabled = false;
            CrossMediaManager.Current.Play("file:///android_asset/Recording.m4a"); 
            StartTimer();
            

        }

        private void Button_ClickedStop(object sender, EventArgs e)
        {
            stepper.IsEnabled = true;
            StopTimer();

        }

        private void RadioButton_CheckedChangedMinutes(object sender, CheckedChangedEventArgs e)
        {
            DurationType = "Minutes";
    }

        private void RadioButton_CheckedChangedSeconds(object sender, CheckedChangedEventArgs e)
        {
            DurationType = "Seconds";
        }
    }
}