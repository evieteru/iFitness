﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace iFitness
{
    //One workout that can have many sets
    public class CardioWorkout : Workout
    {
        [JsonInclude]
        public ObservableCollection<CardioSet> Sets { get; set; } = new ObservableCollection<CardioSet>();
        public override string Summary => $"{Sets.Count} cardio set(s)";
    }
}
