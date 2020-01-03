// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace ReqBot
{
    public class MeansDialog : ComponentDialog
    {
        public MeansDialog()
            : base(nameof(MeansDialog))
        {
            AddDialog(new EndsDialog());
            AddDialog(new ResolveVagueAmbiguityDialog());
            AddDialog(new ResolveReferentialAmbiguityDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
                {
                    RequestMeansStepAsync,
                    CheckMeansStepAsync,
                }));

            InitialDialogId = nameof(WaterfallDialog);

        }

        private async Task<DialogTurnResult> RequestMeansStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            MainFlowDialog.trace.PreviousDialog = MainFlowDialog.trace.CurrentDialog;
            MainFlowDialog.trace.CurrentDialog = "MeansDialog";
            if (!MainFlowDialog.userStory.UserStoryChanged)
            {
                if (MainFlowDialog.userStory.ReqType == "new idea") {
                    var dialogOptions = AllDialog.RequestMeansNew;
                    var msg = OutputRandomizer.StringRandomizer(dialogOptions);
                    var typingMsg = stepContext.Context.Activity.CreateReply();
                    typingMsg.Type = ActivityTypes.Typing;
                    typingMsg.Text = null;
                    await stepContext.Context.SendActivityAsync(typingMsg);
                    await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                    return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                }
                else
                {
                    var dialogOptions = AllDialog.RequestMeansExisting;
                    var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                    var msg = rndmsg.Replace("{MainFlowDialog.userStory.ExistingFeature}", MainFlowDialog.userStory.ExistingFeature);
                    var typingMsg = stepContext.Context.Activity.CreateReply();
                    typingMsg.Type = ActivityTypes.Typing;
                    typingMsg.Text = null;
                    await stepContext.Context.SendActivityAsync(typingMsg);
                    await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                    return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                }    
            }
            else
            {
                MainFlowDialog.userStory.DisambiguationMeansVagueness = null;
                var dialogOptions = AllDialog.RequestChangeMeans;
                var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                var msg = rndmsg.Replace("{MainFlowDialog.userStory.Means}", MainFlowDialog.userStory.Means);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
            }
        }
        private static async Task<DialogTurnResult> CheckMeansStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (MainFlowDialog.AmbiguityDetection)
            {
                string userInput = (string)stepContext.Result;
                int i = AmbiguityCheck.CheckForAmbiguity(userInput);
                /* We only use this part for the ends
                if (i == 1)
                {
                    MainFlowDialog.userStory.Means = (string)stepContext.Result;
                    MainFlowDialog.userStory.CurrentAmbiguityLocation = "MeansDialog";
                    return await stepContext.BeginDialogAsync(nameof(ResolveReferentialAmbiguityDialog), null, cancellationToken);
                }
                */
                if (i == 2)
                {
                    if (!MainFlowDialog.userStory.UserStoryChanged)
                    {
                        MainFlowDialog.userStory.Means = (string)stepContext.Result;
                        MainFlowDialog.userStory.CurrentAmbiguityLocation = "MeansDialog";
                        return await stepContext.BeginDialogAsync(nameof(ResolveVagueAmbiguityDialog), null, cancellationToken);
                    }
                    else
                    {
                        MainFlowDialog.userStory.OldMeans = MainFlowDialog.userStory.Means;
                        MainFlowDialog.userStory.Means = (string)stepContext.Result;
                        MainFlowDialog.userStory.CurrentAmbiguityLocation = "MeansDialog";
                        return await stepContext.BeginDialogAsync(nameof(ResolveVagueAmbiguityDialog), null, cancellationToken);
                    }
                }
            }

            if (!MainFlowDialog.userStory.UserStoryChanged)
            {
                MainFlowDialog.userStory.Means = (string)stepContext.Result;
                var dialogOptions = AllDialog.RespondMeans;
                var msg = OutputRandomizer.StringRandomizer(dialogOptions);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(EndsDialog), null, cancellationToken);
            }
            else
            {
                MainFlowDialog.userStory.OldMeans = MainFlowDialog.userStory.Means;
                MainFlowDialog.userStory.Means = (string)stepContext.Result;
                var dialogOptions = AllDialog.RespondChangedMeans;
                var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                var msg = rndmsg.Replace("{MainFlowDialog.userStory.OldMeans}", MainFlowDialog.userStory.OldMeans).Replace("{MainFlowDialog.userStory.Means}", MainFlowDialog.userStory.Means);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(ConfirmationDialog), null, cancellationToken);
            }
        }


    }
}