﻿using Hospital.Models;
using Hospital.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AppointmentController> _logger;
        private readonly HospitalDbContext _context;
        public AppointmentController(IUnitOfWork unitOfWork, HospitalDbContext context, ILogger<AppointmentController> logger)
        {
            _unitOfWork = unitOfWork;

            _logger = logger;
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult  Create(int id)
        {
            var allDoctor = _unitOfWork.doctorRepository.GetAvailableDoctors();
            _logger.LogWarning("{allDoctor} is inside this method",allDoctor);
            
            var viewmodel = new AppointmentFormViewModel
            {
                Patient = id,
                Heading = "New Appointment",
            };
            foreach (var doctor in allDoctor)
            {
                viewmodel.DoctorList.Add(new SelectListItem()
                {
                    Value = doctor.Id.ToString(),
                    Text = doctor.Name
                });
            }
            return View(viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Create (AppointmentFormViewModel model)
        {
            if(!ModelState.IsValid)
            {
                var allDoctor = _unitOfWork.doctorRepository.GetAvailableDoctors();
                var viewmodel = new AppointmentFormViewModel
                {
                    Patient = model.Patient,
                    Heading = "New Appointment",
                };
                foreach (var doctor in allDoctor)
                {
                    viewmodel.DoctorList.Add(new SelectListItem()
                    {
                        Value = doctor.Id.ToString(),
                        Text = doctor.Name
                    });
                }
                return View(viewmodel);
            }

            var appointment = new Appointment
            {
                DateTime = model.GetStartDateTime(),
                Details = model.Detail,
                contactStatus = ContactStatus.Submitted,
                PatientId = model.Patient,
                DoctorId = model.Doctor,
            };
/*            if(_unitOfWork.appoinmentRepository.ValidateAppointment(appointment.DateTime, model.Doctor))
            {
                return View("Invalid Appointment");
            }*/
            _unitOfWork.appoinmentRepository.Add(appointment);
            _unitOfWork.Complete();

            return RedirectToAction("Appointments", "Appointment");
        }

        public async Task<IActionResult> Appointments()
        {
            var allapointments = _unitOfWork.appoinmentRepository.GetAppoinments();
            var appoint = new AppointmentViewModel();
            appoint.Appointments = allapointments.ToList();

            return View(appoint);
        }

        public IActionResult Details(int id)
        {
            var details = _unitOfWork.appoinmentRepository.GetAppointmentWithPatients(id);
            
            _logger.LogInformation(message:"{details} all details here", details.FirstOrDefault().Patient.Name);
      
            var appo = new AppointmentDetailsViewModel
            {
            
                Appointment = details.FirstOrDefault()
            };

            return View(appo);
        }

        [HttpGet]
        public async Task<IActionResult> Approval(int id)
        {

            var appointment = _unitOfWork.appoinmentRepository.GetAppointmentById(id);
            var d = appointment.Id;
            _logger.LogInformation("{d} Appointment id for Approval",d);

            appointment.contactStatus = ContactStatus.Approved;
            _unitOfWork.Complete();
            return RedirectToAction("Appointments","Appointment");
            
            
        }
    

    [HttpGet]
    public async Task<IActionResult> Rejection(int id)
    {

        var appointment = _unitOfWork.appoinmentRepository.GetAppointmentById(id);
            var d = appointment.Id;
            _logger.LogInformation("{d} Appointment id for Rejection",d);
            
        appointment.contactStatus = ContactStatus.Rejected;
        _unitOfWork.Complete();
        return RedirectToAction("Appointments", "Appointment");


    }

        public ViewResult List(string appointment)
        {
            /*            var result = _unitOfWork.appoinmentRepository.GetAppoinments();

                         IEnumerable<Appointment> appointments;
                        if (ModelState.IsValid) { 
                            if (appointment == "approved")
                            {
                                appointments = result.Where(c => c.contactStatus == ContactStatus.Approved);

                            }
                            if( appointment == "rejected")
                            {
                                appointments = result.Where(c => c.contactStatus == ContactStatus.Rejected);
                            }


                            var viewmodel = new AppointmentViewModel
                            {
                                Appointmentss = appointments,
                            };


                        }*/

            var app = _unitOfWork.appoinmentRepository.FilterAppointments(appointment);

            var viewmodel = new AppointmentViewModel
            {
                Appointment = app,
            };

            return View(viewmodel);
        }

        public async Task<IActionResult> Search()
        {
            return View();
        }
        public IActionResult SearchList()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SearchList(AppointmentViewModel model)
        {
            var result = _unitOfWork.appoinmentRepository.CustomFilterAppointment(model.StringName, model.Date);

            var viewmodel = new AppointmentViewModel
            {
                Appointment = result
            };
            
            return View(viewmodel);
        }
}
}

