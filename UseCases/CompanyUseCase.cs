using BerryNameApi.Entities;
using BerryNameApi.Repositories;
using BerryNameApi.Utils;
using blueberry_homework_dotnet.DTO.Request;
using blueberry_homework_dotnet.DTO.Response;
using blueberry_homework_dotnet.Entities;
using blueberry_homework_dotnet.Repositories;
using blueberry_homework_dotnet.Utils;

namespace blueberry_homework_dotnet.UseCases
{
    public class CompanyUseCase
    {
        private readonly CompanyRepository _companyRepository;
        private readonly AuthRepository _authRepository;

        public CompanyUseCase(CompanyRepository companyRepository, AuthRepository authRepository)
        {
            _companyRepository = companyRepository;
            _authRepository = authRepository;
        }

        public Result<Unit> CreateCompany(CreateCompanyRequest request)
        {
            var user = _authRepository.FindById(request.UserId);
            if (user == null) return Result<Unit>.Fail(Constants.UserNotFound);
            if (user.Role != Role.Boss) return Result<Unit>.Fail(Constants.BossCreateCompany);

            if (_companyRepository.FindByUserId(user.Id) != null)
                return Result<Unit>.Fail(Constants.UserAlreadyCompany);

            var company = new CompanyEntity
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                CompanyName = request.CompanyName,
                CompanyAddress = request.CompanyAddress,
                TotalStaff = request.TotalStaff,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _companyRepository.CreateCompany(company);
            Console.WriteLine($"🏢 Company created: {request.CompanyName} by user {request.UserId}");
            return Result<Unit>.Ok(Unit.Value);
        }

        public Result<CompanyResponse> GetCompany(Guid userId)
        {
            var company = _companyRepository.FindByUserId(userId);
            if (company == null)
            {
                return Result<CompanyResponse>.Fail(Constants.CompanyNotFound);
            }

            var response = new CompanyResponse
            {
                Id = company.Id,
                CompanyName = company.CompanyName,
                CompanyAddress = company.CompanyAddress,
                TotalStaff = company.TotalStaff,
                CreatedAt = company.CreatedAt,
                UpdatedAt = company.UpdatedAt
            };

            return Result<CompanyResponse>.Ok(response);
        }


        public Result<Unit> ChangeCompany(Guid userId, string? name, string? address, int? staff)
        {
            var company = _companyRepository.FindByUserId(userId);
            if (company == null)
            {
                return Result<Unit>.Fail(Constants.CompanyNotFound);
            }

            if (name != null) company.CompanyName = name;
            if (address != null) company.CompanyAddress = address;
            if (staff.HasValue) company.TotalStaff = staff.Value;
            company.UpdatedAt = DateTime.UtcNow;

            _companyRepository.UpdateCompany(company);
            return Result<Unit>.Ok(Unit.Value);
        }

        public Result<Unit> DeleteCompany(Guid userId)
        {
            var company = _companyRepository.FindByUserId(userId);

            if (company == null)
            {
                return Result<Unit>.Fail(Constants.CompanyNotFound);
            }

            _companyRepository.DeleteCompany(company.Id);
            return Result<Unit>.Ok(Unit.Value);
        }
    }
}