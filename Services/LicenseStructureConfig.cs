using System.Collections.Generic;
using TracNghiemLaiXe.Models;

namespace TracNghiemLaiXe.Services
{
    public class ExamStructure
    {
        public int TotalQuestions { get; set; }
        public int RequiredCorrect { get; set; }
        public int DurationMinutes { get; set; }
        public int FatalQuestionCount { get; set; } = 1; // Standard GTVT
        
        // You might expand this with specific Category distributions if needed
        // For now, simple total count + mandatory fatal is the key constraint
    }

    public static class LicenseStructureConfig
    {
        public static readonly Dictionary<LicenseType, ExamStructure> Structures = new()
        {
            {
                LicenseType.A1, new ExamStructure 
                { 
                    TotalQuestions = 25, 
                    RequiredCorrect = 21, 
                    DurationMinutes = 19,
                    FatalQuestionCount = 1
                }
            },
            {
                LicenseType.A2, new ExamStructure 
                { 
                    TotalQuestions = 25, 
                    RequiredCorrect = 23, 
                    DurationMinutes = 19,
                    FatalQuestionCount = 1
                }
            },
            {
                LicenseType.B1, new ExamStructure 
                { 
                    TotalQuestions = 30, 
                    RequiredCorrect = 27, 
                    DurationMinutes = 20,
                    FatalQuestionCount = 1
                }
            },
            {
                LicenseType.B2, new ExamStructure 
                { 
                    TotalQuestions = 35, 
                    RequiredCorrect = 32, 
                    DurationMinutes = 22,
                    FatalQuestionCount = 1
                }
            },
            // Add C, D, E, F as needed mapping to similar logic
            {
                LicenseType.C, new ExamStructure 
                { 
                    TotalQuestions = 40, 
                    RequiredCorrect = 36, 
                    DurationMinutes = 24,
                    FatalQuestionCount = 1
                }
            }
        };

        public static ExamStructure GetStructure(LicenseType type)
        {
            if (Structures.TryGetValue(type, out var structure))
                return structure;
                
            // Fallback default
            return new ExamStructure { TotalQuestions = 25, RequiredCorrect = 21, DurationMinutes = 19 };
        }
    }
}
