using System;
using System.Collections.Generic;

namespace TracNghiemLaiXe.Models
{
    public class LicenseConfig
    {
        public int TotalQuestions { get; set; }
        public int PassingScore { get; set; }
        public int ExamDurationMinutes { get; set; }
        public int FatalQuestionCount { get; set; }

        public static LicenseConfig Get(LicenseType type)
        {
            return type switch
            {
                LicenseType.A1 => new LicenseConfig { TotalQuestions = 25, PassingScore = 21, ExamDurationMinutes = 19, FatalQuestionCount = 2 },
                LicenseType.A2 => new LicenseConfig { TotalQuestions = 25, PassingScore = 23, ExamDurationMinutes = 19, FatalQuestionCount = 2 },
                LicenseType.A3 => new LicenseConfig { TotalQuestions = 25, PassingScore = 23, ExamDurationMinutes = 19, FatalQuestionCount = 2 },
                LicenseType.A4 => new LicenseConfig { TotalQuestions = 25, PassingScore = 23, ExamDurationMinutes = 19, FatalQuestionCount = 2 },
                LicenseType.B1 => new LicenseConfig { TotalQuestions = 30, PassingScore = 27, ExamDurationMinutes = 20, FatalQuestionCount = 2 },
                LicenseType.B2 => new LicenseConfig { TotalQuestions = 35, PassingScore = 32, ExamDurationMinutes = 22, FatalQuestionCount = 3 },
                LicenseType.C => new LicenseConfig { TotalQuestions = 40, PassingScore = 36, ExamDurationMinutes = 24, FatalQuestionCount = 3 },
                LicenseType.D => new LicenseConfig { TotalQuestions = 45, PassingScore = 41, ExamDurationMinutes = 26, FatalQuestionCount = 4 },
                LicenseType.E => new LicenseConfig { TotalQuestions = 45, PassingScore = 41, ExamDurationMinutes = 26, FatalQuestionCount = 4 },
                LicenseType.F => new LicenseConfig { TotalQuestions = 45, PassingScore = 41, ExamDurationMinutes = 26, FatalQuestionCount = 4 },
                _ => new LicenseConfig { TotalQuestions = 25, PassingScore = 21, ExamDurationMinutes = 19, FatalQuestionCount = 2 }
            };
        }
    }
}
