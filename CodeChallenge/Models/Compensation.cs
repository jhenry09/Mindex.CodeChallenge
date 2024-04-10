using System;

namespace CodeChallenge.Models;

public class Compensation
{
    public string CompensationId { get; set; }
    public Employee Employee { get; set; }
    public int Salary { get; set; }
    
    // TODO This date parsing could be more user friendly 
    public DateTime EffectiveDate { get; set; }
}