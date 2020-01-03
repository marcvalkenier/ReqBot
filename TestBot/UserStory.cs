// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace ReqBot
{
    /// <summary>
    /// This is our application state. Just a regular serializable .NET class.
    /// </summary>
    public class UserStory
    {
        public string UserStoryCode { get; set; }

        public string CompleteUserStory { get; set; }

        public string ReqType { get; set; }

        public string ExistingFeature { get; set; }

        public string Means { get; set; }

        public string Ends { get; set; }

        public string Role { get; set; }

        public bool UserStoryChanged { get; set; }

        public string OldMeans { get; set; }

        public string OldEnds { get; set; }

        public string OldRole { get; set; }

        public int RequirementsSubmitted { get; set; }


        //Values to disambiguate vagueness
        public string DetectedVagueness { get; set; }

        public string MethodToDisambiguateVagueness { get; set; }

        public string CurrentAmbiguityLocation { get; set; }



        //Values to disambiguate the content in means
        public string DetectedMeansVagueness { get; set; }

        public string MethodToDisambiguateMeansVagueness { get; set; }

        public string DisambiguationMeansVagueness { get; set; }


        //Values to disambiguate the content in ends
        public string DetectedEndsVagueness { get; set; }

        public string MethodToDisambiguateEndsVagueness { get; set; }

        public string DisambiguationEndsVagueness { get; set; }

        public string DetectedTriggerReferential { get; set; }

        public string DetectedNounsReferential { get; set; }

        public string DetectedReferential { get; set; }

        public string DisambiguationEndsReferential { get; set; }
    }
}