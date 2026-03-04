using System.Collections.Generic;
using System.Threading.Tasks;
using TracNghiemLaiXe.Models;

namespace TracNghiemLaiXe.Services
{
    public interface IExamGeneratorService
    {
        Task<IReadOnlyList<QuizQuestion>> GenerateExamAsync(LicenseType licenseType);
    }
}
