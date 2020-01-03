// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using System.Net.Http;
using System.Text;
using System.Net;

namespace ReqBot
{
    public class MainFlowDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserStory> _userStoryAccessor;

        //Configuration variables
        public static string appName;
        public static int waitParametrics;
        public static int participantNo;
        public static bool AmbiguityDetection;
        public static List<string> Roles = new List<string>();
        public static List<string> Components = new List<string>();

        //Configurating variables for referential ambiguity detection and resolution
        public static List<string> multipleNouns = new List<string>();


        // Define value names for values tracked inside the dialogs.
        private const string UserStoryInfo = "value-userStoryInfo";
        public static User user = new User();
        public static UserStory userStory = new UserStory();
        public static DialogTrace trace = new DialogTrace();
        public static List<string[,]> listOfUserStories = new List<string[,]>();
        public static Random random = new Random();

        public MainFlowDialog(UserState userState) : base(nameof(MainFlowDialog))
        {
            LoadAllDialog.LoadDialog();
            using (var reader = new StreamReader(@"..\input.csv"))
            {
                reader.ReadLine();
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    if (values[3] != "")
                    {
                        MainFlowDialog.appName = values[3];
                    }
                    if (values[4] != "")
                    {
                        MainFlowDialog.waitParametrics = Int32.Parse(values[4]);
                    }
                    if (values[5] != "")
                    {
                        if (values[5] == "on")
                        {
                            MainFlowDialog.AmbiguityDetection = true;
                        }
                        else
                        {
                            MainFlowDialog.AmbiguityDetection = false;
                        }
                    }
                    if (values[6] != "")
                    {
                        MainFlowDialog.participantNo = Int32.Parse(values[6]);
                    }
                    //Set scenario standard input for component and role
                    if (appName == "Maps")
                    {
                        if (values[7] != "")
                        {
                            MainFlowDialog.Components.Add(values[7]);
                        }
                        if (values[8] != "")
                        {
                            MainFlowDialog.Roles.Add(values[8]);
                        }
                    }
                    if (appName == "Facebook")
                    {
                        if (values[9] != "")
                        {
                            MainFlowDialog.Components.Add(values[9]);
                        }
                        if (values[10] != "")
                        {
                            MainFlowDialog.Roles.Add(values[10]);
                        }
                    }
                    if (appName == "Youtube")
                    {
                        if (values[11] != "")
                        {
                            MainFlowDialog.Components.Add(values[11]);

                        }
                        if (values[12] != "")
                        {
                            MainFlowDialog.Roles.Add(values[12]);
                        }
                    }
                    if (appName == "Thuisbezorgd")
                    {
                        if (values[13] != "")
                        {
                            MainFlowDialog.Components.Add(values[13]);

                        }
                        if (values[14] != "")
                        {
                            MainFlowDialog.Roles.Add(values[14]);
                        }
                    }
                    if (appName != "Maps" && appName != "Facebook" && appName != "Youtube" && appName != "Thuisbezorgd")
                    {
                        if (values[1] != "")
                        {
                            MainFlowDialog.Components.Add(values[1]);
                        }
                        if (values[2] != "")
                        {
                            MainFlowDialog.Roles.Add(values[2]);
                        }
                    }
                }
            }


