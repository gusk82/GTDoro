﻿using GTDoro.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTDoro.Core.ViewModels
{
    public class CollectedThingDateViewModel : DateItemViewModel
    {
        public IEnumerable<CollectedThing> Items { get; set; }
    }
}