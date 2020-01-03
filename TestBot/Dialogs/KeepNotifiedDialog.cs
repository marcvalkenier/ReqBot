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
    public class KeepNotifiedDialog : ComponentDialog
    {
        public KeepNotifiedDialog() : base(nameof(KeepNotifiedDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
                {
                    NotificationMethodStepAsync,
                    NotificationMethodResultStepAsync,
                }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> NotificationMethodStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MainFlowDialog.trace.PreviousDialog = MainFlowDialog.trace.CurrentDialog;
            MainFlowDialog.trace.CurrentDialog = "KeepNotifiedDialog";
            if (MainFlowDialog.user.NotificationOption == null)
            {
                var dialogOptions = AllDialog.RequestNotificationOption;
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
                       RetryPrompt = MessageFactory.Text("Please click on one of the options below."),
                       Choices = ChoiceFactory.ToChoices(new List<string> { "Mail me with updates", "Don't mail me"}),
                   }, cancellationToken);
            }
            else
            {
                var msg = "";
                if (MainFlowDialog.user.NotificationOption == "Mail me with updates")
                {
                    var dialogOptions = AllDialog.RequestChangeNotificationOption2;
                    var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                    msg = rndmsg.Replace("{MainFlowDialog.user.NotificationOption}", MainFlowDialog.user.NotificationOption);
                }
                else
                {
                    var dialogOptions = AllDialog.RequestChangeNotificationOption;
                    var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                    msg = rndmsg.Replace("{MainFlowDialog.user.NotificationOption}", MainFlowDialog.user.NotificationOption);
                }
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                   new PromptOptions
                   {
                       Prompt = MessageFactory.Text(msg),
                       RetryPrompt = MessageFactory.Text("Please click on one of the options below."),
                       Choices = ChoiceFactory.ToChoices(new List<string> { "Keep it the same", "Change settings" }),
                   }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> NotificationMethodResultStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (((FoundChoice)stepContext.Result).Value == "Keep it the same")
            {
                return await stepContext.BeginDialogAsync(nameof(ClosingDialog), null, cancellationToken);
            }
            if (((FoundChoice)stepContext.Result).Value == "Change settings" && MainFlowDialog.user.NotificationOption == "Mail me with updates")
            {
                MainFlowDialog.user.NotificationOption = "Don't mail me";
                var dialogOptions = AllDialog.RespondChangeNotificationOption;
                var msg = OutputRandomizer.StringRandomizer(dialogOptions);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(ClosingDialog), null, cancellationToken);
            }
            if (((FoundChoice)stepContext.Result).Value == "Change settings" && MainFlowDialog.user.NotificationOption == "Don't mail me")
            {
                MainFlowDialog.user.NotificationOption = "Mail me with updates";
                return await stepContext.BeginDialogAsync(nameof(RequestMailDialog), null, cancellationToken);
            }
            if (((FoundChoice)stepContext.Result).Value == "Mail me with updates")
            {
                MainFlowDialog.user.NotificationOption = ((FoundChoice)stepContext.Result).Value;
                return await stepContext.BeginDialogAsync(nameof(RequestMailDialog), null, cancellationToken);
            }
            else
            {
                MainFlowDialog.user.NotificationOption = ((FoundChoice)stepContext.Result).Value;
                return await stepContext.BeginDialogAsync(nameof(ClosingDialog), null, cancellationToken);
            }
        }
    }
}