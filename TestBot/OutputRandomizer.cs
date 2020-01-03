// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.IO;
using System;

namespace ReqBot
{
    public class OutputRandomizer
    {
        public static string StringRandomizer(List<string> responseOptions)
        {
            var random = new Random();
            int index = random.Next(responseOptions.Count);
            return (responseOptions[index]);
        }
    }
}