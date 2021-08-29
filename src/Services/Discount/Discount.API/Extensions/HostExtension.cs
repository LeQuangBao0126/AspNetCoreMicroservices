using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discount.API.Extensions
{
    public static class HostExtension
    {
        public static IHost MigrateDatabase<TContext>(this IHost host , int? retry = 0)
        {
            int retryForAvailability = retry.Value;
            using (var scope = host.Services.CreateScope())
            {
                var configuration = scope.ServiceProvider.GetService<IConfiguration>();  
                var logger = scope.ServiceProvider.GetService<ILogger<TContext>>(); 
                try {
                    logger.LogInformation("bat dau seeding du lieu vao dataabase postgre");
                    using (var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionStrings"))) {
                        connection.Open();
                        var command = new NpgsqlCommand() { Connection = connection };
                        command.CommandText = "Drop table if exists Coupon";
                        command.ExecuteNonQuery();
                        command.CommandText = @"CREATE TABLE Coupon(
		                                        ID SERIAL PRIMARY KEY         NOT NULL,
		                                        ProductName     VARCHAR(24) NOT NULL,
		                                        Description     TEXT,
		                                        Amount          INT
	                                             );";
                        command.ExecuteNonQuery();
                        command.CommandText = @"
                                        INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('IPhone X', 'IPhone Discount', 222);
                                        INSERT INTO Coupon (ProductName, Description, Amount) VALUES ('Samsung 10', 'Samsung Discount', 222); ";
                        command.ExecuteNonQuery();
                        logger.LogInformation("Migrate successfull");
                    } 
                  
                }
                catch (Exception e )
                {
                    logger.LogError("Error sedding ");
                    if(retryForAvailability  < 50)
                    {
                        retryForAvailability++;
                        Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                    throw;
                }
                return host;
            }
        }
    }
}
