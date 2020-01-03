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
    public class RequirementTypeDialog : ComponentDialog
    {
        public RequirementTypeDialog() : base(nameof(RequirementTypeDialog))
        {
            AddDialog(new MeansDialog());
            AddDialog(new ExistingFeatureDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
                {
                    RequestEndsStepAsync,
                    CheckEndsStepAsync,
                }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> RequestEndsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var dialogOptions = AllDialog.RequestRequirementType;
            var msg = OutputRandomizer.StringRandomizer(dialogOptions);
            var typingMsg = stepContext.Context.Activity.CreateReply();
            typingMsg.Type = ActivityTypes.Typing;
            typingMsg.Text = null;
            await stepContext.Context.SendActivityAsync(typingMsg);
            await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(msg),
                    RetryPrompt = MessageFactory.Text("Please choose an option from the list."),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Suggest an idea", "Request a change" }),
                }, cancellationToken);
        }
        private static async Task<DialogTurnResult> CheckEndsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (((FoundChoice)stepContext.Result).Value == "Suggest an idea")
            {
                MainFlowDialog.userStory.ReqType = "new idea";
                return await stepContext.BeginDialogAsync("MeansDialog", null, cancellationToken);

            }
            else if (((FoundChoice)stepContext.Result).Value == "Request a change")
            {
                MainFlowDialog.userStory.ReqType = "change request";
                return await stepContext.BeginDialogAsync(nameof(ExistingFeatureDialog), null, cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Sorry I do not support this option yet...") }, cancellationToken);
            }
        }


    }
}