                _userStoryAccessor = userState.CreateProperty<UserStory>("UserProfile");
            AddDialog(new RequirementTypeDialog());
            AddDialog(new ExistingFeatureDialog());
            AddDialog(new MeansDialog());            
            AddDialog(new EndsDialog());
            AddDialog(new RoleDialog());
            AddDialog(new ConfirmationDialog());
            AddDialog(new ResolveVagueAmbiguityDialog());
            AddDialog(new KeepNotifiedDialog());
            AddDialog(new RequestMailDialog());
            AddDialog(new ClosingDialog());


            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                OpeningMessageStepAsync,
                //RequestedNameStepAsync,
                RequestedAnonymousStepAsync,
                //WelcomeMessageStepAsync,
                //ReqTypeStepAsync,
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            //AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), AgePromptValidatorAsync));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
            
        }

        private async Task<DialogTurnResult> OpeningMessageStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var dialogOptions = AllDialog.RequestName;
            var msg = OutputRandomizer.StringRandomizer(dialogOptions);
            var typingMsg = stepContext.Context.Activity.CreateReply();
            typingMsg.Type = ActivityTypes.Typing;
            typingMsg.Text = null;
            await stepContext.Context.SendActivityAsync(typingMsg);
            await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
            /*
            stepContext.Context.Activity.From.Name = "iets";
            var userName = stepContext.Context.Activity.From.Name;
            await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(userName) }, cancellationToken);
            */
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);

        }

        /*
        private static async Task<DialogTurnResult> RequestedNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            user.Name = (string)stepContext.Result;
            var dialogOptions = AllDialog.RequestAnonymity;
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
                    RetryPrompt = MessageFactory.Text("Please choose one of the options below."),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Stay anonymous", "Attach my name" }),
                }, cancellationToken);
        }*/

        private static async Task<DialogTurnResult> RequestedAnonymousStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            /*
            MainFlowDialog.user.Anonymity = ((FoundChoice)stepContext.Result).Value;
            
            var dialogOptions = AllDialog.ExplainHelp;
            var rndmsg = OutputRandomizer.StringRandomizer(dialogOptions);
            var msg = rndmsg.Replace("{MainFlowDialog.user.Name}", MainFlowDialog.user.Name);
            var typingMsg = stepContext.Context.Activity.CreateReply();
            typingMsg.Type = ActivityTypes.Typing;
            typingMsg.Text = null;
            await stepContext.Context.SendActivityAsync(typingMsg);
            await Task.Delay(MainFlowDialog.waitParametrics * (msg.Length));
            await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
            */

            user.Name = (string)stepContext.Result;

            var dialogOptions2 = AllDialog.ExplainHelp2;
            var rndmsg2 = OutputRandomizer.StringRandomizer(dialogOptions2);
            var msg2 = rndmsg2.Replace("{MainFlowDialog.appName}", MainFlowDialog.appName).Replace("{MainFlowDialog.user.Name}", MainFlowDialog.user.Name);
            var typingMsg2 = stepContext.Context.Activity.CreateReply();
            typingMsg2.Type = ActivityTypes.Typing;
            typingMsg2.Text = null;
            await stepContext.Context.SendActivityAsync(typingMsg2);
            await Task.Delay(MainFlowDialog.waitParametrics * (msg2.Length));
            await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg2) }, cancellationToken);
            return await stepContext.BeginDialogAsync(nameof(RequirementTypeDialog), null, cancellationToken);

        }

        /*
        private static async Task<DialogTurnResult> WelcomeMessageStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //            stepContext.Values[UserStoryInfo] = new UserStory();

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            //await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Hello, I am ReqBot and I will assist you in communicating your wishes for and problems with this system.") }, cancellationToken);
            MainFlowDialog.trace.CurrentDialog = "MainFlowDialog";
            if (MainFlowDialog.userStory.RequirementsSubmitted > 0)
            {
                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Do you want to request a new feature or request a change in an existing feature?"),
                        RetryPrompt = MessageFactory.Text("Please choose an option from the list."),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "New feature", "Existing feature"}),
                    }, cancellationToken);
            }
            else
            {


                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Hello, I am ReqBot and I will assist you in communicating your wishes for and problems with this system. If you get stuck at any point, just type \"/help\"" + Environment.NewLine + "First of all, do you want to request a new feature or request a change in an existing feature?"),
                        RetryPrompt = MessageFactory.Text("Please choose an option from the list."),
                        Choices = ChoiceFactory.ToChoices(new List<string> { "New feature", "Existing feature"}),
                    }, cancellationToken);
            }
        }

        private static async Task<DialogTurnResult> ReqTypeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Set the user's name to what they entered in response to the name prompt.
            //var userStory = (UserStory)stepContext.Values[UserStoryInfo];
            //            stepContext.Values["reqtype"] = ((FoundChoice)stepContext.Result).Value;
            userStory.ReqType = ((FoundChoice)stepContext.Result).Value;

            if (userStory.ReqType == "New feature")
            {
                //return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("All right. Can you please describe what you want in one sentence starting with \"I want\"?") }, cancellationToken);
                // Otherwise, start the review selection dialog.
                return await stepContext.BeginDialogAsync("MeansDialog", null, cancellationToken);

            }
            else if (userStory.ReqType == "Existing feature")
            {
                //return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("About what feature are we talking?") }, cancellationToken);
                // If they are too young, skip the review selection dialog, and pass an empty list to the next step.
                //var msg = $"I have constructed the following requirement: As a{MainFlowDialog.userStory.ReqType}";
                //await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
                //return await stepContext.NextAsync(null, cancellationToken);
                return await stepContext.BeginDialogAsync(nameof(ExistingFeatureDialog), null, cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Sorry I do not support this option yet...") }, cancellationToken);
            }
        }
        */
        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnBeginDialogAsync(innerDc, options, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }
        
        private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message)
            {
                var text = innerDc.Context.Activity.Text.ToLowerInvariant();
                /*
                if (text.Length < 3)
                {
                    await innerDc.Context.SendActivityAsync($"Your answer is too short. Please elaborate more", cancellationToken: cancellationToken);
                    return new DialogTurnResult(DialogTurnStatus.Waiting);
                }
                
                if (text == "no")
                {
                    await innerDc.Context.SendActivityAsync($"The answer \"No\" is not considered a valid response. Please elaborate more", cancellationToken: cancellationToken);
                    return new DialogTurnResult(DialogTurnStatus.Waiting);
                }
                switch (text)
                {
                    case "\\help":
                        await innerDc.Context.SendActivityAsync($"It seems you're stuck. You can use any of the commands below or continue the conversation like nothing happened." + Environment.NewLine + 
                            "\\exit - Exit the current " + Environment.NewLine +
                            "\\back - Go 1 step back in the conversation, e.g. to change your last answer" + Environment.NewLine +
                            "", cancellationToken: cancellationToken);
                        return new DialogTurnResult(DialogTurnStatus.Waiting);

                    case "\\cancel":
                        await innerDc.Context.SendActivityAsync($"Cancelling", cancellationToken: cancellationToken);
                        System.Environment.Exit(1);
                        return await innerDc.CancelAllDialogsAsync();
                    case "\\back":
                        await innerDc.Context.SendActivityAsync($"Let's redo that step...", cancellationToken: cancellationToken);
                        return await innerDc.BeginDialogAsync(MainFlowDialog.trace.PreviousDialog, null, cancellationToken);
                }
                */
            }

            return null;
        }        
    }
}