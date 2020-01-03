using System;
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
    public class ResolveReferentialAmbiguityDialog : ComponentDialog
    {
        public ResolveReferentialAmbiguityDialog()
           : base(nameof(ResolveReferentialAmbiguityDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
                {
                    DisambiguateReferentialStepAsync,
                    SaveContinueStepAsync,
                    NotListedSaveContinueStepAsync,
                }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        private static async Task<DialogTurnResult> DisambiguateReferentialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MainFlowDialog.trace.PreviousDialog = MainFlowDialog.trace.CurrentDialog;
            MainFlowDialog.trace.CurrentDialog = "ResolveReferentialAmbiguityDialog";
            var dialogOptions = AllDialog.RequestReferentialDisambiguation;
            var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
            var msg = rndmsg.Replace("{MainFlowDialog.userStory.DetectedTriggerReferential}", MainFlowDialog.userStory.DetectedTriggerReferential);
            var typingMsg = stepContext.Context.Activity.CreateReply();
            typingMsg.Type = ActivityTypes.Typing;
            typingMsg.Text = null;
            await stepContext.Context.SendActivityAsync(typingMsg);
            await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
            MainFlowDialog.multipleNouns.Add("None of the options");
            MainFlowDialog.multipleNouns.Add("Multiple words…");
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text(msg),
                RetryPrompt = MessageFactory.Text("Click one of the options / type the number of one option to indicate to what you did refer."),
                Choices = ChoiceFactory.ToChoices(MainFlowDialog.multipleNouns),
            }, cancellationToken);
        }
        private static async Task<DialogTurnResult> SaveContinueStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MainFlowDialog.multipleNouns.Clear();
            if (((FoundChoice)stepContext.Result).Value == "None of the options" || ((FoundChoice)stepContext.Result).Value == "Multiple words…")
            {
                var dialogOptions = AllDialog.RequestNotListedReferentialDisambiguation;
                var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
                var msg = rndmsg.Replace("{MainFlowDialog.userStory.DetectedTriggerReferential}", MainFlowDialog.userStory.DetectedTriggerReferential);
                var typingMsg = stepContext.Context.Activity.CreateReply();
                typingMsg.Type = ActivityTypes.Typing;
                typingMsg.Text = null;
                await stepContext.Context.SendActivityAsync(typingMsg);
                await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
            }
            if (!MainFlowDialog.userStory.UserStoryChanged)
            {
                if (MainFlowDialog.userStory.CurrentAmbiguityLocation == "EndsDialog")
                {
                    MainFlowDialog.userStory.CurrentAmbiguityLocation = "";
                    MainFlowDialog.userStory.DisambiguationEndsReferential = ((FoundChoice)stepContext.Result).Value;
                    var dialogOptions = AllDialog.RespondReferentialDisambiguation;
                    var msg = OutputRandomizer.StringRandomizer(dialogOptions);
                    var typingMsg = stepContext.Context.Activity.CreateReply();
                    typingMsg.Type = ActivityTypes.Typing;
                    typingMsg.Text = null;
                    await stepContext.Context.SendActivityAsync(typingMsg);
                    await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    return await stepContext.BeginDialogAsync(nameof(RoleDialog), null, cancellationToken);
                }
                else
                {
                    return await stepContext.ContinueDialogAsync(cancellationToken);
                }
            }
            else
            {
                if (MainFlowDialog.userStory.CurrentAmbiguityLocation == "EndsDialog")
                                {
                    MainFlowDialog.userStory.CurrentAmbiguityLocation = "";
                    MainFlowDialog.userStory.DisambiguationEndsReferential = ((FoundChoice)stepContext.Result).Value;
                    var dialogOptions = AllDialog.RespondReferentialDisambiguation;
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
                    return await stepContext.ContinueDialogAsync(cancellationToken);
                }
            }
        }
        private static async Task<DialogTurnResult> NotListedSaveContinueStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!MainFlowDialog.userStory.UserStoryChanged)
            {
                if (MainFlowDialog.userStory.CurrentAmbiguityLocation == "EndsDialog")
                {
                    MainFlowDialog.userStory.CurrentAmbiguityLocation = "";
                    MainFlowDialog.userStory.DisambiguationEndsReferential = (string)stepContext.Result;
                    var dialogOptions = AllDialog.RespondReferentialDisambiguation;
                    var msg = OutputRandomizer.StringRandomizer(dialogOptions);
                    var typingMsg = stepContext.Context.Activity.CreateReply();
                    typingMsg.Type = ActivityTypes.Typing;
                    typingMsg.Text = null;
                    await stepContext.Context.SendActivityAsync(typingMsg);
                    await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    return await stepContext.BeginDialogAsync(nameof(RoleDialog), null, cancellationToken);
                }
                else
                {
                    return await stepContext.ContinueDialogAsync(cancellationToken);
                }
            }
            else
            {
                if (MainFlowDialog.userStory.CurrentAmbiguityLocation == "EndsDialog")
                {
                    MainFlowDialog.userStory.CurrentAmbiguityLocation = "";
                    MainFlowDialog.userStory.DisambiguationEndsReferential = (string)stepContext.Result;
                    var dialogOptions = AllDialog.RespondReferentialDisambiguation;
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
                    return await stepContext.ContinueDialogAsync(cancellationToken);
                }
            }
        }
    }
}
