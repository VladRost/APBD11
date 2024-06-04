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

public class PrescriptionControllerTests
{
    private readonly HospitalDbContext _context;
    private readonly PrescriptionController _controller;

    public PrescriptionControllerTests()
    {
        var options = new DbContextOptionsBuilder<HospitalDbContext>()
            .UseInMemoryDatabase(databaseName: "Testdatabase")
            .Options;
        _context = new HospitalDbContext(options);
        _controller = new PrescriptionController(_context);
        
        _context.Medicaments.AddRange(
            new Medicament { IdMedicament = 1, Name = "Medicament 1", Description = "Description 1" },
            new Medicament { IdMedicament = 2, Name = "Medicament 2", Description = "Description 2" }
        );

        _context.Doctors.Add(new Doctor { IdDoctor = 1, FirstName = "John", LastName = "Doe", Email = "cars@gmail.com" });
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreatePrescription_ReturnsBadRequest_WhenMoreThan10Medicaments()
    {
        var request = new NewPrescriptionRequest
        {
            Patient = new PatientDto { FirstName = "Jane", LastName = "Doe", DateOfBirth = new DateTime(1980, 1, 1) },
            DoctorId = 1,
            Date = DateTime.Now,
            DueDate = DateTime.Now.AddDays(10),
            Medicaments = new List<MedicamentDto>()
        };

        for (int i = 1; i <= 11; i++)
        {
            request.Medicaments.Add(new MedicamentDto { Id = i, Name = $"Medicament {i}", Dose = 1 });
        }

        var result = await _controller.CreatePrescription(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreatePrescription_ReturnsBadRequest_WhenDueDateIsBeforeDate()
    {
        var request = new NewPrescriptionRequest
        {
            Patient = new PatientDto { FirstName = "Jane", LastName = "Doe", DateOfBirth = new DateTime(1980, 1, 1) },
            DoctorId = 1,
            Date = DateTime.Now,
            DueDate = DateTime.Now.AddDays(-1),
            Medicaments = new List<MedicamentDto>
            {
                new MedicamentDto { Id = 1, Name = "Medicament 1", Dose = 1 }
            }
        };

        var result = await _controller.CreatePrescription(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task CreatePrescription_CreatesNewPatient_WhenPatientDoesNotExist()
    {
        var request = new NewPrescriptionRequest
        {
            Patient = new PatientDto { FirstName = "Jane", LastName = "Doe", DateOfBirth = new DateTime(1980, 1, 1) },
            DoctorId = 1,
            Date = DateTime.Now,
            DueDate = DateTime.Now.AddDays(10),
            Medicaments = new List<MedicamentDto>
            {
                new MedicamentDto { Id = 1, Name = "Medicament 1", Dose = 1 }
            }
        };

        var result = await _controller.CreatePrescription(request);
        var okResult = result as OkObjectResult;

        Assert.NotNull(okResult);
        Assert.NotNull(_context.Patients.FirstOrDefault(p => p.FirstName == "Jane" && p.LastName == "Doe"));
    }
}
