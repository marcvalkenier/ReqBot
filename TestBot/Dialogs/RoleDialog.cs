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
    public class RoleDialog : ComponentDialog
    {
        public RoleDialog()
            : base(nameof(RoleDialog))
        {
            AddDialog(new ConfirmationDialog());

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
                {
                    RequestEndsStepAsync,
                    CheckEndsStepAsync,
                    OtherRoleStepAsync,
                }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> RequestEndsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MainFlowDialog.trace.PreviousDialog = MainFlowDialog.trace.CurrentDialog;
            MainFlowDialog.trace.CurrentDialog = "RoleDialog";
            if (!MainFlowDialog.userStory.UserStoryChanged)
            {
                var dialogOptions = AllDialog.RequestRole;
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
                        RetryPrompt = MessageFactory.Text("Please click an option below"),
                        Choices = ChoiceFactory.ToChoices(MainFlowDialog.Roles),
                    }, cancellationToken);
            }
        
            else
            {
                if (MainFlowDialog.Roles.Any())
                {
                    var dialogOptions = AllDialog.RequestChangeRole;
                    var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                    var msg = rndmsg.Replace("{MainFlowDialog.userStory.Role}", MainFlowDialog.userStory.Role);
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
                            Choices = ChoiceFactory.ToChoices(MainFlowDialog.Roles),
                        }, cancellationToken);
                }
                else
                {
                    return await stepContext.ContinueDialogAsync(cancellationToken);
                }
            }
        }
        private static async Task<DialogTurnResult> CheckEndsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!MainFlowDialog.userStory.UserStoryChanged)
            {
                if (((FoundChoice)stepContext.Result).Value == "Other...")
                {
                    var dialogOptions = AllDialog.RequestOtherRole;
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
                    MainFlowDialog.userStory.Role = ((FoundChoice)stepContext.Result).Value;
                    var dialogOptions = AllDialog.RespondRole;
                    var msg = OutputRandomizer.StringRandomizer(dialogOptions);
                    var typingMsg = stepContext.Context.Activity.CreateReply();
                    typingMsg.Type = ActivityTypes.Typing;
                    typingMsg.Text = null;
                    await stepContext.Context.SendActivityAsync(typingMsg);
                    await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    return await stepContext.BeginDialogAsync(nameof(ConfirmationDialog), null, cancellationToken);
                }
            }
            else
            {
                if (!MainFlowDialog.Roles.Any())
                {
                    var dialogOptions = AllDialog.RequestChangeNoRoles;
                    var msg = OutputRandomizer.StringRandomizer(dialogOptions);
                    var typingMsg = stepContext.Context.Activity.CreateReply();
                    typingMsg.Type = ActivityTypes.Typing;
                    typingMsg.Text = null;
                    await stepContext.Context.SendActivityAsync(typingMsg);
                    await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                    return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                }
                if (((FoundChoice)stepContext.Result).Value == "Other...")
                {
                    var dialogOptions = AllDialog.RequestChangeOtherRole;
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
                    MainFlowDialog.userStory.OldRole = MainFlowDialog.userStory.Role;
                    MainFlowDialog.userStory.Role = ((FoundChoice)stepContext.Result).Value;
                    var dialogOptions = AllDialog.RespondChangeRole;
                    var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                    var msg = rndmsg.Replace("{MainFlowDialog.userStory.OldRole}", MainFlowDialog.userStory.OldRole).Replace("{MainFlowDialog.userStory.Role}", MainFlowDialog.userStory.Role);
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
        private static async Task<DialogTurnResult> OtherRoleStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!MainFlowDialog.userStory.UserStoryChanged)
            {
                MainFlowDialog.userStory.Role = (string)stepContext.Result;
                var dialogOptions = AllDialog.RespondOtherRole;
                var msg = OutputRandomizer.StringRandomizer(dialogOptions);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(ConfirmationDialog), null, cancellationToken);
            }
            else
            {
                MainFlowDialog.userStory.OldRole = MainFlowDialog.userStory.Role;
                MainFlowDialog.userStory.Role = (string)stepContext.Result;
                var dialogOptions = AllDialog.RespondChangeOtherRole;
                var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                var msg = rndmsg.Replace("{MainFlowDialog.userStory.OldRole}", MainFlowDialog.userStory.OldRole).Replace("{MainFlowDialog.userStory.Role}", MainFlowDialog.userStory.Role);
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