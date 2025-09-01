using Microsoft.AspNetCore.Identity;
using TaskManagementMvc.Data;
using TaskManagementMvc.Models;
using TaskManagementMvc.Controllers;

namespace TaskManagementMvc.Services.Registration
{
    public record PipelineError(string Key, string Message);

    public class RegisterPipelineResult
    {
        public bool Succeeded { get; init; }
        public ApplicationUser? User { get; init; }
        public Company? Company { get; init; }
        public List<PipelineError> Errors { get; } = new();
        public static RegisterPipelineResult Success(ApplicationUser user, Company company)
            => new RegisterPipelineResult { Succeeded = true, User = user, Company = company };
        public static RegisterPipelineResult Failure(IEnumerable<PipelineError> errors)
        {
            var result = new RegisterPipelineResult { Succeeded = false, User = null, Company = null };
            result.Errors.AddRange(errors);
            return result;
        }
    }

    public class RegisterContext
    {
        public RegisterViewModel Model { get; init; }
        public Company? Company { get; set; }
        public ApplicationUser? User { get; set; }
        public List<PipelineError> Errors { get; } = new();

        public RegisterContext(RegisterViewModel model)
        {
            Model = model;
        }
    }

    public interface IRegisterPipeline
    {
        Task<RegisterPipelineResult> RunAsync(RegisterViewModel model, CancellationToken ct = default);
    }

    public class RegisterPipeline : IRegisterPipeline
    {
        private readonly TaskManagementContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RegisterPipeline(
            TaskManagementContext db,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<RegisterPipelineResult> RunAsync(RegisterViewModel model, CancellationToken ct = default)
        {
            var ctx = new RegisterContext(model);

            // Early validation: email unique
            var email = model.Email.Trim();
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                ctx.Errors.Add(new PipelineError("Email", "این ایمیل قبلاً ثبت شده است."));
                return RegisterPipelineResult.Failure(ctx.Errors);
            }

            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                // 1) Create company
                var company = new Company
                {
                    Name = model.CompanyName.Trim(),
                    Description = model.Description,
                    Phone = model.Phone,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Companies.Add(company);
                await _db.SaveChangesAsync(ct);
                ctx.Company = company;

                // 2) Create user
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = model.FullName.Trim(),
                    CompanyId = company.Id,
                    IsActive = true,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };
                var createRes = await _userManager.CreateAsync(user, model.Password);
                if (!createRes.Succeeded)
                {
                    foreach (var err in createRes.Errors)
                        ctx.Errors.Add(new PipelineError(string.Empty, err.Description));

                    await tx.RollbackAsync(ct);
                    return RegisterPipelineResult.Failure(ctx.Errors);
                }
                ctx.User = user;

                // 3) Ensure CompanyManager role exists
                var cmRole = await _roleManager.FindByNameAsync(Roles.CompanyManager);
                if (cmRole == null)
                {
                    var createRoleRes = await _roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = Roles.CompanyManager,
                        Description = "مدیر شرکت",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                    if (!createRoleRes.Succeeded)
                    {
                        foreach (var err in createRoleRes.Errors)
                            ctx.Errors.Add(new PipelineError(string.Empty, err.Description));

                        await tx.RollbackAsync(ct);
                        return RegisterPipelineResult.Failure(ctx.Errors);
                    }
                }

                // 4) Assign role
                var addToRoleRes = await _userManager.AddToRoleAsync(user, Roles.CompanyManager);
                if (!addToRoleRes.Succeeded)
                {
                    foreach (var err in addToRoleRes.Errors)
                        ctx.Errors.Add(new PipelineError(string.Empty, err.Description));

                    await tx.RollbackAsync(ct);
                    return RegisterPipelineResult.Failure(ctx.Errors);
                }

                // 5) Persist in custom UserRoles table if not exists
                var roleEntity = await _roleManager.FindByNameAsync(Roles.CompanyManager);
                if (roleEntity != null)
                {
                    var existsLink = _db.UserRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == roleEntity.Id && ur.IsActive);
                    if (!existsLink)
                    {
                        _db.UserRoles.Add(new UserRole
                        {
                            UserId = user.Id,
                            RoleId = roleEntity.Id,
                            AssignedAt = DateTime.UtcNow,
                            AssignedById = null,
                            IsActive = true,
                            Notes = "Assigned on registration"
                        });
                        await _db.SaveChangesAsync(ct);
                    }
                }

                await tx.CommitAsync(ct);
                return RegisterPipelineResult.Success(user, company);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(ct);
                ctx.Errors.Add(new PipelineError(string.Empty, "خطا در ثبت‌نام. لطفاً دوباره تلاش کنید."));
                ctx.Errors.Add(new PipelineError("Exception", ex.Message));
                return RegisterPipelineResult.Failure(ctx.Errors);
            }
        }
    }
}

