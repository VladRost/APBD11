namespace APBD10.DTOs;

public class NewPrescriptionRequest
{
    public PatientDto Patient { get; set; }
    public int DoctorId { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
    public List<MedicamentDto> Medicaments { get; set; }
}