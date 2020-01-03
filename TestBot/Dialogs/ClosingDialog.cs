// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace ReqBot
{
    public class ClosingDialog : ComponentDialog
    {
        public ClosingDialog() : base(nameof(ClosingDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
                {
                    ClosingStepAsync,
                    ClosingResponseStepAsync,
                    
                }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ClosingStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MainFlowDialog.trace.PreviousDialog = MainFlowDialog.trace.CurrentDialog;
            MainFlowDialog.trace.CurrentDialog = "ClosingDialog";
            var dialogOptions = AllDialog.RequestAnotherRequirement;
            var msg = OutputRandomizer.StringRandomizer(dialogOptions);
            var typingMsg = stepContext.Context.Activity.CreateReply();
            typingMsg.Type = ActivityTypes.Typing;
            typingMsg.Text = null;
            await stepContext.Context.SendActivityAsync(typingMsg);
            await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length)); return await stepContext.PromptAsync(nameof(ChoicePrompt),
                   new PromptOptions
                   {
                       Prompt = MessageFactory.Text(msg),
                       RetryPrompt = MessageFactory.Text("Please click on one of the options."),
                       Choices = ChoiceFactory.ToChoices(new List<string> { "Submit another", "End conversation" }),
                   }, cancellationToken);
        }

        private async Task<DialogTurnResult> ClosingResponseStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string[,] userStoryData = new string[6, 7] {
                    {$"User story: {MainFlowDialog.userStory.UserStoryCode}","Input","Detected Vagueness","Disambiguation of Vagueness", "Detected referential ambiguity", "Disambiguation of referential ambiguity", "Old input"},
                    {"Role",$"{MainFlowDialog.userStory.Role}","","","","",$"{MainFlowDialog.userStory.OldRole}"},
                    {"Means",$"{MainFlowDialog.userStory.Means}",$"{MainFlowDialog.userStory.DetectedMeansVagueness}",$"{MainFlowDialog.userStory.DisambiguationMeansVagueness}","","",$"{MainFlowDialog.userStory.OldMeans}"},
                    {"Ends",$"{MainFlowDialog.userStory.Ends}",$"{MainFlowDialog.userStory.DetectedEndsVagueness}",$"{MainFlowDialog.userStory.DisambiguationEndsVagueness}",$"{MainFlowDialog.userStory.DetectedReferential}",$"{MainFlowDialog.userStory.DisambiguationEndsReferential}",$"{MainFlowDialog.userStory.OldEnds}"},
                    {"Requirement Type",$"{MainFlowDialog.userStory.ReqType}","","","","",""},
                    {"Concerning feature",$"{MainFlowDialog.userStory.ExistingFeature}","","","","",""} };
            MainFlowDialog.listOfUserStories.Add(userStoryData);
            MainFlowDialog.userStory.ExistingFeature = "";
            MainFlowDialog.userStory.Role = "";
            MainFlowDialog.userStory.Means = "";
            MainFlowDialog.userStory.Ends = "";
            MainFlowDialog.userStory.OldRole = "";
            MainFlowDialog.userStory.OldMeans = "";
            MainFlowDialog.userStory.OldEnds = "";
            MainFlowDialog.userStory.DetectedMeansVagueness = "";
            MainFlowDialog.userStory.DetectedEndsVagueness = "";
            MainFlowDialog.userStory.DisambiguationMeansVagueness = "";
            MainFlowDialog.userStory.DisambiguationEndsVagueness = "";
            MainFlowDialog.userStory.DisambiguationEndsReferential = "";
            MainFlowDialog.userStory.DetectedReferential = "";
            MainFlowDialog.userStory.DetectedTriggerReferential = "";
            MainFlowDialog.userStory.DetectedNounsReferential = "";
            if (((FoundChoice)stepContext.Result).Value == "Submit another")
            {
                return await stepContext.BeginDialogAsync(nameof(RequirementTypeDialog), null, cancellationToken);
            }
            else
            {
                var dialogOptions = AllDialog.RespondClosingDialog;
                var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                var msg = rndmsg.Replace("{MainFlowDialog.user.Name}", MainFlowDialog.user.Name);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                using (StreamWriter outfile = new StreamWriter(@"..\output" +  MainFlowDialog.participantNo + ".csv"))
                {
                    for (int n = 0; n < MainFlowDialog.listOfUserStories.Count; n++)
                    {
                        userStoryData = MainFlowDialog.listOfUserStories[n];
                        for (int x = 0; x < 6; x++)
                        {
                            string content = "";
                            for (int y = 0; y < 7; y++)
                            {
                                content += userStoryData[x, y].ToString() + ";";
                            }
                            //trying to write data to csv
                            outfile.WriteLine(content);
                        }
                        outfile.WriteLine("");
                    }
                }
                System.Environment.Exit(1);
                return await stepContext.BeginDialogAsync(nameof(RequirementTypeDialog), null, cancellationToken);
            }
        }
    }
}