using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.Speech.Synthesis;

namespace SeniorProject
{
    public class Sentence : IEnumerable<string>
    {
        private List<string> words;
        private Timer timer;
        private bool canAdd;
        private bool destroy;
        private bool canDestroy;
        private SpeechSynthesizer synth;

        public bool Destroy
        {
            get { return destroy; }
        }

        public Sentence(string word, SpeechSynthesizer synth, bool canDestroy = true)
        {
            this.synth = synth;

            words = new List<string>();
            timer = new Timer(3000);

            timer.Elapsed += Timer_Elapsed;

            if(canDestroy)
                timer.Start();
                       
            words.Add(word);
            canAdd = canDestroy;
            destroy = false;
            this.canDestroy = canDestroy;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (canAdd)
            {
                canAdd = !canAdd;
                checkContext();
                synth.Speak(this.ToString());
            }
            else if (!destroy)
                destroy = !destroy;
        }

        private void checkContext()
        {
            string temp = String.Join(" ", words.ToArray()).ToLower();

            if(temp.Contains ("now day"))
                temp = temp.Replace("now day", "today");
    
            if (temp.Contains("how you")) 
                temp = temp.Replace("how you", "how are you");

            if(temp.Contains("who like"))
                temp = temp.Replace("who like", "who would like");

            if(temp.Contains("like try"))
                temp = temp.Replace("like try", "like to try");

            if(temp.Contains("help"))
                temp = temp.Replace("help", "help me");

            temp = char.ToUpper(temp[0]) + temp.Substring(1);

            words.Clear();
            words.AddRange(temp.Split(' '));
        }

        public IEnumerator<string> GetEnumerator()
        {
            return words.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool addWord(string word)
        {
            if (!canAdd)
                return false;

            if (words.LastIndexOf(word.ToLower()) != words.Count - 1 && words.LastIndexOf(word) != words.Count -1)
            {
                words.Add(word.ToLower());
                timer.Close();
                timer.Start();
            }

            return true;
        }

        public override string ToString()
        {
            string text = "";

            foreach (string word in words)
                text += word + " ";

            return text;
        }
    }
}
