using MediaManager;
using Plugin.AudioRecorder;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DogChat.Views
{
    public partial class AboutPage : ContentPage
    {
        //string url = "https://ia800605.us.archive.org/32/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3";
        string url = @"C:\Users\tache\Documents\Sound recordings\Recording/m4a";
        string defaultFilePath = "file:///android_asset/Recording.m4a";
        string recordedFilePath = "";
        private static System.Timers.Timer aTimer;
        private int LoopCount = 0;
        private bool Running = true;
        private string DurationType= "Minutes";

        AudioRecorderService recorder;
        Plugin.AudioRecorder.AudioPlayer playerRec;


        public AboutPage()
        {
            InitializeComponent();

            var player = CrossMediaManager.Current;
            player.MediaItemFinished += Current_MediaItemFinished;

            lblDelayValue.Text = stepper.Value.ToString();

            test.Text = "0";
            LoopCount = 0;


            recorder = new AudioRecorderService
            {
                StopRecordingAfterTimeout = true,
                TotalAudioTimeout = TimeSpan.FromSeconds(15),
                AudioSilenceTimeout = TimeSpan.FromSeconds(2)
            };

            playerRec = new Plugin.AudioRecorder.AudioPlayer();
            playerRec.FinishedPlaying += Player_FinishedPlaying;
            recorder.AudioInputReceived += recorder_AudioInputReceived;
            //public player = CrossMediaManager.Current.MediaPlayer;
            ////media.MediaItemFinished -= Current_MediaItemFinished;

        }


        void Delete_Clicked(object sender, EventArgs e)
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CurrentRecording.wav");
            if(File.Exists(fileName))
                File.Delete(fileName);

            DelButton.IsVisible = false;
        }



        void recorder_AudioInputReceived(object sender, string audioFile)
        {

            recordedFilePath = recorder.FilePath;
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CurrentRecording.wav");
            File.Copy(recorder.FilePath, fileName, true);
            recordedFilePath = fileName;

            DelButton.IsVisible = true;

        }

        async void Record_Clicked(object sender, EventArgs e)
        {
            await RecordAudio();
        }


        async Task RecordAudio()
        {
            try
            {
                if (!recorder.IsRecording) //Record button clicked
                {
                    recorder.StopRecordingOnSilence = false;// TimeoutSwitch.IsToggled;

                    RecordButton.IsEnabled = false;
                    PlayButton.IsEnabled = false;

                    //start recording audio
                    var audioRecordTask = await recorder.StartRecording();

                    RecordButton.Text = "Stop Recording";
                    RecordButton.IsEnabled = true;

                    await audioRecordTask;

                    RecordButton.Text = "Record";
                    PlayButton.IsEnabled = true;

                    //var filePath = recorder.GetAudioFilePath();
                    //string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CurrentRecording.txt");
                    //File.Copy(filePath, fileName);


                }
                else //Stop button clicked
                {
                    RecordButton.IsEnabled = false;

                    //stop the recording...
                    await recorder.StopRecording();


                    RecordButton.IsEnabled = true;
                }
                
            }
            catch (Exception ex)
            {
                //blow up the app!
                throw ex;
            }
        }

        void Play_Clicked(object sender, EventArgs e)
        {
            PlayAudioX();
        }

        void PlayAudioX()
        {
            try
            {
                var filePath = recorder.GetAudioFilePath();

                if (filePath != null)
                {
                    PlayButton.IsEnabled = false;
                    RecordButton.IsEnabled = false;

                    playerRec.Play(filePath);
                }
            }
            catch (Exception ex)
            {
                //blow up the app!
                throw ex;
            }
        }

        void Player_FinishedPlaying(object sender, EventArgs e)
        {
            PlayButton.IsEnabled = true;
            RecordButton.IsEnabled = true;
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
            if(aTimer!=null)
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

        private  void PlayAudio(string filePath="")
        {
            

            if (CrossMediaManager.Current.IsPlaying() == true)
                return;

            if (File.Exists(recordedFilePath))
                CrossMediaManager.Current.Play(recordedFilePath);
            else
                CrossMediaManager.Current.Play(defaultFilePath);
           

    }

    private  void PlayButtonClicked(object sender, EventArgs e)
        {

             CrossMediaManager.Current.Play(defaultFilePath);
            //await CrossMediaManager.Current.Play("android.resource://[PackageName]/raw/myfile", MediaFileType.Audio, ResourceAvailability.Local);

            //if (File.Exists("Test.mp3"))
            //    //await CrossMediaManager.Current.Play(url);
            //    await CrossMediaManager.Current.PlayFromResource("Test.mp3");


        }
        private  void PauseButtonClicked(object sender, EventArgs e)
        {

             CrossMediaManager.Current.Play(defaultFilePath);
            //await CrossMediaManager.Current.Play("android.resource://[PackageName]/raw/myfile", MediaFileType.Audio, ResourceAvailability.Local);

            //if (File.Exists("Test.mp3"))
            //    //await CrossMediaManager.Current.Pause();
            //    await CrossMediaManager.Current.PlayFromResource("Test.mp3");
        }

        private  void Button_Clicked(object sender, EventArgs e)
        {
            test.Text = "0";
            LoopCount = 0;

            StartBtn.Text = "Running...";
            StartBtn.TextColor = Color.Green;

            stepper.IsEnabled = false;
            radMin.IsEnabled = false;
            radSec.IsEnabled = false;
            PlayAudio();
            //CrossMediaManager.Current.Play(defaultFilePath); 
            StartTimer();
            

        }

        private void Button_ClickedStop(object sender, EventArgs e)
        {

            StartBtn.Text = "Start";
            StartBtn.TextColor = Color.White;

            stepper.IsEnabled = true;
            radMin.IsEnabled = true;
            radSec.IsEnabled = true;
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