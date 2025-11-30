using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Models;

[Table("ConditionReadings")]
public class ConditionReading
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int AssetId { get; set; }
    
    public DateTime ReadingDate { get; set; } = DateTime.Now;
    
    public double? Temperature { get; set; } // Fahrenheit
    
    public double? Vibration { get; set; } // mm/s
    
    public double? Pressure { get; set; } // PSI
    
    [MaxLength(50)]
    public string? OilAnalysis { get; set; } // Normal, Warning, Critical
    
    public double? Current { get; set; } // Amps
    
    public double? Voltage { get; set; } // Volts
    
    public double? NoiseLevel { get; set; } // Decibels
    
    public double? FlowRate { get; set; } // GPM or similar
    
    [MaxLength(2000)]
    public string? Notes { get; set; }
    
    [MaxLength(200)]
    public string RecordedBy { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string OverallStatus { get; set; } = "Normal"; // Normal, Warning, Critical
    
    public bool AlertGenerated { get; set; }
    
    // Navigation property
    [ForeignKey("AssetId")]
    public virtual Asset? Asset { get; set; }
}
