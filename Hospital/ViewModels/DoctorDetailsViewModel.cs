﻿using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.ViewModels
{
    public class DoctorDetailsViewModel
    {
        public Doctor Doctor { get; set; }
        public List<Doctor> doctor { get; set; }
        public string specializationName { get; set; }
    }
}
