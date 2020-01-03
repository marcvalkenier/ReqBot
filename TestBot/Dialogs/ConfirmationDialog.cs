// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace ReqBot
{
    public class ConfirmationDialog : ComponentDialog
    {
        public ConfirmationDialog()
            : base(nameof(ConfirmationDialog))
        {
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
                {
                    ConfirmUserStoryStepAsync,
                    ProcessConfirmationStepAsync,
                    SelectChangePartStepAsync,
                }));

            InitialDialogId = nameof(WaterfallDialog);
            MainFlowDialog.trace.CurrentDialog = "ConfirmationDialog";
        }

        private async Task<DialogTurnResult> ConfirmUserStoryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MainFlowDialog.trace.PreviousDialog = MainFlowDialog.trace.CurrentDialog;
            MainFlowDialog.trace.CurrentDialog = "ConfirmationDialog";
            if ((MainFlowDialog.userStory.DisambiguationMeansVagueness == null && MainFlowDialog.userStory.DisambiguationEndsVagueness == null) || (MainFlowDialog.userStory.DisambiguationMeansVagueness == "" && MainFlowDialog.userStory.DisambiguationEndsVagueness == ""))
            {
                MainFlowDialog.userStory.CompleteUserStory = $"\nYou have the role of a **{MainFlowDialog.userStory.Role}**. \n You want the following: **{MainFlowDialog.userStory.Means}**. \n And your reason for this is: **{MainFlowDialog.userStory.Ends}**";
            }
            else if ((MainFlowDialog.userStory.DisambiguationMeansVagueness == null && MainFlowDialog.userStory.DisambiguationEndsVagueness != null) || ((MainFlowDialog.userStory.DisambiguationMeansVagueness == "" && MainFlowDialog.userStory.DisambiguationEndsVagueness != "")))
            {
                MainFlowDialog.userStory.CompleteUserStory = $"\nYou have the role of a **{MainFlowDialog.userStory.Role}**. \n You want the following: **{MainFlowDialog.userStory.Means}**. \n And your reason for this is: **{MainFlowDialog.userStory.Ends} ({MainFlowDialog.userStory.DisambiguationEndsVagueness})**";
            }
            else if ((MainFlowDialog.userStory.DisambiguationMeansVagueness != null && MainFlowDialog.userStory.DisambiguationEndsVagueness == null) || (MainFlowDialog.userStory.DisambiguationMeansVagueness != "" && MainFlowDialog.userStory.DisambiguationEndsVagueness == ""))
            {
                MainFlowDialog.userStory.CompleteUserStory = $"\nYou have the role of a **{MainFlowDialog.userStory.Role}**. \n You want the following: **{MainFlowDialog.userStory.Means} ({MainFlowDialog.userStory.DisambiguationMeansVagueness})**. \n And your reason for this is: **{MainFlowDialog.userStory.Ends}**";
            }
            else
            {
                MainFlowDialog.userStory.CompleteUserStory = $"\nYou have the role of a **{MainFlowDialog.userStory.Role}**. \n You want the following: **{MainFlowDialog.userStory.Means} ({MainFlowDialog.userStory.DisambiguationMeansVagueness})**. \n And your reason for this is: **{MainFlowDialog.userStory.Ends} ({MainFlowDialog.userStory.DisambiguationEndsVagueness})**";
            }
            var dialogOptions = AllDialog.RequestConfirmationOrChange;
            var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
            var msg = rndmsg.Replace("{MainFlowDialog.userStory.CompleteUserStory}", MainFlowDialog.userStory.CompleteUserStory).Replace("{MainFlowDialog.userStory.ReqType}", MainFlowDialog.userStory.ReqType).Replace("{Enter}", Environment.NewLine);
            var typingMsg = stepContext.Context.Activity.CreateReply();
            typingMsg.Type = ActivityTypes.Typing;
            typingMsg.Text = null;
            await stepContext.Context.SendActivityAsync(typingMsg);
            await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(msg),
                RetryPrompt = MessageFactory.Text("Please choose an option from the list."),
                Choices = ChoiceFactory.ToChoices(new List<string> {$"Submit", "Make changes"}),
            };

            // Prompt the user for a choice.
            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }
        private static async Task<DialogTurnResult> ProcessConfirmationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var changeUserstory = ((FoundChoice)stepContext.Result).Value;
            if (changeUserstory == "Make changes")
            {
                MainFlowDialog.userStory.UserStoryChanged = true;
            }

            else
            {
                MainFlowDialog.userStory.UserStoryChanged = false;
            }

            if (MainFlowDialog.userStory.UserStoryChanged)
            {
                var dialogOptions = AllDialog.RespondChange;
                var msg = OutputRandomizer.StringRandomizer(dialogOptions);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text(msg),
                    RetryPrompt = MessageFactory.Text("Type the number corresponding to the part you want to change."),
                    Choices = ChoiceFactory.ToChoices(new List<string> { MainFlowDialog.userStory.Means, MainFlowDialog.userStory.Ends, MainFlowDialog.userStory.Role }),
                };

                // Prompt the user for a choice.
                return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);

            }
            else
            {
                MainFlowDialog.userStory.UserStoryCode = "R2019-" + MainFlowDialog.random.Next(10000).ToString();
                MainFlowDialog.userStory.RequirementsSubmitted++;
                MainFlowDialog.userStory.UserStoryChanged = false;
                var dialogOptions = AllDialog.RespondConfirmation;
                var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                var msg = rndmsg.Replace("{MainFlowDialog.user.Name}", MainFlowDialog.user.Name).Replace("{MainFlowDialog.userStory.UserStoryCode}", MainFlowDialog.userStory.UserStoryCode).Replace("{MainFlowDialog.userStory.ReqType}", MainFlowDialog.userStory.ReqType);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(KeepNotifiedDialog), null, cancellationToken);
            }
        }
        private static async Task<DialogTurnResult> SelectChangePartStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var changeUserstoryPart = ((FoundChoice)stepContext.Result).Value;
            if (changeUserstoryPart == MainFlowDialog.userStory.Means)
            {
                return await stepContext.BeginDialogAsync(nameof(MeansDialog), null, cancellationToken);
            }
            else if (changeUserstoryPart == MainFlowDialog.userStory.Ends)
            {
                return await stepContext.BeginDialogAsync(nameof(EndsDialog), null, cancellationToken);
            }
            else if (changeUserstoryPart == MainFlowDialog.userStory.Role)
            {
                return await stepContext.BeginDialogAsync(nameof(RoleDialog), null, cancellationToken);
            }
            else
            {
                //System.IO.File.WriteAllLines(@"C:\Users\mpv95\source\alluserstories.txt", MainFlowDialog.listOfUserStories);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Thank you for participating. You can hand the laptop back.") }, cancellationToken);
            }
        }
    }
}