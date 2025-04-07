using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTechnology.API;
using WebTechnology.Repository.UnitOfWork;

namespace WebTechnology.Service.Services.BackgroundServices
{
    public class UserAuthCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(3);

        public UserAuthCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    try
                    {
                        await unitOfWork.BeginTransactionAsync();

                        var dbContext = scope.ServiceProvider.GetRequiredService<WebTech>();

                        var verifiedUsers = await dbContext.Users
                            .Where(u => u.Authenticate == true &&
                                       u.VerifiedAt != null &&
                                       u.VerifiedAt <= DateTime.UtcNow.AddHours(-3))
                            .ToListAsync();

                        foreach (var user in verifiedUsers)
                        {
                            user.CountAuth = 0;
                        }

                        var unverifiedUsers = await dbContext.Users
                            .Where(u => u.Authenticate == false &&
                                       u.VerifiedAt != null &&
                                       u.VerifiedAt <= DateTime.UtcNow.AddHours(-3))
                            .ToListAsync();

                        dbContext.Users.RemoveRange(unverifiedUsers);

                        await unitOfWork.SaveChangesAsync();
                        await unitOfWork.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        await unitOfWork.RollbackAsync();
                    }
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
