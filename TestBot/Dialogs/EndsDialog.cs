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
    public class EndsDialog : ComponentDialog
    {
        public EndsDialog()
            : base(nameof(EndsDialog))
        {
            AddDialog(new RoleDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
                {
                    RequestEndsStepAsync,
                    CheckEndsStepAsync,
                }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> RequestEndsStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            MainFlowDialog.trace.PreviousDialog = MainFlowDialog.trace.CurrentDialog;
            MainFlowDialog.trace.CurrentDialog = "EndsDialog";
            if (!MainFlowDialog.userStory.UserStoryChanged)
            {
                var dialogOptions = AllDialog.RequestEnds;
                var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                var msg = rndmsg.Replace("{MainFlowDialog.userStory.ReqType}", MainFlowDialog.userStory.ReqType);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
            }
            else
            {
                MainFlowDialog.userStory.DisambiguationEndsVagueness = null;
                var dialogOptions = AllDialog.RequestChangeEnds;
                var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                var msg = rndmsg.Replace("{MainFlowDialog.userStory.Ends}", MainFlowDialog.userStory.Ends);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
            }
        }
        private static async Task<DialogTurnResult> CheckEndsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (MainFlowDialog.AmbiguityDetection)
            {
                string userInput = (string)stepContext.Result;
                int i = AmbiguityCheck.CheckForAmbiguity(userInput);
                if (i == 1)
                {
                    if (!MainFlowDialog.userStory.UserStoryChanged)
                    {
                        MainFlowDialog.userStory.Ends = (string)stepContext.Result;
                        MainFlowDialog.userStory.CurrentAmbiguityLocation = "EndsDialog";
                        return await stepContext.BeginDialogAsync(nameof(ResolveReferentialAmbiguityDialog), null, cancellationToken);
                    }
                    else
                    {
                        MainFlowDialog.userStory.OldEnds = MainFlowDialog.userStory.Ends;
                        MainFlowDialog.userStory.Ends = (string)stepContext.Result;
                        MainFlowDialog.userStory.CurrentAmbiguityLocation = "EndsDialog";
                        return await stepContext.BeginDialogAsync(nameof(ResolveReferentialAmbiguityDialog), null, cancellationToken);
                    }
                }
                if (i == 2)
                {
                    if (!MainFlowDialog.userStory.UserStoryChanged)
                    {
                        MainFlowDialog.userStory.Ends = (string)stepContext.Result;
                        MainFlowDialog.userStory.CurrentAmbiguityLocation = "EndsDialog";
                        return await stepContext.BeginDialogAsync(nameof(ResolveVagueAmbiguityDialog), null, cancellationToken);
                    }
                    else
                    {
                        MainFlowDialog.userStory.OldEnds = MainFlowDialog.userStory.Ends;
                        MainFlowDialog.userStory.Ends = (string)stepContext.Result;
                        MainFlowDialog.userStory.CurrentAmbiguityLocation = "EndsDialog";
                        return await stepContext.BeginDialogAsync(nameof(ResolveVagueAmbiguityDialog), null, cancellationToken);
                    }
                }
            }
            if (!MainFlowDialog.userStory.UserStoryChanged)
            {
                MainFlowDialog.userStory.Ends = (string)stepContext.Result;
                var dialogOptions = AllDialog.RespondEnds;
                var msg = OutputRandomizer.StringRandomizer(dialogOptions);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                if (MainFlowDialog.Roles.Any())
                {
                    return await stepContext.BeginDialogAsync(nameof(RoleDialog), null, cancellationToken);
                }
                else
                {
                    MainFlowDialog.userStory.Role = "General User";
                    return await stepContext.BeginDialogAsync(nameof(ConfirmationDialog), null, cancellationToken);
                }
            }
            else
            {
                MainFlowDialog.userStory.OldEnds = MainFlowDialog.userStory.Ends;
                MainFlowDialog.userStory.Ends = (string)stepContext.Result;
                var dialogOptions = AllDialog.RespondChangeEnds;
                var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                var msg = rndmsg.Replace("{MainFlowDialog.userStory.OldEnds}", MainFlowDialog.userStory.OldEnds).Replace("{MainFlowDialog.userStory.Ends}", MainFlowDialog.userStory.Ends);
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