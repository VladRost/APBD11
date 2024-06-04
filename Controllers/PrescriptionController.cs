using APBD10.Context;
using APBD10.DTOs;
using APBD10.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APBD10.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PrescriptionController:ControllerBase
{
    private readonly HospitalDbContext _dbContext;

    public PrescriptionController(HospitalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePrescription([FromBody] NewPrescriptionRequest prescriptionDto)
    {
        if (prescriptionDto.DueDate < prescriptionDto.Date)
        {
            return BadRequest("Due date must be greater than date.");
        }
        if (prescriptionDto.Medicaments.Count > 10)
        {
            return BadRequest("Prescription 10 medicaments max.");
        }
        var patient = await _dbContext.Patients.FindAsync(prescriptionDto.Patient.Id);
        
        if (patient == null)
        {
            patient = new Patient
            {
                FirstName = prescriptionDto.Patient.FirstName,
                LastName = prescriptionDto.Patient.LastName,
                Birthdate = prescriptionDto.Patient.DateOfBirth
            };
            _dbContext.Patients.Add(patient);
            await _dbContext.SaveChangesAsync();
            
            var doctor = await _dbContext.Doctors.FindAsync(prescriptionDto.DoctorId);
            if (doctor == null)
            {
                return BadRequest("Doctor not found.");
            }

            foreach (var med in prescriptionDto.Medicaments)
            {
                if (!await _dbContext.Medicaments.AnyAsync(m => m.IdMedicament == med.Id))
                {
                    return BadRequest($"Medicament with Id {med.Id} not found.");
                }
            }

            var prescription = new Prescription
            {
                Date = prescriptionDto.Date,
                DueDate = prescriptionDto.DueDate,
                IdPatient = patient.IdPatient,
                IdDoctor= doctor.IdDoctor,
                PrescriptionMedicaments = prescriptionDto.Medicaments.Select(m => new Prescription_Medicament
                {
                    IdMedicament= m.Id,
                    Dose = m.Dose
                }).ToList()
            };

            _dbContext.Prescriptions.Add(prescription);
            await _dbContext.SaveChangesAsync();

            return Ok(prescription);
        }
        return Ok();
    }
}