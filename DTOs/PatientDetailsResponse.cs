namespace APBD10.DTOs;

public class PatientDetailsResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<PrescriptionDto> Prescriptions { get; set; }
}