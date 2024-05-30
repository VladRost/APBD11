namespace APBD10.DTOs;

public class PrescriptionDto
{
    public PatientDto Patient { get; set; }
    public DoctorDto Doctor { get; set; }
    public int IdPatient { get; set; }
    public int DoctorId { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public List<MedicamentDto> Medicaments { get; set; }
}