// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.IO;
using System.Net;
using System;

namespace ReqBot
{
    public class AmbiguityCheck
    {
        public static int CheckForAmbiguity(string userInput)
        {
            if (MainFlowDialog.trace.CurrentDialog == "EndsDialog")
            {
                try
                {
                    int r = CheckForReferentialAmbiguity(userInput);
                    if (r > 0)
                    {
                        return 1;
                    }
                }
                catch(Exception)
                {

                }
            }

            int v = CheckForVagueness(userInput);
            if (v > 0)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }

        public static int CheckForReferentialAmbiguity(string userInput)
        {
            var httpWebRequest2 = (HttpWebRequest)WebRequest.Create("http://text-processing.com/api/tag/");
            httpWebRequest2.ContentType = "application/json";
            httpWebRequest2.Method = "POST";

            using (var streamWriter2 = new StreamWriter(httpWebRequest2.GetRequestStream()))
            {
                string json = "text=" + MainFlowDialog.userStory.Means;

                streamWriter2.Write(json);
            }
            var httpResponse2 = (HttpWebResponse)httpWebRequest2.GetResponse();
            using (var streamReader2 = new StreamReader(httpResponse2.GetResponseStream()))
            {
                var response = streamReader2.ReadToEnd();
                var responseString = response.ToString();
                var responseStringAdjusted = responseString.Substring(13, responseString.Length - 13);
                string[] sentencesInMeans = responseStringAdjusted.Split($"\\n(S ");
                int nounCounterMeans = 0;
                List<string> nounsInMeans = new List<string>();

                foreach (string sentenceInMeans in sentencesInMeans)
                {
                    nounCounterMeans = 0;
                    string[] wordsInMeans = sentenceInMeans.Split($" ");
                    foreach (string wordInMeans in wordsInMeans)
                    {
                        if (wordInMeans.Contains("/NN"))
                        {
                            nounCounterMeans++;
                            var cleanedWord = wordInMeans.Substring(0, wordInMeans.LastIndexOf("/NN"));
                            nounsInMeans.Add(cleanedWord);
                        }
                    }
                    if (nounCounterMeans >= 2)
                    {
                        MainFlowDialog.multipleNouns = nounsInMeans;
                    }
                    else
                    {
                        nounCounterMeans = 0;
                        nounsInMeans.Clear();
                    }
                
                }
            }

            //Checks if 2 nouns exist in one sentence before a personal or possessive pronoun 
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://text-processing.com/api/tag/");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "text=" + userInput;

                streamWriter.Write(json);
            }

            //Example output: {“text”: “(S Hey/PRP how/WRB are/VBP you/PRP doing/VBG ?/.)\n(S I/PRP am/VBP doing/VBG great/JJ ,/, thank/NN you/PRP)”}
            //Testing in CMD: curl -d "text=Hello, it is a new day. He pick up my book and cat." http://text-processing.com/api/tag/
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var output = result.ToString();
                var output2 = output.Substring(13, output.Length - 13);
                string[] sentences = output2.Split($"\\n(S ");
                int nounCounter = 0;
                List<string> nouns = new List<string>();
                foreach (string sentence in sentences)
                {
                    nounCounter = 0;
                    string[] words = sentence.Split($" ");
                    foreach (string word in words)
                    {
                        if (word.Contains("/PRP") && !word.Contains("I/PRP") && !word.Contains("i/PRP") && !word.Contains("my/PRP") && !word.Contains("me/PRP") && !word.Contains("My/PRP") && !word.Contains("Me/PRP") && MainFlowDialog.multipleNouns.Any())
                        {
                            var cleanedTrigger = word.Substring(0, word.LastIndexOf("/PRP"));
                            MainFlowDialog.userStory.DetectedTriggerReferential = cleanedTrigger;
                            MainFlowDialog.userStory.DetectedNounsReferential = string.Join(" ",MainFlowDialog.multipleNouns);
                            MainFlowDialog.userStory.DetectedReferential = MainFlowDialog.userStory.DetectedTriggerReferential + " = " + MainFlowDialog.userStory.DetectedNounsReferential;
                            return 1;
                        }
                        if (word.Contains("/NN"))
                        {
                            nounCounter++;
                            var cleanedWord = word.Substring(0, word.LastIndexOf("/NN"));
                            nouns.Add(cleanedWord);
                        }
                    }
                    if (nounCounter >= 2)
                    {
                        MainFlowDialog.multipleNouns = nouns;
                    }
                    else
                    {
                        nounCounter = 0;
                        nouns.Clear();
                    }

                }
                return 0;
            }
        }

        public static int CheckForVagueness(string userInput)
        {
            if (File.Exists(@"..\vagueness.csv"))
            {
                using (var reader = new StreamReader(@"..\vagueness.csv"))
                {
                    int t = 0;
                    List<string> listA = new List<string>();
                    List<string> listB = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');

                        listA.Add(values[0]);
                        listB.Add(values[1]);
                    }
                    foreach (string line in listA)
                    {
                        t++;
                        if (userInput.Contains(" " + line + " ") || userInput.Contains(" " + line + ".") || userInput.Contains(" " + line + ","))
                        {
                            MainFlowDialog.userStory.DetectedVagueness = line;
                            MainFlowDialog.userStory.MethodToDisambiguateVagueness = listB[t - 1];
                            if (MainFlowDialog.trace.CurrentDialog == "MeansDialog")
                            {
                                MainFlowDialog.userStory.DetectedMeansVagueness = line;
                                MainFlowDialog.userStory.MethodToDisambiguateMeansVagueness = listB[t - 1];
                            }
                            else if(MainFlowDialog.trace.CurrentDialog == "EndsDialog")
                            {
                                MainFlowDialog.userStory.DetectedEndsVagueness = line;
                                MainFlowDialog.userStory.MethodToDisambiguateEndsVagueness = listB[t - 1];
                            }
                            return 1;
                        }
                    }
                }
            }
            return 0;
        }
    }
}