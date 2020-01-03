// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace ReqBot
{
    public class RequestMailDialog : ComponentDialog
    {

        public RequestMailDialog() : base(nameof(RequestMailDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
                {
                    RequestMailStepAsync,
                    ReceiveMailStepAsync,
                }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> RequestMailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MainFlowDialog.trace.PreviousDialog = MainFlowDialog.trace.CurrentDialog;
            MainFlowDialog.trace.CurrentDialog = "RequestMailDialog";

            if (!MainFlowDialog.user.CorrectEmailFormat && MainFlowDialog.user.Email != null)
            {
                var dialogOptions = AllDialog.RequestMailBadFormat;
                var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                var msg = rndmsg.Replace("{MainFlowDialog.user.Email}", MainFlowDialog.user.Email);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
            }
            else
            {
                var dialogOptions = AllDialog.RequestMail;
                var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                var msg = rndmsg.Replace("{MainFlowDialog.user.Name}", MainFlowDialog.user.Name);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ReceiveMailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MainFlowDialog.user.CorrectEmailFormat = Regex.IsMatch((string)stepContext.Result, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            MainFlowDialog.user.Email = (string)stepContext.Result;
            if (MainFlowDialog.user.CorrectEmailFormat)
            {
                var dialogOptions = AllDialog.RespondToMailRequest;
                var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                var msg = rndmsg.Replace("{MainFlowDialog.user.Email}", MainFlowDialog.user.Email);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(ClosingDialog), null, cancellationToken);
            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(RequestMailDialog), null, cancellationToken);
            }
        }
    }
}