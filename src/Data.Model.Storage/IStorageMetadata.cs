﻿using System;
using System.Collections.Generic;

namespace Data.Model.Storage
{
    public interface IStorageMetadata
    {
        /// <summary>
        ///     Provides flexible tag options for each derived type
        /// </summary>
        List<Tag> Tags { get; set; }
        long Index { get; set; }
    }
}
