// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace ReqBot
{
    /// <summary>
    /// This is our application state. Just a regular serializable .NET class.
    /// </summary>
    public class DialogTrace
    {
        public string CurrentDialog { get; set; }

        public string PreviousDialog { get; set; }
    }
}