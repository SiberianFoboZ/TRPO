using System;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.FileFormats;
using NAudio.CoreAudioApi;
using NAudio;
using Microsoft.Speech.Recognition;
using System.Globalization;


namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        WaveIn waveIn;
        WaveFileWriter writer;
        string OutputAudio = "Test.wav";
        static CultureInfo ci = new CultureInfo("ru-RU");
        static SpeechRecognitionEngine sre = new SpeechRecognitionEngine(ci);
        public Form1()
        {
            InitializeComponent();
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += sre_SpeechRecognized;
            Grammar g_HelloGoodbye = GetHelloGoodbyeGrammar();
            Grammar g_SetTextBox = GetTextBox1TextGrammar();
            sre.LoadGrammarAsync(g_HelloGoodbye);
            sre.LoadGrammarAsync(g_SetTextBox);

        }
        static Grammar GetHelloGoodbyeGrammar()
        {
            Choices ch_HelloGoodbye = new Choices();
            ch_HelloGoodbye.Add("привет");
            ch_HelloGoodbye.Add("пока");
            /*расписать звуковые файлы граматики на пассивное значение слов, понимает только конкретных слов 
             конкетрые слова и буквы произношения имеются в тексте граматике было протестированное на не стандартных сложных словах, возникают проблемы с заикание
             поправки неизвестны, использование только по друсскую речь,есть вариант английской, но тога возникает заскакивание слов, 
             звук можно писать парраленьно с распозннованием*/
            GrammarBuilder gb_result =
              new GrammarBuilder(ch_HelloGoodbye);
            Grammar g_result = new Grammar(gb_result);
            return g_result;
        }

        static Grammar GetTextBox1TextGrammar()
        {
            Choices ch_Colors = new Choices();
            ch_Colors.Add(new string[] { "red", "white", "blue" });
            GrammarBuilder gb_result = new GrammarBuilder();
            gb_result.Append("текст в коробку один");
            gb_result.Append(ch_Colors);
            Grammar g_result = new Grammar(gb_result);
            return g_result;
        }
        void waveIN_DataAvailable ( object sender, WaveInEventArgs e )
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new EventHandler<WaveInEventArgs>(waveIN_DataAvailable), sender, e);
            else
                writer.Write(e.Buffer, 0, e.BytesRecorded);
        }
        void StopRecording()
        {
            MessageBox.Show("Stop Record");
            waveIn.StopRecording();
        }
        private void waveIn_RecordingStopped(object sender,EventArgs e)
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new EventHandler(waveIn_RecordingStopped), sender, e);
            else
            {
                waveIn.Dispose();
                waveIn = null;
                writer.Close();
                writer = null;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (waveIn != null)
            {
                StopRecording();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {            try

            {
                MessageBox.Show("Start Recording");
                waveIn = new WaveIn();
                waveIn.DeviceNumber = 0;
                waveIn.DataAvailable += waveIN_DataAvailable;
                waveIn.RecordingStopped += waveIn_RecordingStopped;
                waveIn.WaveFormat = new WaveFormat(8000, 1);
                writer = new WaveFileWriter(OutputAudio, waveIn.WaveFormat);
                waveIn.StartRecording();
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
           if (checkBox1.Checked == true)
                sre.RecognizeAsync(RecognizeMode.Multiple);
            else if (checkBox1.Checked == false) 
                sre.RecognizeAsyncCancel();
        }
        void sre_SpeechRecognized(
      object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float conf = e.Result.Confidence;

            if (conf < 0.65) return;

            this.Invoke(new MethodInvoker(() =>
            {
                listBox1.Items.Add("Услышала: " +
                txt);
            })); 

            if (txt.IndexOf("text") >= 0 && txt.IndexOf("box") >=
              0 && txt.IndexOf("1") >= 0)
            {
                string[] words = txt.Split(' ');
                this.Invoke(new MethodInvoker(() =>
                { textBox1.Text = words[4]; }));
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
