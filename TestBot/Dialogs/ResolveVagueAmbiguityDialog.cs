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
    public class ResolveVagueAmbiguityDialog : ComponentDialog
    {
        public ResolveVagueAmbiguityDialog()
           : base(nameof(ResolveVagueAmbiguityDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
                {
                    DisambiguateVaguenessStepAsync,
                    SaveContinueStepAsync,
                }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        private static async Task<DialogTurnResult> DisambiguateVaguenessStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            MainFlowDialog.trace.PreviousDialog = MainFlowDialog.trace.CurrentDialog;
            MainFlowDialog.trace.CurrentDialog = "ResolveVagueAmbiguityDialog";
            var dialogOptions = AllDialog.RequestVagueDisambiguation;
            var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
            var msg = rndmsg.Replace("{MainFlowDialog.userStory.DetectedVagueness}", MainFlowDialog.userStory.DetectedVagueness).Replace("{MainFlowDialog.userStory.MethodToDisambiguateVagueness}", MainFlowDialog.userStory.MethodToDisambiguateVagueness);
            var typingMsg = stepContext.Context.Activity.CreateReply();
            typingMsg.Type = ActivityTypes.Typing;
            typingMsg.Text = null;
            await stepContext.Context.SendActivityAsync(typingMsg);
            await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
        }
        private static async Task<DialogTurnResult> SaveContinueStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!MainFlowDialog.userStory.UserStoryChanged)
            {
                if (MainFlowDialog.userStory.CurrentAmbiguityLocation == "MeansDialog")
                {
                    MainFlowDialog.userStory.CurrentAmbiguityLocation = "";
                    MainFlowDialog.userStory.DisambiguationMeansVagueness = (string)stepContext.Result;
                    var dialogOptions = AllDialog.RespondVagueDisambiguation;
                    var msg = OutputRandomizer.StringRandomizer(dialogOptions);
                    var typingMsg = stepContext.Context.Activity.CreateReply();
                    typingMsg.Type = ActivityTypes.Typing;
                    typingMsg.Text = null;
                    await stepContext.Context.SendActivityAsync(typingMsg);
                    await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    return await stepContext.BeginDialogAsync(nameof(EndsDialog), null, cancellationToken);
                }
                if (MainFlowDialog.userStory.CurrentAmbiguityLocation == "EndsDialog")
                {
                    MainFlowDialog.userStory.CurrentAmbiguityLocation = "";
                    MainFlowDialog.userStory.DisambiguationEndsVagueness = (string)stepContext.Result;
                    var dialogOptions = AllDialog.RespondVagueDisambiguation;
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
                if (MainFlowDialog.userStory.CurrentAmbiguityLocation == "MeansDialog")
                {
                    MainFlowDialog.userStory.CurrentAmbiguityLocation = "";
                    MainFlowDialog.userStory.DisambiguationMeansVagueness = (string)stepContext.Result;
                    var dialogOptions = AllDialog.RespondVagueDisambiguation;
                    var msg = OutputRandomizer.StringRandomizer(dialogOptions);
                    var typingMsg = stepContext.Context.Activity.CreateReply();
                    typingMsg.Type = ActivityTypes.Typing;
                    typingMsg.Text = null;
                    await stepContext.Context.SendActivityAsync(typingMsg);
                    await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
                    await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
                    return await stepContext.BeginDialogAsync(nameof(ConfirmationDialog), null, cancellationToken);
                }
                else if (MainFlowDialog.userStory.CurrentAmbiguityLocation == "EndsDialog")
                {
                    MainFlowDialog.userStory.CurrentAmbiguityLocation = "";
                    MainFlowDialog.userStory.DisambiguationEndsVagueness = (string)stepContext.Result;
                    var dialogOptions = AllDialog.RespondVagueDisambiguation;
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
