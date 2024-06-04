using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APBD10.Context;
using APBD10.Controllers;
using APBD10.DTOs;
using APBD10.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class PatientControllerTests
{
    private readonly HospitalDbContext _context;
    private readonly PatientController _controller;

    public PatientControllerTests()
    {
        var options = new DbContextOptionsBuilder<HospitalDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new HospitalDbContext(options);
        _controller = new PatientController(_context);
        
        var patient = new Patient
        {
            IdPatient = 1,
            FirstName = "John",
            LastName = "Doe",
            Birthdate = new DateTime(1990, 1, 1),
            Prescriptions = new List<Prescription>
            {
                new Prescription
                {
                    IdPrescription = 1,
                    Date = DateTime.Now.AddDays(-10),
                    DueDate = DateTime.Now.AddDays(10),
                    Doctor = new Doctor { IdDoctor = 1, FirstName = "Alice", LastName = "Smith", Email = "pediatrics@gmail.com" },
                    PrescriptionMedicaments = new List<Prescription_Medicament>
                    {
                        new Prescription_Medicament
                        {
                            Medicament = new Medicament { IdMedicament = 1, Name = "Medicament 1", Description = "Description 1" },
                            Dose = 1
                        }
                    }
                }
            }
        };
        _context.Patients.Add(patient);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetPatientDetails_ReturnsPatientDetails_WhenPatientExists()
    {
        var result = await _controller.GetPatient(1);
        var okResult = result as OkObjectResult;
        var patientDetails = okResult.Value as PatientDetailsResponse;

        Assert.NotNull(okResult);
        Assert.Equal(1, patientDetails.Id);
        Assert.Equal("John", patientDetails.FirstName);
        Assert.Equal("Doe", patientDetails.LastName);
        Assert.Single(patientDetails.Prescriptions);
        Assert.Equal("Medicament 1", patientDetails.Prescriptions[0].Medicaments[0].Name);
    }

    [Fact]
    public async Task GetPatientDetails_ReturnsNotFound_WhenPatientDoesNotExist()
    {
        var result = await _controller.GetPatient(999);
        Assert.IsType<NotFoundResult>(result);
    }
}
