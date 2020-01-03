using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReqBot
{
    public class LoadAllDialog
    {
        public static void LoadDialog()
        {
            using (var reader = new StreamReader(@"..\dialog.csv"))
            {
                int t = 0;
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    listA.Add(values[0]);
                    listB.Add(values[1]);
                }
                foreach (string line in listA)
                {
                    List<string> dialogOpltions = new List<string>();
                    var options = listB[t];
                    var optionsB = options;
                    dialogOpltions = options.Split(';').ToList();
                    if("RequestName" == listA[t])
                    {
                        AllDialog.RequestName = dialogOpltions;
                    }
                    if ("RequestAnonymity" == listA[t])
                    {
                        AllDialog.RequestAnonymity = dialogOpltions;
                    }
                    if ("ExplainHelp" == listA[t])
                    {
                        AllDialog.ExplainHelp = dialogOpltions;
                    }
                    if ("ExplainHelp2" == listA[t])
                    {
                        AllDialog.ExplainHelp2 = dialogOpltions;
                    }

                    if ("RequestRequirementType" == listA[t])
                    {
                        AllDialog.RequestRequirementType = dialogOpltions;
                    }

                    if ("RequestExistingFeature" == listA[t])
                    {
                        AllDialog.RequestExistingFeature = dialogOpltions;
                    }
                    if ("RequestOtherExistingFeature" == listA[t])
                    {
                        AllDialog.RequestOtherExistingFeature = dialogOpltions;
                    }

                    if ("RequestMeansNew" == listA[t])
                    {
                        AllDialog.RequestMeansNew = dialogOpltions;
                    }
                    if ("RequestMeansExisting" == listA[t])
                    {
                        AllDialog.RequestMeansExisting = dialogOpltions;
                    }
                    if ("RequestChangeMeans" == listA[t])
                    {
                        AllDialog.RequestChangeMeans = dialogOpltions;
                    }
                    if ("RespondMeans" == listA[t])
                    {
                        AllDialog.RespondMeans = dialogOpltions;
                    }
                    if ("RespondChangedMeans" == listA[t])
                    {
                        AllDialog.RespondChangedMeans = dialogOpltions;
                    }

                    if ("RequestEnds" == listA[t])
                    {
                        AllDialog.RequestEnds = dialogOpltions;
                    }
                    if ("RequestChangeEnds" == listA[t])
                    {
                        AllDialog.RequestChangeEnds = dialogOpltions;
                    }
                    if ("RespondEnds" == listA[t])
                    {
                        AllDialog.RespondEnds = dialogOpltions;
                    }
                    if ("RespondChangeEnds" == listA[t])
                    {
                        AllDialog.RespondChangeEnds = dialogOpltions;
                    }

                    if ("RequestRole" == listA[t])
                    {
                        AllDialog.RequestRole = dialogOpltions;
                    }
                    if ("RequestChangeRole" == listA[t])
                    {
                        AllDialog.RequestChangeRole = dialogOpltions;
                    }
                    if ("RequestOtherRole" == listA[t])
                    {
                        AllDialog.RequestOtherRole = dialogOpltions;
                    }
                    if ("RequestChangeOtherRole" == listA[t])
                    {
                        AllDialog.RequestChangeOtherRole = dialogOpltions;
                    }
                    if ("RequestChangeNoRoles" == listA[t])
                    {
                        AllDialog.RequestChangeNoRoles = dialogOpltions;
                    }
                    if ("RespondRole" == listA[t])
                    {
                        AllDialog.RespondRole = dialogOpltions;
                    }
                    if ("RespondChangeRole" == listA[t])
                    {
                        AllDialog.RespondChangeRole = dialogOpltions;
                    }
                    if ("RespondOtherRole" == listA[t])
                    {
                        AllDialog.RespondOtherRole = dialogOpltions;
                    }
                    if ("RespondChangeOtherRole" == listA[t])
                    {
                        AllDialog.RespondChangeOtherRole = dialogOpltions;
                    }
                    if ("RequestConfirmationOrChange" == listA[t])
                    {
                        AllDialog.RequestConfirmationOrChange = dialogOpltions;
                    }
                    if ("RespondChange" == listA[t])
                    {
                        AllDialog.RespondChange = dialogOpltions;
                    }
                    if ("RespondConfirmation" == listA[t])
                    {
                        AllDialog.RespondConfirmation = dialogOpltions;
                    }
                    if ("RequestNotificationOption" == listA[t])
                    {
                        AllDialog.RequestNotificationOption = dialogOpltions;
                    }
                    if ("RequestChangeNotificationOption" == listA[t])
                    {
                        AllDialog.RequestChangeNotificationOption = dialogOpltions;
                    }
                    if ("RequestChangeNotificationOption2" == listA[t])
                    {
                        AllDialog.RequestChangeNotificationOption2 = dialogOpltions;
                    }

                    if ("RespondChangeNotificationOption" == listA[t])
                    {
                        AllDialog.RespondChangeNotificationOption = dialogOpltions;
                    }
                    if ("RequestMail" == listA[t])
                    {
                        AllDialog.RequestMail = dialogOpltions;
                    }
                    if ("RespondToMailRequest" == listA[t])
                    {
                        AllDialog.RespondToMailRequest = dialogOpltions;
                    }
                    if ("RequestMailBadFormat" == listA[t])
                    {
                        AllDialog.RequestMailBadFormat = dialogOpltions;
                    }
                    if ("RequestAnotherRequirement" == listA[t])
                    {
                        AllDialog.RequestAnotherRequirement = dialogOpltions;
                    }
                    if ("RespondClosingDialog" == listA[t])
                    {
                        AllDialog.RespondClosingDialog = dialogOpltions;
                    }
                    if ("RequestVagueDisambiguation" == listA[t])
                    {
                        AllDialog.RequestVagueDisambiguation = dialogOpltions;
                    }
                    if ("RespondVagueDisambiguation" == listA[t])
                    {
                        AllDialog.RespondVagueDisambiguation = dialogOpltions;
                    }
                    if ("RequestReferentialDisambiguation" == listA[t])
                    {
                        AllDialog.RequestReferentialDisambiguation = dialogOpltions;
                    }
                    if ("RequestNotListedReferentialDisambiguation" == listA[t])
                    {
                        AllDialog.RequestNotListedReferentialDisambiguation = dialogOpltions;
                    }
                    if ("RespondReferentialDisambiguation" == listA[t])
                    {
                        AllDialog.RespondReferentialDisambiguation = dialogOpltions;
                    }

                    t++;
                }
            }
        }

    }

}

