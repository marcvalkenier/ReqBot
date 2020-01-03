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
    public class ExistingFeatureDialog : ComponentDialog
    {
        public ExistingFeatureDialog()
            : base(nameof(ExistingFeatureDialog))
        {
            AddDialog(new MeansDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
                {
                    RequestExistingFeatureStepAsync,
                    CheckExistingFeatureStepAsync,
                    CheckExistingFeatureNotListedStepAsync,
                }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> RequestExistingFeatureStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            MainFlowDialog.trace.PreviousDialog = MainFlowDialog.trace.CurrentDialog;
            MainFlowDialog.trace.CurrentDialog = "ExistingFeatureDialog";
            //Max length individual string 20 characters to be button 
            var dialogOptions = AllDialog.RequestExistingFeature;
            var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
            var msg = rndmsg.Replace("{MainFlowDialog.appName}", MainFlowDialog.appName);
            var typingMsg = stepContext.Context.Activity.CreateReply();
            typingMsg.Type = ActivityTypes.Typing;
            typingMsg.Text = null;
            await stepContext.Context.SendActivityAsync(typingMsg);
            await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(msg),
                    RetryPrompt = MessageFactory.Text("Please click an option below"),
                    Choices = ChoiceFactory.ToChoices(MainFlowDialog.Components),
                }, cancellationToken);
        }

        private static async Task<DialogTurnResult> CheckExistingFeatureStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (((FoundChoice)stepContext.Result).Value == "Other...")
            {
                var dialogOptions = AllDialog.RequestOtherExistingFeature;
                var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                var msg = rndmsg.Replace("{MainFlowDialog.appName}", MainFlowDialog.appName);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
            }
            else
            {
                MainFlowDialog.userStory.ExistingFeature = ((FoundChoice)stepContext.Result).Value;
                return await stepContext.BeginDialogAsync(nameof(MeansDialog), null, cancellationToken);
            }
        }

        private static async Task<DialogTurnResult> CheckExistingFeatureNotListedStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MainFlowDialog.userStory.ExistingFeature = (string)stepContext.Result;
            return await stepContext.BeginDialogAsync(nameof(MeansDialog), null, cancellationToken);
        }



    }
}